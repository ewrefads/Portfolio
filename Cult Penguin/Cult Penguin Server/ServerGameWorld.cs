using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Cult_Penguin_Server
{
    internal class ServerGameWorld
    {
        private static ServerGameWorld instance;
        public System.Timers.Timer timer;
        public int latestUpdateTimeStamp = ((int)DateTime.Now.ToFileTimeUtc());
        int snapShotId = 0;
        int playerPosX = 300;
        int playerPosY = 300;
        int playerMoveSpeed = 5;

        public ServerGameWorld() { }


        internal static ServerGameWorld Instance { get {
                if (instance == null) {
                    instance = new ServerGameWorld();
                }
                return instance;
            } 
        }

        

        public void UpdatePlayerMovement(MovementUpdate movement, ClientInfo client)
        {

            //if (movement.Moveleft)
            //{
            //    client.xPosition -= 1 * playerMoveSpeed;
            //}
            //else
            //{
            //    client.xPosition += 1 * playerMoveSpeed;
            //}

            


            snapShotId = movement.SequenceNumber;

            client.xPosition = movement.PositionX;
            client.yPosition = movement.PositionY;

            //if (movement.Moveup)
            //{
            //    client.yPosition -= 1 * playerMoveSpeed;
            //}
            //else
            //{
            //    client.yPosition += 1 * playerMoveSpeed;
            //}

            //latestUpdateTimeStamp = ((int)DateTime.Now.ToFileTimeUtc());

            //snapShotId = movement.SequenceNumber;

        }

        public SnapShot GetWorldStateSnapShot()
        {
            Console.WriteLine(snapShotId);
            Console.WriteLine("ballpos: " + playerPosX);
            return new SnapShot() { playerPosY = playerPosY, playerPosX = playerPosX, SnapSeqId = snapShotId };
        }

    }

    
}
