namespace SharpECS.Tests
{
    internal class HealthRegenSystem : System
    {
        public HealthRegenSystem()
            : base(new Group().AllOf(typeof(HealthComponent)))
        {
        }

        public bool ExecuteCalled { get; set; }
        public bool OnAddEntityCalled { get; set; }
        public bool OnRemoveEntityCalled { get; set; }

        public override void Execute()
        {
            ExecuteCalled = true;
        }

        protected override void OnAddEntity(Entity entity)
        {
            OnAddEntityCalled = true;
        }

        protected override void OnRemoveEntity(Entity entity)
        {
            OnRemoveEntityCalled = true;
        }
    }
}