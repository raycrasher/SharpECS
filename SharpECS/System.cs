using System;
using System.Collections.Generic;
using System.Numerics;

namespace SharpECS
{
    public abstract class System
    {
        public World World { get; internal set; }
        public Group Group { get; internal set; }

        protected System(Group group)
        {
            Group = group;
            Group.EntityAdded += OnAddEntity;
            Group.EntityRemoved += OnRemoveEntity;
        }

        public abstract void Execute();

        protected internal virtual void OnRemoveEntity(Entity entity)
        {
        }

        protected internal virtual void OnAddEntity(Entity entity)
        {
        }
    }
}