using Meerkat_Mining.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meerkat_Mining.Builderpattern
{
    public class DrillBuilder : IBuilder
    {
        private GameObject gameObject;

        public void BuildGameObject()
        {
            gameObject = new GameObject();
            BuildComponents();
        }

        private void BuildComponents()
        {
            gameObject.AddComponent(new Drill());
            gameObject.AddComponent(new SpriteRenderer());

        }

        public GameObject GetResult()
        {
            return gameObject;
        }
    }
}
