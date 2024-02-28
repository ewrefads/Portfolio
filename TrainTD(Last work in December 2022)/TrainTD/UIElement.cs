using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TrainTD
{
    /// <summary>
    /// Generel klasse for ui Elementer
    /// </summary>
    public abstract class UIElement
    {
        protected bool hover;
        protected bool active;
        protected bool idleActive;
        protected Player activePlayer;
        protected string[] assets;
        protected Texture2D[] sprites;
        protected Color currentColor;
        protected Color activeColor;
        protected Color hoverColor;
        protected Color clickColor;
        protected Color inActiveColor;
        protected Vector2 position;
        protected float animationTime = 0;
        protected float scale;
        protected bool shouldRemove;

        protected float opacity;

        public Vector2 Position
        {
            set{ position = value; }
        }

        public bool ShouldRemove
        {
            get { return shouldRemove; }
            set { shouldRemove = value; }
        }

        public bool Active
        {
            set { active = value; }
        }

        protected UIElement(Player activePlayer, Texture2D[] sprites, Color activeColor, Color hoverColor, Color clickColor, Color inActiveColor, Vector2 position, float animationTime)
        {
            this.activePlayer = activePlayer;
            this.sprites = sprites;
            this.activeColor = activeColor;
            this.hoverColor = hoverColor;
            this.clickColor = clickColor;
            this.inActiveColor = inActiveColor;
            this.currentColor = activeColor;
            this.position = position;
            this.animationTime = animationTime;
            scale = 0.5f;

            opacity = 1;
        }

        public virtual void Update(GameTime gameTime) {
            HandleInput();
            if (idleActive) {
                currentColor = clickColor;
            }
        }
        public virtual void Draw(SpriteBatch _spriteBatch) {
            if (currentColor == hoverColor) {
                hoverColor = hoverColor;
            }
            //_spriteBatch.Draw(sprites[0], new Rectangle((int)position.X, (int)position.Y, (int)spriteSize.X, (int)spriteSize.Y), currentColor);
            Vector2 origin = new Vector2(sprites[(int)animationTime].Width / 256, sprites[(int)animationTime].Height / 32);

            _spriteBatch.Draw(sprites[(int)animationTime], position, null, currentColor * opacity, 0, origin, scale, SpriteEffects.None, 0.7f);
        }

        protected virtual void HandleInput() {
            Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            MouseState mouse = Mouse.GetState();
            if (CollisionBox.Contains(mouse.Position) && active)
            {
                hover = true;
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && !idleActive) {
                    
                    idleActive = true;
                    OnCLick();

                }
                if (active)
                {
                    currentColor = hoverColor;
                }
                else {
                    currentColor = inActiveColor;
                }
            }
            else {
                hover = false;
                if (active)
                {
                    currentColor = activeColor;
                }
                else {
                    currentColor = inActiveColor;
                }
            }
        }

        protected virtual void OnCLick() {

            AudioManager.PlaySFX("click");

        }

        public Rectangle CollisionBox
        {
            get
            {
                return new Rectangle(
                    (int)(position.X),
                    (int)(position.Y),
                    (int)spriteSize.X,
                    (int)spriteSize.Y

                    );
            }
        }

        public Vector2 spriteSize
        {
            get
            {
                return new Vector2(sprites[(int)animationTime].Width * scale, sprites[(int)animationTime].Height * scale);

            }
        }

        public bool IdleActive { get => idleActive; set => idleActive = value; }
    }
}
