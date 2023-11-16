using Pinky.Localization.Containers;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Pinky.Localization.Editor
{
    [CustomEditor(typeof(LocalizedTextBehaviour))]
    public class LocalizedTextBehaviourEditor : UnityEditor.Editor
    {
        private const string NOT_SELECTED = "Not Selected";
        private SerializedProperty keyProperty;
        private SystemLanguage language;
        private LocalizedTextBehaviour localizedTextBehaviour;
        private LocalesTextContainer localesTextContainer;
        private List<string> keys;
        private int selectedKeyIndex;

        public void OnEnable()
        {
            keyProperty = serializedObject.FindProperty("key");
            localizedTextBehaviour = (LocalizedTextBehaviour)target;
            localesTextContainer = Resources.Load<LocalesTextContainer>("Locales Container");

            keys = new List<string>();
            keys.Insert(0, NOT_SELECTED);
        }

        public void OnDisable()
        {
            keyProperty = null;
            localizedTextBehaviour = null;
            localesTextContainer = null;
            keys = null;
            selectedKeyIndex = 0;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            keys.Add(NOT_SELECTED);
            keys.AddRange(TXTLoader.ParseTXT(TXTLoader.DefaultLanguage, localesTextContainer).Keys);
            selectedKeyIndex = Mathf.Clamp(keys.IndexOf(keyProperty.stringValue), 0, keys.Count - 1);

            EditorGUILayout.BeginVertical();
            selectedKeyIndex = EditorGUILayout.Popup("Key:", selectedKeyIndex, keys.ToArray());
            keyProperty.stringValue = selectedKeyIndex == 0 ? string.Empty : keys[selectedKeyIndex];
            EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            language = (SystemLanguage)EditorGUILayout.EnumPopup("Language:", language);
            EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            DrawApplyButton();
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawApplyButton()
        {
            bool isPressed = GUILayout.Button("Display", EditorStyles.miniButton);

            if (!isPressed)
                return;

            if(!localizedTextBehaviour.TryGetComponent(out TMPro.TMP_Text tmpText))
            {
                Debug.LogError($"{localizedTextBehaviour} doesn't have TMP_Text component on it. Add either TextMeshPro or TextMeshProUGUI");
                return;
            }

            Dictionary<string, string> localizationMap = TXTLoader.ParseTXT(language, localesTextContainer);

            if(!localizationMap.TryGetValue(keyProperty.stringValue, out string localizedText))
            {
                Debug.LogError($"There is no {keyProperty.stringValue} key");
                return;
            }

            tmpText.SetText(localizedText);
        }
    }
}