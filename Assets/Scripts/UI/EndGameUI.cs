namespace Oatsbarley.GameJams.LD52.UI
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public class EndGameUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private EndGameStatUIItem itemPrefab;
        [SerializeField] private Transform itemsTransform;

        private List<EndGameStatUIItem> items = new List<EndGameStatUIItem>();

        public event Action Hiding;

        private void Awake()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        public void Show(Dictionary<string, int> stats)
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            // ShowStat(stats, "stat_max_leaves", "You grew to {0} leaves in total");
            ShowStat(stats, "stat_max_stalks", "You grew to {0} branches in total");

            // debug
            foreach (var stat in stats)
            {
                Debug.Log($"Stat - {stat.Key}: {stat.Value}");
            }
        }

        private void ShowStat(Dictionary<string, int> stats, string key, string format)
        {
            if (!stats.TryGetValue(key, out int value))
            {
                value = 0;
            }

            var item = Instantiate(itemPrefab, itemsTransform, false);
            items.Add(item);

            var text = string.Format(format, value);
            item.SetText(text);
        }

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
            
            Hiding?.Invoke();
        }
    }
}