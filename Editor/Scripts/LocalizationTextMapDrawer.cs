using Pinky.Localization.Utility;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Pinky.Localization.Editor
{
    [CustomPropertyDrawer(typeof(LocalizationTextMap))]
    public class LocalizationTextMapDrawer : PropertyDrawer
    {
        private const string FORMAT = "bytes";
        private TextAsset[] localizationFiles;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            localizationFiles = Resources.LoadAll<TextAsset>("Localization");
            Object targetObject = property.serializedObject.targetObject;
            LocalizationTextMap map = fieldInfo.GetValue(targetObject) as LocalizationTextMap;
            return EditorGUIUtility.singleLineHeight * map.Count + EditorGUIUtility.standardVerticalSpacing * map.Count + EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LocalizationTextMap targetMap = fieldInfo.GetValue(property.serializedObject.targetObject) as LocalizationTextMap;

            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginProperty(position, GUIContent.none, property);

            Rect line = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            float oneFifth = position.width / 5;
            float twoFifth = oneFifth * 2f;

            SerializedProperty keysProperty = property.FindPropertyRelative("keys");
            SerializedProperty valuesProperty = property.FindPropertyRelative ("values");
            GUIContent languageContent = new("Language:");
            GUIContent txtContent = new("Binary:");
            float languageContentWidth = GUI.skin.label.CalcSize(languageContent).x;
            float txtContentWidth = GUI.skin.label.CalcSize(txtContent).x;

            for (int i = 0; i < targetMap.Count; i++)
            {
                SerializedProperty iKeyProperty = keysProperty.GetArrayElementAtIndex(i);
                SerializedProperty iValueProperty = valuesProperty.GetArrayElementAtIndex(i);
                EditorGUI.PrefixLabel(new Rect(line.x, line.y, twoFifth, line.height), languageContent, EditorStyles.label); 
                EditorGUI.PropertyField(new Rect(line.x + languageContentWidth, line.y, twoFifth - languageContentWidth, line.height), iKeyProperty, GUIContent.none);
                EditorGUI.PrefixLabel(new Rect(line.x + twoFifth, line.y, twoFifth, line.y), txtContent);

                string fileName = ((SystemLanguage)iKeyProperty.intValue).ToString()[0..3].ToLower();
                bool isLocalizationFileFound = TryFindFile(fileName, iValueProperty);
                EditorGUI.ObjectField(new Rect(line.x + twoFifth + txtContentWidth, line.y, twoFifth - txtContentWidth, line.height), iValueProperty, GUIContent.none);

                if (isLocalizationFileFound)
                {
                    DrawRemoveButton(new Rect(line.x + twoFifth * 2, line.y, oneFifth, line.height), targetMap, (SystemLanguage)iKeyProperty.intValue, EditorStyles.miniButtonMid);
                }
                else
                {
                    DrawCreateButton(new Rect(line.x + twoFifth * 2, line.y, oneFifth * 0.5f, line.height), iValueProperty, fileName);
                    DrawRemoveButton(new Rect(line.x + twoFifth * 2 + oneFifth * 0.5f, line.y, oneFifth * 0.5f, line.height), targetMap, (SystemLanguage)iKeyProperty.intValue, EditorStyles.miniButtonRight);
                }

                line = new Rect(line.x, line.y + EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight, line.width, line.height);
            }

            DrawAddButton(new Rect(line.x + twoFifth, line.y, oneFifth, line.height), targetMap);
            Validate(targetMap);
            EditorGUI.EndProperty();
            if (EditorGUI.EndChangeCheck())
            {
                localizationFiles = Resources.LoadAll<TextAsset>("Localization");
            }
        }

        private void DrawAddButton(Rect rect, LocalizationTextMap targetMap)
        {
            bool isPressed = GUI.Button(rect, "+", EditorStyles.miniButtonMid);

            if (!isPressed) 
                return;

            bool isSuccess = targetMap.TryAdd(SystemLanguage.Unknown, null);

            if (!isSuccess) 
                throw new System.Exception($"You already have {nameof(SystemLanguage.Unknown)} key. Change it before adding new one");
        }

        private void DrawRemoveButton(Rect rect, LocalizationTextMap targetMap, SystemLanguage key, GUIStyle buttonStyle) 
        {
            bool isPressed = GUI.Button(rect, "-", buttonStyle);

            if (!isPressed) 
                return;

            targetMap.Remove(key);
        }

        private void Validate(LocalizationTextMap targetMap)
        {
            List<SystemLanguage> keysOfValuesToRemove = new();

            foreach (KeyValuePair<SystemLanguage, TextAsset> kvp in targetMap)
            {
                if(kvp.Value == null)
                    continue;

                string path = AssetDatabase.GetAssetPath(kvp.Value);

                if (path.Split('.')[^1] != FORMAT)
                {
                    Debug.LogError($"{kvp.Value.name} is not in .{FORMAT} format");
                    keysOfValuesToRemove.Add(kvp.Key);
                }
            }

            for (int i = 0; i < keysOfValuesToRemove.Count; i++)
                targetMap[keysOfValuesToRemove[i]] = null;
        }

        private bool TryFindFile(string fileName, SerializedProperty valueProperty)
        {
            TextAsset targetAsset = System.Array.Find(localizationFiles, asset => asset.name == fileName);

            if (targetAsset)
            {
                valueProperty.objectReferenceValue = targetAsset;
                return true;
            }

            return false;
        }

        private void DrawCreateButton(Rect buttonRect, SerializedProperty valueProperty, string futureFileName)
        {
            bool isPressed = GUI.Button(buttonRect, "create", EditorStyles.miniButtonLeft);

            if (!isPressed)
                return;

            string path = $"Resources/Localization/{futureFileName}.{FORMAT}";

            var map = localizationFiles.Length > 0 ? LocaleSerializer.Deserialize(localizationFiles[0]) : new Dictionary<string, string>();

            Dictionary<string, string> mapWithoutValues = new();

            foreach (var key in map.Keys)
                mapWithoutValues[key] = string.Empty;

            LocalizationEditorWindow.WriteChangesToFile(Application.dataPath + '/' + path, mapWithoutValues);
            valueProperty.objectReferenceValue = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/" + path);
        }
    }
}
