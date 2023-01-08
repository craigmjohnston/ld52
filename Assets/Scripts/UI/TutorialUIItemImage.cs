namespace Oatsbarley.GameJams.LD52.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class TutorialUIItemImage : MonoBehaviour
    {
        [SerializeField] private Image imageComponent;

        public void SetSprite(Sprite sprite)
        {
            imageComponent.sprite = sprite;
        }
    }
}