using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainTD
{
    /// <summary>
    /// GrundLæggende Klasse for alle objekter i spillet der ikke er ui
    /// </summary>
    public abstract class GameObject
    {
        //Grafiske variabler
        protected Vector2 position;
        protected float rotation;
        protected float scale;
        protected float layer;
        protected String[] assets;
        protected Texture2D[] sprites;
        protected Color color = Color.White;
        protected float opacity;
        protected SpriteEffects spriteEffect;


        //Hvor meget man får når objektet Lootes
        protected int lootValue;

        //Variabler til placering af købte objekter
        public bool atSnapPoint = false;
        public bool lockedY = false;
        public bool lockedX = false;
        protected bool beingPlaced;
        
        //Løse logik variabler
        protected int updateCalls = 0;
        protected bool shouldRemove;
        protected TurnPoint[] turnPoints;

        //animations variabler
        protected float animationSpeed = 0;
        private float animationTime = 0;
        
        public Rectangle CollisionBox
        {
            get
            {
                return new Rectangle(
                    (int)(Position.X - spriteSize.X / 2),
                    (int)(Position.Y - spriteSize.Y / 2),
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

        public bool ShouldRemove { get => shouldRemove; set { shouldRemove = value; } }

        public Vector2 Position { get => position; set => position = value; }
        public Color Color { get => color; set => color = value; }
        public bool BeingPlaced { get => beingPlaced; set => beingPlaced = value; }
        public float Rotation { get => rotation; }
        public float Layer { get => layer; set => layer = value; }

        protected GameObject(Vector2 position, Texture2D[] sprites, SpriteEffects spriteEffect, int lootValue, float rotation, float animationSpeed)
        {
            this.position = position;
            this.sprites = sprites;
            this.spriteEffect = spriteEffect;
            this.lootValue = lootValue;
            this.animationSpeed = animationSpeed;
            this.rotation = rotation;
            this.opacity = 1;
            scale = 1;
            Layer = 0.6f;
        }

        protected GameObject(Vector2 position, string[] assets, float animationSpeed)
        {
            this.position = position;
            this.assets = assets;
            this.animationSpeed = animationSpeed;

            this.opacity = 1;

            Layer = 0.6f;

            scale = 1;
        }


        /// <summary>
        /// Generel funktion til kode der skal kaldes hver gang GameWorlds update bliver kaldt
        /// </summary>
        /// <param name="gameTime">GameWorlds GameTime</param>
        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// Metode der bliver kaldt i GameWorlds LoadContent. 
        /// Kun Relevant for objekter lavet inden første kald til Update
        /// </summary>
        /// <param name="content">GameWorlds content Manager</param>
        public abstract void LoadContent(ContentManager content);


        public virtual void OnCollision(GameObject other)
        {

        }

        /// <summary>
        /// Metode der bliver kaldt hvis et andet objekt kan plyndre det
        /// </summary>
        /// <returns>Hvor meget stål der bliver plyndret</returns>
        public abstract int lootFromObject();

        /// <summary>
        /// Metode til at tegne objeket når GameWorlds Draw metode bliver kaldt
        /// </summary>
        /// <param name="_spriteBatch">Gameworlds spriteBatch</param>
        public virtual void Draw(SpriteBatch _spriteBatch)
        {
            Vector2 origin = new Vector2(sprites[(int)animationTime].Width / 2, sprites[(int)animationTime].Height / 2);


            _spriteBatch.Draw(sprites[(int)animationTime], Position, null, Color * opacity, rotation, origin, scale, spriteEffect, Layer);


        }

        /// <summary>
        /// Metode til at finde hvor langt i den nuværende animations cyklus man er nået
        /// </summary>
        /// <param name="gameTime">Gameworlds gameTime</param>
        protected void Animate(GameTime gameTime)
        {
            animationTime += (float)gameTime.ElapsedGameTime.TotalSeconds * animationSpeed;
            if (animationTime > sprites.Length - 1)
            {
                animationTime = 0;
            }
        }

        public bool IsColliding(GameObject other)
        {
            if (this == other)
            {
                return false;
            }
            return CollisionBox.Intersects(other.CollisionBox);
        }

        /// <summary>
        /// Returnerer et nyt gameObject med de grundlæggende værdier af det nuværende
        /// </summary>
        /// <returns>et nyt gameobject af samme klasse som det nuværende</returns>
        public abstract GameObject getCopy();

        /// <summary>
        /// Hvis der er undtagelser fra de generelle regler for at placere et objekt
        /// </summary>
        /// <param name="go"> objektet der skal undersøges om der er undtagelser for</param>
        /// <returns></returns>
        protected abstract bool PlacementExceptions(GameObject go);

        /// <summary>
        /// Metode til at styre positionen og fastlåsningen af et objekt der manuelt skal placeres
        /// </summary>
        protected virtual void placement() {
            DetermineSnapPointLock();
            if (BeingPlaced)
            {
                if (lockedX && !lockedY)
                {
                    position = new Vector2(position.X, Mouse.GetState().Position.Y);
                }
                else if (!lockedX && lockedY)
                {
                    position = new Vector2(Mouse.GetState().Position.X, position.Y);
                }
                else if (!lockedX && !lockedY) {
                    position = Mouse.GetState().Position.ToVector2();
                }
                
                bool validPlacement = true;
                foreach (GameObject go in GameWorld.GetGameObjects)
                {
                    PlacementExceptions(go);
                    if (go != this && go.CollisionBox.Contains(position))
                    {
                        validPlacement = false;
                    }
                }
                foreach (UIElement ue in GameWorld.GetUIElements()) {
                    if (ue.CollisionBox.Contains(position)) {
                        validPlacement = false;
                    }
                }

                if (Mouse.GetState().LeftButton == ButtonState.Pressed && validPlacement)
                {
                    beingPlaced = false;
                    
                    

                    AudioManager.PlaySFX("cntPlace");
                }
            }
        }

        public virtual void Bought(GameObject obj)
        {
            position = obj.position;
            rotation = obj.rotation;
            scale = obj.scale;
            assets = obj.assets;
            sprites = obj.sprites;
            color = obj.color;
            spriteEffect = obj.spriteEffect;
            turnPoints = obj.turnPoints;
            lootValue = obj.lootValue;
            atSnapPoint = obj.atSnapPoint;
            
            

            shouldRemove = obj.shouldRemove;

            animationSpeed = obj.animationSpeed;
            animationTime = obj.animationTime;
    }

        /// <summary>
        /// Tjekker om der er en eller begge akser som bør fastlåses.
        /// </summary>
        protected virtual void DetermineSnapPointLock()
        {
            
        }

        /// <summary>
        /// Er en virtual metode af tekniske årsager
        /// </summary>
        /// <returns></returns>
        public virtual bool ConnectedToTurnPoint()
        {
            return true;
        }
    }
}
