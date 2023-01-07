namespace Oatsbarley.GameJams.LD52
{
    using NaughtyAttributes;
    using UnityEngine;

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GridManager<Piece> grid;

        [Button]
        public void Tick()
        {
            foreach (var piece in grid.AllWithPositions)
            {
                var surrounding = grid.GetSurroundingObjects(piece.gridPos);
                (piece.obj).Definition.Tick(surrounding);
            }
        }
    }
}