using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Cult_Penguin_Server
{
    public class ConnectedClients
    {
        //dictionary over id og clientInfo
        Dictionary<Guid, ClientInfo> clientsByGuid = new Dictionary<Guid, ClientInfo>();

        public Dictionary<Guid, ClientInfo> ClientsByGuid { get => clientsByGuid; set => clientsByGuid = value; }

        //tilføjer en ny client til dictionaryen
        public void AddClient(Guid clientGuid, TcpClient client)
        {
            clientsByGuid.Add(clientGuid, new ClientInfo(client) { ClientGuid = clientGuid });

        }

        //fjerner en client fra dictionaryen over clients
        public void RemoveClient(Guid clientGuid)
        {
            clientsByGuid[clientGuid].Dispose();
            clientsByGuid.Remove(clientGuid);

        }


        public ClientInfo this[Guid clientGuid]
        {
            get { return clientsByGuid[clientGuid]; }
            set { clientsByGuid[clientGuid] = value; }
        }

        //får alle brugers navne som strings
        public List<string> GetNameOfAllAsStringList()
        {
            List<string> names = new List<string>();
            foreach (var clientInfo in clientsByGuid.Values)
            {
                names.Add(clientInfo.Name);
            }
            return names;
            //return string.Join(Environment.NewLine, names);
        }

        public List<Vector2> GetPositionOfAllAsVector2List()
        {
            List<Vector2> positions = new List<Vector2>();
            foreach (var clientInfo in clientsByGuid.Values)
            {
                positions.Add(new Vector2(clientInfo.xPosition,clientInfo.yPosition));
            }
            return positions;
            //return string.Join(Environment.NewLine, names);
        }
        //sender en besked til alle brugere
        public void SendMessageToAll(byte[] message, MessageType msgType)
        {
            
            foreach (Guid g in clientsByGuid.Keys)
            {
                
                clientsByGuid[g].SendMessage(message, msgType);
            }
        }

        public void SendEncrpytedMessageToAll(NetworkMessage message, MessageType msgType)
        {
            Console.WriteLine("sending Encrypted message to all");

            foreach (Guid g in clientsByGuid.Keys)
            {
                byte[] messageBytes = new byte[1024];

                switch (msgType)
                {
                    case MessageType.GlobalMessage:
                        GlobalMessage sendingGMes = ((GlobalMessage)message);
                        sendingGMes.EncryptedMessage = EncryptDataWithBinaryArray(sendingGMes.Message, clientsByGuid[g].clientPublicKey);
                        
                        messageBytes = MessagePackSerializer.Serialize((GlobalMessage)sendingGMes);
                        break;
                    case MessageType.JoinedClient:
                        break;
                    case MessageType.ServerMessage:
                        break;
                    case MessageType.SnapShot:
                        break;
                    case MessageType.MovementUpdate:
                        break;
                    case MessageType.AccountLogin:
                        break;
                    case MessageType.CreateAccount:
                        break;
                    case MessageType.GetKey:
                        break;
                    case MessageType.SendKey:
                        break;
                    default:
                        break;
                }
                Console.WriteLine("ID: " + g);
                clientsByGuid[g].SendMessage(messageBytes, msgType);
            }
        }

        byte[] EncryptDataWithBinaryArray(string data, byte[] publicKey)
        {
            using (RSACryptoServiceProvider rsaService = new RSACryptoServiceProvider())
            {
                rsaService.ImportCspBlob(publicKey);
                RSAParameters publicKeyAsParams = rsaService.ExportParameters(false);
                return EncryptData(data, publicKeyAsParams);
            }

        }
        byte[] EncryptData(string data, RSAParameters publicKey)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(data);

            using (RSA rsa = RSA.Create())
            {
                rsa.ImportParameters(publicKey);
                return rsa.Encrypt(byteData, RSAEncryptionPadding.Pkcs1);
            }
        }

        //sender en direkte besked til en anden bruger
        /*
        public void SendDirectMessage(string message, string recipient, Guid sender)
        {
            var foundClient = clientsByGuid.Values.FirstOrDefault(clientInfo => clientInfo.Name == recipient);
            if (foundClient != null)
            {
                foundClient.SendMessage(message);
            }
            else
            {
                clientsByGuid[sender].SendMessage(recipient + " is not online!");
            }

        }*/

    }
    //en klasse med info omkring clienten
    public class ClientInfo
    {
        public Guid ClientGuid { get; set; }
        public TcpClient client;
        public string Name { get; set; } = string.Empty;
        BinaryWriter bWriter;
        public byte[] clientPublicKey;
        public float xPosition;
        public float yPosition;

        public ClientInfo(TcpClient client)
        {
            bWriter = new BinaryWriter(client.GetStream());
            this.client = client;
        }
        public void SendMessage(byte[] msg, MessageType msgType)
        {
            byte[] messageBytes = new byte[1024];
            //ServerMessage mes = new ServerMessage() { Info = message };
            Console.WriteLine("sending message");
            //messageBytes = MessagePackSerializer.Serialize((GlobalMessage)msg);
            //messageBytes = msg;
            messageBytes = msg;

            bWriter.Write(messageBytes.Length);
            bWriter.Write((byte)msgType);
            bWriter.Write(messageBytes);
            bWriter.Flush();
        }
        public void Dispose()
        {
            bWriter.Dispose();
            client.Dispose();
        }
    }
}
