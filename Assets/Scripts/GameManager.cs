namespace Oatsbarley.GameJams.LD52
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NaughtyAttributes;
    using Newtonsoft.Json;
    using Oatsbarley.GameJams.LD52.Definitions;
    using Oatsbarley.GameJams.LD52.Pieces;
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

        private Dictionary<string, int> stats = new Dictionary<string, int>();

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
            stats.Clear();
            
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
            pieceShelf.GeneratePiece(tag);
        }

        public void ConsumePiece(Piece piece, Vector2Int gridPosition)
        {
            Debug.Log($"Consume piece '{piece.Definition.Tag}'", piece);
            grid.RemoveObject(gridPosition);
            Object.Destroy(piece.gameObject);
        }

        public void ReplacePiece(Piece piece, Vector2Int gridPosition, string definitionTag)
        {
            var definition = pieceShelf.GetDefinition(definitionTag);
            piece.ReplaceDefinition(definition);
            
            piece.Definition.OnReplace(piece, grid.GetSurroundingObjects(gridPosition));
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
            EndScenario();
        }

        private void EndScenario()
        {
            // stats
            var leaves = grid.All.Count(p => p.Definition.Tag == PieceTag.Leaf);
            SetMaxStat("stat_max_leaves", leaves);

            if (leaves == 0)
            {
                EndGame();
                return;
            }
            
            currentScenarioIndex += 1;
            RunCurrentScenario();
        }

        private void EndGame()
        {
            var cachedStats = stats.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            ClearLevel();
            AppManager.Instance.ShowEndGame(cachedStats);
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

        public void IncrementStat(string key, int amount)
        {
            if (!stats.TryGetValue(key, out int value))
            {
                value = 0;
            }

            value += amount;
            stats[key] = value;
        }

        public void SetStat(string key, int amount)
        {
            stats[key] = amount;
        }

        public void SetMaxStat(string key, int amount)
        {
            var value = GetStat(key);
            stats[key] = Mathf.Max(value, amount);
        }

        public int GetStat(string key)
        {
            if (!stats.TryGetValue(key, out int value))
            {
                return 0;
            }

            return value;
        }
    }
}