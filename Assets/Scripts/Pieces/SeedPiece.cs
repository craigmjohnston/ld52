namespace Oatsbarley.GameJams.LD52.Pieces
{
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "SeedPiece", menuName = "Custom/Pieces/Seed Piece", order = 1)]
    public class SeedPiece : PieceDefinition
    {
        public override void Tick(Piece piece, SurroundingObjects<Piece> surroundingPieces)
        {
            if (surroundingPieces.Any(p => p.obj.Definition.Tag == "piece_water"))
            {
                // todo spawn something
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
    }
}