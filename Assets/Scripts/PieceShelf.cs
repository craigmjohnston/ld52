namespace Oatsbarley.GameJams.LD52
{
    using System;
    using System.Collections.Generic;
    using NaughtyAttributes;
    using Oatsbarley.GameJams.LD52.Pieces;
    using UnityEngine;

    public class PieceShelf : MonoBehaviour
    {
        [SerializeField] private PieceGridManager shelfGrid;
        [SerializeField] private Piece piecePrefab;

        [SerializeField] private int length;

        [Header("Debugging (TO BE REMOVED)")] 
        [SerializeField] private PieceDefinition[] definitions;

        [Button()]
        public void FillShelf()
        {
            for (var i = 0; i < length; i++)
            {
                var piece = InstantiatePiece(definitions[i % definitions.Length]);
                shelfGrid.PlaceObject(piece, new Vector2Int(i, 0));
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

        public void ReplacePiece(Piece piece, Vector2Int gridPosition)
        {
            var selectStart = gridPosition.x;
            ShiftPieces(selectStart, length - selectStart, +1);
            shelfGrid.PlaceObject(piece, gridPosition);
        }

        private Piece InstantiatePiece(PieceDefinition definition)
        {
            var piece = Instantiate(piecePrefab);
            piece.SetDefinition(definition);
            
            return piece;
        }
    }
}