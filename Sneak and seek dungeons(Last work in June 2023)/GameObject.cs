using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons
{
    public class GameObject
    {
        private Transform transform;
        private List<Component> components;
        private string tag;
        //private SCENEPROPERTY sceneProperty;
        private bool threaded;
        private bool enabled;

        public GameObject()
        {
            components = new List<Component>();
            transform = new Transform();
        }

        public Transform Transform { get => transform; set => transform = value; }
        public string Tag { get => tag; set => tag = value; }
        public bool Threaded { get => threaded; set => threaded = value; }
        public List<Component> Components { get => components; set => components = value; }
        public bool Enabled { get => enabled; set => enabled = value; }
        //public SCENEPROPERTY SceneProperty { get => sceneProperty; set => sceneProperty = value; }

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
            enabled = true;
            foreach (Component component in components)
            {
                component.Start();
            }
        }

        public void Update()
        {
            if (enabled) {
                foreach (Component component in components)
                {
                    component.Update();
                }
            }
        }

        /*public void ThreadedUpdate() {
            Update();
            GameWorld.Instance.m.WaitOne();
            GameWorld.Instance.ActiveThreads--;
            GameWorld.Instance.m.ReleaseMutex();
        }*/

        public void Draw(SpriteBatch spriteBatch)
        {
            if (enabled)
            {
                foreach (Component component in components)
                {
                    component.Draw(spriteBatch);
                }
            }
        }
    }
}
