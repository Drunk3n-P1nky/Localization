using UnityEditor;
using UnityEngine;

namespace Pinky.Localization.Editor
{
    public class KeyValuePopupContent : PopupWindowContent
    {
        public override Vector2 GetWindowSize()
        {
            return new Vector2(250f, 50f);
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.Label("You're trying to set the key,\nwhich localization file already has.\nPlease use another one.");
        }
    }
}