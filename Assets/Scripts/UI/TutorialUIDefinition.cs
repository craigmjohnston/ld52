namespace Oatsbarley.GameJams.LD52.UI
{
    public class TutorialUIDefinition
    {
        public string BodyText { get; set; }
        public TutorialUIItemDefinition[] Items { get; set; }
    }

    public class TutorialUIItemDefinition
    {
        public string Description { get; set; }
        public string[] ImagePaths { get; set; }
    }
}