using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace TrainTD
{
    /// <summary>
    /// Et spor for tog at køre på
    /// </summary>
    public class Track : GameObject
    {
        //Hvorvidt sporet indeholder en togvogn
        protected bool containsCarriage;

        //Hvilke spor det er forbundet til
        protected List<Track> connectedTracks;

        //Hvilket spor som tog skal bevæge sig i mod
        protected Track direction;

        //bruges i forbindelse med at tjekke om sporet er forbundet til et specifikt TurnPoint
        public bool currentlySearching = false;

        //Hvilken retning et tog skal vende for at komme til direction
        protected float turnDirection;
        
        //En semi erstatning af direction
        protected Track nextTrack;

        //Bruges i circuitTest
        protected bool reached = false;
        public Track(Vector2 position, Texture2D[] sprites, SpriteEffects spriteEffect, int lootValue, float rotation, float animationSpeed) : base(position, sprites, spriteEffect, lootValue, rotation, animationSpeed)
        {
            connectedTracks = new List<Track>();
            Layer = 0.5f;
        }


        public bool HasCarriage(TrainCarriage sender)
        {
            foreach (GameObject go in GameWorld.GetGameObjects)
            {
                if (go is TrainCarriage && CollisionBox.Intersects(go.CollisionBox) && go != sender && !(sender.CarriageInTrain((TrainCarriage)go)))
                {
                    return true;
                }
            }
            return false;
        }

        public bool ContainsCarriage { get => containsCarriage; set => containsCarriage = value; }
        public List<Track> ConnectedTracks { get => connectedTracks; }
        public float TurnDirection { get => turnDirection; set => turnDirection = value; }
        public Track NextTrack { get => nextTrack; set => nextTrack = value; }

        public void SetNextTrack(Track sender)
        {
            if (this is not TurnPoint)
            {
                foreach (Track tr in connectedTracks)
                {
                    if (tr != sender)
                    {
                        nextTrack = tr;
                        nextTrack.SetNextTrack(this);
                        break;
                    }
                }
            }
        }

        public virtual void SetDirection(Track sender) {
            foreach (Track track in connectedTracks) {
                if (track != sender) {
                    direction = track;
                    direction.SetDirection(this);
                    break;
                }
            }
        }

        public void AddTrack(Track track) {
            ConnectedTracks.Add(track);
        }

        public override GameObject getCopy()
        {
            return null;
        }


        public override void LoadContent(ContentManager content)
        {
            throw new System.NotImplementedException();
        }

        public override int lootFromObject()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Tjekker om der ved en fejl er blevet tilføjet en forbindelse for meget til et spor der ikke er et TurnPoint
        /// </summary>
        /// <param name="gameTime"></param>
        /// <exception cref="Exception"></exception>
        public override void Update(GameTime gameTime)
        {
            if (connectedTracks.Count > 2 && this is not TurnPoint) {
                throw new Exception("A normal track cannot be connected to more than two tracks");
            }
            /*if (nextTrack == null && connectedTracks.Count > 0 || nextTrack != connectedTracks[connectedTracks.Count - 1])
            {
                if (connectedTracks.Count > 0)
                {
                    nextTrack = connectedTracks[connectedTracks.Count - 1];
                }
                else
                {
                    nextTrack = connectedTracks[connectedTracks.Count - 1];
                }
            }*/
        }

        public Track getDirection()
        {
            return direction;
        }

        public bool ConnectedToSpecificTurnPoint(TurnPoint tp) {
            if (this == tp)
            {
                return true;
            }
            else {
                currentlySearching = true;
                bool foundTp = false;
                foreach (Track track in ConnectedTracks) {
                    if (track.ConnectedToSpecificTurnPoint(tp)) {
                        foundTp = true;
                    }
                }
                currentlySearching = false;
                return foundTp;
            }
        }

        protected override bool PlacementExceptions(GameObject go)
        {
            return false;
        }

        /// <summary>
        /// Debug metode
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="vias"></param>
        /// <returns></returns>
        public bool CircuitTest(Track startPoint, int vias) {
            if (reached && startPoint == this && vias == 0)
            {
                return true;
            }
            else if (nextTrack == null)
            {
                return false;
            }
            else if (startPoint == this)
            {
                reached = true;
                return nextTrack.CircuitTest(startPoint, vias);
            }
            else if (startPoint == this && reached && vias > 0)
            {
                return false;
            }
            else {
                int t = vias;
                if (this is TurnPoint && !reached)
                {
                    t--;
                    reached = true;
                }
                return nextTrack.CircuitTest(startPoint, t);
            }
        }
    }
}