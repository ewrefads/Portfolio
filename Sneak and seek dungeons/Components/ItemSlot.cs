using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Sneak_and_seek_dungeons.CommandPattern;
using Sneak_and_seek_dungeons.ObserverPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.Components
{
    /// <summary>
    /// Itemslot er en plads i spillerens inventory som kan rumme 1 item
    /// Frederik
    /// </summary>
    internal class ItemSlot : Component, IGameListner
    {
        private Item item;

        private List<ITEMTYPE> acceptableItems = new List<ITEMTYPE>();


        internal Item Item { get => item; set => item = value; }
        public List<ITEMTYPE> AcceptableItems { get => acceptableItems; set => acceptableItems = value; }

        public ItemSlot(ITEMTYPE type)
        {
            acceptableItems.Add(type);
        }

        //tilføjer itemslotted til spillerens ui liste
        public override void Awake()
        {
            ((GameWorld.Instance.FindObjectOfType<Player>()).GameObject.GetComponent<Player>() as Player).PlayerUI.Add(GameObject);
        }


        public override void Start()
        {
            SpriteRenderer sr = GameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            sr.SetSprite("PlaceHolder sprites/placeholderItem");
            sr.Scale = new Vector2(2f,2f);
            sr.LayerDepth = 0.9f;

            (GameObject.GetComponent<Collider>() as Collider).ActiveCollider = false;

            GameObject.Transform.Position -= (GameWorld.ScreenSize / 2) - (new Vector2(GameWorld.ScreenSize.X/3, GameWorld.ScreenSize.X / 8));

            Camera cam = ((GameWorld.Instance.FindObjectOfType<Camera>()).GameObject.GetComponent<Camera>() as Camera);
            GameObject.Transform.Position -= cam.CamOffset;

            if (Item != null)
            {
                item.GameObject.Transform.Position = GameObject.Transform.Position;
            }

        }
        /// <summary>
        /// Checker om spilleren klikker på dette itemslot
        /// </summary>
        public override void Update()
        {

            InputHandler.Instance.Execute(this);
            if (item != null) {
                SpriteRenderer sr = (SpriteRenderer)item.GameObject.GetComponent<SpriteRenderer>();
                if (sr.LayerDepth != 1) {
                    sr.LayerDepth = 1;
                }
                item.GameObject.Transform.Position = GameObject.Transform.Position;
            }
        }

        //fjerne itemet fra itemslottet
        public void RemoveItem()
        {
            item.removeEvent -= RemoveItem;
            item = null;
            
        }

        /// <summary>
        /// når spilleren klikker på itemslottet
        /// </summary>
        public void ItemClicked()
        {
            ItemSlot t = this;
            //spilleren trækker ikke rundet på et item i forvejen
            if (Inventory.Instance.DraggedItem==null) {
                if (item==null) {
                    //error sound
                }
                else
                {
                    Inventory.Instance.DraggedItem = item;
                    ItemPickedUp(item);
                    RemoveItem();
                }
                
            }
            //spilleren trækker rundt på et item i forvejen
            else
            {
                
                if (acceptableItems[0] == ITEMTYPE.ANY || acceptableItems.Contains(Inventory.Instance.DraggedItem.ItemType)) {
                    if (item==null) {

                        item = Inventory.Instance.DraggedItem;
                        ItemDropped();
                    }
                    else
                    {
                        Item tmp = item;
                        item = Inventory.Instance.DraggedItem;
                        ItemDropped();

                        Inventory.Instance.DraggedItem = tmp;

                        
                        ItemPickedUp(tmp);

                    }
                }
            }
        }
        /// <summary>
        /// når spilleren dropper et item i pladsen
        /// </summary>
        public virtual void ItemDropped()
        {
            

            Inventory.Instance.DraggedItem = null;
            item.IsBeingDragged = false;
            item.GameObject.Transform.Position = GameObject.Transform.Position;
            item.removeEvent += RemoveItem;
        }

        /// <summary>
        /// når spilleren trækker itemet væk fra pladsen
        /// </summary>
        /// <param name="it"></param>
        public virtual void ItemPickedUp(Item it)
        {
            
            it.IsBeingDragged = true;
            it.removeEvent -= RemoveItem;
            it = null;
        }

        //notifier når der bliver clicked på itemslotted
        public void Notify(GameEvent gameEvent)
        {
            if (gameEvent is ClickEvent)
            {
                ItemClicked();
            }
        }
    }
}
