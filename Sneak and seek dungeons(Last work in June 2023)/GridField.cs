using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons
{
    internal class GridField
    {
        public Rectangle position;
        public bool enterExitTop;
        public bool enterExitBot;
        public bool enterExitLef;
        public bool enterExitRig;
        public bool containsExit = false;
        public bool containsPassage = false;
        public bool emptySpace = false;
        private Texture2D texture = GameWorld.Instance.Content.Load<Texture2D>("Wall");

        public GridField(Rectangle position, bool enterExitTop, bool enterExitBot, bool enterExitLef, bool enterExitRig)
        {
            this.position = position;
            this.enterExitTop = enterExitTop;
            this.enterExitBot = enterExitBot;
            this.enterExitLef = enterExitLef;
            this.enterExitRig = enterExitRig;
        }

        public void DrawRectangle(Rectangle collisionBox, SpriteBatch spriteBatch)
        {
            Rectangle topLine = new Rectangle(collisionBox.X, collisionBox.Y, collisionBox.Width, 2);
            Rectangle bottomLine = new Rectangle(collisionBox.X, collisionBox.Y + collisionBox.Height, collisionBox.Width, 2);
            Rectangle rightLine = new Rectangle(collisionBox.X + collisionBox.Width, collisionBox.Y, 2, collisionBox.Height);
            Rectangle leftLine = new Rectangle(collisionBox.X, collisionBox.Y, 2, collisionBox.Height);

            spriteBatch.Draw(texture, topLine, null, Color.Blue, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(texture, bottomLine, null, Color.Blue, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(texture, rightLine, null, Color.Blue, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(texture, leftLine, null, Color.Blue, 0, Vector2.Zero, SpriteEffects.None, 1);
        }
    }
}
