namespace Oatsbarley.GameJams.LD52
{
    using Oatsbarley.GameJams.LD52.Pieces;
    using UnityEngine;

    public class Piece : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        [SerializeField] private float scaleToUnits = 1f;
        
        public PieceDefinition Definition { get; private set; }
        public bool IsLocked { get; private set; }

        private void Start()
        {
            transform.localScale = Vector3.one * GameManager.Instance.CellSize;
        }

        public void SetDefinition(PieceDefinition definition)
        {
            Definition = definition;
            SetSprite(definition.Sprite);
        }

        public void Lock()
        {
            IsLocked = true;
            // todo some sort of visual indication
        }

        private void SetSprite(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
            
            Scale();
        }

        private void Scale()
        {
            var ppu = spriteRenderer.sprite.pixelsPerUnit;
            
            var spriteW = spriteRenderer.sprite.rect.width / ppu;
            var spriteH = spriteRenderer.sprite.rect.height / ppu;

            var aspectRatio = spriteH / spriteW;
            float scaleFactor = scaleToUnits / (aspectRatio <= 1 ? spriteW : spriteH);
            var scale = new Vector3(scaleFactor, scaleFactor * aspectRatio, 1);
            
            spriteRenderer.transform.localScale = scale;
        }
    }
}