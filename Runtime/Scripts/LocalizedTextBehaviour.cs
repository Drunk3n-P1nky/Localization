using UnityEngine;
using TMPro;

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
            tmpText.text = LocalizationSystem.GetLocalizedText(key);
        }
    }
}