using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace TrainTD
{
    /// <summary>
    /// En speciel form for Track der kan forbinde mere end 2 spor og justere hvilket det sender tog videre imod. desuden fungerer de også som start og slutpunkter
    /// </summary>
    public class TurnPoint : Track
    {
        //private bool displayVersion;
        //Hvilke Turnpoints der er en direkte forbindelse til
        private List<TurnPoint> connectedTurnPoints;
        //Alle de spor der ligger i forbindelserne mod andre TurnPoints
        private List<List<Track>> connections;

        //Nruges under placering af TurnPoint
        private bool atExistingTurnPoint;
        private TurnPoint replacingTurnPoint;

        //Fortæller om TurnPointet er et start/slutpunkt
        private bool isEndNode;

        //Bruges af endNodes
        private bool hasContainedTrains = false;
        public TurnPoint(Vector2 position, Texture2D[] sprites, SpriteEffects spriteEffect, int lootValue, bool isEndNode) : base(position, sprites, spriteEffect, lootValue, 1.57f, 0)
        {
            connectedTurnPoints = new List<TurnPoint>();
            connections = new List<List<Track>>();
            this.IsEndNode = isEndNode;
        }


        public List<List<Track>> Connections { get => connections;}
        public List<TurnPoint> ConnectedTurnPoints { get => connectedTurnPoints;}
        public bool IsEndNode { get => isEndNode; set => isEndNode = value; }

        public override void Draw(SpriteBatch _spriteBatch)
        {
                base.Draw(_spriteBatch);
        }

        public void addConnection(TurnPoint connection) {
            ConnectedTurnPoints.Add(connection);
            //calculateTracks(connection);
        }
        public override GameObject getCopy()
        {
            return new TurnPoint(position, sprites, spriteEffect, lootValue, IsEndNode);
        }

        public override void LoadContent(ContentManager content)
        {
            throw new NotImplementedException();
        }

        public override int lootFromObject()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Genererer spor mellem to TurnPoints
        /// </summary>
        /// <param name="tp">Det turnPoint der skal laves spor mod</param>
        private void calculateTracks(TurnPoint tp) {
            
            //fjerner eventuel eksisterende forbindelse
            List<Track> con1 = ClearConnection(tp);
            GameWorld.ClearTracks(con1);
                    
            //Finder afstanden mellem de to TurnPoints
            float deltaX = tp.CollisionBox.Center.ToVector2().X - CollisionBox.Center.ToVector2().X;
            float deltaY = tp.CollisionBox.Center.ToVector2().Y - CollisionBox.Center.ToVector2().Y;
                    
                    
            Vector2 startPoint = CollisionBox.Center.ToVector2();
            Vector2 endPoint = tp.CollisionBox.Center.ToVector2();

            //Tjekker om retningen bør vendes
            bool flipped = false;
            if (deltaX < 0) {
                deltaX = CollisionBox.Center.ToVector2().X - tp.CollisionBox.Center.ToVector2().X;
                deltaY = CollisionBox.Center.ToVector2().Y - tp.CollisionBox.Center.ToVector2().Y;
                startPoint = tp.CollisionBox.Center.ToVector2();
                endPoint = CollisionBox.Center.ToVector2();
                flipped = true;
            }

            float primary = deltaX;
            float secondary = deltaY;
            float startPointPrimary = startPoint.X;
            float startPointSecondary = startPoint.Y;
            float endPointPrimary = endPoint.X;
            bool vertical = false;
            if (Math.Abs(deltaY) > Math.Abs(deltaX)) {
                primary = deltaY;
                secondary = deltaX;
                if (primary < 0)
                {
                    primary = CollisionBox.Center.ToVector2().Y - tp.CollisionBox.Center.ToVector2().Y;
                    secondary = CollisionBox.Center.ToVector2().X - tp.CollisionBox.Center.ToVector2().X;
                    startPoint = tp.CollisionBox.Center.ToVector2();
                    endPoint = CollisionBox.Center.ToVector2();
                    flipped = true;
                }
                startPointPrimary = startPoint.Y;
                startPointSecondary = startPoint.X;
                endPointPrimary = endPoint.Y;
                vertical = true;
            }
            //Udregner vinklen som sporene mellem dem skal have
            float ratio = secondary / primary;
            float constant = startPointSecondary - ratio * startPointPrimary;
            float angle = (float)-Math.Atan(primary / secondary);
            if (deltaX == 0) {
                angle = 0;
            }
            turnDirection = angle;
            rotation = angle;
            tp.rotation = angle;
            List<Track> tracks = new List<Track>
            {
                this
            };
            
            //Kode der køres hvis den oprindelige rækkefølge var fin
            if (!flipped)
            {
                        
                for (float i = startPointPrimary; i < endPointPrimary; i++)
                {
                            
                    Vector2 cPos = new Vector2(i, i * ratio + constant);
                    if (vertical) {
                        cPos = new Vector2(i * ratio * constant, i);
                    }
                    bool containsPosition = false;
                    //Tjekker om et spor kan placeres på denne position uden at overlappe med et eksisterende
                    foreach (Track track in tracks)
                    {
                        Vector2 center = track.CollisionBox.Center.ToVector2();
                        if (Vector2.Distance(center, cPos) < track.CollisionBox.Height || Vector2.Distance(center, cPos) < track.CollisionBox.Width)
                        {
                            containsPosition = true;
                        }

                    }
                    if (!containsPosition)
                    {
                        Track t =   new Track(cPos, sprites, spriteEffect, 0, angle, 0);
                        t.TurnDirection = angle;
                        if (tracks.Count > 1)
                        {
                            tracks[tracks.Count - 1].AddTrack(t);
                            t.AddTrack(tracks[tracks.Count - 1]);
                        }
                        else
                        {
                            t.AddTrack(this);
                            AddTrack(t);
                        }

                        tracks.Add(t);
                    }
                }
                int tracksPlaced = tracks.Count;
                float d = endPoint.X;
                //Køres hvis tp er et nyt turnpoint. Gør det samme som det tidligere loop bare for tp
                if (tp.beingPlaced)
                {
                    while (tracks.Count == tracksPlaced)
                    {
                        Vector2 cPos = new Vector2(d, d * ratio + constant);
                        if (vertical) {
                            cPos = new Vector2(d * ratio + constant, d);
                        }
                        bool containsPosition = false;
                        foreach (Track track in tracks)
                        {
                            Vector2 center = track.CollisionBox.Center.ToVector2();
                            if (Vector2.Distance(center, cPos) < track.CollisionBox.Height)
                            {
                                containsPosition = true;
                            }
                        }
                        if (!containsPosition)
                        {
                            tp.Position = cPos;
                            tracks[tracks.Count - 1].AddTrack(tp);
                            tp.AddTrack(tracks[tracks.Count - 1]);
                            tracks.Add(tp);
                        }
                        d++;
                    }
                }
                else {
                    tracks[tracks.Count - 1].AddTrack(tp);
                    tp.AddTrack(tracks[tracks.Count - 1]);
                    tracks.Add(tp);
                }
                        
            }
            //Det samme som før bare omvendt
            else {
                for (float i = endPointPrimary; i > startPointPrimary; i--)
                {
                    Vector2 cPos = new Vector2(i, i * ratio + constant);
                    if (vertical) {
                        cPos = new Vector2(i * ratio + constant, i);
                    }  
                    bool containsPosition = false;
                    foreach (Track track in tracks)
                    {
                        Vector2 center = track.CollisionBox.Center.ToVector2();
                        if (Vector2.Distance(center, cPos) < track.CollisionBox.Height || Vector2.Distance(center, cPos) < track.CollisionBox.Width)
                        {
                            containsPosition = true;
                        }
                    }
                    if (!containsPosition)
                    {
                        Track t = new Track(cPos, sprites, spriteEffect, 0, angle, 0);
                        t.TurnDirection = angle;
                        if (tracks.Count > 1)
                        {
                            tracks[tracks.Count - 1].AddTrack(t);
                            t.AddTrack(tracks[tracks.Count - 1]);
                        }
                        else
                        {
                            t.AddTrack(this);
                            AddTrack(t);
                        }
                        tracks.Add(t);
                    }
                }
                float d = endPoint.X;
                int tracksPlaced = tracks.Count;
                if (tp.beingPlaced)
                {
                    while (tracks.Count == tracksPlaced)
                    {
                        Vector2 cPos = new Vector2(d, d * ratio + constant);
                        if (vertical) {
                            cPos = new Vector2(d * ratio + constant, d);
                        }
                        bool containsPosition = false;
                        foreach (Track track in tracks)
                        {
                            Vector2 center = track.CollisionBox.Center.ToVector2();
                            if (Vector2.Distance(center, cPos) < track.CollisionBox.Height)
                            {
                                containsPosition = true;
                            }

                        }
                        if (!containsPosition)
                        {
                            tp.Position = cPos;
                            tracks[tracks.Count - 1].AddTrack(tp);
                            tp.AddTrack(tracks[tracks.Count - 1]);
                            tracks.Add(tp);
                        }
                        d--;
                    }
                }
                else
                {
                    tracks[tracks.Count - 1].AddTrack(tp);
                    tp.AddTrack(tracks[tracks.Count - 1]);
                    tracks.Add(tp);
                }
            }
            connections.Add(tracks);
            tp.Connections.Add(tracks);
            tp.atSnapPoint = true;
            direction = tp;
            turnDirection = angle;
            foreach (Track track in tracks) {
                if (track is not TurnPoint) {
                    GameWorld.InstantiateGameObject(track);
                }
            }
        
        }
        
        /// <summary>
        /// Rydder forbindelsen mellem to TurnPoints
        /// </summary>
        /// <param name="turnPoint"> det TurnPoints forbindelsen skal fjernes til</param>
        /// <returns></returns>
        private List<Track> ClearConnection(TurnPoint turnPoint)
        {
            List<Track> connectionToRemove = new List<Track>();
            foreach (List<Track> connection in Connections) {
                if (connection.Contains(turnPoint)) {
                    connectionToRemove = connection;
                }
            }
            Connections.Remove(connectionToRemove);
            turnPoint.Connections.Remove(connectionToRemove);
            connectionToRemove.Remove(this);
            connectionToRemove.Remove(turnPoint);
            
            return connectionToRemove;
        }

        /// <summary>
        /// Sikrer at de spor som skal placeres bliver placeret samt kalder koden for endNodes
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (beingPlaced)
            {
                placement();
                if (!beingPlaced && ConnectedTurnPoints.Count == 0 && !shouldRemove)
                {
                    TurnPoint connectedPoint = new TurnPoint(position, sprites, spriteEffect, lootValue, false);
                    connectedPoint.BeingPlaced = true;
                    TurnPoint connector = this;
                    connector.AddNewConnection(connectedPoint);
                    GameWorld.InstantiateGameObject(connectedPoint);
                }
            }
            else
            {
                foreach (TurnPoint tp in ConnectedTurnPoints)
                {
                    if (tp.BeingPlaced)
                    {
                        calculateTracks(tp);
                    }
                    else if (connectedTurnPoints.Count > connections.Count) {
                        calculateTracks(tp);
                    }
                }
                if (isEndNode) {
                    EndNodeManagement();
                }
            }
        }

        /// <summary>
        /// Sikrer at tog bliver fjernet når de når der til efter at de eventuelt har forladt den en gang
        /// </summary>
        private void EndNodeManagement() {
            if (!hasContainedTrains)
            {
                bool containsTrains = false;
                foreach (GameObject go in GameWorld.GetGameObjects)
                {
                    if (go is TrainCarriage && go.CollisionBox.Intersects(CollisionBox))
                    {
                        containsTrains = true;
                    }
                }
                 if (!containsTrains)
                {
                    hasContainedTrains = true;
                }
            }
            else {
                foreach (GameObject go in GameWorld.GetGameObjects)
                {
                    if (go is TrainCarriage && go.CollisionBox.Intersects(CollisionBox))
                    {
                        go.ShouldRemove = true;
                    }
                }
            }
            
        }

        public override bool ConnectedToTurnPoint()
        {
            return true;
        }

        /// <summary>
        /// Fastlåser positionen helt eller delvist hvis der er tale om små ændringer
        /// </summary>
        protected override void DetermineSnapPointLock()
        {
            Vector2 mousePos = Mouse.GetState().Position.ToVector2();
            if (atSnapPoint)
            {
                if (CollisionBox.Contains(mousePos))
                {
                    float difX = mousePos.X - position.X;
                    if (difX < 0) {
                        difX = -difX;
                    }
                    if (difX < CollisionBox.Width) {
                        lockedX = true;
                    }
                    float difY = mousePos.Y - position.Y;
                    if (difY < 0)
                    {
                        difY = -difY;
                    }
                    if (difY < 10) {
                        lockedY = true;
                    }
                }
                else {
                    atSnapPoint = false;
                    lockedX = false;
                    lockedY = false;
                }
            }
            else {
                lockedX = false;
                lockedY = false;
            }
        }

        /// <summary>
        /// Håndterer hvis et TurnPoint der skal placeres er ovenpå et eksisterende TurnPoint.
        /// </summary>
        /// <param name="go">TurnPointet som det er placeret ovenpå</param>
        /// <returns></returns>
        protected override bool PlacementExceptions(GameObject go)
        {
            if (go is TurnPoint && go != this && go.CollisionBox.Intersects(CollisionBox))
            {
                TurnPoint replacingTurnPointTemp = (TurnPoint)go;
                if (!ConnectedTurnPoints.Contains(replacingTurnPointTemp))
                {
                    position = replacingTurnPointTemp.Position;
                    atExistingTurnPoint = true;
                    replacingTurnPoint = replacingTurnPointTemp;
                    return true;
                }
                else if (atExistingTurnPoint)
                {
                    return true;
                }
                else
                {
                    atExistingTurnPoint = false;
                    return false;
                }
            }
            else if (atExistingTurnPoint) {
                return true;
            }
            else
            {
                atExistingTurnPoint = false;
                return false;
            }
            
        }

        /// <summary>
        /// Håndterer hvis et Turnpoint er placeret ovenpå et Turnpoint og spilleren ønsker at placere det
        /// </summary>
        protected override void placement()
        {
            base.placement();

            if (atExistingTurnPoint && Mouse.GetState().LeftButton == ButtonState.Pressed && beingPlaced)
            {
                if (connectedTurnPoints.Count == 0)
                {
                    TurnPoint connectedPoint = new TurnPoint(position, sprites, spriteEffect, lootValue, false);
                    connectedPoint.BeingPlaced = true;
                    replacingTurnPoint.AddNewConnection(connectedPoint);
                    beingPlaced = false;
                    shouldRemove = true;
                    GameWorld.InstantiateGameObject(connectedPoint);
                }
                else {
                    replacingTurnPoint.Combine(this);
                    beingPlaced = false;
                }
                

                
            }
            else {
                atExistingTurnPoint = false;
            }
        }

        public void AddNewConnection(TurnPoint tp)
        {
            tp.addConnection(this);
            addConnection(tp);
            calculateTracks(tp);
            nextTrack = connectedTracks[connectedTracks.Count - 1];
            nextTrack.SetNextTrack(this);

        }

        public void Combine(TurnPoint tp) {
            foreach (TurnPoint con in tp.ConnectedTurnPoints) {
                addConnection(con);
                List<Track> oldConnection = tp.ClearConnection(con);
                GameWorld.ClearTracks(oldConnection);
                
            }
            tp.ShouldRemove = true;
        }

        public override void SetDirection(Track sender)
        {
            
        }
    }
}
