namespace Oatsbarley.GameJams.LD52
{
    using System.Linq;
    using Oatsbarley.GameJams.LD52.Pieces;
    using UnityEngine;

    public enum PieceStatus
    {
        None,
        Bad
    }

    public class Piece : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteRenderer statusSpriteRenderer;

        [SerializeField] private float scaleToUnits = 1f;

        private PieceStatus status = PieceStatus.None;
        
        public PieceDefinition Definition { get; private set; }
        public bool IsLocked { get; private set; }
        public bool ReplacedThisTick { get; private set; }

        private void Start()
        {
            transform.localScale *= GameManager.Instance.CellSize;
        }

        public void SetDefinition(PieceDefinition definition)
        {
            Definition = definition;
            SetSprite(definition.Sprite);
            statusSpriteRenderer.enabled = false;
        }

        public PieceStatus GetStatus()
        {
            return status;
        }

        public void SetStatus(PieceStatus status)
        {
            var statusDefinition = GameManager.Instance.GetStatusDefinition(status);
            this.status = status;

            statusSpriteRenderer.color = statusDefinition.Colour;
            statusSpriteRenderer.enabled = true;
        }

        public void ClearStatus()
        {
            status = PieceStatus.None;
            statusSpriteRenderer.enabled = false;
        }

        public void Lock()
        {
            IsLocked = true;
            // todo some sort of visual indication
        }

        public void ReplaceDefinition(PieceDefinition definition)
        {
            ReplacedThisTick = true;
            SetDefinition(definition);

            if (IsLocked)
            {
                this.IsLocked = false;
                // todo visual
            }
        }

        public void OnTick()
        {
            ReplacedThisTick = false;
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