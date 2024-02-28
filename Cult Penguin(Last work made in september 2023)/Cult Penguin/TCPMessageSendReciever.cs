using Cult_Penguin;
using Cult_Penguin.Components;
using MessagePack;
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

namespace Network2023AsymEncryption
{
    public class TCPMessageSendReciever
    {
        protected RSAParameters privateKey;
        protected RSAParameters publicKey;
        byte[] serverPublicKey;
        bool readyToSendMessage = false;

        private Queue<UpdateMessage> updateMessages = new Queue<UpdateMessage>();

        public bool ReadyToSendMessage { get => readyToSendMessage; }
        public Queue<UpdateMessage> UpdateMessages { get => updateMessages; set => updateMessages = value; }

        public TCPMessageSendReciever()
        {
            

        }
        protected void RecieveMessageFromConenction(TcpClient client, BinaryReader bwr,BinaryWriter bw)
        {
            

            try
            {
                while (client.Connected)
                {

                    // Read the message data
                    int messageLength = bwr.ReadInt32();
                    byte messageType = bwr.ReadByte();
                    // Read the message data
                    byte[] messageBytes = bwr.ReadBytes(messageLength);

                    MessageType receivedType = (MessageType)messageType;
                    HandleMessageType(receivedType, messageBytes,bw);


                }
                Console.WriteLine("client closed");
            }
            catch (Exception e)
            {
                Console.WriteLine("something happened" + e.Message);
            }
            finally
            {

                client.Close();
            }
        }
        protected virtual void HandleMessageType(MessageType recievedType, byte[] messageBytes,BinaryWriter bw)
        {
            switch (recievedType)
            {

                case MessageType.GlobalMessage:
                    GlobalMessage gMes = MessagePackSerializer.Deserialize<GlobalMessage>(messageBytes);
                    gMes.Message = DecryptData(gMes.EncryptedMessage, privateKey);

                    ChatHandler.Instance.DisplayMessage(gMes.senderName, gMes.Message);
                    break;

                case MessageType.JoinedClient:
                    JoinedClient jMes = MessagePackSerializer.Deserialize<JoinedClient>(messageBytes);
                    /*if (jMes.info != GameWorld.Instance.PlayerName)
                    {
                        GameWorld.Instance.SpawnNewPlayer(jMes.info, new Microsoft.Xna.Framework.Vector2(jMes.));
                    }*/
                    
                    break;
                case MessageType.SendKey:
                    SendKeyMessage sendKeyMessage = MessagePackSerializer.Deserialize<SendKeyMessage>(messageBytes);
                    serverPublicKey = sendKeyMessage.publicKey;
                    readyToSendMessage = true;
                    break;
                case MessageType.DirectMessage:
                    DirectMessage dMes = MessagePackSerializer.Deserialize<DirectMessage>(messageBytes);
                    switch (dMes.Info)
                    {
                        case "login error":
                            LoginHandler.Instance.ShowMessage("username eller password er forkert :(");
                            //GameWorld.Instance.LoginResponse();
                            break;
                        case "account creation error":
                            LoginHandler.Instance.ShowMessage("invalid username :(");
                            break;
                        case "login success":
                            GameWorld.Instance.LoggedIn = true;
                            GameWorld.Instance.LoginResponse();
                            break;
                        case "account creation success":
                            LoginHandler.Instance.ShowMessage("Account created successfully");
                            break;
                        default:
                            break;
                    }
                    break;
                case MessageType.UpdateMessage:
                    UpdateMessage uMes = MessagePackSerializer.Deserialize<UpdateMessage>(messageBytes);
                    updateMessages.Enqueue(uMes);
                    break;

                default:
                    Console.WriteLine("ukendt");
                    break;
            }
        }

        protected void SendTCPMessage(NetworkMessage message, BinaryWriter bw)
        {
            byte[] messageBytes = new byte[1024];
            
            switch(message.MessageType) 
            {

                case MessageType.GlobalMessage:
                    GlobalMessage gMes = ((GlobalMessage)message);
                    gMes.EncryptedMessage = EncryptDataWithBinaryArray(gMes.Message, serverPublicKey);
                    messageBytes = MessagePackSerializer.Serialize(gMes);
                    break;

                case MessageType.JoinedClient:
                    messageBytes = MessagePackSerializer.Serialize((JoinedClient)message);
                    break;

                case MessageType.GetKey:
                    messageBytes = MessagePackSerializer.Serialize((GetKeyMessage)message);
                    break;
                case MessageType.SendKey:
                    SendKeyMessage sMes = ((SendKeyMessage)message);
                    sMes.publicKey = ExportPublicKeyAsBinary(publicKey);
                    messageBytes = MessagePackSerializer.Serialize(sMes);
                    break;
                case MessageType.AccountLogin:
                    AccountLogin login = ((AccountLogin)message);
                    login.encryptedUsername = EncryptDataWithBinaryArray(login.username, serverPublicKey);
                    login.encryptedPassword = EncryptDataWithBinaryArray(login.password, serverPublicKey);
                    messageBytes = MessagePackSerializer.Serialize(login);

                    break;

                case MessageType.CreateAccount:
                    CreateAccount create = ((CreateAccount)message);
                    create.encryptedUsername = EncryptDataWithBinaryArray(create.username, serverPublicKey);
                    create.encryptedPassword = EncryptDataWithBinaryArray(create.password, serverPublicKey);
                    messageBytes = MessagePackSerializer.Serialize(create);
                    break;
                case MessageType.MovementUpdate:
                    MovementUpdate mMes = ((MovementUpdate)message);
                    messageBytes = MessagePackSerializer.Serialize((MovementUpdate)mMes);
                    break;

                default:
                    Console.WriteLine("ukendt");
                    break;


                /*case MessageType.GetKey:
                    messageBytes = MessagePackSerializer.Serialize((GetKeyMessage)message);
                    break;
                case MessageType.SendKey:
                    messageBytes = MessagePackSerializer.Serialize((SendKeyMessage)message);
                    break;
                case MessageType.SendEncryptedMes:
                    messageBytes = MessagePackSerializer.Serialize((SendEncryptedMessage)message);
                    break;
                default:
                    break;*/
            }
            /* bw.Write(messageBytes.Length);
             bw.Write(message.GetMessageTypeAsByte);
             bw.Write(messageBytes);
             bw.Flush();*/

            SendBytesToConnection(messageBytes, message.MessageType, bw);

        }

        protected void SendBytesToConnection(byte[] messageBytes,MessageType messageType,BinaryWriter bw)
        {
            bw.Write(messageBytes.Length);
            bw.Write((byte)messageType);
            bw.Write(messageBytes);

            bw.Flush();
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

        string DecryptData(byte[] encryptedData, RSAParameters privateKey)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportParameters(privateKey);
                byte[] decryptedData = rsa.Decrypt(encryptedData, RSAEncryptionPadding.Pkcs1);
                return Encoding.UTF8.GetString(decryptedData);
            }
        }

        protected byte[] ExportPublicKeyAsBinary(RSAParameters publicKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(publicKey);
                byte[] publicKeyBytes = rsa.ExportCspBlob(false);
                return publicKeyBytes;
            }
        }

        
        protected void GenerateKeypair()
        {
            using (RSA rsa = RSA.Create())
            {
                privateKey = rsa.ExportParameters(true);
                publicKey = rsa.ExportParameters(false);
            }

            
        }

        

    }
}
