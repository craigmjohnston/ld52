namespace Oatsbarley.GameJams.LD52.Pieces
{
    using UnityEngine;
    
    // a piece that is consumed at the end of the scenario

    [CreateAssetMenu(fileName = "ConsumablePiece", menuName = "Custom/Pieces/Consumable Piece", order = 1)]
    public class ConsumablePiece : PieceDefinition
    {
        public override void AfterTick(Piece piece, SurroundingObjects<Piece> surroundingPieces)
        {
            GameManager.Instance.ConsumePiece(piece, surroundingPieces.gridPosition);
        }
    }
}