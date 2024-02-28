using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.Xna.Framework;

namespace Cult_Penguin
{


    //de forskellige messagetyper der kan blive sendt
    //de forskellige messagetyper der kan blive sendt
    public enum MessageType { GlobalMessage, JoinedClient, ServerMessage, SnapShot, MovementUpdate, AccountLogin, CreateAccount, GetKey, SendKey, DirectMessage, UpdateMessage }


    //en overklasse der definere hvad en besked er
    [MessagePackObject]
    public abstract class NetworkMessage
    {
        //public TcpClient sender;

        [IgnoreMember]
        public abstract MessageType MessageType { get; }
        [IgnoreMember]
        public byte GetMessageTypeAsByte
        {
            get { return (byte)MessageType; }
        }
    }
    //public class HelloMessage : NetworkMessage
    //{
    //    [Key(0)]
    //    public string? UserName;
    //    [IgnoreMember]
    //    public override MessageType MessageType => MessageType.Hello;
    //}

    //når brugeren sender en besked
    public class GlobalMessage : NetworkMessage
    {
        
        [IgnoreMember]
        public string Message;

        [Key(0)]
        public byte[] EncryptedMessage;

        [Key(1)]
        public string senderName;

        [IgnoreMember]
        public override MessageType MessageType => MessageType.GlobalMessage;
    }


    [MessagePackObject]
    public class DirectMessage : NetworkMessage
    {
        [Key(0)]
        public string? Info { get; set; }

        [IgnoreMember]
        public override MessageType MessageType => MessageType.DirectMessage;
    }
    //public class DirectMessage : NetworkMessage
    //{
    //    [Key(0)]
    //    public string? Message;
    //    [Key(1)]
    //    public string? recipient;
    //    [IgnoreMember]
    //    public override MessageType MessageType => MessageType.DirectMessage;
    //}
    //public class ListMessage : NetworkMessage
    //{
    //    [Key(0)]
    //    public List<string?> UserNames;
    //    [IgnoreMember]
    //    public override MessageType MessageType => MessageType.List;
    //}

    //når serveren sender en besked
    public class ServerMessage : NetworkMessage
    {
        [Key(0)]
        public string info;
        [IgnoreMember]
        public override MessageType MessageType => MessageType.ServerMessage;
    }

    //en besked der bliver skrevet når brugeren joiner
    public class JoinedClient : NetworkMessage
    {
        [Key(0)]
        public string info;
        [IgnoreMember]
        public override MessageType MessageType => MessageType.JoinedClient;
    }

    public class SnapShot : NetworkMessage
    {
        [Key(0)]
        public int SnapSeqId;

        [Key(1)]
        public float playerPosX;

        [Key(2)]
        public float playerPosY;
        [IgnoreMember]
        public override MessageType MessageType => MessageType.SnapShot;
    }


    public class MovementUpdate : NetworkMessage
    {
        [Key(0)]
        public float PositionX { get; set; }

        [Key(1)]
        public float PositionY { get; set; }

        [Key(2)]
        public int SequenceNumber { get; set; }
        [IgnoreMember]
        public override MessageType MessageType => MessageType.MovementUpdate;

    }

    public class AccountLogin : NetworkMessage
    {
        
        [IgnoreMember]
        public string username { get; set; }

        [Key(0)]
        public byte[] encryptedUsername;

        [IgnoreMember]
        public string password { get; set; }

        [Key(1)]
        public byte[] encryptedPassword;

        

        [IgnoreMember]
        public override MessageType MessageType => MessageType.AccountLogin;
    }

    [MessagePackObject]
    public class UpdateMessage : NetworkMessage
    {
        [Key(0)]
        public string[] Names { get; set; }

        [Key(1)]
        public float[] xPos { get; set; }

        [Key(2)]
        public float[] yPos { get; set; }


        [IgnoreMember]
        public override MessageType MessageType => MessageType.UpdateMessage;

    }

    public class CreateAccount : NetworkMessage
    {
        [IgnoreMember]
        public string username { get; set; }

        [Key(0)]
        public byte[] encryptedUsername;

        [IgnoreMember]
        public string password { get; set; }

        [Key(1)]
        public byte[] encryptedPassword;

        

        [IgnoreMember]
        public override MessageType MessageType => MessageType.CreateAccount;
    }
    public class GetKeyMessage : NetworkMessage
    {
        [IgnoreMember]
        public override MessageType MessageType => MessageType.GetKey;
    }

    public class SendKeyMessage : NetworkMessage
    {
        [Key(0)]
        public byte[] publicKey;
        [IgnoreMember]
        public override MessageType MessageType => MessageType.SendKey;
    }
}
