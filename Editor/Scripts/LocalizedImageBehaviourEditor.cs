using Pinky.Localization.Containers;
using Pinky.Localization.Utility;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Pinky.Localization.Editor
{
    [CustomEditor(typeof(LocalizedImageBehaviour))]
    public class LocalizedImageBehaviourEditor : UnityEditor.Editor
    {
        private const string NOT_SELECTED = "Not Selected";
        private SerializedProperty keyProperty;
        private SystemLanguage language;
        private LocalizedImageBehaviour localizedImageBehaviour;
        private LocalesSpriteContainer localesTextContainer;
        private List<string> keys;
        private int selectedKeyIndex;

        public void OnEnable()
        {
            keyProperty = serializedObject.FindProperty("key");
            localizedImageBehaviour = (LocalizedImageBehaviour)target;
            localesTextContainer = Resources.Load<LocalesSpriteContainer>("Localization/Locales Sprite Container");

            keys = new List<string>();
            keys.Insert(0, NOT_SELECTED);
        }

        public void OnDisable()
        {
            keyProperty = null;
            localizedImageBehaviour = null;
            localesTextContainer = null;
            keys = null;
            selectedKeyIndex = 0;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            keys.Add(NOT_SELECTED);

            EditorGUILayout.BeginVertical();
            language = (SystemLanguage)EditorGUILayout.EnumPopup("Language:", language);

            if (!localesTextContainer.TryGetLocalizationAsset(language, out StringSpriteMapContainer container))
            {
                EditorGUILayout.EndVertical();
                return;
            }

            System.Reflection.FieldInfo fieldInfo = typeof(StringSpriteMapContainer).GetField("stringSpriteMap", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            keys.AddRange((fieldInfo.GetValue(container) as StringSpriteMap).Keys);
            selectedKeyIndex = Mathf.Clamp(keys.IndexOf(keyProperty.stringValue), 0, keys.Count - 1);
            EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);

            selectedKeyIndex = EditorGUILayout.Popup("Key:", selectedKeyIndex, keys.ToArray());
            keyProperty.stringValue = selectedKeyIndex == 0 ? string.Empty : keys[selectedKeyIndex];
            
            
            EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            DrawApplyButton(container);
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawApplyButton(StringSpriteMapContainer container)
        {
            bool isPressed = GUILayout.Button("Display", EditorStyles.miniButton);

            if (!isPressed)
                return;

            if (!localizedImageBehaviour.TryGetComponent(out UnityEngine.UI.Image image))
            {
                Debug.LogError($"{localizedImageBehaviour} doesn't have Image component on it.");
                return;
            }

            if (!container.TryGetSprite(keyProperty.stringValue, out Sprite localizedSprite))
            {
                Debug.LogError($"There is no {keyProperty.stringValue} key");
                return;
            }

            image.sprite = localizedSprite;
            SceneView.RepaintAll();
        }
    }
}