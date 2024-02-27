using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace TrainTD
{

    /// <summary>
    /// Tower er den klasse som alle towers nedarver fra. Det var meningen der skulle være flere end 1 tårn men det nåede vi ikke :(
    /// </summary>
    public class Tower : GameObject
    {
        //rangen som tårnet kan finde et target indenfor
        private float range;
        //tårnets helbred som holder styr på hvornår tårnet skal disbles
        private int health;
        //hvor meget skade et skud fra tårnet gør
        private int damage;
        //hvor hurtigt flyver de projektiler som tårnet skyder
        private float bulletSpeed;
        //angrebshastigheden bestemmer hvor hurtigt et nyt projektil kan blive spawned
        private float attackSpeed;
        //Holder styr på hvor lang tid der gået siden sidste skud
        private float attackSpeedTimer;
        //hvor lang tid tårnet skal bruge for at stoppe med at være stunned
        private float wakeUpTime;
        //holder styr på om tårnet er stunned
        private bool disabled;

        //Hvem skyder tårnet efter
        private GameObject target;
        //Hvem ejer tårnet
        private Player owner;

        //skal tårnets range cirkel tegnes i drawmetoden
        private bool showRange;
        //er tårnet placeret på banen
        private bool isPlaced;
        //er tårnet stadig i shoppen
        public bool inShop;
        //er tårnet clicked på
        private bool towerSelected;

        //spriten for tårnets range cirkel
        private Texture2D rangeSprite;
        //spriten for detprojektil tårnet skyder med
        private Texture2D projectileSprite;
        //tårnets animations sprites
        private Texture2D[] attackAnimation = new Texture2D[3] {null,null,null};
        //sprites til de knapper som tårnet har
        private Texture2D buttonSprite;
        //fonten som tårnets knapper bruger
        private SpriteFont text;

        //knappen til at sælge tårnet
        private TowerButton sell;
        //knappen til at opgradere tårnet
        private TowerButton upgrade;

        //Positionerne for de knapper som tårnet bruger
        private Vector2[] buttonPositions = new Vector2[2] {Vector2.Zero,Vector2.Zero};

        protected List<float> upgradeList = new List<float>();

        public enum upgradeType { none, range, damage, attackSpeed, custom}
        public List<upgradeType> upgradeListType = new List<upgradeType>();

        private Descriptionbox upgradeInfo;

        protected int upgradeCount;

        
        public bool IsPlaced
        {
            get { return isPlaced; }
        }
        
        //laver et nyt tårn og giver det nogen stats
        public Tower(Player owner, Vector2 position, Texture2D[] s) : base(position,s,SpriteEffects.None,0,0,1)
        {
            attackSpeed = 1f;
            attackSpeedTimer = 0;
            this.owner = owner;
            disabled = false;

            scale = 2;
            range = 300;

            bulletSpeed = 10f;
            damage = 5;

            inShop = false;

            
        }

        //Hvis man laver et tårn som kun skal vises på en knap eller man skal have en kopi
        public Tower(Vector2 position, Texture2D[] sprites, SpriteEffects spriteEffect, int lootValue, float animationSpeed) : base(position, sprites, spriteEffect, lootValue, 0, animationSpeed)
        {
        }

        //returnere en kopi af et tårn
        public override GameObject getCopy()
        {
            return new Tower(position, sprites, spriteEffect, lootValue, 1);
        }

        /// <summary>
        /// Angriber ved at finde et target indenfor tårnets range og spawner et projektil som flyver i den retning
        /// </summary>
        private void Attack()
        {
            //angrebs animation afspilles
            if (sprites!=attackAnimation) {
                sprites = attackAnimation;
            }


            
            //afspiller en skyde lyd
            AudioManager.PlaySFX("twrFire1");

            //retningen som vores projektil skal bevæge sig i
            Vector2 direction = FindTargetInRange();

            //laver et nyt instans af projectile og spawner den i gameworld
            Projectile projectile = new Projectile(damage, direction, bulletSpeed,new Texture2D[] { projectileSprite}, new Vector2(0,spriteSize.Y/2)+position);
            GameWorld.InstantiateGameObject(projectile);
        }

        /// <summary>
        /// Tårnet taget skade og hvis det har 0 eller mindre liv så bliver det disabled
        /// </summary>
        /// <param name="damage">Mængden af skade som tårnet tager</param>
        public void TakeDamage(int damage)
        {
            health -= damage;
            if (health<=0)
            {
                disabled = true;
                AudioManager.PlaySFX("twrDisabled");
            }
        }

        /// <summary>
        /// Finder et target indenfor tårnets rækkevide
        /// </summary>
        /// <returns>returnere en vector2 som hviser retningen fra tårnet til target</returns>
        private Vector2 FindTargetInRange()
        {
            //får en liste af alle spilobjekter i vores verden
            List<GameObject> gameObjects = GameWorld.GetGameObjects;
            //laver en float som holder styr på hvad tårn er tættest på
            float length=99999999;

            //retningsvectoren som vi ender med at returnere
            Vector2 direction = Vector2.Zero;

            //looper igennem alle spil objekter
            foreach (GameObject obj in gameObjects)
            {
                //hvis det en del af et tog så checker vi distancen
                if (obj is TrainCarriage && obj is not Locomotive) {
                    float l = (position - obj.Position).Length();

                    //hvis distance er mindre end den tidligere og er indenfor tårnets range
                    if (l < length && l < range)
                    {
                        //den nye distance
                        length = obj.Position.Length();
                        //retningsvektoren hen til target
                        direction = obj.Position - position;
                        //target bliver erklæret
                        target = obj;
                    }
                }
            }


            //returnere retningen
            return direction;
        }

        /// <summary>
        /// Tårnet rotere så det altid kigger på det target som det skyder på
        /// </summary>
        void rotateTowerTowardsTarget()
        {
            //Vector2 direction = target.Position - position;

            //finder vinklen mellem to vektore
            float angle = MathF.Atan2(position.Y-target.Position.Y,position.X-target.Position.X);

            //sætter den vinkel til tårnets rotation
            //1.55f er et offset da tårnene kigger ned af by default
            rotation = angle +1.55f;
        }

        /// <summary>
        /// Når tårnet bliver købt fra en knap så kopier vi værdierne fra det tårn over i det her tårn
        /// </summary>
        /// <param name="obj">det tårn som er på knappen</param>
        public override void Bought(GameObject obj)
        {
            
            
            //sætter tårnets range til at hvise 
            showRange = true;
            //gør tårnets range gennemsigtig
            opacity = 0.7f;

            //assigner variabler

            Tower tower = ((Tower)obj);

            range = tower.range;
            health = tower.health;
            damage = tower.damage;
            attackSpeed = tower.attackSpeed;
            wakeUpTime = tower.wakeUpTime;
            owner = tower.owner;

            rangeSprite = tower.rangeSprite;
            projectileSprite = tower.projectileSprite;
            attackAnimation = tower.attackAnimation;
            buttonSprite = tower.buttonSprite;

            bulletSpeed = tower.bulletSpeed;

            text = tower.text;

            isPlaced = true;

            //laver en sell knap og en opgrade knap
            if (!inShop)
            {
                //instansiere kanpper
                sell = new TowerButton(this, TowerButton.type.sell, owner, new Texture2D[] { buttonSprite }, Color.White, Color.Blue, Color.Red, Color.Green, buttonPositions[0], 0);
                upgrade = new TowerButton(this, TowerButton.type.upgrade, owner, new Texture2D[] { buttonSprite }, Color.White, Color.Blue, Color.Red, Color.Green, buttonPositions[1], 0);

                //giver begge knapper en spritefont de kan bruge til at skrive med
                sell.ButtonText = text;
                upgrade.ButtonText = text;

                //ændre knappernes text til hvad de gør
                sell.Text = "Sell";
                upgrade.Text = "Upgrade";

                //indsætter knapperne i vores spil verden
                GameWorld.InstantiateUIElement(sell);
                GameWorld.InstantiateUIElement(upgrade);


                //upgrades
                upgradeList.Add(1);
                upgradeList.Add(100);
                upgradeList.Add(-0.1f);
                upgradeList.Add(-0.2f);
                upgradeList.Add(100);
                upgradeList.Add(3);
                upgradeList.Add(-0.3f);

                upgradeListType.Add(upgradeType.damage);
                upgradeListType.Add(upgradeType.range);
                upgradeListType.Add(upgradeType.attackSpeed);
                upgradeListType.Add(upgradeType.attackSpeed);
                upgradeListType.Add(upgradeType.range);
                upgradeListType.Add(upgradeType.damage);
                upgradeListType.Add(upgradeType.attackSpeed);

                //upgradeinfo setup
                upgradeInfo = new Descriptionbox(owner, new Texture2D[] { buttonSprite }, Color.White, Color.White, Color.White, Color.White, buttonPositions[0], 0);
                upgradeInfo.font = text;

                List<string> upgradeStrings = new List<string>();

                for (int i = 0; i < upgradeList.Count; i++)
                {
                    upgradeStrings.Add(upgradeListType[i].ToString()+" +" + upgradeList[i].ToString());
                }

                upgradeInfo.text = upgradeStrings.ToArray();
                

                GameWorld.InstantiateUIElement(upgradeInfo);
            }

            base.Bought(obj);
        }
       
        /// <summary>
        /// Loader sprites som viser tårnet
        /// </summary>
        /// <param name="content"></param>
        public override void LoadContent(ContentManager content)
        {
            attackAnimation[0] = content.Load<Texture2D>("tower1/ta01");
            attackAnimation[1] = content.Load<Texture2D>("tower1/ta02");
            attackAnimation[2] = content.Load<Texture2D>("tower1/ts01");
            
            //loader sprites som tårnet også bruger
            loadSubSprites(content);

            
        }

        /// <summary>
        /// loader andre sprites som tårnet også bruger
        /// </summary>
        /// <param name="content"></param>
        public virtual void loadSubSprites(ContentManager content)
        {
            rangeSprite = content.Load<Texture2D>("range");

            projectileSprite = content.Load<Texture2D>("smoke1");

            buttonSprite = content.Load<Texture2D>("tempButton");

            text = content.Load<SpriteFont>("Main");

        }


        public override int lootFromObject()
        {
            throw new NotImplementedException();
        }

        //kaldes hver gang der er en ny itteration i vores spil
        public override void Update(GameTime gameTime)
        {
            //Får musens position i en variabel
            Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            MouseState mouse = Mouse.GetState();

            //placere tårnet hvis det ikke placeret endu
            placement();

            //finder et target i range
            target = null;
            FindTargetInRange();

            //hvis vi kan skyd og tårnet ikke er igang med at blive placeret
            if (gameTime.TotalGameTime.TotalSeconds>attackSpeed + attackSpeedTimer && !disabled && !beingPlaced && target!= null)
            {
                //spawn bullet
                Attack();

                //reseter attack timeren
                attackSpeedTimer = (float)gameTime.TotalGameTime.TotalSeconds;
            }

            //så længe vi har et target så rotere tårnet mod det target
            if (target!=null)
            {
                rotateTowerTowardsTarget();
            }

            //hvis vi ikke er i gang med at placere tårnet
            if (beingPlaced == false)
            {
                //hvis tårnet ikke er valgt så gem rangen
                if (!towerSelected) {
                    showRange = false;
                }
                
                //Hvis vi trykker på tårnet så bliver det valgt
                if (CollisionBox.Contains(mouse.Position) && Mouse.GetState().LeftButton == ButtonState.Pressed && isPlaced)
                {
                    TowerSelected();
                }
            }

            
            //hvis tårnet er valgt og vi trykker et sted
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && towerSelected)
            {
                //så længe spilleren ikke trykkede på tårnet eller en af knapperne så bliver tårnet deselected
                if (!sell.CollisionBox.Contains(mousePosition)&&!upgrade.CollisionBox.Contains(mousePosition)&&!CollisionBox.Contains(mousePosition))
                {
                    TowerDeselect();
                }

                /*float distance = (float)(Math.Pow(mousePosition.X - position.X, 2) + Math.Pow(mousePosition.Y-position.Y,2)) ;

                if (distance> 132896 || distance<-132896)
                {
                    TowerDeselect();
                }*/
            }

        }
        /// <summary>
        /// viser tårnets range og giver spilleren muligheden for at opgradere tårnet eller sælge
        /// </summary>
        public void TowerSelected()
        {
            float xOffset = 150;

            sell.Position = position - sell.spriteSize/2 - new Vector2( xOffset,0);
            upgrade.Position = position - upgrade.spriteSize/2 + new Vector2(xOffset, 0);

            showRange = true;

            towerSelected = true;

            sell.Show();
            upgrade.Show();
        }

        /// <summary>
        /// gemmer knapperne og rangen væk
        /// </summary>
        public void TowerDeselect()
        {
            sell.Hide();
            upgrade.Hide();

            showRange = false;

            towerSelected = false;
        }

        /// <summary>
        /// opgradere tårnet ved at give det bedre stats
        /// </summary>
        public void UpgradeTower()
        {

            if (upgradeCount<upgradeList.Count) {
                if (upgradeListType[upgradeCount] is upgradeType.damage)
                {
                    damage += (int)upgradeList[upgradeCount];
                }
                else if (upgradeListType[upgradeCount] is upgradeType.range)
                {
                    range += (int)upgradeList[upgradeCount];
                }
                else if (upgradeListType[upgradeCount] is upgradeType.attackSpeed)
                {
                    if (attackSpeed > 0.3f)
                    {
                        attackSpeed += upgradeList[upgradeCount];
                    }
                }
            }

            upgradeCount++;
            /*
            //sætter en grænse på 0.3 sekunder som bestemmer hvor hurtigt tårnet kan maksimalt skyde
            if (attackSpeed>0.3f) {
                attackSpeed -= 0.1f;
            }
            damage += 1;
            health += 1;
            */
        }

        public void IsUpgradingTower()
        {
            //fafafwafa
            upgradeInfo.Show();
            upgradeInfo.Position = position - buttonPositions[1] + new Vector2(25, 100);
            upgradeInfo.towerLevel = upgradeCount;
        }

        public void IsNotUpgradingTower()
        {
            upgradeInfo.Hide();
        }

        /// <summary>
        /// Sletter tårnet og de knapper som høre til tårnet
        /// </summary>
        public void SellTower()
        {
            sell.ShouldRemove = true;
            upgrade.ShouldRemove = true;
            shouldRemove = true;
        }

        /// <summary>
        /// Returnere true eller false som bestemmer om et tårn kan placeres ovenpå dette tårn
        /// </summary>
        /// <param name="go"></param>
        /// <returns>returnere false fordi objekter ikke kan placeres ovenpå tårnet</returns>
        protected override bool PlacementExceptions(GameObject go)
        {
            //man kan ikke sætte et tårn ovenpå tårnet
            return false;
        }

        
        /// <summary>
        /// tegner rangen på tårnet hvis vi skal vise rangen
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public void drawRange(SpriteBatch _spriteBatch)
        {
            if (showRange) {
                Vector2 origin = new Vector2(rangeSprite.Width / 2, rangeSprite.Height / 2);
                _spriteBatch.Draw(rangeSprite, Position, null, Color * 0.5f, Rotation, origin, range * 2 / 2048, spriteEffect, 0);
            }
        }

        
    }
}
