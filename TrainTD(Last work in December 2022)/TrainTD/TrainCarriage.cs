using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace TrainTD
{
    /// <summary>
    /// Generel Klasse for togVogne
    /// </summary>
    public abstract class TrainCarriage:GameObject
    {
        //Vognene henholdsvis foran og bagved denne vogn. Kan være null
        protected TrainCarriage carriageAhead;
        protected TrainCarriage carriageBehind;

        //Hvor meget liv vognen har
        protected int health;

        //Hvor hurtigt vognen kører
        public int speed = 0;

        //Hvilken spiller ejer vognen
        protected Player owner;

        //Hvorvidt dette er den sidste vogn i toget
        private bool isLastCarriage = true;
        
        //Det foreste spor vognen er på
        protected Track currentTrack;

        //Hvilke spor vogne har været på
        protected List<Track> trackHistory;

        //Hvilket spor vognen bevæger sig imod
        protected Track direction;

        //Retningen vognen bevæger sig
        protected Vector2 velocity;

        protected Player Owner { get => owner; }
        public bool IsLastCarriage { get => isLastCarriage;}
        public virtual Track CurrentTile { get => currentTrack; set => currentTrack = value; }



        public void SetSpeed(int speed) {
            this.speed = speed;
            if (carriageBehind != null) {
                carriageBehind.SetSpeed(speed);
            }
        }

        

        public TrainCarriage(TrainCarriage carrigeAhead, Player owner, Vector2 position, Texture2D[] sprites, SpriteEffects spriteEffect, int lootValue, int health) : base(position, sprites, spriteEffect, lootValue, 0, 0)
        {
            this.carriageAhead = carrigeAhead;
            if (carriageAhead != null) {
                carriageAhead = carriageAhead.ConnectCarriage(this);
            }
            this.owner = owner;
            this.health = health;
            carriageBehind = null;
            trackHistory = new List<Track>();
            checkCurrentTrack();
        }

        public void setDirection(Track track)
        {
            direction = track;
            if (!isLastCarriage)
            {
                carriageBehind.setDirection(currentTrack);
            }
        }

        /// <summary>
        /// Tjekker om toget befinder sig på et givent spor
        /// </summary>
        /// <param name="track">Det spor der tjekkes om toget befinder sig på</param>
        /// <returns></returns>
        protected bool trainContainsTrack(Track track)
        {
            if (currentTrack == track && carriageBehind != null)
            {
                return true;
            }
            else if (carriageBehind != null)
            {
                return carriageBehind.trainContainsTrack(track);
            }
            else
            {
                return false;
            }
        }

        public virtual int trainLength(bool ahead) {
            if (carriageBehind != null && !ahead)
            {
                return 1 + carriageBehind.trainLength(ahead);
            }
            else if (carriageAhead != null && ahead) {
                return 1 + carriageAhead.trainLength(ahead);
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// Funktion til at bevæge toget
        /// </summary>
        /// <param name="gameTime">GameWorlds gameTime</param>
        public void Move(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            position += ((velocity * speed) * deltaTime);

        }

        /// <summary>
        /// Fjerner den sidste vogn og sætter den næstsidste til at være den sidste
        /// </summary>
        protected void DetacthLastCarriage() {
            if (carriageBehind == null)
            {
                carriageAhead = null;
            }
            else if (carriageBehind.IsLastCarriage)
            {
                carriageBehind.DetacthLastCarriage();
                carriageBehind = null;
                isLastCarriage = true;
            }
            else {
                carriageBehind.DetacthLastCarriage();
            }
        }

        public abstract bool ConnectedToLocomotive();
        public abstract bool ConnectedToEmptyCargoCapacity(bool ahead, int amount);
        public abstract void HandleLoot(int loot, bool ahead);

        /// <summary>
        /// Forbinder en vogn bagerst på toget
        /// </summary>
        /// <param name="carriage">Vognen der skal forbindes</param>
        /// <returns>Den bagerste vogn</returns>
        public TrainCarriage ConnectCarriage(TrainCarriage carriage)
        {
            if (carriageBehind == null)
            {
                carriageBehind = carriage;
                isLastCarriage = false;
                return this;
            }
            else {
                return carriageBehind.ConnectCarriage(carriage);
            }
        }

        public override int lootFromObject()
        {
            return lootValue;
        }

        public void AddToTrackHistory(Track track) {
            trackHistory.Add(track);
        }
        
        /// <summary>
        /// Fjerner skade fra health og hvis health er under eller lig 0 destrueres vognen
        /// </summary>
        /// <param name="damage"></param>
        public virtual void TakeDamage(int damage)
        {
            health -= damage;
            if (health <= 0) {
                shouldRemove = true;
                GameWorld.GetCurrentPlayer().Steel += lootFromObject();
                if (carriageAhead != null) {
                    carriageAhead.DetachRest();
                    carriageAhead = null;
                }
                if (carriageBehind != null) {
                    SetSpeed(0);
                    DetachRest();
                }
                
            }
        }
        
        /// <summary>
        /// Ændrer currentTrack og sikrer at vognen bliver på sporet når den når til et TurnPoint
        /// </summary>
        /// <param name="newTrack"></param>
        public void changeCurrentTrack(Track newTrack) {
            if (newTrack != currentTrack) {
                currentTrack = newTrack;
                if (currentTrack is TurnPoint && currentTrack.TurnDirection != rotation) {
                    position = currentTrack.Position;
                }
                direction = currentTrack.NextTrack;
                rotation = currentTrack.TurnDirection;
                trackHistory.Add(newTrack);

            }
        }

        /// <summary>
        /// Tjekker om vognen er nået til et nyt spor og hvis det er røget helt af sporet får bragt det tilbage på sporet
        /// </summary>
        private void checkCurrentTrack() {
            List<Track> tracks = GameWorld.getTracks();
            List<Track> intersects = new List<Track>();
            foreach (Track track in tracks) {
                if (IsColliding(track)) {
                    intersects.Add(track);
                }
            }
            if (intersects.Count == 0) {
                if (Vector2.Distance(position, direction.Position) > Vector2.Distance(currentTrack.Position, direction.Position)) {
                    changeCurrentTrack(direction);
                    position = currentTrack.Position;
                }
                
            }
            foreach (Track track in intersects) {
                if (track == direction || currentTrack == null) {
                    changeCurrentTrack(track);
                    break;
                }
            }
        }

        /// <summary>
        /// Metode der rykker vognen fremad hvis den er ovenpå vognen bagved
        /// </summary>
        public void MoveForward() {
            if (carriageBehind != null && Vector2.Distance(position, carriageBehind.CollisionBox.Center.ToVector2()) < carriageBehind.CollisionBox.Height && (currentTrack is not TurnPoint || trackHistory[0] == currentTrack))
            {
                if (currentTrack != carriageBehind.direction || currentTrack != carriageBehind.currentTrack)
                {
                    changeCurrentTrack(carriageBehind.direction);
                }
                if (carriageBehind != null) {
                    position = carriageBehind.CollisionBox.Center.ToVector2() + velocity *  carriageBehind.CollisionBox.Height;
                }
                
            }
            
            checkCurrentTrack();
            if (carriageBehind != null)
            {
                carriageBehind.MoveForward();
            }
            
            

        }
        /*public void MoveBackwards()
        {
            if (carriageAhead != null && Vector2.Distance(position, carriageAhead.CollisionBox.Center.ToVector2()) != carriageAhead.CollisionBox.Height && (currentTrack is not TurnPoint || trackHistory[0] == currentTrack))
            {
                position = carriageAhead.CollisionBox.Center.ToVector2() - velocity * (carriageAhead.CollisionBox.Height - Vector2.Distance(position, carriageAhead.CollisionBox.Center.ToVector2()));

            }
            
            if (carriageAhead != null)
            {
                carriageAhead.MoveForward();
            }

        }*/

        /// <summary>
        /// Bevæger vognen fremad hvis den er forbundet Til et lokomotiv
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (!ConnectedToLocomotive())
            {
                speed = 0;
            }
            else {
                checkCurrentTrack();
                if (direction != null)
                {
                    rotation = currentTrack.TurnDirection;
                    if (direction.Position.X > currentTrack.Position.X)
                    {
                        rotation += (float)((Math.PI / 180) * 180);
                    }
                    CalculateVelocity();
                    //speed = 100;
                    Move(gameTime);
                }
                else {
                    ShouldRemove = true;
                }
            }
            
            placement();

        }

        public bool CarriageInTrain(TrainCarriage carriage)
        {
            if (carriage == this)
            {
                return true;
            }
            else if (carriageBehind != null)
            {
                return carriageBehind.CarriageInTrain(carriage);
            }
            else {
                return false;
            }

        }


        /// <summary>
        /// Frakobler vognene bagved
        /// </summary>
        public void DetachRest() {
            carriageBehind = null;
            
            isLastCarriage = true;
        }

        private void CalculateVelocity()
        {
            
            velocity = new Vector2((float)Math.Sin(rotation), -(float)Math.Cos(rotation));
            
        }

        protected override bool PlacementExceptions(GameObject go)
        {
            return false;
        }
    }
}
