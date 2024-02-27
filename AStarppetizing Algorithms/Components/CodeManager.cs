using AStarppetizing_Algorithms.FactoryPattern;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;

namespace AStarppetizing_Algorithms.Components
{
    public class CodeManager:Component
    {
        private static CodeManager instance;

       
        private List<CodeBlock> codeLog = new List<CodeBlock>();
        private bool displayMode = false;
        private bool finishedRun = false;
        private float elapsedTime = 0;
        private float delayTime = 0.5f;

        //Liste over de kodeblokke der er blevet tilføjet
        private List<CodeBlock> codeToRun = new List<CodeBlock>();

        //kø for når koden i kodeblokkene skal udføres
        private Queue<CodeBlock> queuedCode = new Queue<CodeBlock>();

        //Variabler til at holde styr på forskellige aspekter af a* mellem de forskellige metoder
        private Dictionary<Vector2, Vector2> parents = new Dictionary<Vector2, Vector2>();
        private Dictionary<Vector2, int> closedList = new Dictionary<Vector2, int>();
        private GameObject startPoint;
        private Vector2 startPointPos;
        private GameObject endPoint;
        private Vector2 endPointPos;
        private Tile[,] tiles;
        int addedToOpenList = 0;
        int addedToClosedList = 0;
        private Dictionary<Vector2, int> openList = new Dictionary<Vector2, int>();
        private Dictionary<Vector2, int> openListF = new Dictionary<Vector2, int>();
        private GameObject cTile;
        private Vector2 cTilePos;
        private Queue<GameObject> neighbours = new Queue<GameObject>();
        private Dictionary<GameObject, Vector2> neighboursReference = new Dictionary<GameObject, Vector2>();
        private Queue<CodeBlock> queuedCodeTemp;
        private List<CodeBlock> tempCodeToRun = new List<CodeBlock>();
        private GameObject cNeighbour;
        private CodeBlock checKNeighboursTemp;
        private int currentIndex;
        private int cF;
        private int cG;
        private int cH;
        private bool onOpenList = false;
        private bool invalidTile = false;

        //Den vej som der ender med at blive brugt
        private List<GameObject> path = new List<GameObject>();
        private CodeManager()
        {
        }

        public List<CodeBlock> CodeToRun { get => codeToRun;}
        public bool DisplayMode { get => displayMode; set => displayMode = value; }
        public static CodeManager Instance { get {
                if (instance == null) {
                    instance = new CodeManager();
                }
                return instance;
            } 
        }

        public bool FinishedRun { get => finishedRun; set => finishedRun = value; }
        public Dictionary<Vector2, int> ClosedList { get => closedList;}
        public Dictionary<Vector2, int> OpenList { get => openList;}
        public List<GameObject> Path { get => path;}
        public Tile[,] Tiles { get => tiles;}


        /// <summary>
        /// Kode der udfører de tilføjede kodeblokke medmindre displaymode er slået til. Den opsætte desuden lokat et array med tiles og start og slut position
        /// </summary>
        public void RunCode() {
            finishedRun = false;
            queuedCode = new Queue<CodeBlock>(codeToRun);
            Tile[,] level = GameWorld.Instance.Levels[GameWorld.Instance.CurrenLevel].t;
            tiles = level;
            GameObject player = GameWorld.Instance.Player.GameObject;
            bool foundStartPoint = false;
            bool foundEndPoint = false;
            for (int i = 0; i < Tiles.GetUpperBound(0); i++)
            {
                for (int j = 0; j < Tiles.GetUpperBound(1); j++)
                {
                    Tile t = Tiles[i, j];
                    Collider c = (Collider)t.GameObject.GetComponent<Collider>();
                    if (t.GetType() == typeof(Ingredient))
                    {
                        endPoint = t.GameObject;
                        endPointPos = new Vector2(i, j);
                        foundEndPoint = true;
                    }
                    if (c.CollisionBox.Contains(player.Transform.Position))
                    {
                        startPoint = t.GameObject;
                        startPointPos = new Vector2(i, j);
                        foundStartPoint = true;
                    }
                }
                if (foundStartPoint && foundEndPoint)
                {
                    break;
                }
            }
            foreach (Tile t in level)
            {

                Collider c = (Collider)t.GameObject.GetComponent<Collider>();
                if (t.GetType() == typeof(Ingredient))
                {
                    endPoint = t.GameObject;
                }
                if (c.CollisionBox.Contains(player.Transform.Position))
                {
                    startPoint = t.GameObject;
                }
            }
            parents.Clear();
            ClosedList.Clear();
            OpenList.Clear();
            openListF.Clear();
            Path.Clear();
            if (!DisplayMode)
            {
                while (queuedCode.Count > 0)
                {
                    CodeBlock cCode = queuedCode.Dequeue();
                    codeLog.Add(cCode);
                    currentIndex = codeToRun.IndexOf(cCode);
                    cCode.Method();
                }
            }
        }

        /// <summary>
        /// Udfører koden hvis displaymode er slået til
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {

            if (queuedCode.Count > 0 && elapsedTime == 0)
            {
                CodeBlock cCode = queuedCode.Dequeue();
                
                currentIndex = codeToRun.IndexOf(cCode);
                cCode.Method();
                elapsedTime += GameWorld.DeltaTime;
            }
            else if (queuedCode.Count > 0)
            {
                elapsedTime += GameWorld.DeltaTime;
                if (elapsedTime >= delayTime)
                {
                    elapsedTime = 0;
                }
            }
            else if (queuedCode.Count == 0 && !finishedRun)
            {
                finishedRun = true;
            }
            
            base.Update(gameTime);
        }


        /// <summary>
        /// Placerer kodeblokkene i en liste
        /// </summary>
        private void SetPositions()
        {
            int currentY = 40;
            for (int i = 0; i < codeToRun.Count; i++) {
                Vector2 pos = new Vector2(1575, currentY);
                codeToRun[i].GameObject.Transform.Position = pos;
                SpriteRenderer sr = (SpriteRenderer)codeToRun[i].GameObject.GetComponent<SpriteRenderer>();
                sr.LayerDepth = 0.5f;
                Texture2D sprite = sr.Sprite;
                currentY += (int)(sprite.Height * sr.Scale);
            }
        }

        /// <summary>
        /// Tilføjer nogle tomme kodeblokke mellem de eksisterende som kan erstattes af den nye kodeblok spilleren vil tilføje
        /// </summary>
        public void prepareForNewCodeBlock() {
            GameObject box = CodeBlockFactory.Instance.Create(CODEBLOCKTYPES.empty);
            SpriteRenderer sr = (SpriteRenderer)box.GetComponent<SpriteRenderer>();
            sr.SetSprite("greybox");
            GameWorld.Instance.NewGameObjects.Add(box);
            CodeBlock c = (CodeBlock)box.GetComponent<CodeBlock>();
            c.index = 0;
            List<CodeBlock> temp = new List<CodeBlock>
            {
                c
            };
            for (int i = 0; i < codeToRun.Count; i++) {
                temp.Add(codeToRun[i]);
                codeToRun[i].index = temp.IndexOf(codeToRun[i]);
                box = CodeBlockFactory.Instance.Create(CODEBLOCKTYPES.empty);
                sr = (SpriteRenderer)box.GetComponent<SpriteRenderer>();
                sr.SetSprite("greybox");
                GameWorld.Instance.NewGameObjects.Add(box);
                c = (CodeBlock)box.GetComponent<CodeBlock>();
                temp.Add(c);
                c.index = temp.IndexOf(c);
                Button but = (Button)codeToRun[i].GameObject.GetComponent<Button>();
                c = (CodeBlock)codeToRun[i].GameObject.GetComponent<CodeBlock>(); ;
                c.ButtonBackUp = but;
                codeToRun[i].GameObject.ComponentsToRemove.Add(but);
            }
            codeToRun = temp;
            SetPositions();
        }


        /// <summary>
        /// Fjerner de tomme kodeblokke når spilleren er færdig med at placere kodeblokken
        /// </summary>
        public void CleanUp() {
            List<CodeBlock> codeToRemove = new List<CodeBlock>();
            foreach (CodeBlock c in codeToRun) {
                if (!c.ContainsMethod) {
                    codeToRemove.Add(c);
                }
            }
            foreach (CodeBlock c in codeToRemove) {
                codeToRun.Remove(c);
                GameWorld.Instance.DestroyedGameObjects.Add(c.GameObject);
            }
            foreach (CodeBlock c in codeToRun) {
                if (c.ButtonBackUp != null) {
                    c.GameObject.ComponentsToAdd.Add(c.ButtonBackUp);
                    c.ButtonBackUp.CPress = true;
                    c.ButtonBackUp = null;
                    c.index = codeToRun.IndexOf(c);
                }
            }
            SetPositions();
        }

        /// <summary>
        /// Opsætter listerne algoritmen skal bruge
        /// </summary>
        internal void CreateLists()
        {
            if (!ClosedList.ContainsKey(startPointPos)) {
                OpenList.Add(startPointPos, 0);
                openListF.Add(startPointPos, 0);
                addedToOpenList++;
            }
            
            
            
        }

        /// <summary>
        /// Tilføjer en tile til den åbne liste og farver den lyseblå
        /// </summary>
        /// <param name="tile">Den tile der skal tilføjes</param>
        /// <param name="g">g-værdien for tile</param>
        /// <param name="f">f-værdien for tile</param>
        private void AddToOpenList(Vector2 tile, int g, int f) {
            if (!OpenList.Keys.Contains(tile) && !openListF.Keys.Contains(tile)) {
                OpenList.Add(tile, g);
                openListF.Add(tile, f);
                SpriteRenderer sr = (SpriteRenderer)Tiles[(int)tile.X, (int)tile.Y].GameObject.GetComponent<SpriteRenderer>();
                sr.Color = Color.LightBlue;
                addedToOpenList++;
            }
        }

        /// <summary>
        /// Tilføjer en tile til den lukkede liste
        /// </summary>
        /// <param name="tile">tile der skal tilføjes</param>
        /// <param name="g">g-værdien for den tile</param>
        private void AddToClosedList(Vector2 tile, int g) {
            ClosedList.Add(tile, g);
            SpriteRenderer sr = (SpriteRenderer)Tiles[(int)tile.X, (int)tile.Y].GameObject.GetComponent<SpriteRenderer>();
            sr.Color = Color.Blue;
            addedToClosedList++;
        }

        /// <summary>
        /// Finder den tile på den åbne liste der har den korteste estimerede rute til slutpunktet
        /// </summary>
        /// <exception cref="Exception"></exception>
        internal void FindShortestDistance()
        {
            int shortest = int.MaxValue;
            cTile = null;
            foreach (Vector2 c in openListF.Keys)
            {
                if (openListF[c] <= shortest)
                {
                    shortest = openListF[c];
                    cTile = Tiles[(int)c.X, (int)c.Y].GameObject;
                    cTilePos = c;
                }
            }
            if (cTile == null) {
                throw new Exception("cTile is null");
            }
        }

        /// <summary>
        /// Rykker den nuværende tile til den lukkede liste og fjerner den fra den åbne
        /// </summary>
        internal void MoveToClosedList()
        {
            AddToClosedList(cTilePos, openListF[cTilePos]);
            OpenList.Remove(cTilePos);
            openListF.Remove(cTilePos);
        }

        /// <summary>
        /// Finder alle naboer og tilføjer dem til en kø. Derefter finder den alt kode som ligger mellem denne kodeblok og første kodeblok hvor metoden er checkneighboursEnd. 
        /// De kodeblokke bliver så tilføjet til en kø der midlertidigt erstatter den primære kodekø indtil den er blevet udført for alle naboer
        /// </summary>
        internal void CheckNeighbours()
        {
            invalidTile = false;
            
            if (neighbours.Count > 1)
            {
                cNeighbour = neighbours.Dequeue();
                queuedCode = new Queue<CodeBlock>(tempCodeToRun);
            }
            else if (neighbours.Count == 1) {
                cNeighbour = neighbours.Dequeue();
                queuedCode = new Queue<CodeBlock>();
                for (int i = 0; i < tempCodeToRun.Count - 1; i++) {
                    queuedCode.Enqueue(tempCodeToRun[i]);
                }
                queuedCode.Enqueue(checKNeighboursTemp);
            }
            else
            {
                queuedCodeTemp = queuedCode;
                queuedCode = new Queue<CodeBlock>();
                for (int i = (int)cTilePos.X - 1; i <= (int)cTilePos.X + 1; i++)
                {
                    for (int j = (int)cTilePos.Y - 1; j <= (int)cTilePos.Y + 1; j++)
                    {
                        Vector2 cPos = new Vector2(i, j);

                        if (i >= 0 && j >= 0 && i <= 32 && j <= 32 && cPos != cTilePos)
                        {
                            neighbours.Enqueue(Tiles[i, j].GameObject);
                            neighboursReference.Add(Tiles[i, j].GameObject, cPos);
                        }
                    }
                }
                int index = currentIndex + 1;
                checKNeighboursTemp = queuedCodeTemp.Dequeue();
                while (checKNeighboursTemp.Method != CheckNeighboursEnd)
                {
                    tempCodeToRun.Add(checKNeighboursTemp);
                    checKNeighboursTemp = queuedCodeTemp.Dequeue();
                }
                tempCodeToRun.Add(codeToRun[currentIndex]);
                cNeighbour = neighbours.Dequeue();
                queuedCode = new Queue<CodeBlock>(tempCodeToRun);

            }
        }

        /// <summary>
        /// Markerer slutningen på checkNeighbours loopet og rydder op efter det og fortsætter så hovedkøen
        /// </summary>
        internal void CheckNeighboursEnd() {
            queuedCode = queuedCodeTemp;
            neighbours.Clear();
            tempCodeToRun.Clear();
            neighboursReference.Clear();
        }

        /// <summary>
        /// Checker om den nuværende nabo er en obstacle
        /// </summary>
        /// <exception cref="Exception"></exception>
        internal void checkForObstacles()
        {
            if (cNeighbour != null) {
                Vector2 cPos = neighboursReference[cNeighbour];
                Tile cT = (Tile)cNeighbour.GetComponent<Floor>();
                if (cT == null)
                {
                   cT = (Tile)cNeighbour.GetComponent<Obstacle>();
                }
                if (cT == null) {
                    cT = (Tile)cNeighbour.GetComponent<Ingredient>();
                }
                if (cT == null) {
                    throw new Exception("cNeighbour is not a Tile");
                }
                else if (cT.GetType() == typeof(Obstacle))
                {
                    InvalidNeighbour();
                }
                else if ((cPos.X == cTilePos.X - 1 || cPos.X == cTilePos.X + 1) && (cPos.Y == cTilePos.Y - 1 || cPos.Y == cTilePos.Y + 1))
                {
                    Vector2 p = new Vector2(cTilePos.X, cPos.Y);
                    Vector2 c = new Vector2(cPos.X, cTilePos.Y);
                    Tile t = Tiles[(int)p.X, (int)p.Y];
                    Tile k = Tiles[(int)c.X, (int)c.Y];
                    if (t.GetType() == typeof(Obstacle) || k.GetType() == typeof(Obstacle))
                    {
                        InvalidNeighbour();
                    }

                }
            }
        }

        /// <summary>
        /// Bliver kaldt hvis et tjek viser at tilen enten er en obstacle eller der ikke er noget der skal ændres i dens nuværende status
        /// </summary>
        private void InvalidNeighbour() {
            invalidTile = true;
        }

        /// <summary>
        /// Tjekker om tilen er på den lukkede liste
        /// </summary>
        internal void IsOnClosedList()
        {
            if (ClosedList.Keys.Contains(neighboursReference[cNeighbour]) || invalidTile) {
                InvalidNeighbour();
            }
        }

        /// <summary>
        /// Udregner f
        /// </summary>
        internal void CalculateF()
        {
            if (!invalidTile) {
                cF = cG + cH;
            }
            
        }

        /// <summary>
        /// Udregner g
        /// </summary>
        internal void CalculateG()
        {
            Vector2 cPos = neighboursReference[cNeighbour];
            if ((cPos.X == cTilePos.X - 1 || cPos.X == cPos.X + 1) && (cPos.Y == cPos.Y - 1 || cPos.Y == cTilePos.Y + 1) && !invalidTile)
            {
                cG = 14 + ClosedList[cTilePos];
            }
            else if(!invalidTile)
            {
                cG = 10 + ClosedList[cTilePos];
            }
        }

        /// <summary>
        /// Udregner h
        /// </summary>
        internal void CalculateH()
        {
            if (!invalidTile) {
                Vector2 cPos = neighboursReference[cNeighbour];
                int dx = (int)Math.Abs(cPos.X - endPointPos.X);
                int dy = (int)Math.Abs(cPos.Y - endPointPos.Y);
                cH = dy + dx;
            }
            
        }

        /// <summary>
        /// Tjekker om et tile er på den åbne liste og hvis den er om ruten gennem den nuværende tile er hurtigere
        /// </summary>
        internal void IsOnOpenList()
        {
            if (OpenList.ContainsKey(neighboursReference[cNeighbour]) && OpenList[neighboursReference[cNeighbour]] < cG)
            {
                InvalidNeighbour();
            }
            else if (OpenList.ContainsKey(neighboursReference[cNeighbour]) && OpenList[neighboursReference[cNeighbour]] > cG && !invalidTile)
            {
                onOpenList = true;
            }
            else if (!OpenList.ContainsKey(neighboursReference[cNeighbour]) && !invalidTile)
            {
                onOpenList = false;
            }
            else {
            }
        }

        /// <summary>
        /// Tilføjer den nuværende tile som parent hvis en neighbour ikke allerede er på den åbne liste
        /// </summary>
        internal void SetCurrentNodeAsParent()
        {
            if (!onOpenList && !invalidTile) {
                parents.Add(neighboursReference[cNeighbour], cTilePos);
            }
        }

        /// <summary>
        /// Tilføjer en tile til den åbne liste hvis den ikke allerede er på den.
        /// </summary>
        internal void AddToOpenList()
        {
            if (!onOpenList && !invalidTile)
            {
                AddToOpenList(neighboursReference[cNeighbour], cG, cF);
            }
        }

        /// <summary>
        /// Ændre parent hvis ruten gennem den nuværende tile er hurtigere end den som tidligere var parent for en neighbour. opdaterer også de gemte værdier for f og g
        /// </summary>
        internal void ChangeParent()
        {
            if (onOpenList && !invalidTile) {
                parents[neighboursReference[cNeighbour]] = cTilePos;
                OpenList[neighboursReference[cNeighbour]] = cG;
                openListF[neighboursReference[cNeighbour]] = cF;
            }
        }

        /// <summary>
        /// Tjekker om den nuværende position svarer til den for slutpunktet. Hvis den gør så afsluttes koden og den fundne rute bliver vist
        /// </summary>
        internal void IsGoalReached()
        {
            if (cTilePos == endPointPos) {
                queuedCode.Clear();
                
                DisplayPath();
            }
        }

        /// <summary>
        /// Ændrer farven på felter som er en del af ruten til grøn
        /// </summary>
        private void DisplayPath()
        {
            GameObject cTile = endPoint;
            Vector2 cTilePos = endPointPos;
            SpriteRenderer sR;
            while (cTile != startPoint)
            {
                Path.Add(cTile);
                sR = (SpriteRenderer)cTile.GetComponent<SpriteRenderer>();
                sR.Color = Color.Green;
                cTilePos = parents[cTilePos];
                cTile = Tiles[(int)cTilePos.X, (int)cTilePos.Y].GameObject;
            }
            sR = (SpriteRenderer)startPoint.GetComponent<SpriteRenderer>();
            Path.Add(startPoint);
            sR.setTempColor(Color.Green);
        }

        /// <summary>
        /// Tjekker om den åbne liste er tom. Hvis den er så afsluttes koden. Hvis ikke så starter den forfra, undtagen det første skridt da det formodes at være listeopsætningen.
        /// </summary>
        internal void IsOpenListEmpty()
        {
            if (OpenList.Count > 0 || openListF.Count > 0)
            {
                queuedCode = new Queue<CodeBlock>(codeToRun);
                queuedCode.Dequeue();
            }
            else {
                queuedCode.Clear();
                
            }
        }
    }
}
