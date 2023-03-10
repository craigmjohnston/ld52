namespace Oatsbarley.GameJams.LD52.UI
{
    using System.Linq;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class TutorialUIItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textComponent;
        [SerializeField] private TutorialUIItemImage imagePrefab;
        [SerializeField] private Transform imagesTransform;
        [SerializeField] private HorizontalLayoutGroup layoutGroup;

        public void Show(TutorialUIItemDefinition definition)
        {
            textComponent.text = definition.Description;

            for (var i = 0; i < definition.ImagePaths.Length; i++)
            {
                var imageComponent = InstantiateImage();
                imageComponent.transform.SetParent(imagesTransform, false);
                imageComponent.SetSprite(Resources.Load<Sprite>(definition.ImagePaths[i]));
            }

            if (!definition.ImagePaths.Any())
            {
                Object.Destroy(imagesTransform.gameObject);
            }
            
            Canvas.ForceUpdateCanvases();
            layoutGroup.enabled = true;
        }

        private TutorialUIItemImage InstantiateImage()
        {
            return Instantiate(imagePrefab);
        }
    }
}