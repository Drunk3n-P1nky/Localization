using UnityEngine;
using UnityEngine.UI;

namespace Pinky.Localization
{
    public class LocalizedImageBehaviour : MonoBehaviour
    {
        [SerializeField]
        private string key;

        private Image image;

        private void Awake()
        {
            image = GetComponent<Image>();
            image.sprite = LocalizationService.GetLocalizedSprite(key);
        }
    }
}