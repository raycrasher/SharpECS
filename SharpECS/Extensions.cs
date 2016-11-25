using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpECS
{
    public static class Extensions
    {
        public static void ExecuteAll(this IEnumerable<System> systems)
        {
            foreach(var system in systems)
            {
                system.Execute();
            }
        }
    }
}
