using System;
using System.Collections.Generic;
using System.Numerics;

namespace SharpECS
{
    public abstract class System
    {
        public World World { get; internal set; }

        readonly internal HashSet<Entity> EntitiesImpl = new HashSet<Entity>();
        public IEnumerable<Entity> Entities => EntitiesImpl;
        internal BigInteger RequiredComponentsFlag = 0, OneOfComponentsFlag = 0, ForbiddenComponentsFlag = 0;

        public abstract void Execute();

        protected void RequireAll(params Type[] types)
        {
            BigInteger flag = 0;
            foreach (var type in types)
            {
                flag |= World.GetComponentFlag(type);
            }

            RequiredComponentsFlag = flag;
        }

        protected void RequireOne(params Type[] types)
        {
            BigInteger flag = 0;
            foreach (var type in types)
            {
                flag |= World.GetComponentFlag(type);
            }
            OneOfComponentsFlag = flag;
        }

        protected void Forbid(params Type[] types)
        {
            BigInteger flag = 0;
            foreach (var type in types)
            {
                flag |= World.GetComponentFlag(type);
            }
            ForbiddenComponentsFlag = flag;
        }

        internal bool IsInterestedIn(Entity entity)
        {
            return
                ((RequiredComponentsFlag == 0) || (entity.ComponentFlags & RequiredComponentsFlag) == RequiredComponentsFlag) &&
                ((OneOfComponentsFlag == 0) || (entity.ComponentFlags & OneOfComponentsFlag) != 0) &&
                ((ForbiddenComponentsFlag == 0) || (~entity.ComponentFlags & ForbiddenComponentsFlag) == 0);
        }

        protected internal virtual void OnRemoveEntity(Entity entity)
        {
        }

        protected internal virtual void OnAddEntity(Entity entity)
        {
        }
    }
}