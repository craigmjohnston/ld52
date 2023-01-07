namespace Oatsbarley.GameJams.LD52
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class GridManager<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] private Grid grid;
        [SerializeField] private float zPosition;

        private Dictionary<Vector2Int, T> placedObjects = new Dictionary<Vector2Int, T>();

        public IEnumerable<T> All => placedObjects.Values;
        public IEnumerable<(Vector2Int gridPos, T obj)> AllWithPositions => placedObjects.Select(kvp => (kvp.Key, kvp.Value));

        private void Start()
        {
            grid.cellSize = new Vector3(GameManager.Instance.CellSize, GameManager.Instance.CellSize, GameManager.Instance.CellSize);
        }

        public T GetObjectInGridPosition(Vector2Int gridPosition)
        {
            if (placedObjects.TryGetValue(gridPosition, out T obj))
            {
                return obj;
            }

            return null;
        }

        public SurroundingObjects<T> GetSurroundingObjects(Vector2Int gridPosition)
        {
            return new SurroundingObjects<T>
            {
                gridPosition = gridPosition,
                subjectObject = GetObjectInGridPosition(gridPosition),
                above = GetObjectInGridPosition(gridPosition + Vector2Int.up),
                left = GetObjectInGridPosition(gridPosition + Vector2Int.left),
                right = GetObjectInGridPosition(gridPosition + Vector2Int.right),
                below = GetObjectInGridPosition(gridPosition + Vector2Int.down)
            };
        }

        public Vector2Int WorldPositionToGridPosition(Vector3 worldPosition)
        {
            var gridPos = grid.WorldToCell(worldPosition);
            return new Vector2Int(gridPos.x, gridPos.y);
        }
        
        public bool PlaceObject(T obj, Vector2Int gridPosition, bool allowOverlap = false)
        {
            if (!allowOverlap && GetObjectInGridPosition(gridPosition) != null)
            {
                return false;
            } 
            
            var worldPos = grid.GetCellCenterWorld(new Vector3Int(gridPosition.x, gridPosition.y));
            obj.transform.position = new Vector3(worldPos.x, worldPos.y, zPosition);

            placedObjects[gridPosition] = obj;

            return true;
        }

        public bool RemoveObject(Vector2Int gridPosition)
        {
            return placedObjects.Remove(gridPosition);
        }
    }

    public struct SurroundingObjects<T> : IEnumerable<(T obj, Vector2Int direction)>
    {
        public Vector2Int gridPosition;
        public T subjectObject;

        public T above;
        public T left;
        public T right;
        public T below;

        public IEnumerator<(T obj, Vector2Int direction)> GetEnumerator()
        {
            yield return (above, Vector2Int.up);
            yield return (left, Vector2Int.left);
            yield return (right, Vector2Int.right);
            yield return (below, Vector2Int.down);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}