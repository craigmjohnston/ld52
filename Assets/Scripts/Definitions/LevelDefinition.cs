namespace Oatsbarley.GameJams.LD52.Definitions
{
    public class LevelDefinition
    {
        public ScenarioDefinition[] Scenarios { get; set; }
    }

    public class ScenarioDefinition
    {
        public string Name { get; set; }
        public string[] StartingShelf { get; set; }
        public int ShelfLength { get; set; }
        public DropChance[] DropChances { get; set; }
    }

    public class DropChance
    {
        public string Tag { get; set; }
        public float Chance { get; set; }
    }
}