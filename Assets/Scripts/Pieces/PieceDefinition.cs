namespace Oatsbarley.GameJams.LD52.Pieces
{
    using UnityEngine;

    public abstract class PieceDefinition : ScriptableObject
    {
        public string Tag;
        public string Name;
        
        // visual
        public Sprite Sprite;
        public Color colour;
        public float scale = 1f;

        public virtual void OnPlace(Piece piece, SurroundingObjects<Piece> surroundingPieces)
        {
        }

        public virtual void OnReplace(Piece piece, SurroundingObjects<Piece> surroundingPieces)
        {
        }

        public virtual void Tick(Piece piece, SurroundingObjects<Piece> surroundingPieces)
        {
        }

        public virtual void AfterTick(Piece piece, SurroundingObjects<Piece> surroundingPieces)
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

    public static class PieceTag
    {
        public const string Leaf = "piece_leaf";
        public const string Stalk = "piece_stalk";
        public const string Sunshine = "piece_sunshine";
        public const string Seed = "piece_seed";
        
        // debug only
        public const string Bin = "piece_bin";
    }
}