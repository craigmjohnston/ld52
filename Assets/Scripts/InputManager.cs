namespace Oatsbarley.GameJams.LD52
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class InputManager : MonoBehaviour
    {
        [SerializeField] private Camera inputCamera;
        [SerializeField] private PieceGridManager gridManager;
        [SerializeField] private PieceShelf pieceShelf;

        private Piece currentPiece;
        private Vector3 mouseOffset;

        private void Update()
        {
            if (currentPiece != null)
            {
                var mouseWorldPosition = inputCamera.ScreenToWorldPoint(Input.mousePosition);
                currentPiece.transform.position = mouseWorldPosition - mouseOffset;
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
            var mouseWorldPos = inputCamera.ScreenToWorldPoint(Input.mousePosition);

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
            var mouseWorldPos = inputCamera.ScreenToWorldPoint(Input.mousePosition);

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
            var mouseWorldPos = inputCamera.ScreenToWorldPoint(Input.mousePosition);
            var gridPos = gridManager.WorldPositionToGridPosition(mouseWorldPos);
            
            return gridPos;
        }
    }
}