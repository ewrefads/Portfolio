using System.Net.Sockets;
using System.Net;
using MessagePack;
using System.Text;
using System.Reflection;
using Cult_Penguin_Server;
using System.Timers;
using System.Security.Cryptography;
using System.Numerics;

RSAParameters privateKey;
RSAParameters publicKey;

GenerateKeypair();

ServerGameWorld world = ServerGameWorld.Instance;
float snapshotSpeed = 3;

Console.WriteLine("Starter server");

//et objekt som holder styr på de clients der conecter til serveren
ConnectedClients connectedClients;


//Access test/admin page for REST service at:http://localhost:5000/swagger/index.html
RESTManager rm = RESTManager.Instance;
string[] RESTargs = { };

Thread RESTthread = new Thread(() => rm.RunREST(RESTargs));
RESTthread.Start();
//lytter efter beskeder fra en specefik port
TcpListener server;

LoginHandler.Instance.TestMethod();

//forsøger på at starte server og opretter tråde når bruger conecter til server
try{
    connectedClients = new ConnectedClients();
    server = new TcpListener(IPAddress.Any, 12001);
    server.Start();
    Console.WriteLine("Server started... listening on port 12000");

    /*world = new ServerGameWorld();

    world.timer = new System.Timers.Timer();
    world.timer.Interval = 1000f / snapshotSpeed;
    world.timer.Elapsed += TimerElapsed;*/

    Thread updateClientThread = new Thread(() => UpdatePlayers());
    updateClientThread.Start();
    updateClientThread.IsBackground = true;

    while (true)
    {
        TcpClient client = server.AcceptTcpClient();
        Thread clientThread = new Thread(() => HandleClient(client));
        clientThread.Start();
    }

}
//hvis server ikke virker så skriv trist besked
catch
{
    Console.WriteLine("Server Error... :(");
    Console.ReadLine();
}




void HandleClient(TcpClient client)
{
    BinaryReader bwr = new BinaryReader(client.GetStream());

    Guid clientGuid = Guid.NewGuid();
    Console.WriteLine($"Client {clientGuid} connected.");

    connectedClients.AddClient(clientGuid, client);
    //deep reference to inner dict.
    ClientInfo clientInfo = connectedClients[clientGuid];
    bool loggedIn = false;
    try
    {
        while (client.Connected)
        {

            int messageLength = bwr.ReadInt32();
            int messageTypeAsByte = bwr.ReadByte();
            MessageType recievedType = (MessageType)messageTypeAsByte;
            byte[] payLoadAsBytes = bwr.ReadBytes(messageLength);

            // Klient skal logge ind (+ lave en account hvis de ikke har en) før de kan komme ud af loop
            
            int attemptedLogins = 0;
            while (!loggedIn)
            {
                messageLength = bwr.ReadInt32();
                messageTypeAsByte = bwr.ReadByte();
                // Read the message data
                payLoadAsBytes = bwr.ReadBytes(messageLength);
                recievedType = (MessageType)messageTypeAsByte;

                Console.WriteLine(recievedType);

                switch (recievedType)
                {
                    case MessageType.AccountLogin:
                        AccountLogin LoginMsg = MessagePackSerializer.Deserialize<AccountLogin>(payLoadAsBytes);

                        LoginMsg.username = DecryptData(LoginMsg.encryptedUsername, privateKey);
                        LoginMsg.password = DecryptData(LoginMsg.encryptedPassword, privateKey);
                        if (LoginHandler.Instance.Login(LoginMsg.username, LoginMsg.password, LoginMsg.user))

                        {
                            Console.WriteLine($"{clientGuid} has logged in successfully");
                            clientInfo.Name = LoginMsg.username;
                            loggedIn = true;

                            //sender besked til bruger at de er logged in
                            DirectMessage dMes = new DirectMessage() { Info = "login success" };
                            byte[] mesBytes = MessagePackSerializer.Serialize((DirectMessage)dMes);
                            clientInfo.SendMessage(mesBytes, dMes.MessageType);

                            //spawner spiller ind for andre clients
                            JoinedClient jc = new JoinedClient() { Info = LoginMsg.username};
                            byte[] jMesBytes = MessagePackSerializer.Serialize((JoinedClient)jc);
                            connectedClients.SendMessageToAll(jMesBytes,jc.MessageType);

                            //spawner andre spillere for spillere som joinede
                            /*List<string> names = connectedClients.GetNameOfAllAsStringList();
                            foreach (string name in names)
                            {
                                if (name == "")
                                {
                                    continue;
                                }
                                JoinedClient joinedMes = new JoinedClient() { Info = name};
                                byte[] joinedMesBytes = MessagePackSerializer.Serialize((JoinedClient)joinedMes);
                                clientInfo.SendMessage(joinedMesBytes, MessageType.JoinedClient); 
                            }*/
                        }
                        else
                        {
                            ++attemptedLogins;
                            Console.WriteLine($"{clientGuid} has {attemptedLogins} failed logins :)");

                            // Send besked tilbage til klienten for at informere dem om at login fejlede
                            DirectMessage dMes = new DirectMessage() { Info = "login error"};
                            byte[] mesBytes = MessagePackSerializer.Serialize((DirectMessage)dMes);
                            clientInfo.SendMessage(mesBytes,dMes.MessageType);
                        }

                        break;
                    case MessageType.CreateAccount:

                        CreateAccount CreateMsg = MessagePackSerializer.Deserialize<CreateAccount>(payLoadAsBytes);
                        CreateMsg.username = DecryptData(CreateMsg.encryptedUsername, privateKey);
                        CreateMsg.password = DecryptData(CreateMsg.encryptedPassword, privateKey);
                        //Console.WriteLine(CreateMsg.username+"---"+CreateMsg.password);
                        if (LoginHandler.Instance.CreateAccount(CreateMsg.username, CreateMsg.password))
                        {
                            Console.WriteLine($"{clientGuid} has created an account");

                            //sender besked til bruger
                            DirectMessage dMes = new DirectMessage() { Info = "account creation success" };
                            byte[] mesBytes = MessagePackSerializer.Serialize((DirectMessage)dMes);
                            clientInfo.SendMessage(mesBytes, dMes.MessageType);
                        }
                        else
                        {
                            Console.WriteLine($"{clientGuid} has attempted to create an account. (Account creation failed)");
                            // Send besked tilbage til klienten for at informere dem om at account creation fejlede

                            //sender besked til bruger
                            DirectMessage dMes = new DirectMessage() { Info = "account creation error" };
                            byte[] mesBytes = MessagePackSerializer.Serialize((DirectMessage)dMes);
                            clientInfo.SendMessage(mesBytes, dMes.MessageType);
                        }
                    


                        break;

                    case MessageType.GetKey:
                        SendKeyMessage kMes = new SendKeyMessage() { publicKey = ExportPublicKeyAsBinary(publicKey) };
                        byte[] messageBytes = new byte[1024];
                        //ServerMessage mes = new ServerMessage() { Info = message };
                        Console.WriteLine("sending public key");
                        messageBytes = MessagePackSerializer.Serialize((SendKeyMessage)kMes);
                        clientInfo.SendMessage(messageBytes, MessageType.SendKey);
                        Console.WriteLine("Send public key as bytes to client");
                        break;
                    case MessageType.SendKey:
                        SendKeyMessage sMes = MessagePackSerializer.Deserialize<SendKeyMessage>(payLoadAsBytes);
                        clientInfo.clientPublicKey = sMes.publicKey;
                        Console.WriteLine($"modtaget bruger: {clientGuid} offentlige nøgle!!!");
                        break;

                    default:
                        Console.WriteLine($"{clientGuid} message error. Unknown or wrong messagetype used. Send them to the shadow realm");
                        break;
                }
            }


            Console.WriteLine(recievedType);
                switch (recievedType)
                {
                    

                    case MessageType.JoinedClient:
                        Console.WriteLine("joinedclient");
                        JoinedClient hMes = MessagePackSerializer.Deserialize<JoinedClient>(payLoadAsBytes);
                        Console.WriteLine("New user joined!! Welcome: " + hMes.Info);
                        ServerGameWorld.Instance.latestUpdateTimeStamp = ((int)DateTime.Now.ToFileTimeUtc());
                    //myInfo.Name = hMes.UserName;

                    break;
                    case MessageType.GlobalMessage:
                        //udpakker beskeden og dekryptere den

                        GlobalMessage gMes = MessagePackSerializer.Deserialize<GlobalMessage>(payLoadAsBytes);
                        gMes.Message = DecryptData(gMes.EncryptedMessage, privateKey);
                        Console.WriteLine("decryptedMessage: " + gMes.Message);

                        //Laver en ny besked og sender den til alle med kryptering
                        GlobalMessage sendingGMes = new GlobalMessage() { Message = gMes.Message, senderName = gMes.senderName };
                        connectedClients.SendEncrpytedMessageToAll(sendingGMes, MessageType.GlobalMessage);
                        ServerGameWorld.Instance.latestUpdateTimeStamp = ((int)DateTime.Now.ToFileTimeUtc());
                        break;


                    case MessageType.ServerMessage:
                        Console.WriteLine("Wut da heelll");
                        break;


                    case MessageType.GetKey:
                        SendKeyMessage kMes = new SendKeyMessage() { publicKey = ExportPublicKeyAsBinary(publicKey) };
                        byte[] messageBytes = new byte[1024];
                        //ServerMessage mes = new ServerMessage() { Info = message };
                        Console.WriteLine("sending public key");
                        messageBytes = MessagePackSerializer.Serialize((SendKeyMessage)kMes);
                        clientInfo.SendMessage(messageBytes, MessageType.SendKey);
                        Console.WriteLine("Send public key as bytes to client");
                        break;
                    case MessageType.SendKey:
                        SendKeyMessage sMes = MessagePackSerializer.Deserialize<SendKeyMessage>(payLoadAsBytes);
                        clientInfo.clientPublicKey = sMes.publicKey;
                        Console.WriteLine($"modtaget bruger: {clientGuid} offentlige nøgle!!!");
                        break;
                case MessageType.MovementUpdate:
                    MovementUpdate mMes = MessagePackSerializer.Deserialize<MovementUpdate>(payLoadAsBytes);
                    world.UpdatePlayerMovement(mMes,clientInfo);
                    ServerGameWorld.Instance.latestUpdateTimeStamp = ((int)DateTime.Now.ToFileTimeUtc());
                    break;
                    default:
                        Console.WriteLine($"{clientGuid} goofed up!!! Send them to the shadow realm");
                        break;
                }






            /*
            if ((clientInfo.Name == string.Empty) && recievedType == MessageType.JoinedClient)

            {
                //Oh no error? break return?
                //clientInfo.SendMessage("SERVER: You dont have a user name set!");
                Console.WriteLine("should probably tell the user somehting is wrong!");
                continue;
            }*/

            /*foreach (byte b in messageBytes)
            {
                Console.Write(b+",");
            }*/
            //switch case der tager sig af de forskellige slags af beskeder
            //switch (recievedType)
            //{
                
            //}
        }
    }
    //hvis der opstår en fejl når brugeren sender en besked til serveren
    catch (Exception ex)
    {
        Console.WriteLine($"Exception occurred for client {clientInfo.Name}: {ex.Message}");
    }
    //køre denne kode selvom der opstår en fejl i programmet
    finally
    {
        Console.WriteLine($"Client disconnected: {clientInfo.Name}");
        connectedClients.RemoveClient(clientGuid);
        //connectedClients.SendMessageToAll(clientInfo.Name + " left the server...");
    }
}

//void HandleDirectMessage(DirectMessage dMes, Guid sender)
//{
//    connectedClients.SendDirectMessage("Direct: " + connectedClients[sender].Name + " : " + dMes.Message, dMes.recipient, sender);
//}

//sender besked til alle clienter som connectedclientsobjektet har

void UpdatePlayers()
{

    while (true)
    {
        Thread.Sleep(100);
        //Console.WriteLine(connectedClients.ClientsByGuid.Keys.Count);
        if (connectedClients.GetNameOfAllAsStringList().Count == 0)
            continue;
        //laver besked
        UpdateMessage updateMessage = new UpdateMessage();
        updateMessage.Names = connectedClients.GetNameOfAllAsStringList().ToArray();
        Vector2[] positions = connectedClients.GetPositionOfAllAsVector2List().ToArray();
        List<float> xPositions = new List<float>();
        List<float> yPositions = new List<float>();
        

        for (int i = 0; i < positions.Length; i++)
        {
            xPositions.Add(positions[i].X);
            yPositions.Add(positions[i].Y);
        }
        updateMessage.xPos = xPositions.ToArray();
        updateMessage.yPos = yPositions.ToArray();
         

        byte[] messageBytes = MessagePackSerializer.Serialize((UpdateMessage)updateMessage);

        foreach (ClientInfo client in connectedClients.ClientsByGuid.Values)
        {
            if (client.clientPublicKey == null)
                continue;
            client.SendMessage(messageBytes, MessageType.UpdateMessage);
        }
    }
}

static void SendMessageToClient(BinaryWriter writer, byte[] serializedData, MessageType messageType)
{

    // Send the length of the message as 4-byte integer
    writer.Write(serializedData.Length);
    writer.Write((byte)messageType);
    writer.Write(serializedData);

    writer.Flush();
}


void TimerElapsed(object sender, ElapsedEventArgs e)
{
    
    //SendDataToClient(world.GetWorldStateSnapShot());
    
}

void SendDataToClient(NetworkMessage message)
{
    byte[] messageBytes = new byte[1024];
    byte messageTypeByte = message.GetMessageTypeAsByte;
    switch (message.MessageType)
    {
        //We dont wont to send snapshots, only recive :)
        case MessageType.SnapShot:
            messageBytes = MessagePackSerializer.Serialize((SnapShot)message);
            break;
        default:
            break;
    }
    byte[] combinedBytes = new byte[1 + messageBytes.Length];
    combinedBytes[0] = messageTypeByte;
    Buffer.BlockCopy(messageBytes, 0, combinedBytes, 1, messageBytes.Length);
    //udpServer.Send(combinedBytes, clientEndPoint);
    //HandleGlobelMessage(message, );
    //HandleGlobelMessage();
}

void GenerateKeypair()
{
    using (RSA rsa = RSA.Create())
    {
        privateKey = rsa.ExportParameters(true);
        publicKey = rsa.ExportParameters(false);
    }
}

byte[] ExportPublicKeyAsBinary(RSAParameters publicKey)
{
    using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
    {
        rsa.ImportParameters(publicKey);
        byte[] publicKeyBytes = rsa.ExportCspBlob(false);
        return publicKeyBytes;
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

