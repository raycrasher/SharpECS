using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace SharpECS
{
    public class World
    {
        internal readonly static Dictionary<Type, BigInteger> ComponentFlags = new Dictionary<Type, BigInteger>();
        internal static BigInteger NextComponentFlag = 1;

        internal readonly HashSet<Group> Groups = new HashSet<Group>();
        internal readonly Dictionary<Type, System> SystemsImpl = new Dictionary<Type, System>();
        internal readonly HashSet<Entity> EntitiesImpl = new HashSet<Entity>();

        public IEnumerable<Entity> Entities => EntitiesImpl;
        public IEnumerable<System> Systems => SystemsImpl.Values;

        public int NumEntities => EntitiesImpl.Count;

        public Action<Entity> EntityAdded, EntityRemoved;
        
        public World()
        {
        }

        public void RegisterGroups(params Group[] groups)
        {
            foreach (var group in groups)
            {
                Groups.Add(group);
                group.World = this;

                foreach(var entity in Entities)
                {
                    if (group.IsInterestedIn(entity))
                        group.EntitiesImpl.Add(entity);
                }
            }
        }

        public IEnumerable<System> RegisterSystems(params System[] systems)
        {
            foreach (var system in systems)
            {
                SystemsImpl[system.GetType()] = system;
                system.World = this;
            }
            RegisterGroups(systems.Select(s => s.Group).ToArray());
            return systems;
        }

        public T GetSystem<T>()
            where T : System
        {
            System s;
            if (SystemsImpl.TryGetValue(typeof(T), out s))            
                return (T)s;            
            else return null;
        }

        public Entity CreateEntity()
        {
            var entity = new Entity
            {
                World = this
            };

            EntitiesImpl.Add(entity);
            return entity;
        }

        public void DeleteEntity(Entity entity)
        {
            EntitiesImpl.Remove(entity);
            foreach (var group in Groups)
            {
                if (group.EntitiesImpl.Remove(entity))
                {
                    group.OnRemoveEntity(entity);
                }
            }
            entity.IsDeleted = true;
            entity.World = null;
        }

        private static BigInteger RegisterComponent(Type type)
        {
            var flag = NextComponentFlag;
            if (!typeof(IComponent).IsAssignableFrom(type))
                throw new InvalidOperationException($"{type} is not derived from IComponent!");
            ComponentFlags[type] = NextComponentFlag;
            NextComponentFlag <<= 1;
            return flag;
        }

        public void DeRegisterGroup(Group group)
        {
            Groups.Remove(group);
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

        internal void UpdateGroupEntityLists(Entity entity)
        {
            foreach (var group in Groups)
            {
                if (group.IsInterestedIn(entity)) {
                    if (group.EntitiesImpl.Add(entity))
                        group.OnAddEntity(entity);
                }
                else {
                    if (group.EntitiesImpl.Remove(entity))
                        group.OnRemoveEntity(entity);
                }
            }
        }
    }
}