using MessagePack;
using Network2023AsymEncryption;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cult_Penguin
{


    public class Client : TCPMessageSendReciever
    {

       

        TcpClient server;
        BinaryReader br;
        BinaryWriter bw;

        public Client()
        {
            GenerateKeypair();

            //sætter forbindelse op
            server = new TcpClient("localhost", 12001);
            br = new BinaryReader(server.GetStream());
            bw = new BinaryWriter(server.GetStream());
            Thread serverListenThread = new Thread(() => RecieveMessageFromConenction(server, br, bw));
            serverListenThread.Start();

            //giver serveren denne client public key

            //while (publicKey.Equals(null)) { }
            Thread.Sleep(1000);

            SendTCPMessage(new JoinedClient() { info = "Bonjour" }, bw);
            SendTCPMessage(new SendKeyMessage(), bw);
            //byte[] messagebytes = MessagePackSerializer.Serialize<SendKeyMessage>(sendKeyMessage);
            //SendBytesToConnection(messagebytes, MessageType.SendKey,bw);

            //får serverens public key
            Console.WriteLine("Requesting public key from server!");
            SendTCPMessage(new GetKeyMessage(), bw);
            

            
        }


        /*protected override void HandleMessageType(MessageType recievedType, byte[] messageBytes, BinaryWriter bw)
        {
            switch (recievedType) 
            {
                case MessageType.SendKey:
                    SendKeyMessage sendKeyMessage = MessagePackSerializer.Deserialize<SendKeyMessage>(messageBytes);
                    byte[] encryptedWithPubFromServer = EncryptDataWithBinaryArray("Such Secrecy! Only on client and only on fridays!", sendKeyMessage.publicKey);
                    SendTCPMessage(new SendEncryptedMessage() { encryptedMessage = encryptedWithPubFromServer }, bw);
                    Console.WriteLine("recived public key from server! encrypting message using server public key and sending back to server");
                    break;
            }

        }*/

       

        public void SendMessage(NetworkMessage message)
        {
            //server = new TcpClient("localhost", 12000);
            //bw = new BinaryWriter(server.GetStream());
            SendTCPMessage(message,bw);
        }

        
    }
}
