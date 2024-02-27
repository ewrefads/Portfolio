using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sneak_and_seek_dungeons.FactoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.Components
{
    internal class Door : Component, IInteractable
    {
        bool open = false;
        DOORDIRECTION direction;
        GridField[] neighbours = new GridField[2];
        bool locked;
        Item key;
        Collider col;
        Texture2D[] sprites;
        SpriteRenderer sr;
        Collider lastEnemy;

        public Door(Texture2D[] sprites, DOORDIRECTION direction)
        {
            Sprites = sprites;
            this.direction = direction;
        }

        internal GridField[] Neighbours { get => neighbours;}
        public bool Locked { get => locked; set => locked = value; }
        internal Item Key { get => key; set => key = value; }
        public bool Open { get => open; private set => open = value; }
        public Texture2D[] Sprites { get => sprites; set => sprites = value; }

        public void Interact()
        {
            if (Locked) {
                List<Item> items = Inventory.Instance.GetItems();
                if (items.Contains(Key) && Key != null) {
                    Unlock();
                    Key.Activate();
                }
            }
            if (!Locked && !Open)
            {
                OpenDoor();
            }
            else if (Open) {
                Close();
            }
        }

        private void Close()
        {
            Open = false;
            Collider col = (Collider)GameObject.GetComponent<Collider>();
            col.ActiveCollider = true;
            if (direction == DOORDIRECTION.vertical)
            {
                sr.Sprite = Sprites[0];
                
            }
            else {
                sr.Sprite = Sprites[0];
            }
            //GameObject.Transform.Position = new Vector2(GameObject.Transform.Position.X + (GameWorld.Instance.GridSize / 2), GameObject.Transform.Position.Y + (GameWorld.Instance.GridSize / 2));
        }

        private void OpenDoor()
        {
            Open = true;
            Collider col = (Collider)GameObject.GetComponent<Collider>();
            col.ActiveCollider = false;
            if (direction == DOORDIRECTION.vertical)
            {
                sr.Sprite = Sprites[1];
                
            }
            else
            {
                sr.Sprite = Sprites[1];
            }
            //GameObject.Transform.Position = new Vector2(GameObject.Transform.Position.X - (GameWorld.Instance.GridSize / 2), GameObject.Transform.Position.Y - (GameWorld.Instance.GridSize / 2));
        }

        public void Unlock()
        {
            Locked = false;
            if (neighbours[0].position.Y == neighbours[1].position.Y)
            {
                if (neighbours[0].position.X < neighbours[1].position.X)
                {
                    neighbours[0].enterExitRig = true;
                    neighbours[1].enterExitLef = true;
                }
                else
                {
                    neighbours[1].enterExitRig = true;
                    neighbours[0].enterExitLef = true;
                }
            }
            else
            {
                if (neighbours[0].position.Y < neighbours[1].position.Y)
                {
                    neighbours[0].enterExitTop = true;
                    neighbours[1].enterExitBot = true;
                }
                else
                {
                    neighbours[1].enterExitTop = true;
                    neighbours[0].enterExitBot = true;
                }
            }
        }
        public void Lock()
        {
            Locked = true;
            if (neighbours[0].position.Y == neighbours[1].position.Y)
            {
                if (neighbours[0].position.X < neighbours[1].position.X)
                {
                    neighbours[0].enterExitRig = false;
                    neighbours[1].enterExitLef = false;
                }
                else
                {
                    neighbours[1].enterExitRig = false;
                    neighbours[0].enterExitLef = false;
                }
            }
            else
            {
                if (neighbours[0].position.Y < neighbours[1].position.Y)
                {
                    neighbours[0].enterExitTop = false;
                    neighbours[1].enterExitBot = false;
                }
                else
                {
                    neighbours[1].enterExitTop = false;
                    neighbours[0].enterExitBot = false;
                }
            }
        }

        public override void Start()
        {
            sr = (SpriteRenderer)GameObject.GetComponent<SpriteRenderer>();
            if (Locked) {
                if (neighbours[0].position.Y == neighbours[1].position.Y)
                {
                    if (neighbours[0].position.X < neighbours[1].position.X)
                    {
                        neighbours[0].enterExitRig = false;
                        neighbours[1].enterExitLef = false;
                    }
                    else
                    {
                        neighbours[1].enterExitRig = false;
                        neighbours[0].enterExitLef = false;
                    }
                }
                else {
                    if (neighbours[0].position.Y < neighbours[1].position.Y)
                    {
                        neighbours[0].enterExitTop = false;
                        neighbours[1].enterExitBot = false;
                    }
                    else
                    {
                        neighbours[1].enterExitTop = false;
                        neighbours[0].enterExitBot = false;
                    }
                }
            }
            base.Start();
        }

        public override void Update()
        {
            if (col == null) {
                col = (Collider)GameObject.GetComponent<Collider>();
            }
            if (!open)
            {
                foreach (Collider coll in GameWorld.Instance.Colliders)
                {
                    if (coll != col && coll.CollisionBox.Intersects(col.CollisionBox))
                    {
                        Enemy e = (Enemy)coll.GameObject.GetComponent<Enemy>();
                        if (e != null)
                        {
                            Interact();
                            lastEnemy = coll;
                        }

                    }
                    if (open)
                    {
                        break;
                    }
                }
            }
            else if (open && lastEnemy != null) {
                Rectangle originalPos = new Rectangle((int)GameObject.Transform.Position.X + (GameWorld.Instance.GridSize / 2), (int)GameObject.Transform.Position.Y + (GameWorld.Instance.GridSize / 2), col.CollisionBox.Height, col.CollisionBox.Width);
                if (!lastEnemy.CollisionBox.Intersects(col.CollisionBox) && !lastEnemy.CollisionBox.Intersects(originalPos)) {
                    Interact();
                    lastEnemy = null;
                }
            }
            
            base.Update();
        }
    }
}
