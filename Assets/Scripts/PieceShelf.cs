namespace Oatsbarley.GameJams.LD52
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Oatsbarley.GameJams.LD52.Definitions;
    using Oatsbarley.GameJams.LD52.Pieces;
    using UnityEngine;
    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;

    public class PieceShelf : MonoBehaviour
    {
        [SerializeField] private PieceGridManager shelfGrid;
        [SerializeField] private Piece piecePrefab;
        [SerializeField] private float shiftDuration;

        [SerializeField] private PieceDefinition[] definitions;

        public int Length => shelfGrid.All.Count();

        public PieceDefinition GetDefinition(string tag)
        {
            var definition = definitions.FirstOrDefault(d => d.Tag == tag);

            if (definition == null)
            {
                Debug.LogError($"Can't find piece definition with tag '{tag}'");
            }

            return definition;
        }

        public void FillShelf(string[] required, DropChance[] dropChances, int shelfLength)
        {
            var currentLength = Length;
            int count = 0;
            
            var requiredDefinitions = required.Select(r => definitions.FirstOrDefault(d => d.Tag == r));
            foreach (var def in requiredDefinitions)
            {
                var piece = InstantiatePiece(def);
                piece.transform.SetParent(transform);
                
                shelfGrid.PlaceObject(piece, new Vector2Int(currentLength + count, 0));
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

            for (int i = 0; i < shelfLength - count; i++)
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
                    piece.transform.SetParent(transform);
                    
                    shelfGrid.PlaceObject(piece, new Vector2Int(currentLength + count + i, 0));

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

        // public bool IsWorldPositionInShelf(Vector3 worldPosition)
        // {
        //     return TryGetGridPosition(worldPosition, out Vector2Int? _);
        // }

        private bool IsGridPositionInShelf(Vector2Int gridPosition)
        {
            if (gridPosition.y != 0)
            {
                return false;
            }

            if (gridPosition.x < 0 || gridPosition.x >= Length + 1) // allow an extra 1 so it can be dropped on the end
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
            var piece = shelfGrid.GetObjectInGridPosition(gridPosition);
            
            if (!shelfGrid.RemoveObject(gridPosition))
            {
                // this should never happen
                throw new Exception($"Couldn't remove piece from gridPosition ({gridPosition})");
            }

            piece.transform.SetParent(null, false);

            var selectStart = gridPosition.x + 1;
            ShiftPieces(
                selectStart, 
                Length - selectStart + 1, // +1 to account for the one we just removed
                -1);
        }

        private void ShiftPieces(int selectStart, int selectLength, int shiftDistance, bool shouldTween = true)
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
                shelfGrid.PlaceObject(piece, new Vector2Int(selectStart + i + shiftDistance, 0), tweenDuration:(shouldTween ? shiftDuration : null));
            }
        }

        public void GeneratePiece(string tag)
        {
            var piece = InstantiatePiece(tag);
            ReplacePiece(piece, new Vector2Int(Length, 0));
        }

        public void ReplacePiece(Piece piece, Vector2Int? gridPosition = null)
        {
            if (gridPosition == null)
            {
                gridPosition = new Vector2Int(Length, 0);
            }
            
            var selectStart = gridPosition.Value.x;
            ShiftPieces(selectStart, Length - selectStart, +1);
            shelfGrid.PlaceObject(piece, gridPosition.Value);
            
            piece.transform.SetParent(transform);
        }

        public bool HasPieces()
        {
            return shelfGrid.All.Any();
        }

        public Piece InstantiatePiece(string tag)
        {
            var def = definitions.FirstOrDefault(d => d.Tag == tag);
            if (def == null)
            {
                return null;
            }

            return InstantiatePiece(def);
        }

        public void EmptyShelf()
        {
            var pieces = shelfGrid.AllWithPositions.ToArray();
            foreach (var piece in pieces)
            {
                shelfGrid.RemoveObject(piece.gridPos);
                Object.Destroy(piece.obj.gameObject);
            }
        }

        private Piece InstantiatePiece(PieceDefinition definition)
        {
            var piece = Instantiate(piecePrefab);
            piece.SetDefinition(definition);
            
            return piece;
        }
    }
}