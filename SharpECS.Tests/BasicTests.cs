using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace SharpECS.Tests
{
    [TestClass]
    public class BasicTests
    {
        [TestMethod]
        public void ComponentTest()
        {
            var world = new World();
            var entity = world.CreateEntity();
            Assert.IsNotNull(entity);
            var cmp = new HealthComponent() { Health = 12345 };
            entity.AddComponent(cmp);
            Assert.IsTrue(entity.HasComponent<HealthComponent>());
            Assert.AreEqual(cmp, entity.GetComponent<HealthComponent>());

            var cmp2 = new PowerComponent();
            entity.AddComponent(cmp2);

            Assert.IsTrue(entity.HasComponent<PowerComponent>());
            Assert.AreEqual(cmp2, entity.GetComponent<PowerComponent>());
            entity.RemoveComponent(cmp2);
            Assert.IsFalse(entity.HasComponent<PowerComponent>());
        }

        [TestMethod]
        public void SystemTest()
        {
            var world = new World();
            var entity = world.CreateEntity();
            var system = new HealthRegenSystem();
            world.RegisterSystem(system);
            Assert.IsNotNull(system.World);
            Assert.IsFalse(system.OnAddEntityCalled);
            Assert.IsTrue(world.Systems.Count == 1);
            entity.AddComponent(new HealthComponent());
            Assert.IsTrue(system.OnAddEntityCalled);
            Assert.IsFalse(system.OnRemoveEntityCalled);
            Assert.IsTrue(system.Entities.Count() == 1);
            entity.RemoveComponent<HealthComponent>();
            Assert.IsTrue(system.Entities.Count() == 0);
            Assert.IsTrue(system.OnRemoveEntityCalled);
            entity.AddComponent(new PowerComponent());
            Assert.IsTrue(system.Entities.Count() == 0);
        }
    }
}