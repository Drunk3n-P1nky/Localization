using Pinky.Localization.Containers;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Pinky.Localization.Editor
{
    public class KeyValueEditorWindow : EditorWindow
    {
        private LocalesTextContainer localesTextContainer;
        private Dictionary<string, string> localizationMap;
        private KeyValuePair<string, string> initialKVP;
        private TextAsset txtFile;
        private string changedKey;
        private string changedValue;

        public void Open(KeyValuePair<string, string> kvp, TextAsset txtFile, LocalesTextContainer localesTextContainer)
        {
            initialKVP = kvp;
            initialKVP.Deconstruct(out changedKey, out changedValue);
            this.txtFile = txtFile;
            localizationMap = LocaleSerializer.Deserialize(this.txtFile);
            this.localesTextContainer = localesTextContainer;
            titleContent = new GUIContent("Key Value Editor");
            minSize = new Vector2(512f, 256f);
            position = new Rect(Event.current.mousePosition, minSize);
            Show();
        }

        public void OnGUI()
        {
            float widthUnit = position.width / 6;
            GUIContent keyContent = new("Key:");
            GUI.skin.label.CalcMinMaxWidth(keyContent, out float keyLabelMinWidth, out float keyLabelMaxWidth);

            GUIContent valueContent = new("Value:");
            GUI.skin.label.CalcMinMaxWidth(valueContent, out float valueLabelMinWidth, out float valueLabelMaxWidth);

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label(keyContent, GUILayout.MinWidth(keyLabelMinWidth), GUILayout.MaxWidth(keyLabelMaxWidth));
            changedKey = EditorGUILayout.TextField(changedKey, GUILayout.MaxWidth(widthUnit * 2));

            float textAreaMaxHeight = GUI.skin.textArea.CalcHeight(new GUIContent(changedValue), widthUnit * 3) + EditorGUIUtility.standardVerticalSpacing * 2;
            GUILayout.Label(valueContent, GUILayout.MinWidth(valueLabelMinWidth), GUILayout.MaxWidth(valueLabelMaxWidth));
            changedValue = EditorGUILayout.TextArea(changedValue, GUILayout.MaxHeight(textAreaMaxHeight));
            GUILayout.EndHorizontal();

            Rect buttonRect = new(widthUnit * 5f, textAreaMaxHeight + EditorGUIUtility.singleLineHeight * 0.5f, widthUnit * 0.95f, EditorGUIUtility.singleLineHeight);
            DrawSaveButton(buttonRect);
            buttonRect.x -= widthUnit;
            DrawCancelButton(buttonRect);

            GUILayout.EndVertical();
        }

        private void DrawSaveButton(Rect buttonRect)
        {
            bool isPressed = GUI.Button(buttonRect, "Save");

            if (!isPressed)
                return;

            if(initialKVP.Key != changedKey)
            {
                if (localizationMap.ContainsKey(changedKey))
                {
                    PopupWindow.Show(buttonRect, new KeyValuePopupContent());
                    return;
                }

                var allLocales = LocaleSerializer.DeserializeLocales();

                foreach (var kvp in allLocales)
                {
                    string oldValue = kvp.Value[initialKVP.Key];
                    kvp.Value.Remove(initialKVP.Key);
                    kvp.Value.Add(changedKey, oldValue);
                    LocalizationEditorWindow.WriteChangesToFile(localesTextContainer.GetLocalizationAsset(kvp.Key), kvp.Value);
                }
            }

            localizationMap.Remove(initialKVP.Key);
            localizationMap.Add(changedKey, changedValue);

            LocalizationEditorWindow.WriteChangesToFile(txtFile, localizationMap);
            Close();
        }

        private void DrawCancelButton(Rect buttonRect)
        {
            bool isPressed = GUI.Button(buttonRect, "Cancel");

            if (!isPressed)
                return;

            if (!localizationMap.ContainsKey(changedKey))
            {
                changedKey = initialKVP.Key;
                changedValue = initialKVP.Value;
                return;
            }

            localizationMap.Remove(changedKey);
            localizationMap.Add(initialKVP.Key, initialKVP.Value);

            LocalizationEditorWindow.WriteChangesToFile(txtFile, localizationMap);

            changedKey = initialKVP.Key;
            changedValue = initialKVP.Value;
        }

        private void OnDisable()
        {
            txtFile = null;
            initialKVP = default;
            changedKey = null;
            changedValue = null;
            localizationMap = null;
        }
    }
}
