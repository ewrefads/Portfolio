using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.Components
{
    internal class PlayerCity : Component
    {
        private KeyboardState keyState;
        private KeyboardState lastKeyState;

        private int position=0;

        public override void Start()
        {
            SpriteRenderer sr = GameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            sr.SetSprite("otherSprites/player_facing_down1");
            sr.Scale = new Vector2(3f, 3f);
            GameObject.Transform.Position = City.Instance.Positions[position];

            City.Instance.Buildings[position].Activate();
        }

        public override void Update()
        {

            keyState = Keyboard.GetState();
            CheckInput();
        }


        public void CheckInput()
        {
            if (keyState.Equals(lastKeyState))
                return;

            //right
            if (keyState.IsKeyDown(Keys.D))
            {
                if (position+1 >= City.Instance.Positions.Count)
                    return;
                City.Instance.Buildings[position].DeActivate();
                position++;
                City.Instance.Buildings[position].Activate();
                GameObject.Transform.Position = City.Instance.Positions[position];
            }
            //right
            if (keyState.IsKeyDown(Keys.A))
            {
                if (position <= 0)
                    return;
                City.Instance.Buildings[position].DeActivate();
                position--;
                City.Instance.Buildings[position].Activate();
                GameObject.Transform.Position = City.Instance.Positions[position];

            }
            if (keyState.IsKeyDown(Keys.Space))
            {
                City.Instance.Buildings[position].Interact();
            }

            lastKeyState = keyState;
        }
    }
}
