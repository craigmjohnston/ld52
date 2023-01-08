namespace Oatsbarley.GameJams.LD52.Pieces
{
    using System.Linq;
    using UnityEngine;
    
    // a piece that consumes all its surrounding pieces

    [CreateAssetMenu(fileName = "BinPiece", menuName = "Custom/Pieces/Bin Piece", order = 1)]
    public class BinPiece : PieceDefinition
    {
        public override void Tick(Piece piece, SurroundingObjects<Piece> surroundingPieces)
        {
            var surrounding = surroundingPieces.Where(p => p.obj != null).ToArray();
            foreach (var surroundingPiece in surrounding)
            {
                GameManager.Instance.ConsumePiece(surroundingPiece.obj, surroundingPieces.gridPosition + surroundingPiece.direction);
            }
        }
    }
}