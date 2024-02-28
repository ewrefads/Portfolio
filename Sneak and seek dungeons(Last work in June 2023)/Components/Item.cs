using Microsoft.VisualBasic.Devices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Mouse = Microsoft.Xna.Framework.Input.Mouse;

namespace Sneak_and_seek_dungeons.Components
{
    public enum ITEMTYPE { ANY, WEAPON, HELMET, CHESTPLATE, PANTS,POTION, SWORD, KEY }

    /// <summary>
    /// Item som et våben, potion eller andet
    /// Frederik
    /// </summary>
    internal class Item : Component, IInteractable
    {
        //en delegate som informere det itemslot der holder itemet at det bliver fjernet fx efter brug af en potion forsvinder den
        public delegate void RemoveDelegate();
        public RemoveDelegate removeEvent;

        //hvor mange brug et item har 
        private int uses;
        protected bool hasInfiniteUses;

        //bliver der trukket i itemet nuværende
        private bool isBeingDragged;

        //hvad for en slags item er det
        protected ITEMTYPE itemType;

        //er itemet i inventoriet
        private bool isInInventory;

        public bool IsBeingDragged { get => isBeingDragged; set => isBeingDragged = value; }
        public ITEMTYPE ItemType { get => itemType; set => itemType = value; }
        internal bool IsInInventory { get => isInInventory; set => isInInventory = value; }
        public int Uses { get => uses; set => uses = value; }

        public Item()
        {
            itemType = ITEMTYPE.ANY;
        }
        //item activated
        public virtual void Activate()
        {
            //maybe add cool effect here

            Uses--;

            if (Uses<=0 && !hasInfiniteUses)
            {
                removeEvent();
                GameWorld.Instance.DestroyedGameObjects.Add(GameObject);
            }
        }

        /// <summary>
        /// når spilleren trækker i itemet i inventoriet
        /// </summary>
        public void Drag()
        {
            Camera cam = ((GameWorld.Instance.FindObjectOfType<Camera>()).GameObject.GetComponent<Camera>() as Camera);
            Vector2 camOffset = new Vector2(cam.transform.Translation.X, cam.transform.Translation.Y);
            MouseState mouse = Mouse.GetState();
            Vector2 mousePos = new Vector2(mouse.X, mouse.Y);
            Vector2 adjustedMousePos = (mousePos - camOffset) / cam.zoom;
            GameObject.Transform.Position = adjustedMousePos;
        }

        public override void Start()
        {
            SpriteRenderer sr = GameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            sr.LayerDepth = 0.2f;
            if (itemType == ITEMTYPE.KEY) {
                Random rnd = new Random();
                int keyTexture = rnd.Next(4);
                switch (keyTexture) {
                    case (0):
                        sr.Sprite = GameWorld.Instance.Content.Load<Texture2D>("Basic Key");
                        break;
                    case (1):
                        sr.Sprite = GameWorld.Instance.Content.Load<Texture2D>("Skele Key");
                        break;
                    case (2):
                        sr.Sprite = GameWorld.Instance.Content.Load<Texture2D>("Double Key");
                        break;
                    case (3):
                        sr.Sprite = GameWorld.Instance.Content.Load<Texture2D>("Eye Key");
                        break;
                }
            }
            //sr.Scale = new Vector2(0.5f,0.5f);
        }

        public override void Update()
        {
            if(IsBeingDragged)
                Drag();

        }

        /// <summary>
        /// når itemet bliver interegeret med bliver det tilføjet til inventoriet som om man samler det op
        /// </summary>
        public void Interact()
        {
            Inventory.Instance.AddItemToInventory(this);
        }
    }
}
