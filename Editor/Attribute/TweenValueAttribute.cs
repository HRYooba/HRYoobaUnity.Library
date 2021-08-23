using System;
using UnityEngine;
using UnityEditor;
using HRYooba.Library;

namespace HRYooba.Editor
{
    [CustomPropertyDrawer(typeof(FloatTweenProperty))]
    public class FloatTweenPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property.FindPropertyRelative("_value"), label);
        }
    }

    [CustomPropertyDrawer(typeof(Vector2TweenProperty))]
    public class Vector2TweenPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property.FindPropertyRelative("_value"), label);
        }
    }

    [CustomPropertyDrawer(typeof(Vector3TweenProperty))]
    public class Vector3TweenPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property.FindPropertyRelative("_value"), label);
        }
    }

    [CustomPropertyDrawer(typeof(ColorTweenProperty))]
    public class ColorTweenPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property.FindPropertyRelative("_value"), label);
        }
    }
}