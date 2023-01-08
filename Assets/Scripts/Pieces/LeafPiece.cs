namespace Oatsbarley.GameJams.LD52.Pieces
{
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "LeafPiece", menuName = "Custom/Pieces/Leaf Piece", order = 1)]
    public class LeafPiece : PieceDefinition
    {
        public override void Tick(Piece piece, SurroundingObjects<Piece> surroundingPieces)
        {
            var surroundingSun = surroundingPieces.Count(p => p.obj != null && p.obj.Definition.Tag == PieceTag.Sunshine);

            // if (surroundingPieces.left != null && surroundingPieces.right != null)
            // {
            //     if (surroundingPieces.left.Definition.Tag == PieceTag.Stalk && !surroundingPieces.left.ReplacedThisTick)
            //     {
            //         if (surroundingPieces.right.Definition.Tag == PieceTag.Leaf)
            //         {
            //             GameManager.Instance.ReplacePiece(piece, PieceTag.Stalk);
            //             return;
            //         }
            //     }
            //     else if (surroundingPieces.right.Definition.Tag == PieceTag.Stalk && !surroundingPieces.right.ReplacedThisTick)
            //     {
            //         if (surroundingPieces.left.Definition.Tag == PieceTag.Leaf)
            //         {
            //             GameManager.Instance.ReplacePiece(piece, PieceTag.Stalk);
            //         }
            //     }
            // }
            
            if (surroundingSun == 0)
            {
                if (piece.GetStatus() == PieceStatus.Bad)
                {
                    GameManager.Instance.ConsumePiece(piece, surroundingPieces.gridPosition);
                    return;
                }
                
                piece.SetStatus(PieceStatus.Bad);
                return;
            }

            if (piece.GetStatus() == PieceStatus.Bad)
            {
                piece.ClearStatus();
                return;
            }
            
            if (surroundingSun > 1)
            {
                GameManager.Instance.GeneratePiece(PieceTag.Leaf);
            }
        }

        public override bool CanPlace(SurroundingObjects<Piece> surroundingPieces)
        {
            return surroundingPieces.Any(p => p.obj != null && p.obj.Definition.Tag is PieceTag.Stalk or PieceTag.Bin);
        }
    }
}