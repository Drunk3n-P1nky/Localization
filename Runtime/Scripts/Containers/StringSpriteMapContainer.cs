using UnityEngine;

namespace Pinky.Localization.Utility
{
    [CreateAssetMenu(fileName = "String Sprite Map", menuName = "Pinky Localization/Create New String Sprite Map")]
    public sealed class StringSpriteMapContainer : ScriptableObject
    {
        [SerializeField]
        private StringSpriteMap stringSpriteMap;

        public bool TryGetSprite(string key, out Sprite sprite)
        {
            bool isSuccess = stringSpriteMap.TryGetValue(key, out sprite);
            return isSuccess;
        }

        public Sprite GetSprite(string key)
        {
            return stringSpriteMap[key];
        }
    }
}