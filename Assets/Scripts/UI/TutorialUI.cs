namespace Oatsbarley.GameJams.LD52.UI
{
    using System.Collections.Generic;
    using NaughtyAttributes;
    using Newtonsoft.Json;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class TutorialUI : MonoBehaviour
    {
        [SerializeField] private TextAsset definitionJson;
        [SerializeField] private TutorialUIItem itemPrefab;

        [SerializeField] private TextMeshProUGUI bodyTextComponent;
        [SerializeField] private Transform itemsTransform;

        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private VerticalLayoutGroup layoutGroup;
        [SerializeField] private VerticalLayoutGroup itemsLayoutGroup;

        private List<TutorialUIItem> items = new List<TutorialUIItem>();

        [Button()]
        public void Show()
        {
            var json = definitionJson.text;
            var definition = JsonConvert.DeserializeObject<TutorialUIDefinition>(json);

            bodyTextComponent.text = definition.BodyText;

            foreach (var itemDefinition in definition.Items)
            {
                var item = InstantiateItem();
                item.transform.SetParent(itemsTransform, false);
                
                item.Show(itemDefinition);
                items.Add(item);
            }

            Canvas.ForceUpdateCanvases();
            layoutGroup.enabled = true;
            itemsLayoutGroup.enabled = true;

            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        [Button()]
        public void Hide()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            var toDestroy = items.ToArray();
            items.Clear();

            foreach (var item in toDestroy)
            {
                Object.Destroy(item.gameObject);
            }
        }

        private TutorialUIItem InstantiateItem()
        {
            return Instantiate(itemPrefab);
        }
    }
}