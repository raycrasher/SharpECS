using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SharpECS
{
    public sealed class Group
    {
        readonly internal HashSet<Entity> EntitiesImpl = new HashSet<Entity>();
        public IEnumerable<Entity> Entities => EntitiesImpl;

        public World World { get; internal set; }

        internal BigInteger RequireAllOfFlag = 0, RequireOneOfFlag = 0, ForbidAnyFlag = 0, ForbidAllFlag;

        public Group AllOf(params Type[] types)
        {
            BigInteger flag = 0;
            foreach (var type in types)
            {
                flag |= World.GetComponentFlag(type);
            }

            RequireAllOfFlag = flag;
            return this;
        }

        public Group OneOf(params Type[] types)
        {
            BigInteger flag = 0;
            foreach (var type in types)
            {
                flag |= World.GetComponentFlag(type);
            }
            RequireOneOfFlag = flag;
            return this;
        }

        public Group NoneOfAny(params Type[] types)
        {
            BigInteger flag = 0;
            foreach (var type in types)
            {
                flag |= World.GetComponentFlag(type);
            }
            ForbidAnyFlag = flag;
            return this;
        }

        public Group NoneOfAll(params Type[] types)
        {
            BigInteger flag = 0;
            foreach (var type in types)
            {
                flag |= World.GetComponentFlag(type);
            }
            ForbidAllFlag = flag;
            return this;
        }

        internal bool IsInterestedIn(Entity entity)
        {
            return
                ((RequireOneOfFlag == 0) || (entity.ComponentFlags & RequireOneOfFlag) != 0) &&
                ((RequireAllOfFlag == 0) || (entity.ComponentFlags & RequireAllOfFlag) == RequireAllOfFlag) &&                
                ((ForbidAnyFlag == 0) || !((entity.ComponentFlags & ForbidAnyFlag) == 0)) && 
                ((ForbidAllFlag == 0) || !((entity.ComponentFlags & ForbidAllFlag) == ForbidAllFlag))
                ;
        }

        public event Action<Entity> EntityAdded, EntityRemoved;

        internal void OnRemoveEntity(Entity entity)
        {
            EntityRemoved?.Invoke(entity);
        }

        internal void OnAddEntity(Entity entity)
        {
            EntityAdded?.Invoke(entity);
        }

        public void Deregister()
        {
            World?.Groups.Remove(this);
        }
    }
}
