namespace Oatsbarley.GameJams.LD52.Pieces
{
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "StalkPiece", menuName = "Custom/Pieces/Stalk Piece", order = 1)]
    public class StalkPiece : PieceDefinition
    {
        public override void Tick(Piece piece, SurroundingObjects<Piece> surroundingPieces)
        {
            if (surroundingPieces.Any(p => p.obj != null && p.obj.Definition.Name == "Wind"))
            {
                Debug.Log("Whoooooooooooo.....");
            }
        }

        public override bool CanPlace(SurroundingObjects<Piece> surroundingPieces)
        {
            return surroundingPieces.Any(p => p.obj != null && p.obj.Definition.Tag is "piece_stalk" or "piece_seed" && p.direction.y == -1);
        }
    }
}