namespace Oatsbarley.GameJams.LD52
{
    using System;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Object = UnityEngine.Object;

    public class InputManager : MonoBehaviour
    {
        [SerializeField] private Camera inputCamera;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float cameraMoveSpeed;
        
        [SerializeField] private PieceGridManager gridManager;
        [SerializeField] private PieceShelf pieceShelf;
        
        [SerializeField] private float dragZLevel;
        [SerializeField] private float baseZLevel = 0f;

        public event Action AllPiecesUsed;

        private Piece currentPiece;
        private Vector3 mouseOffset;
        private Vector2 cameraVector;

        private void Update()
        {
            if (currentPiece != null)
            {
                var mouseWorldPos = GetMouseWorldPosition();
                var offsetPos = mouseWorldPos - mouseOffset;
                currentPiece.transform.position = new Vector3(offsetPos.x, offsetPos.y, dragZLevel);
            }
            
            cameraTransform.Translate(cameraVector * cameraMoveSpeed * Time.deltaTime);
        }

        public void Clear()
        {
            if (this.currentPiece == null)
            {
                return;
            }
            
            Object.Destroy(this.currentPiece.gameObject);
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

        public void CameraMoved(InputAction.CallbackContext context)
        {
            cameraVector = context.ReadValue<Vector2>();
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

            if (currentPiece == null)
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
            var cursorPosition = GetCursorPosition();
            var surrounding = gridManager.GetSurroundingObjects(cursorPosition);

            bool placedSuccessfully = true;

            if (cursorPosition.y < 0)
            {
                Debug.LogError("Can't place a piece at or below the seed level");
                placedSuccessfully = false;
            }
            else if (!currentPiece.Definition.CanPlace(surrounding))
            {
                Debug.LogError("Couldn't place piece in position, it doesn't want to go there.");
                placedSuccessfully = false;
            }
            else if (surrounding.Any(p => p.obj != null && !p.obj.Definition.CanPlaceOther(currentPiece, p.direction * -1, surrounding)))
            {
                Debug.LogError("Couldn't place piece in position, something else doesn't want it to go there.");
                placedSuccessfully = false;
            } 
            else if (!gridManager.PlaceObject(currentPiece, cursorPosition))
            {
                Debug.LogError("Couldn't place piece in position, something is already there.");
                placedSuccessfully = false;
            }

            if (!placedSuccessfully)
            {
                pieceShelf.ReplacePiece(currentPiece);
            }
            else
            {
                currentPiece.Definition.OnPlace(currentPiece, gridManager.GetSurroundingObjects(cursorPosition));
            }

            currentPiece = null;

            if (!pieceShelf.HasPieces())
            {
                AllPiecesUsed?.Invoke();
            }
            
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