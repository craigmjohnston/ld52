namespace Oatsbarley.GameJams.LD52
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class InputManager : MonoBehaviour
    {
        [SerializeField] private Camera inputCamera;
        [SerializeField] private PieceGridManager gridManager;
        [SerializeField] private PieceShelf pieceShelf;
        
        [SerializeField] private float dragZLevel;
        [SerializeField] private float baseZLevel = 0f;

        private Piece currentPiece;
        private Vector3 mouseOffset;

        private void Update()
        {
            if (currentPiece != null)
            {
                var mouseWorldPos = GetMouseWorldPosition();
                var offsetPos = mouseWorldPos - mouseOffset;
                currentPiece.transform.position = new Vector3(offsetPos.x, offsetPos.y, dragZLevel);
            }
        }
        
        public void PlayerClicked(InputAction.CallbackContext context)
        {
            if (!context.canceled)
            {
                if (!context.performed)
                {
                    MouseDown();
                }
                
                return;
            }
            
            MouseUp();
        }

        private void MouseDown()
        {
            if (TrySelectShelfPiece())
            {
                return;
            }
        }

        private void MouseUp()
        {
            if (TryReplaceCurrentPieceIntoShelf())
            {
                return;
            }
            
            if (TryPlaceCurrentPiece())
            {
                return;
            }
        }

        private bool TrySelectShelfPiece()
        {
            var mouseWorldPos = GetMouseWorldPosition();

            if (!pieceShelf.TryGetGridPosition(mouseWorldPos, out var gridPos))
            {
                return false;
            }
            
            if (!pieceShelf.TryGetPieceAtPosition(gridPos.Value, out var piece))
            {
                return false;
            }

            currentPiece = piece;
            mouseOffset = mouseWorldPos - piece.transform.position;
            pieceShelf.RemovePiece(gridPos.Value);
            
            return true;
        }

        private bool TryReplaceCurrentPieceIntoShelf()
        {
            var mouseWorldPos = GetMouseWorldPosition();

            if (!pieceShelf.TryGetGridPosition(mouseWorldPos, out var gridPos))
            {
                return false;
            }
            
            pieceShelf.ReplacePiece(currentPiece, gridPos.Value);
            this.currentPiece = null;

            return true;
        }

        private bool TryPlaceCurrentPiece()
        {
            if (currentPiece == null)
            {
                return false;
            }
            
            var cursorPosition = GetCursorPosition();
            if (!gridManager.PlaceObject(currentPiece, cursorPosition))
            {
                Debug.LogError("Couldn't place piece in position, something is already there.");
            }

            currentPiece = null;
            return true;
        }

        private Vector2Int GetCursorPosition()
        {
            // todo ability to get "cursor position" for gamepads/keyboard
            var mouseWorldPos = GetMouseWorldPosition();
            var gridPos = gridManager.WorldPositionToGridPosition(mouseWorldPos);
            
            return gridPos;
        }

        private Vector3 GetMouseWorldPosition()
        {
            var mousePos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, baseZLevel));
            float distance;
            xy.Raycast(ray, out distance);
            return ray.GetPoint(distance);
        }
    }
}