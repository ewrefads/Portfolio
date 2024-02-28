using AStarppetizing_Algorithms.FactoryPattern;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStarppetizing_Algorithms.Components
{
    public class CodeBlock:Component
    {
        private CODEBLOCKTYPES type;
        public delegate void executeMethod();
        private executeMethod method;
        private bool containsMethod = false;
        private bool beingPlaced = false;
        private bool ongoingClick = false;
        private Button buttonBackUp;
        public int index;

        public executeMethod Method { get => method; set {
                method = value;
                ContainsMethod = true;
            } 
        }

        public bool ContainsMethod { get => containsMethod; set => containsMethod = value; }
        internal Button ButtonBackUp { get => buttonBackUp; set => buttonBackUp = value; }
        public CODEBLOCKTYPES Type { get => type; set => type = value; }


        /// <summary>
        /// Kode der skal køres når en ny kodeblok bliver lavet
        /// </summary>
        public void newCodeBlock() {
            GameObject c = CodeBlockFactory.Instance.Create(Type);
            Button b = (Button)c.GetComponent<Button>();
            CodeBlock c1 = (CodeBlock)c.GetComponent<CodeBlock>();
            c1.beingPlaced = true;
            c1.buttonBackUp = b;
            b.OnClick = c1.startMove;
            c.ComponentsToRemove.Add(b);
            GameWorld.Instance.NewGameObjects.Add(c);
            CodeManager.Instance.prepareForNewCodeBlock();
        }

        /// <summary>
        /// Kode der bliver kørt hvis en kodebloks position i codemanagerens kø skal ændres.
        /// </summary>
        public void startMove() {
            if (!beingPlaced) {
                beingPlaced = true;
                CodeManager.Instance.CodeToRun.RemoveAt(index);
                CodeManager.Instance.prepareForNewCodeBlock();
                Button b = (Button)GameObject.GetComponent<Button>();

                GameObject.ComponentsToRemove.Add(b);
                buttonBackUp = b;
            }
        }

        /// <summary>
        /// Kode for rykke rundt på kodeblokken
        /// </summary>
        public void Move() {
            Vector2 mousePos = Mouse.GetState().Position.ToVector2();
            GameObject.Transform.Position = mousePos;
            if (Mouse.GetState().RightButton == ButtonState.Pressed && beingPlaced) {
                GameWorld.Instance.DestroyedGameObjects.Add(GameObject);
                CodeManager.Instance.CleanUp();
            }
            if (Mouse.GetState().LeftButton == ButtonState.Released && ongoingClick) {
                ongoingClick = false;
            }
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && !ongoingClick) {
                Collider col = (Collider)GameObject.GetComponent<Collider>();
                foreach (CodeBlock cb in CodeManager.Instance.CodeToRun) {
                    GameObject g = cb.GameObject;
                    Collider c = (Collider)g.GetComponent<Collider>();
                    if (g.GetComponent<CodeBlock> != null && c.CollisionBox.Contains(mousePos)) {
                        if (!cb.ContainsMethod)
                        {
                            GameObject.Transform.Position = g.Transform.Position;
                            beingPlaced = false;
                            GameWorld.Instance.DestroyedGameObjects.Add(g);
                            int index = cb.index;
                            CodeManager.Instance.CodeToRun.Insert(index, this);
                            buttonBackUp.CPress = true;
                            CodeManager.Instance.CleanUp();
                            if (Method == CodeManager.Instance.CheckNeighbours) {
                                GameObject end = CodeBlockFactory.Instance.Create(CODEBLOCKTYPES.checkNeighboursEnd);
                                Button b = (Button)end.GetComponent<Button>();
                                CodeBlock c1 = (CodeBlock)end.GetComponent<CodeBlock>();
                                c1.beingPlaced = false;
                                c1.buttonBackUp = b;
                                b.OnClick = c1.startMove;
                                b.CPress = true;
                                end.ComponentsToRemove.Add(b);
                                GameWorld.Instance.NewGameObjects.Add(end);
                                CodeManager.Instance.prepareForNewCodeBlock();
                                index = CodeManager.Instance.CodeToRun.IndexOf(this);
                                CodeManager.Instance.CodeToRun.Insert(index + 1, c1);
                                CodeManager.Instance.CleanUp();
                            }
                            break;
                        }
                    }
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (beingPlaced) {
                Move();
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// Kode til at sørge for teksten fortsat bliver skrevet når kodeblokken kører rundt
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (buttonBackUp != null) {
                buttonBackUp.Draw(spriteBatch);
            }
        }
    }
}
