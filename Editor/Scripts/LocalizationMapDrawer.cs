using Pinky.Localization.Utility;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Pinky.Localization.Editor
{
    [CustomPropertyDrawer(typeof(LocalizationTextMap))]
    public class LocalizationMapDrawer : PropertyDrawer
    {
        private const string TXT = "txt";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Object targetObject = property.serializedObject.targetObject;
            LocalizationTextMap map = fieldInfo.GetValue(targetObject) as LocalizationTextMap;
            return EditorGUIUtility.singleLineHeight * map.Count + EditorGUIUtility.standardVerticalSpacing * map.Count + EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LocalizationTextMap targetMap = fieldInfo.GetValue(property.serializedObject.targetObject) as LocalizationTextMap;

            EditorGUI.BeginProperty(position, GUIContent.none, property);

            Rect line = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            float oneFifth = position.width / 5;
            float twoFifth = oneFifth * 2f;

            SerializedProperty keysProperty = property.FindPropertyRelative("keys");
            SerializedProperty valuesProperty = property.FindPropertyRelative ("values");
            GUIContent languageContent = new("Language:");
            GUIContent csvContent = new("TXT:");
            float languageContentWidth = GUI.skin.label.CalcSize(languageContent).x;
            float csvContentWidth = GUI.skin.label.CalcSize(csvContent).x;

            for (int i = 0; i < targetMap.Count; i++)
            {
                SerializedProperty iKeyProperty = keysProperty.GetArrayElementAtIndex(i);
                EditorGUI.PrefixLabel(new Rect(line.x, line.y, twoFifth, line.height), languageContent, EditorStyles.label); 
                EditorGUI.PropertyField(new Rect(line.x + languageContentWidth, line.y, twoFifth - languageContentWidth, line.height), iKeyProperty, GUIContent.none);
                EditorGUI.PrefixLabel(new Rect(line.x + twoFifth, line.y, twoFifth, line.y), csvContent);
                EditorGUI.ObjectField(new Rect(line.x + twoFifth + csvContentWidth, line.y, twoFifth - csvContentWidth, line.height), valuesProperty.GetArrayElementAtIndex(i), GUIContent.none);
                DrawRemoveButton(new Rect(line.x + twoFifth * 2, line.y, oneFifth, line.height), targetMap, (SystemLanguage)iKeyProperty.intValue);
                line = new Rect(line.x, line.y + EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight, line.width, line.height);
            }

            DrawAddButton(new Rect(line.x + twoFifth, line.y, oneFifth, line.height), targetMap);
            Validate(targetMap);
            EditorGUI.EndProperty();
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

        private void DrawRemoveButton(Rect rect, LocalizationTextMap targetMap, SystemLanguage key) 
        {
            bool isPressed = GUI.Button(rect, "-", EditorStyles.miniButtonMid);

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

                if (path.Split('.')[^1] != TXT)
                {
                    Debug.LogError($"{kvp.Value.name} is not in .{TXT} format");
                    keysOfValuesToRemove.Add(kvp.Key);
                }
            }

            for (int i = 0; i < keysOfValuesToRemove.Count; i++)
                targetMap[keysOfValuesToRemove[i]] = null;
        }
    }
}