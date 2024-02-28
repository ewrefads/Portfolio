using Meerkat_Mining.Components;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Meerkat_Mining
{
    public class GameObject
    {
        private Transform transform;
        private List<Component> components;
        private string tag;

        public GameObject()
        {
            components = new List<Component>();
            transform = new Transform();
        }

        public Transform Transform { get => transform; set => transform = value; }
        public string Tag { get => tag; set => tag = value; }

        public Component AddComponent(Component component)
        {
            component.GameObject = this;
            components.Add(component);

            return component;
        }

        public Component GetComponent<T>() where T : Component
        {
            return components.Find(x => x.GetType() == typeof(T));
        }

        public void RemoveComponent(Component component)
        {
            components.Remove(component);
        }

        public void Awake()
        {
            foreach (Component component in components)
            {
                component.Awake();
            }
        }
        public void Start()
        {
            foreach (Component component in components)
            {
                component.Start();
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Component component in components)
            {
                component.Draw(spriteBatch);
            }
        }

        public void Update()
        {
            foreach (Component component in components)
            {
                component.Update();
            }
        }


    }
}