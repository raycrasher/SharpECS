using System;
using System.Collections.Generic;
using System.Numerics;

namespace SharpECS
{
    public class World
    {
        internal readonly static Dictionary<Type, BigInteger> ComponentFlags = new Dictionary<Type, BigInteger>();
        internal static BigInteger NextComponentFlag = 1;

        private List<System> _systems = new List<System>();

        public IReadOnlyList<System> Systems => _systems;
        public HashSet<Entity> Entities { get; } = new HashSet<Entity>();

        public World()
        {
        }

        public void RegisterSystem(params System[] systems)
        {
            foreach (var system in systems)
            {
                _systems.Add(system);
                system.World = this;

                foreach(var entity in Entities)
                {
                    if (system.IsInterestedIn(entity))
                        system.EntitiesImpl.Add(entity);
                }
            }
        }

        public Entity CreateEntity()
        {
            var entity = new Entity
            {
                World = this
            };

            Entities.Add(entity);
            return entity;
        }

        public void DeleteEntity(Entity entity)
        {
            Entities.Remove(entity);
            foreach (var sys in Systems)
            {
                if (sys.EntitiesImpl.Remove(entity))
                {
                    sys.OnRemoveEntity(entity);
                }
            }
            entity.IsDeleted = true;
            entity.World = null;
        }

        internal static BigInteger RegisterComponent(Type type)
        {
            var flag = NextComponentFlag;
            ComponentFlags[type] = NextComponentFlag;
            NextComponentFlag <<= 1;
            return flag;
        }

        internal static BigInteger GetComponentFlag(Type type)
        {
            BigInteger flag;
            if (!ComponentFlags.TryGetValue(type, out flag))
            {
                flag = RegisterComponent(type);
            }
            return flag;
        }

        internal void UpdateSystemEntityLists(Entity entity)
        {
            foreach (var system in Systems)
            {
                if (!system.EntitiesImpl.Contains(entity))
                {
                    if (system.IsInterestedIn(entity))
                    {
                        system.EntitiesImpl.Add(entity);
                        system.OnAddEntity(entity);
                    }
                }
                else
                {
                    if (!system.IsInterestedIn(entity))
                    {
                        system.OnRemoveEntity(entity);
                        system.EntitiesImpl.Remove(entity);
                    }
                }
            }
        }
    }
}