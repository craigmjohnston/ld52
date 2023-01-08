namespace Oatsbarley.GameJams.LD52
{
    using System;
    using System.Linq;
    using NaughtyAttributes;
    using Newtonsoft.Json;
    using Oatsbarley.GameJams.LD52.Definitions;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [Serializable]
    public class PieceStatusDefinition
    {
        public PieceStatus Status;
        public Color Colour;
    }

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GridManager<Piece> grid;
        [SerializeField] private PieceShelf pieceShelf;
        [SerializeField] private InputManager inputManager;

        [SerializeField] private float cellSize;
        
        [SerializeField] private TextAsset levelJson;

        [SerializeField] private PieceStatusDefinition[] statusDefinitions;

        public static GameManager Instance => Object.FindObjectOfType<GameManager>(); // don't ask

        public float CellSize => cellSize;
        // public PieceStatusDefinition[] StatusDefinitions => statusDefinitions;

        private LevelDefinition currentLevel;
        private int currentScenarioIndex;

        private void Start()
        {
            inputManager.AllPiecesUsed += OnAllPiecesUsed;
        }

        [Button()]
        public void RunLevel()
        {
            if (currentLevel != null)
            {
                Debug.LogError("Can't run a level when there's already one running. Clear the current level first.");
                return;
            }
            
            var json = levelJson.text;
            var definition = JsonConvert.DeserializeObject<LevelDefinition>(json);
            
            RunLevel(definition);
        }

        [Button()]
        public void ClearLevel()
        {
            if (currentLevel == null)
            {
                Debug.LogWarning("Level already cleared.");
                return;
            }
            
            ClearGrid();
            pieceShelf.EmptyShelf();
            inputManager.Clear();
            
            currentLevel = null;
            currentScenarioIndex = 0;
        }

        public void GeneratePiece(string tag)
        {
            Debug.Log($"Generate '{tag}'");
            var piece = pieceShelf.InstantiatePiece(tag);
            pieceShelf.ReplacePiece(piece, new Vector2Int(pieceShelf.Length, 0));
        }

        public void ConsumePiece(Piece piece, Vector2Int gridPosition)
        {
            Debug.Log($"Consume piece '{piece.Definition.Tag}'", piece);
            grid.RemoveObject(gridPosition);
            Object.Destroy(piece.gameObject);
        }

        public void ReplacePiece(Piece piece, string definitionTag)
        {
            var definition = pieceShelf.GetDefinition(definitionTag);
            piece.ReplaceDefinition(definition);
        }

        public PieceStatusDefinition GetStatusDefinition(PieceStatus status)
        {
            var definition = statusDefinitions.FirstOrDefault(d => d.Status == status);
            if (definition == null)
            {
                Debug.LogError($"Can't find definition for status '{status}'");
            }

            return definition;
        }

        private void RunLevel(LevelDefinition definition)
        {
            currentLevel = definition;
            PlacePieces(definition.StartingPieces);
            
            currentScenarioIndex = 0;
            RunCurrentScenario();
        }

        private void PlacePieces(PieceLocation[] pieceLocations)
        {
            foreach (var pieceLocation in pieceLocations)
            {
                var piece = pieceShelf.InstantiatePiece(pieceLocation.Tag);
                var gridPosition = new Vector2Int(pieceLocation.Position[0], pieceLocation.Position[1]);
                
                grid.PlaceObject(piece, gridPosition);
            }
        }

        private void RunCurrentScenario()
        {
            var scenarioDefinition = currentLevel.Scenarios[currentScenarioIndex % currentLevel.Scenarios.Length];
            RunScenario(scenarioDefinition);
        }

        private void RunScenario(ScenarioDefinition scenarioDefinition)
        {
            Debug.Log($"Running scenario: {scenarioDefinition.Name}");
            pieceShelf.FillShelf(scenarioDefinition.StartingShelf, scenarioDefinition.DropChances, scenarioDefinition.ShelfLength);
        }

        private void OnAllPiecesUsed()
        {
            Tick();
            
            currentScenarioIndex += 1;
            RunCurrentScenario();
        }

        private void ClearGrid()
        {
            var pieces = grid.AllWithPositions.ToArray();
            foreach (var piece in pieces)
            {
                grid.RemoveObject(piece.gridPos);
                Object.Destroy(piece.obj.gameObject);
            }
        }

        [Button]
        public void Tick()
        {
            var all = grid.AllWithPositions.ToArray();
            
            foreach (var piece in all)
            {
                // don't tick locked pieces
                if (piece.obj.IsLocked)
                {
                    continue;
                }
                
                var surrounding = grid.GetSurroundingObjects(piece.gridPos);
                
                piece.obj.OnTick();
                (piece.obj).Definition.Tick(piece.obj, surrounding);
            }
            
            // after-tick
            foreach (var piece in all)
            {
                // don't tick locked pieces
                if (piece.obj.IsLocked)
                {
                    continue;
                }
                
                var surrounding = grid.GetSurroundingObjects(piece.gridPos);
                (piece.obj).Definition.AfterTick(piece.obj, surrounding);
            }
        }
    }
}