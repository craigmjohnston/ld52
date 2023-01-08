namespace Oatsbarley.GameJams.LD52.UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class TutorialUIItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textComponent;
        [SerializeField] private Image imagePrefab;
        [SerializeField] private Transform imagesTransform;
        [SerializeField] private HorizontalLayoutGroup layoutGroup;

        public void Show(TutorialUIItemDefinition definition)
        {
            textComponent.text = definition.Description;

            for (var i = 0; i < definition.ImagePaths.Length; i++)
            {
                var imageComponent = InstantiateImage();
                imageComponent.transform.SetParent(imagesTransform, false);
                imageComponent.sprite = Resources.Load<Sprite>(definition.ImagePaths[i]);
            }
            
            Canvas.ForceUpdateCanvases();
            layoutGroup.enabled = true;
        }

        private Image InstantiateImage()
        {
            return Instantiate(imagePrefab);
        }
    }
}