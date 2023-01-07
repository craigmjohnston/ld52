namespace Oatsbarley.GameJams.LD52.Pieces
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "BasicPiece", menuName = "Custom/Pieces/Basic Piece", order = 1)]
    public class BasicPiece : PieceDefinition
    {
        public override void Tick(SurroundingObjects<Piece> surroundingPieces)
        {
            
        }
    }
}