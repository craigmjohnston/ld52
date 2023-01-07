namespace Oatsbarley.GameJams.LD52.Pieces
{
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "LeafPiece", menuName = "Custom/Pieces/Leaf Piece", order = 1)]
    public class LeafPiece : PieceDefinition
    {
        public override void Tick(Piece piece, SurroundingObjects<Piece> surroundingPieces)
        {
            if (surroundingPieces.Count(p => p.obj.Definition.Tag == "piece_sunshine") > 1)
            {
                // todo generate a new piece
            }
        }
    }
}