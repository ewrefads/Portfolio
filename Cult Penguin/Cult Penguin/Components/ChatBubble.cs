using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cult_Penguin.Components
{
    internal class ChatBubble : Component
    {
        private bool display = false;
        private float timeRemaining = 0;
        private string text;
        private Texture2D sprite = GameWorld.Instance.Content.Load<Texture2D>("ChatBubble");
        private Vector2 stringLength;
        private List<string> subStrings;

        public string Text { private get => text; set {
                text = value;
                stringLength = GameWorld.Instance.defaultFont.MeasureString(text);
                if (stringLength.X > 150) {
                    subStrings = SplitString(text);
                }
                
                Display = true;
                timeRemaining = GameWorld.Instance.MessageDisplayTime * text.Length * 0.5f;
            } 
        }

        private List<string> SplitString(string text)
        {
            List<string> strings = new List<string>();
            SpriteFont defaultFont = GameWorld.Instance.defaultFont;
            int maxLength = 100;
            string[] words = text.Split(' ');
            string currentSubstring = "";
            Vector2 currentSubStringLength = new Vector2(0, 0);
            for (int i = 0; i < words.Length; i++) {
                Vector2 wordLength = defaultFont.MeasureString(words[i]);
                if (wordLength.X > maxLength)
                {
                    if (currentSubstring.Length > 0)
                    {
                        strings.Add(currentSubstring);
                        currentSubstring = "";
                    }
                    string tempWord = words[i];
                    string tempSubString = "";
                    while (wordLength.X > maxLength)
                    {
                        tempSubString += tempWord[0];
                        if (defaultFont.MeasureString(tempSubString).X == maxLength) {
                            strings.Add(tempSubString);
                            tempSubString = "";

                        }
                        tempWord = tempWord.Substring(1);
                        wordLength = defaultFont.MeasureString(tempWord);
                    }
                    currentSubstring = tempWord;
                }
                else if (currentSubStringLength.X + wordLength.X > maxLength)
                {
                    strings.Add(currentSubstring);
                    currentSubstring = words[i];
                    currentSubstring += " ";
                    currentSubStringLength = defaultFont.MeasureString(currentSubstring);
                }
                else {
                    currentSubstring += words[i];
                    currentSubstring += " ";
                    currentSubStringLength = defaultFont.MeasureString(currentSubstring);
                }
            }
            strings.Add(currentSubstring);
            return strings;
        }

        public bool Display { get => display; private set => display = value; }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Display) {
                if (stringLength.X < 64)
                {
                    Vector2 origin = new Vector2(stringLength.X/2, stringLength.Y/2);
                    SpriteRenderer sr = (SpriteRenderer)GameObject.GetComponent<SpriteRenderer>();
                    float scaleFactor = ((stringLength.X + stringLength.X/4) /64);
                    if (scaleFactor < 1) {
                        scaleFactor = 1;
                    }
                    Rectangle bubblePos = new Rectangle((int)GameObject.Transform.Position.X + (sr.Sprite.Width / 2) + 20, (int)GameObject.Transform.Position.Y - (sr.Sprite.Height / 2), 100, 50);
                    spriteBatch.Draw(sprite, new Vector2(bubblePos.X, bubblePos.Y), null, Color.White, 0f, origin, new Vector2(scaleFactor, 1), SpriteEffects.None, 0.8f);
                    //spriteBatch.Draw(sprite, bubblePos, Color.White);
                    Vector2 textOrigin = new Vector2(stringLength.X / 2, stringLength.Y / 2);
                    spriteBatch.DrawString(GameWorld.Instance.defaultFont, text, new Vector2(bubblePos.X - 20, bubblePos.Y - 10), Color.Black, 0, textOrigin, new Vector2(1, 1), SpriteEffects.None, 0.9f);
                }
                else {
                    Vector2 origin = new Vector2(50, 25);
                    float heightIncrease = subStrings.Count * 20;
                    if (subStrings.Count > 3) {
                        //heightIncrease += 1 * subStrings.Count;
                    }
                    SpriteRenderer sr = (SpriteRenderer)GameObject.GetComponent<SpriteRenderer>();
                    //Rectangle bubblePos = new Rectangle((int)GameObject.Transform.Position.X + (sr.Sprite.Width / 2) + 40, (int)GameObject.Transform.Position.Y - (sr.Sprite.Height / 2) - 40 - heightIncrease / 2, 100, 50 - heightIncrease);
                    float scaleFactorY = (heightIncrease + 50) / 50;
                    float scaleFactorX = 1.1f;
                    Vector2 pos = new Vector2(GameObject.Transform.Position.X + (sr.Sprite.Width / 2) + 40, GameObject.Transform.Position.Y - (sr.Sprite.Height / 2) - heightIncrease);
                    spriteBatch.Draw(sprite, pos, null, Color.White, 0f, origin, new Vector2(scaleFactorX, scaleFactorY), SpriteEffects.None, 0.8f);
                    //spriteBatch.Draw(sprite, bubblePos, Color.White);
                    int currentHeightOffset = 0;
                    if (subStrings.Count > 2) {
                        currentHeightOffset = -20;
                        if (subStrings.Count > 3) {
                            currentHeightOffset -= 1;
                            if (subStrings.Count > 4) {
                                currentHeightOffset -= 20;
                            }
                        }
                    }
                    Vector2 textOrigin = new Vector2(50 / 2, stringLength.Y / 2);
                    for (int i = 0; i < subStrings.Count; i++) {
                        
                        spriteBatch.DrawString(GameWorld.Instance.defaultFont, subStrings[i], new Vector2(pos.X, pos.Y + currentHeightOffset), Color.Black, 0, origin, new Vector2(1, 1), SpriteEffects.None, 0.9f);
                        currentHeightOffset += 20;
                    }
                }
            }
            base.Draw(spriteBatch);
        }

        public override void Update()
        {
            timeRemaining -= GameWorld.DeltaTime;
            if (timeRemaining <= 0 && Display) {
                Display = false;
            }
            base.Update();
        }
    }
}
