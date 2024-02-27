using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Sneak_and_seek_dungeons.FactoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.Components
{
    internal class Button : Component
    {
        private Texture2D sprite;
        private Vector2 origin;
        private Color color = Color.Black;
        private float scale = 2.5f;
        private float layerDepth = 0.3f;
        private bool cPress = false;
        public delegate void buttonMethod();
        private buttonMethod onClick;

        internal bool isOnClick = true;

        public delegate void buttonMethodParam(int nmb);
        internal buttonMethodParam onClickParam;

        private int index;

        private SpriteRenderer sr;
        private Collider col;

        private string buttonText;

        private Vector2 offset;


        public Texture2D Sprite { get => sprite; set => sprite = value; }
        public Vector2 Origin { get => origin; set => origin = value; }
        public float Scale { get => scale; set => scale = value; }
        public float LayerDepth { get => layerDepth; set => layerDepth = value; }
        public Color Color { get => color; set => color = value; }
        public int Index { get => index; set => index = value; }
        public string ButtonText { get => buttonText; set => buttonText = value; }
        internal buttonMethod OnClick { get => onClick; set => onClick = value; }

        internal buttonMethodParam OnClickParam { get => onClickParam; set => onClickParam = value; }

        public bool CPress { get => cPress; set => cPress = value; }
        public Vector2 Offset { get => offset; set => offset = value; }

        public override void Awake()
        {
            SetSprite();
        }
        public override void Start()
        {

            layerDepth = 1;
            col = (Collider)GameObject.GetComponent<Collider>();

        }
        public override void Draw(SpriteBatch spriteBatch)
        {

            if (sr.Sprite != null)
            {
                Sprite = sr.Sprite;
                Origin = new Vector2(Sprite.Width / 2, Sprite.Height / 2);
                Vector2 textPos = GameObject.Transform.Position + Origin * scale + new Vector2(-4 * buttonText.Length, 0);
                //spriteBatch.Draw(sprite, new Rectangle((int)Origin.X, (int)Origin.Y, sprite.Width, sprite.Height), Color.White);
                spriteBatch.DrawString(GameWorld.Instance.defaultFont, buttonText, textPos, color, 0, Origin, scale, SpriteEffects.None, 0.9f);
            }
        }

        public void SetSprite()
        {
            sr = (SpriteRenderer)GameObject.GetComponent<SpriteRenderer>();
            sr.LayerDepth = 0.8f;

        }

        //public override void Update()
        //{
        //    MouseState mouse = Mouse.GetState();
        //    Camera camera = GameWorld.Instance.Camera;
        //    Vector2 mousePos = new Vector2(mouse.X+camera.CamOffset.X, mouse.Y + camera.CamOffset.Y);
        //    if (col == null)
        //    {
        //        col = (Collider)GameObject.GetComponent<Collider>();
        //    }
        //    //når man klikker på knappen
        //    if (col.CollisionBox.Contains(mousePos) && mouse.LeftButton == ButtonState.Pressed && !CPress)
        //    {
        //        CPress = true;
        //        sr.setTempColor(Color.Gray, 0.1f);

        //        if (!isOnClick)
        //        {
        //            onClickParam(index - 1);
        //        }
        //        else
        //        {
        //            onClick();
        //        }


        //    }
        //    else if (col.CollisionBox.Contains(mousePos) && mouse.LeftButton == ButtonState.Pressed && CPress)
        //    {

        //        cPress = true;
        //        sr.setTempColor(Color.Gray, 0.1f);
        //    }

        //    //når man ikke klikker på knappen :(
        //    else if (CPress && mouse.LeftButton == ButtonState.Released)
        //    {
        //        CPress = false;
        //    }

        //}
        public override void Update()
        {
            GameObject.Transform.Position = new Vector2(GameWorld.Instance.Player.Transform.Position.X + Offset.X, GameWorld.Instance.Player.Transform.Position.Y + Offset.Y);

            SpriteRenderer sr = (SpriteRenderer)GameObject.GetComponent<SpriteRenderer>();
            if (sr.Color != Color.AntiqueWhite)
            {
                sr.Color = Color.AntiqueWhite;
            }
            Camera camera = GameWorld.Instance.Camera;
            MouseState mouse = Mouse.GetState();
            Vector2 mousePos = new Vector2(mouse.X, mouse.Y);
            Camera cam = ((GameWorld.Instance.Camera));
            Vector2 camOffset = new Vector2(cam.transform.Translation.X, cam.transform.Translation.Y);

            Vector2 adjustedMousePos = (mousePos - camOffset) / cam.zoom;
            //+ camera.CamOffset.X
            Collider collider = (Collider)GameObject.GetComponent<Collider>();
            if (collider.CollisionBox.Contains(adjustedMousePos))
            {
                sr.Color = Color.DarkSalmon;
            }
            else
            {
                sr.Color = Color.AntiqueWhite;
            }
            if (collider.CollisionBox.Contains(adjustedMousePos) && mouse.LeftButton == ButtonState.Pressed && !cPress)
            {
                cPress = true;
                sr.Color = Color.Brown;
                onClick();

            }

            else if (cPress && mouse.LeftButton == ButtonState.Released)
            {
                cPress = false;
            }
            else if (cPress)
            {
                sr.Color = Color.Brown;
            }
            base.Update();
        }
    }
}
