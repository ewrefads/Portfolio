using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TrainTD
{
    /// <summary>
    /// Towerbutton er et ui element som popper op når et tårn bliver klikket på
    /// Towerbutton kan enten give muligheden for at sælge tårnet eller opgradere det
    /// </summary>
    internal class TowerButton : UIElement
    {
        //opgradere eller sælger denne knap?
        public enum type { none, sell, upgrade }
        public type buttonType;

        //Det tårn der blevet trykket på
        public Tower tower;
        
        //knappens textfont
        private SpriteFont buttonText;
        //den tekst som står på knappen
        private string text;

        public SpriteFont ButtonText
        {
            set { buttonText = value; }
        }

        public string Text
        {
            set { text = value; }
        }

        /// <summary>
        /// sætter knappens start vædier op
        /// </summary>
        /// <param name="tower"></param>
        /// <param name="t"></param>
        /// <param name="activePlayer"></param>
        /// <param name="sprites"></param>
        /// <param name="activeColor"></param>
        /// <param name="hoverColor"></param>
        /// <param name="clickColor"></param>
        /// <param name="inActiveColor"></param>
        /// <param name="position"></param>
        /// <param name="animationTime"></param>
        public TowerButton(Tower tower,type t ,Player activePlayer, Texture2D[] sprites, Color activeColor, Color hoverColor, Color clickColor, Color inActiveColor, Vector2 position, float animationTime) : base(activePlayer, sprites, activeColor, hoverColor, clickColor, inActiveColor, position, animationTime)
        {
            //reference til det tårn som er blevet klikket på
            this.tower = tower;
            //er det en sælg eller opgrader knap
            buttonType = t;
            //starter med at være usynlig
            opacity = 0;
        }

        public override void Draw(SpriteBatch _spriteBatch)
        {
            //tegner teksten som står på knappen i midten af knappen
            Vector2 textPos = new Vector2(-7*text.Length,-25)+spriteSize/2+position;
            _spriteBatch.DrawString(buttonText,text,textPos,Color.Wheat*opacity,0f, Vector2.Zero, 1, SpriteEffects.None, 1);

            //tegner knappen
            base.Draw(_spriteBatch);
            

        }

        public override void Update(GameTime gameTime)
        {
            //checker spillerens input
            HandleInput();

            //hvis knappen er blevet aktiveret så får den en ny farve
            if (idleActive)
            {
                currentColor = clickColor;
            }

            if (buttonType is type.upgrade && hover)
            {
                tower.IsUpgradingTower();
            }
            else if(buttonType is type.upgrade && !hover)
            {
                tower.IsNotUpgradingTower();
            }

            
        }

        protected override void HandleInput()
        {
            //hvis vi ikke har klikket knappen denne iteration så knappen den klikkes igen
            if(Mouse.GetState().LeftButton != ButtonState.Pressed)
            {
                idleActive = false;
            }

            //kun hvis tårnet er placeret kan knappen tage imod input
            if (tower.IsPlaced) {
                base.HandleInput();
            }
        }

        /// <summary>
        /// når spilleren trykker på knappen
        /// </summary>
        protected override void OnCLick()
        {
            base.OnCLick();

            //hvis spilleren kan se knappen
            if (opacity >=1) {
                //vi trykke på knappen
                active = true;

                //checker knap typen
                //sell
                if (buttonType is type.sell)
                {
                    tower.SellTower();
                }
                //upgrade
                else if (buttonType is type.upgrade)
                {
                    tower.UpgradeTower();
                }
            }
        }

        //viser knappen ved at justere opacity
        public void Show()
        {
            opacity = 1;
            active = true;
        }

        //gemmer knappen ved at justere opacity
        public void Hide()
        {
            opacity = 0;
            active = false;
        }
    }
}
