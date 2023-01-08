namespace Oatsbarley.GameJams.LD52.Pieces
{
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "StalkPiece", menuName = "Custom/Pieces/Stalk Piece", order = 1)]
    public class StalkPiece : PieceDefinition
    {
        public override void Tick(Piece piece, SurroundingObjects<Piece> surroundingPieces)
        {
            // GameManager.Instance.GeneratePiece(PieceTag.Leaf);
        }

        public override void AfterTick(Piece piece, SurroundingObjects<Piece> surroundingPieces)
        {
            if (piece.ReplacedThisTick)
            {
                return;
            }
            
            var leafPieces = surroundingPieces
                .Where(p => p.obj != null && p.obj.Definition.Tag == PieceTag.Leaf)
                .ToArray();
            
            if (leafPieces.Length >= 2)
            {
                var randomPiece = leafPieces[Random.Range(0, leafPieces.Length)];
                GameManager.Instance.ReplacePiece(randomPiece.obj, surroundingPieces.gridPosition + randomPiece.direction, PieceTag.Stalk);
            }
        }

        public override bool CanPlace(SurroundingObjects<Piece> surroundingPieces)
        {
            return surroundingPieces.Any(p => p.obj != null && p.obj.Definition.Tag == PieceTag.Stalk);
        }
    }
}