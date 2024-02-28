using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStarppetizing_Algorithms
{
    public class GameObject
    {
        private Transform transform;
        private List<Component> components;
        private string tag;
        private bool threaded;

        private List<Component> componentsToAdd = new List<Component>();

        private List<Component> componentsToRemove = new List<Component>();

        public GameObject()
        {
            components = new List<Component>();
            transform = new Transform();
        }

        public Transform Transform { get => transform; set => transform = value; }
        public string Tag { get => tag; set => tag = value; }
        public bool Threaded { get => threaded; set => threaded = value; }
        public List<Component> ComponentsToAdd { get => componentsToAdd;}
        public List<Component> ComponentsToRemove { get => componentsToRemove; set => componentsToRemove = value; }

        public Component AddComponent(Component component)
        {
            component.GameObject = this;
            components.Add(component);

            return component;
        }


        public Component GetComponent<T>() where T : Component
        {
            Component c = components.Find(x => x.GetType() == typeof(T));
            return c;
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

        public void Update(GameTime gameTime)
        {
            foreach (Component component in components)
            {
                component.Update(gameTime);
            }
            foreach (Component component in componentsToAdd) {
                AddComponent(component);
            }
            foreach (Component component in componentsToRemove)
            {
                RemoveComponent(component);
            }
            componentsToAdd.Clear();
            componentsToRemove.Clear();
        }

        /*public void ThreadedUpdate() {
            Update();
            GameWorld.Instance.m.WaitOne();
            GameWorld.Instance.ActiveThreads--;
            GameWorld.Instance.m.ReleaseMutex();
        }*/

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Component component in components)
            {
                component.Draw(spriteBatch);
            }
        }
    }
}
