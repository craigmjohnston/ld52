namespace Oatsbarley.GameJams.LD52.Pieces
{
    using UnityEngine;

    public abstract class PieceDefinition : ScriptableObject
    {
        public string Tag;
        public string Name;
        public Sprite Sprite;

        public virtual void Tick(Piece piece, SurroundingObjects<Piece> surroundingPieces)
        {
        }

        public virtual bool CanPlace(SurroundingObjects<Piece> surroundingPieces) // todo return a description of why not
        {
            return true;
        }

        public virtual bool CanPlaceOther(Piece other, Vector2Int direction, SurroundingObjects<Piece> othersSurroundingPieces) // todo return a description of why not
        {
            return true;
        }
    }
}