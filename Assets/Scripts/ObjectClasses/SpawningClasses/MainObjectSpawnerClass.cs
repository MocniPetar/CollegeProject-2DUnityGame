namespace ObjectClasses.SpawningClasses
{
    public class MainObjectSpawnerClass
    {
        public string LevelName { get; set; }
        public PlayerSpawnClass PlayerSpawnClass = new ();
        public EnemySpawnClass EnemySpawnClass = new ();
        public bool HasDoor { get; set; }
    }
}