namespace Oatsbarley.GameJams.LD52.Pieces
{
    using UnityEngine;

    public abstract class PieceDefinition : ScriptableObject
    {
        public string Name;
        public Sprite Sprite;

        public abstract void Tick(SurroundingObjects<Piece> surroundingPieces);
    }
}