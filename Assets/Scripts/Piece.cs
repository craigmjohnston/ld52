namespace Oatsbarley.GameJams.LD52
{
    using System;
    using DG.Tweening;
    using NaughtyAttributes;
    using Oatsbarley.GameJams.LD52.Pieces;
    using UnityEngine;

    public enum PieceStatus
    {
        None,
        Bad
    }

    [Serializable]
    public class PieceSide
    {
        public SpriteRenderer spriteRenderer;
        public SpriteRenderer statusSpriteRenderer;

        public void SetDefinition(PieceDefinition definition, float scaleToUnits)
        {
            spriteRenderer.sprite = definition.Sprite;
            spriteRenderer.color = definition.colour;
            
            Scale(scaleToUnits, definition.scale);
        }

        private void Scale(float scaleToUnits, float spriteScale)
        {
            var ppu = spriteRenderer.sprite.pixelsPerUnit;
            
            var spriteW = spriteRenderer.sprite.rect.width / ppu;
            var spriteH = spriteRenderer.sprite.rect.height / ppu;

            var aspectRatio = spriteH / spriteW;
            float scaleFactor = scaleToUnits / (aspectRatio <= 1 ? spriteW : spriteH);
            var scale = new Vector3(scaleFactor, scaleFactor * aspectRatio, 1);
            
            spriteRenderer.transform.localScale = scale * spriteScale;
        }
    }

    public class Piece : MonoBehaviour
    {
        [SerializeField] private PieceSide[] sides;
        [SerializeField] private float scaleToUnits = 1f;
        [SerializeField] private float flipSpeed = 1f;
        [SerializeField] private MeshRenderer meshRenderer;

        private PieceStatus status = PieceStatus.None;
        
        public PieceDefinition Definition { get; private set; }
        public bool IsLocked { get; private set; }
        public bool ReplacedThisTick { get; private set; }
        
        private int currentSideIndex = 0;

        private PieceSide CurrentSide => sides[currentSideIndex];
        private PieceSide OtherSide => sides[(currentSideIndex + 1) % 2];

        private void Start()
        {
            transform.localScale *= GameManager.Instance.CellSize;
        }

        public void SetDefinition(PieceDefinition definition, PieceSide side = null)
        {
            side ??= CurrentSide;
            
            Definition = definition;
            side.SetDefinition(definition, scaleToUnits);
            side.statusSpriteRenderer.enabled = false;
        }

        public PieceStatus GetStatus()
        {
            return status;
        }

        public void SetStatus(PieceStatus status)
        {
            var statusDefinition = GameManager.Instance.GetStatusDefinition(status);
            this.status = status;

            sides[currentSideIndex].statusSpriteRenderer.color = statusDefinition.Colour;
            sides[currentSideIndex].statusSpriteRenderer.enabled = true;
        }

        public void ClearStatus()
        {
            status = PieceStatus.None;
            sides[currentSideIndex].statusSpriteRenderer.enabled = false;
        }

        public void Lock()
        {
            IsLocked = true;
            
            this.meshRenderer.enabled = false;
            this.OtherSide.spriteRenderer.enabled = false;
            this.OtherSide.statusSpriteRenderer.enabled = false;
        }

        public void ReplaceDefinition(PieceDefinition definition)
        {
            ReplacedThisTick = true;
            SetDefinition(definition, OtherSide);

            if (IsLocked)
            {
                this.IsLocked = false;
                // todo visual
            }
            
            Flip();
        }

        public void OnTick()
        {
            ReplacedThisTick = false;
        }
        
        [Button()]
        public void Flip()
        {
            transform.DORotate(Vector3.up * 180f, flipSpeed, RotateMode.LocalAxisAdd).SetEase(Ease.InOutQuad);
            
            currentSideIndex = currentSideIndex == 0 ? 1 : 0;
        }
    }
}