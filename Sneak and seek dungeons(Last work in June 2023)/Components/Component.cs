using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons
{
    public class Component
    {
        protected bool isEnabled = true;
        protected GameObject gameObject;

        public bool IsEnabled { get => isEnabled; set => isEnabled = value; }
        internal GameObject GameObject { get => gameObject; set => gameObject = value; }

        public virtual void Awake()
        {

        }

        public virtual void Start()
        {

        }

        public virtual void Update()
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}

