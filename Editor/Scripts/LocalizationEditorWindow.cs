using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Pinky.Localization.Containers;
using System;

namespace Pinky.Localization.Editor
{
    public class LocalizationEditorWindow : EditorWindow
    {
        private const string NOT_SELECTED = "Not selected";
        private LocalesTextContainer localesTextContainer;
        private Vector2 scroll = Vector2.zero;
        private string searchRequest = string.Empty;
        private List<string> keysToRemove;
        private string[] localesOptions;
        private int selectedLanguageIndex;

        [MenuItem("Localization/Open Table Editor")]
        public static void OpenWindow()
        {
            LocalizationEditorWindow window = GetWindow<LocalizationEditorWindow>();
            window.titleContent = new GUIContent("Localization Table Editor");
            window.minSize = new Vector2(512f, 812f);
            window.Show();
        }

        public static void WriteChangesToFile(TextAsset txtFile, Dictionary<string, string> updatedMap)
        {
            string path = AssetDatabase.GetAssetPath(txtFile);

            WriteChangesToFile(path, updatedMap);
        }

        public static void WriteChangesToFile(string path, Dictionary<string, string> updatedMap)
        {
            byte[] bytes = LocaleSerializer.Serialize(updatedMap);
            FileStream stream = new(path, FileMode.OpenOrCreate, FileAccess.Write);
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(bytes, 0, bytes.Length);
            stream.Close();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        public void OnGUI()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            selectedLanguageIndex = EditorGUILayout.Popup(new GUIContent("Language:"),selectedLanguageIndex, localesOptions);

            if (selectedLanguageIndex == 0)
            {
                GUILayout.EndVertical();
                return;
            }

            SystemLanguage currentKey = Enum.Parse<SystemLanguage>(localesOptions[selectedLanguageIndex]);

            localesTextContainer.TryGetLocalizationAsset(currentKey, out TextAsset localizationFile);

            if(!localizationFile)
            {
                Debug.LogError($"There is no text asset by {currentKey} key.");
                GUILayout.EndVertical();
                return;
            }

            Dictionary<string, string> localizationMap = LocaleSerializer.Deserialize(localizationFile);

            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            GUILayout.BeginHorizontal();
            searchRequest = EditorGUILayout.TextField(searchRequest, EditorStyles.toolbarSearchField);
            DrawAddButton(localizationMap);
            GUILayout.EndHorizontal();
            DrawTable(localizationMap);
            GUILayout.EndVertical();
        }

        private void DrawTable(Dictionary<string, string> localizationMap)
        {
            scroll = GUILayout.BeginScrollView(scroll);

            float widthUnit = position.width / 10f;
            float valueWidth = widthUnit * 7f;

            GUIContent keyContent = new("Key:");
            GUI.skin.label.CalcMinMaxWidth(keyContent, out float keyLabelMinWidth, out float keyLabelMaxWidth);

            GUIContent valueContent = new("Value:");
            GUI.skin.label.CalcMinMaxWidth(valueContent, out float valueLabelMinWidth, out float valueLabelMaxWidth);

            foreach (KeyValuePair<string, string> kvp in localizationMap)
            {
                bool shouldBeDisplayed = string.IsNullOrWhiteSpace(searchRequest) || kvp.Key.Contains(searchRequest, StringComparison.InvariantCultureIgnoreCase) || kvp.Value.Contains(searchRequest, StringComparison.InvariantCultureIgnoreCase);

                if (!shouldBeDisplayed)
                    continue;

                GUILayout.BeginHorizontal();
                
                GUILayout.Label(keyContent, GUILayout.MinWidth(keyLabelMinWidth), GUILayout.MaxWidth(keyLabelMaxWidth));
                EditorGUILayout.SelectableLabel(kvp.Key, EditorStyles.textField, GUILayout.MaxWidth(widthUnit * 2), GUILayout.Height(EditorGUIUtility.singleLineHeight));

                float textAreaMaxHeight = GUI.skin.textArea.CalcHeight(new GUIContent(kvp.Value), valueWidth) + EditorGUIUtility.standardVerticalSpacing * 2;
                GUILayout.Label(valueContent, GUILayout.MinWidth(valueLabelMinWidth), GUILayout.MaxWidth(valueLabelMaxWidth)); 
                EditorGUILayout.SelectableLabel(kvp.Value, EditorStyles.textArea, GUILayout.MaxHeight(textAreaMaxHeight));
                DrawEditButton(kvp, widthUnit);
                DrawRemoveButton(kvp.Key, widthUnit * 0.5f);
                GUILayout.EndHorizontal();
            }

            if(keysToRemove.Count > 0)
            {
                var allLocales = LocaleSerializer.DeserializeLocales();

                foreach (var kvp in allLocales) 
                {
                    keysToRemove.ForEach(key => kvp.Value.Remove(key));
                    WriteChangesToFile(localesTextContainer.GetLocalizationAsset(kvp.Key), kvp.Value);
                }

                keysToRemove.Clear();
            }

            GUILayout.EndScrollView();
        }

        private void DrawEditButton(KeyValuePair<string, string> kvp, float buttonWidth)
        {
            bool isPressed = GUILayout.Button("edit", EditorStyles.miniButtonLeft, GUILayout.MaxWidth(buttonWidth));

            if (!isPressed)
                return;

            KeyValueEditorWindow kvEditorWindow = GetWindow<KeyValueEditorWindow>();
            kvEditorWindow.Open(kvp, localesTextContainer.GetLocalizationAsset(Enum.Parse<SystemLanguage>(localesOptions[selectedLanguageIndex])), localesTextContainer);
        }

        private void DrawRemoveButton(string key, float buttonWidth)
        {
            bool isPressed = GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.MaxWidth(buttonWidth));

            if (!isPressed)
                return;

            keysToRemove.Add(key);            
        }

        private void DrawAddButton(Dictionary<string, string> localizationMap)
        {
            bool isPressed = GUILayout.Button("+", GUILayout.MaxWidth(position.width * 0.05f));

            if (!isPressed)
                return;

            if (localizationMap.ContainsKey(searchRequest))
            {
                Debug.LogWarning($"{searchRequest} is already in localization file. Make a new one.");
                return;
            }

            var allLocales = LocaleSerializer.DeserializeLocales();

            foreach (var kvp in allLocales)
            {
                kvp.Value.Add(searchRequest, string.Empty);
                WriteChangesToFile(localesTextContainer.GetLocalizationAsset(kvp.Key), kvp.Value);
            }
        }

        private void OnEnable()
        {
            keysToRemove = new List<string>(1);
            localesTextContainer = Resources.Load<LocalesTextContainer>("Localization/Locales Text Container");
            SystemLanguage[] keys = localesTextContainer.GetKeys();
            localesOptions = new string[keys.Length + 1];

            localesOptions[0] = NOT_SELECTED;

            for (int i = 1; i < localesOptions.Length; i++)
                localesOptions[i] = keys[i - 1].ToString();

            selectedLanguageIndex = 0;
        }

        private void OnDisable()
        {
            keysToRemove = null;
        }
    }
}