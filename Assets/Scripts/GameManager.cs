namespace Oatsbarley.GameJams.LD52
{
    using NaughtyAttributes;
    using Newtonsoft.Json;
    using Oatsbarley.GameJams.LD52.Definitions;
    using UnityEngine;

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GridManager<Piece> grid;
        [SerializeField] private PieceShelf pieceShelf;
        [SerializeField] private InputManager inputManager;

        [SerializeField] private float cellSize;
        
        [SerializeField] private TextAsset levelJson;

        public static GameManager Instance => Object.FindObjectOfType<GameManager>(); // don't ask

        public float CellSize => cellSize;

        private LevelDefinition currentLevel;
        private int currentScenarioIndex;

        private void Start()
        {
            inputManager.AllPiecesUsed += OnAllPiecesUsed;
        }

        [Button()]
        public void RunLevel()
        {
            var json = levelJson.text;
            var definition = JsonConvert.DeserializeObject<LevelDefinition>(json);
            
            RunLevel(definition);
        }

        private void RunLevel(LevelDefinition definition)
        {
            currentLevel = definition;
            currentScenarioIndex = 0;
            
            RunCurrentScenario();
        }

        private void RunCurrentScenario()
        {
            var scenarioDefinition = currentLevel.Scenarios[currentScenarioIndex % currentLevel.Scenarios.Length];
            RunScenario(scenarioDefinition);
        }

        private void RunScenario(ScenarioDefinition scenarioDefinition)
        {
            Debug.Log($"Running scenario: {scenarioDefinition.Name}");
            pieceShelf.FillShelf(scenarioDefinition.StartingShelf, scenarioDefinition.DropChances);
        }

        private void OnAllPiecesUsed()
        {
            currentScenarioIndex += 1;
            RunCurrentScenario();
        }

        [Button]
        public void Tick()
        {
            foreach (var piece in grid.AllWithPositions)
            {
                // don't tick locked pieces
                if (piece.obj.IsLocked)
                {
                    continue;
                }
                
                var surrounding = grid.GetSurroundingObjects(piece.gridPos);
                (piece.obj).Definition.Tick(piece.obj, surrounding);
            }
        }
    }
}