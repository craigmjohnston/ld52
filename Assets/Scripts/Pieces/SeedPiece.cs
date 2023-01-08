namespace Oatsbarley.GameJams.LD52.Pieces
{
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "SeedPiece", menuName = "Custom/Pieces/Seed Piece", order = 1)]
    public class SeedPiece : PieceDefinition
    {
        public override void OnPlace(Piece piece, SurroundingObjects<Piece> surroundingPieces)
        {
            piece.Lock();
        }

        public override void Tick(Piece piece, SurroundingObjects<Piece> surroundingPieces)
        {
            if (surroundingPieces.Any(p => p.obj != null && p.obj.Definition.Tag == PieceTag.Sunshine))
            {
                GameManager.Instance.GeneratePiece(PieceTag.Stalk);
                piece.Lock();
            }
        }

        public override bool CanPlace(SurroundingObjects<Piece> surroundingPieces)
        {
            if (surroundingPieces.Any(p => p.obj != null))
            {
                return false;
            }

            return true;
        }

        public override bool CanPlaceOther(Piece other, Vector2Int direction, SurroundingObjects<Piece> othersSurroundingPieces)
        {
            return other.Definition.Tag is PieceTag.Stalk or PieceTag.Sunshine && direction.y != 0;
        }
    }
}