namespace Oatsbarley.GameJams.LD52.UI
{
    using TMPro;
    using UnityEngine;

    public class EndGameStatUIItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textComponent;

        public void SetText(string text)
        {
            textComponent.text = text;
        }
    }
}