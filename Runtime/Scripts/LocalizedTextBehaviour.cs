using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Pinky.Localization
{
    public class LocalizedTextBehaviour : MonoBehaviour
    {
        [SerializeField]
        private string key;

        private TMP_Text tmpText;

        private void Awake()
        {
            tmpText = GetComponent<TMP_Text>();
            tmpText.text = LocalizationService.GetLocalizedText(key);
        }
    }
}