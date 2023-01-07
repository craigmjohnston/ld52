namespace Oatsbarley.GameJams.LD52.Pieces
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "StalkPiece", menuName = "Custom/Pieces/Stalk Piece", order = 1)]
    public class StalkPiece : PieceDefinition
    {
        public override void Tick(SurroundingObjects<Piece> surroundingPieces)
        {
            if (surroundingPieces.Any(p => p != null && p.Definition.Name == "Wind"))
            {
                Debug.Log("Whoooooooooooo.....");
            }
        }
    }
}