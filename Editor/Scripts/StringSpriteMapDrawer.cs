using UnityEngine;
using UnityEditor;
using Pinky.Localization.Utility;

namespace Pinky.Localization.Editor
{
    [CustomPropertyDrawer(typeof(StringSpriteMap))]
    public class StringSpriteMapDrawer : PropertyDrawer
    {
        private const string DEFAULT_KEY = "new key";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Object targetObject = property.serializedObject.targetObject;
            StringSpriteMap map = fieldInfo.GetValue(targetObject) as StringSpriteMap;
            return EditorGUIUtility.singleLineHeight * map.Count + EditorGUIUtility.standardVerticalSpacing * map.Count + EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            StringSpriteMap map = fieldInfo.GetValue(property.serializedObject.targetObject) as StringSpriteMap;

            EditorGUI.BeginProperty(position, label, property);
            Rect line = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            float oneFifth = position.width / 5;
            float twoFifth = oneFifth * 2f;

            SerializedProperty keysProperty = property.FindPropertyRelative("keys");
            SerializedProperty valuesProperty = property.FindPropertyRelative("values");
            GUIContent keyContent = new("Key:");
            GUIContent spriteContent = new("Sprite:");
            float keyContentWidth = GUI.skin.label.CalcSize(keyContent).x;
            float spriteContentWidth = GUI.skin.label.CalcSize(spriteContent).x;

            for (int index = 0; index < map.Count; index++)
            {
                SerializedProperty indexedKeyProperty = keysProperty.GetArrayElementAtIndex(index);
                SerializedProperty indexedValueProperty =valuesProperty.GetArrayElementAtIndex(index);

                EditorGUI.PrefixLabel(new Rect(line.x, line.y, twoFifth, line.height), keyContent, EditorStyles.label);
                EditorGUI.DelayedTextField(new Rect(line.x + keyContentWidth, line.y, twoFifth - keyContentWidth, line.height), indexedKeyProperty, GUIContent.none);
                EditorGUI.PrefixLabel(new Rect(line.x + twoFifth, line.y, twoFifth, line.y), spriteContent);
                EditorGUI.ObjectField(new Rect(line.x + twoFifth + spriteContentWidth, line.y, twoFifth - spriteContentWidth, line.height), indexedValueProperty, GUIContent.none);

                DrawRemoveButton(new Rect(line.x + twoFifth * 2, line.y, oneFifth, line.height), map, indexedKeyProperty.stringValue);
                line.y = line.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            DrawAddButton(new Rect(line.x + twoFifth, line.y, oneFifth, line.height), map);
            EditorGUI.EndProperty();
        }

        private void DrawRemoveButton(Rect rect, StringSpriteMap targetMap, string key)
        {
            bool isPressed = GUI.Button(rect, "-", EditorStyles.miniButtonMid);

            if (!isPressed)
                return;

            targetMap.Remove(key);
        }

        private void DrawAddButton(Rect rect, StringSpriteMap targetMap)
        {
            bool isPressed = GUI.Button(rect, "+", EditorStyles.miniButtonMid);

            if (!isPressed)
                return;

            string newKey = DEFAULT_KEY;
            int index = 1;

            while (targetMap.ContainsKey(newKey))
            {
                newKey = DEFAULT_KEY + ' ' + index;
                index++;
            }

            targetMap.Add(newKey, null);
        }
    }
}