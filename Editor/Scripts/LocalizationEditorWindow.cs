using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Pinky.Localization.Editor
{
    public class LocalizationEditorWindow : EditorWindow
    {
        private TextAsset currentCSVFile = null;
        private const string CSV_FORMAT = "csv";
        private Vector2 scroll = Vector2.zero;
        private string searchRequest = string.Empty;

        [MenuItem("Localization/Open Table Editor")]
        public static void OpenWindow()
        {
            LocalizationEditorWindow window = GetWindow<LocalizationEditorWindow>();
            window.titleContent = new GUIContent("Localization Table Editor");
            window.minSize = new Vector2(512f, 1024f);
        }

        public void OnGUI()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            currentCSVFile = (TextAsset)EditorGUILayout.ObjectField(currentCSVFile, typeof(TextAsset), false);

            if (currentCSVFile == null)
            {
                GUILayout.EndVertical();
                return;
            }

            string path = AssetDatabase.GetAssetPath(currentCSVFile);

            if (path.Split('.')[^1] != CSV_FORMAT)
            {
                Debug.LogError($"{currentCSVFile.name}'s extension is not .csv");
                currentCSVFile = null;
                GUILayout.EndVertical();
                return;
            }

            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            searchRequest = EditorGUILayout.TextField(searchRequest, EditorStyles.toolbarSearchField);
            DrawTable();
            GUILayout.EndVertical();
        }

        private void DrawTable()
        {
            scroll = GUILayout.BeginScrollView(scroll);
            Dictionary<string, string> localizationMap = CSVLoader.ParseCSV(currentCSVFile);
            float keyWidth = position.width / 3f;
            float valueWidth = keyWidth * 2f;

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
                EditorGUILayout.TextField(kvp.Key, GUILayout.MaxWidth(keyWidth));

                float textAreaMaxHeight = GUI.skin.textArea.CalcHeight(new GUIContent(kvp.Value), valueWidth) + EditorGUIUtility.standardVerticalSpacing * 2;
                GUILayout.Label(valueContent, GUILayout.MinWidth(valueLabelMinWidth), GUILayout.MaxWidth(valueLabelMaxWidth)); 
                EditorGUILayout.TextArea(kvp.Value, GUILayout.MaxHeight(textAreaMaxHeight));
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }
    }
}