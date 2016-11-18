using System;
using System.Collections.Generic;
using System.Numerics;

namespace SharpECS
{
    public class Entity
    {
        public World World { get; internal set; }
        public bool IsDeleted { get; internal set; } = false;
        public IReadOnlyDictionary<Type, IComponent> Components => _components;

        private Dictionary<Type, IComponent> _components = new Dictionary<Type, IComponent>();
        internal BigInteger ComponentFlags = 0;

        internal Entity()
        {
        }

        public bool HasComponent<T>()
            where T : IComponent
        {
            return _components.ContainsKey(typeof(T));
        }

        public void RemoveComponent(IComponent component)
        {
            RemoveComponent(component.GetType());
        }

        public void RemoveComponent<T>()
            where T : IComponent
        {
            RemoveComponent(typeof(T));
        }

        public void RemoveComponent(Type type)
        {
            var flag = World.GetComponentFlag(type);
            ComponentFlags &= ~flag;

            foreach (var system in World.Systems)
            {
                if (!system.IsInterestedIn(this) && system.EntitiesImpl.Contains(this))
                {
                    system.OnRemoveEntity(this);
                    system.EntitiesImpl.Remove(this);
                }
            }
            _components.Remove(type);
        }

        public T GetComponent<T>()
            where T : IComponent
        {
            IComponent component;
            if (_components.TryGetValue(typeof(T), out component))
                return (T)component;
            else
                return default(T);
        }

        public void AddComponent(IComponent component)
        {
            var type = component.GetType();
            _components[type] = component;
            ComponentFlags |= World.GetComponentFlag(type);
            World.UpdateSystemEntityLists(this);
        }

        public void AddComponent(params IComponent[] components)
        {
            for (int i = 0; i < components.Length; i++)
            {
                var component = components[i];
                var type = component.GetType();
                _components[type] = component;
                ComponentFlags |= World.GetComponentFlag(type);
            }
            World.UpdateSystemEntityLists(this);
        }

        public void Delete()
        {
            World.DeleteEntity(this);
        }
    }
}