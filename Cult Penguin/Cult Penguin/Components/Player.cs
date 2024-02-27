using Cult_Penguin.CommandPattern;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cult_Penguin.Components
{
    internal class Player:Component
    {
        public bool controlled = false;
        Vector2 velocity = new Vector2(0, 0);
        public Vector2 destination = new Vector2(-1, -1);
        public Queue<Vector2> destinationQueue = new Queue<Vector2>();
        int speed = 5;
        public string name;
        private Vector2 nameLength;

        public int Speed { get => speed;  }

        public void ChangeVelocity(Vector2 delta) {
            if (velocity.X + delta.X >= -1 && velocity.X + delta.X <= 1)
            {
                velocity.X += delta.X;
            }
            else if (velocity.X + delta.X < -1) {
                velocity.X = -1;
            }
            else if (velocity.X + delta.X > 1)
            {
                velocity.X = 1;
            }
            if (velocity.Y + delta.Y >= -1 && velocity.Y + delta.Y <= 1)
            {
                velocity.Y += delta.Y;
            }
            else if (velocity.Y + delta.Y < -1)
            {
                velocity.Y = -1;
            }
            else if (velocity.Y + delta.Y > 1)
            {
                velocity.Y = 1;
            }
        }

        public override void Update()
        {
            if (controlled) {
                InputHandler.Instance.Execute(this);
            }
            else {
                MovementHandling();
            }
            GameObject.Transform.Translate(new Vector2(velocity.X * Speed, velocity.Y * Speed));

            if (velocity.X != 0 || velocity.Y != 0 && controlled)
            {
                MovementUpdate mMes = new MovementUpdate() { PositionX = GameObject.Transform.Position.X, PositionY = GameObject.Transform.Position.Y };
                GameWorld.Instance.Client.SendMessage(mMes);
                //if (velocity.X < 0) { mMes.Moveleft = true; }
                //if (velocity.X > 0) { mMes.Moveleft = false; }

                //if (velocity.Y > 0) { mMes.Moveup = false; }
                //if (velocity.Y < 0) { mMes.Moveup = true; }


            }

            velocity = new Vector2(0, 0);
            CollisionHandling();
            base.Update();
        }

        private void MovementHandling() {
            //if(destinationQueue.Count > 0 && destination == GameObject.)
            if (!controlled && destination != new Vector2(-1, -1) && destination != GameObject.Transform.Position) {
                velocity = new Vector2(GameObject.Transform.Position.X - destination.X, GameObject.Transform.Position.Y - destination.Y);
                if (Math.Abs(GameObject.Transform.Position.X - destination.X) < speed && Math.Abs(GameObject.Transform.Position.Y - destination.Y) < speed) {
                    float velX = Math.Abs(GameObject.Transform.Position.X - destination.X) / speed;
                    float velY = Math.Abs(GameObject.Transform.Position.Y - destination.Y) / speed;
                    velocity *= new Vector2(velX, velY);
                }
            }
        }

        private void CollisionHandling()
        {
            Vector2 deltaPos = GameObject.Transform.Position;
            SpriteRenderer sr = (SpriteRenderer)GameObject.GetComponent<SpriteRenderer>();
            if (GameObject.Transform.Position.X - (sr.Sprite.Width / 2) < 0)
            {
                deltaPos.X = sr.Sprite.Width / 2;
            }
            else if (GameObject.Transform.Position.X + (sr.Sprite.Width / 2) > GameWorld.Instance.Graphics.PreferredBackBufferWidth) {
                deltaPos.X = GameWorld.Instance.Graphics.PreferredBackBufferWidth - sr.Sprite.Width / 2;
            }
            if (GameObject.Transform.Position.Y - (sr.Sprite.Height / 2) < 0)
            {
                deltaPos.Y = sr.Sprite.Height / 2;
            }
            else if (GameObject.Transform.Position.Y + (sr.Sprite.Height / 2) > GameWorld.Instance.Graphics.PreferredBackBufferHeight)
            {
                deltaPos.Y = GameWorld.Instance.Graphics.PreferredBackBufferHeight - sr.Sprite.Height / 2;
            }
            GameObject.Transform.Position = deltaPos;
            Collider col = (Collider)GameObject.GetComponent<Collider>();
            if (col.CollisionBox.Contains(destination.X, destination.Y))
            {
                GameObject.Transform.Position = destination;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (nameLength == null) {
                nameLength = GameWorld.Instance.defaultFont.MeasureString(name);
            }
            Vector2 origin = new Vector2(nameLength.X / 2, nameLength.Y / 2);
            Vector2 textPos = GameObject.Transform.Position + new Vector2(0, 40);
            //spriteBatch.Draw(sprite, new Rectangle((int)Origin.X, (int)Origin.Y, sprite.Width, sprite.Height), Color.White);
            spriteBatch.DrawString(GameWorld.Instance.defaultFont, name, textPos, Color.Black, 0, origin, 1, SpriteEffects.None, 0.7f);
            base.Draw(spriteBatch);
        }
    }
}
