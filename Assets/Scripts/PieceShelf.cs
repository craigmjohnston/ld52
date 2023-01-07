namespace Oatsbarley.GameJams.LD52
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Oatsbarley.GameJams.LD52.Definitions;
    using Oatsbarley.GameJams.LD52.Pieces;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class PieceShelf : MonoBehaviour
    {
        [SerializeField] private PieceGridManager shelfGrid;
        [SerializeField] private Piece piecePrefab;

        [SerializeField] private int length;

        [SerializeField] private PieceDefinition[] definitions;

        public void FillShelf(string[] required, DropChance[] dropChances)
        {
            int count = 0;
            
            var requiredDefinitions = required.Select(r => definitions.FirstOrDefault(d => d.Tag == r));
            foreach (var def in requiredDefinitions)
            {
                var piece = InstantiatePiece(def);
                shelfGrid.PlaceObject(piece, new Vector2Int(count, 0));
                count += 1;
            }

            float totalChance = dropChances.Sum(d => d.Chance);
            Dictionary<float, DropChance> normalisedChances = new Dictionary<float, DropChance>();
            var ordered = dropChances.OrderBy(d => d.Chance);

            float runningTotal = 0;
            foreach (var chance in ordered)
            {
                normalisedChances[runningTotal] = chance;
                runningTotal += chance.Chance / totalChance;
            }

            var highestChance = normalisedChances.OrderBy(kvp => kvp.Key).LastOrDefault().Value;

            for (int i = 0; i < length - count; i++)
            {
                var value = Random.value;
                foreach (var kvp in normalisedChances.OrderBy(kvp => kvp.Key))
                {
                    if (kvp.Key < value && kvp.Value != highestChance)
                    {
                        continue;
                    }

                    var def = definitions.FirstOrDefault(d => d.Tag == kvp.Value.Tag);
                    var piece = InstantiatePiece(def);
                    shelfGrid.PlaceObject(piece, new Vector2Int(count + i, 0));

                    break;
                }
            }
        }

        public bool TryGetGridPosition(Vector3 worldPosition, out Vector2Int? gridPosition)
        {
            gridPosition = null;
            var gridPos = shelfGrid.WorldPositionToGridPosition(worldPosition);

            if (!IsGridPositionInShelf(gridPos))
            {
                return false;
            }

            gridPosition = gridPos;
            return true;
        }

        public bool IsWorldPositionInShelf(Vector3 worldPosition)
        {
            return TryGetGridPosition(worldPosition, out Vector2Int? _);
        }

        private bool IsGridPositionInShelf(Vector2Int gridPosition)
        {
            if (gridPosition.y != 0)
            {
                return false;
            }

            if (gridPosition.x < 0 || gridPosition.x >= length)
            {
                return false;
            }

            return true;
        }

        public bool TryGetPieceAtPosition(Vector2Int gridPosition, out Piece piece)
        {
            piece = null;

            if (!IsGridPositionInShelf(gridPosition))
            {
                return false;
            }

            piece = shelfGrid.GetObjectInGridPosition(gridPosition);

            return piece != null;
        }

        public void RemovePiece(Vector2Int gridPosition)
        {
            if (!shelfGrid.RemoveObject(gridPosition))
            {
                // this should never happen
                throw new Exception($"Couldn't remove piece from gridPosition ({gridPosition})");
            }

            var selectStart = gridPosition.x + 1;
            ShiftPieces(selectStart, length - selectStart, -1);
        }

        private void ShiftPieces(int selectStart, int selectLength, int shiftDistance)
        {
            var pieces = new List<Piece>();
            
            for (var i = selectStart; i < selectStart + selectLength; i++)
            {
                var gridPos = new Vector2Int(i, 0);
                var piece = shelfGrid.GetObjectInGridPosition(gridPos);
                if (piece == null)
                {
                    continue;
                }

                shelfGrid.RemoveObject(gridPos);
                pieces.Add(piece);
            }

            for (var i = 0; i < pieces.Count; i++)
            {
                var piece = pieces[i];
                shelfGrid.PlaceObject(piece, new Vector2Int(selectStart + i + shiftDistance, 0));
            }
        }

        public void ReplacePiece(Piece piece, Vector2Int? gridPosition = null)
        {
            if (gridPosition == null)
            {
                gridPosition = new Vector2Int(length - 1, 0);
            }
            
            var selectStart = gridPosition.Value.x;
            ShiftPieces(selectStart, length - selectStart, +1);
            shelfGrid.PlaceObject(piece, gridPosition.Value);
        }

        public bool HasPieces()
        {
            return shelfGrid.All.Any();
        }

        private Piece InstantiatePiece(PieceDefinition definition)
        {
            var piece = Instantiate(piecePrefab);
            piece.SetDefinition(definition);
            
            return piece;
        }
    }
}