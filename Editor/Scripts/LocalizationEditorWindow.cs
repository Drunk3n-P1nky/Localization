using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace Pinky.Localization.Editor
{
    public class LocalizationEditorWindow : EditorWindow
    {
        private TextAsset currentTXTFile = null;
        private const string TXT = "txt";
        private Vector2 scroll = Vector2.zero;
        private string searchRequest = string.Empty;
        private List<string> keysToRemove;

        [MenuItem("Localization/Open Table Editor")]
        public static void OpenWindow()
        {
            LocalizationEditorWindow window = GetWindow<LocalizationEditorWindow>();
            window.titleContent = new GUIContent("Localization Table Editor");
            window.minSize = new Vector2(512f, 1024f);
            window.Show();
        }

        public static void WriteChangesToFile(TextAsset txtFile, Dictionary<string, string> updatedMap)
        {
            string path = AssetDatabase.GetAssetPath(txtFile);

            TextWriter tw = new StreamWriter(path, false);

            int lastLineIndex = updatedMap.Count - 1;
            int index = 0;

            string pattern = "\"{0}\", \"{1}\";";

            foreach (KeyValuePair<string, string> kvp in updatedMap)
            {
                if (index == lastLineIndex)
                    tw.Write(string.Format(pattern, kvp.Key, kvp.Value));
                else
                    tw.WriteLine(string.Format(pattern, kvp.Key, kvp.Value));

                index++;
            }

            tw.Close();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        public void OnGUI()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            currentTXTFile = (TextAsset)EditorGUILayout.ObjectField(currentTXTFile, typeof(TextAsset), false);

            if (currentTXTFile == null)
            {
                GUILayout.EndVertical();
                return;
            }

            string path = AssetDatabase.GetAssetPath(currentTXTFile);

            if (path.Split('.')[^1] != TXT)
            {
                Debug.LogWarning($"{currentTXTFile.name}'s extension is not .txt");
                currentTXTFile = null;
                GUILayout.EndVertical();
                return;
            }

            Dictionary<string, string> localizationMap = TXTLoader.ParseTXT(currentTXTFile);

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
                bool shouldBeDisplayed = string.IsNullOrWhiteSpace(searchRequest) || kvp.Key.Contains(searchRequest, System.StringComparison.InvariantCultureIgnoreCase) || kvp.Value.Contains(searchRequest, System.StringComparison.InvariantCultureIgnoreCase);

                if (!shouldBeDisplayed)
                    continue;

                GUILayout.BeginHorizontal();
                
                GUILayout.Label(keyContent, GUILayout.MinWidth(keyLabelMinWidth), GUILayout.MaxWidth(keyLabelMaxWidth));
                EditorGUILayout.TextField(kvp.Key, GUILayout.MaxWidth(widthUnit * 2));

                float textAreaMaxHeight = GUI.skin.textArea.CalcHeight(new GUIContent(kvp.Value), valueWidth) + EditorGUIUtility.standardVerticalSpacing * 2;
                GUILayout.Label(valueContent, GUILayout.MinWidth(valueLabelMinWidth), GUILayout.MaxWidth(valueLabelMaxWidth)); 
                EditorGUILayout.TextArea(kvp.Value, GUILayout.MaxHeight(textAreaMaxHeight));
                DrawEditButton(kvp, widthUnit);
                DrawRemoveButton(kvp.Key, widthUnit * 0.5f);
                GUILayout.EndHorizontal();
            }

            if(keysToRemove.Count > 0)
            {
                keysToRemove.ForEach(key => localizationMap.Remove(key));
                keysToRemove.Clear();
                WriteChangesToFile(currentTXTFile, localizationMap);
            }

            GUILayout.EndScrollView();
        }

        private void DrawEditButton(KeyValuePair<string, string> kvp, float buttonWidth)
        {
            bool isPressed = GUILayout.Button("edit", EditorStyles.miniButtonLeft, GUILayout.MaxWidth(buttonWidth));

            if (!isPressed)
                return;

            KeyValueEditorWindow kvEditorWindow = GetWindow<KeyValueEditorWindow>();
            kvEditorWindow.Open(kvp, currentTXTFile);
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

            localizationMap.Add(searchRequest, string.Empty);

            WriteChangesToFile(currentTXTFile, localizationMap);
        }

        private void OnEnable()
        {
            keysToRemove = new List<string>(1);
        }

        private void OnDisable()
        {
            currentTXTFile = null;
            keysToRemove = null;
        }
    }
}