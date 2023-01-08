namespace Oatsbarley.GameJams.LD52.UI
{
    using UnityEngine;

    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameObject tutorialButtonObject;

        private void Awake()
        {
            // just to make sure
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        public void Show()
        {
            tutorialButtonObject.SetActive(AppManager.Instance.HasShownTutorial);
            
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        public void Hide()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        
        public void OnPlayClicked()
        {
            AppManager.Instance.BeginGameplay();
        }

        public void OnShowTutorialClicked()
        {
            AppManager.Instance.ShowTutorial();
        }
    }
}