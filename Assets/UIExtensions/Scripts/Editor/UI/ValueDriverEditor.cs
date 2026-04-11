using eviltwo.UIExtensions.UI;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace eviltwo.UIExtensions
{
    [CustomEditor(typeof(ValueDriver), true)]
    public class ValueDriverEditor : SelectableEditor
    {
        private static readonly string[] FormatPresets =
        {
            "Preset...", "F", "F1", "F2", "F3", "D", "D2", "N", "N1", "N2", "P", "P1", "P2", "0", "0.0", "0.00"
        };

        private SerializedProperty _wholeNumbers;
        private SerializedProperty _hasMinValue;
        private SerializedProperty _minValue;
        private SerializedProperty _hasMaxValue;
        private SerializedProperty _maxValue;
        private SerializedProperty _deltaValue;
        private SerializedProperty _value;
        private SerializedProperty _direction;
        private SerializedProperty _onValueChanged;
        private SerializedProperty _eventStringFormat;
        private SerializedProperty _onValueChangedString;
        private SerializedProperty _invokeOnStart;

        protected override void OnEnable()
        {
            base.OnEnable();
            _wholeNumbers = serializedObject.FindProperty("_wholeNumbers");
            _hasMinValue = serializedObject.FindProperty("_hasMinValue");
            _minValue = serializedObject.FindProperty("_minValue");
            _hasMaxValue = serializedObject.FindProperty("_hasMaxValue");
            _maxValue = serializedObject.FindProperty("_maxValue");
            _deltaValue = serializedObject.FindProperty("_deltaValue");
            _value = serializedObject.FindProperty("_value");
            _direction = serializedObject.FindProperty("Direction");
            _onValueChanged = serializedObject.FindProperty("_onValueChanged");
            _eventStringFormat = serializedObject.FindProperty("_eventStringFormat");
            _onValueChangedString = serializedObject.FindProperty("_onValueChangedString");
            _invokeOnStart = serializedObject.FindProperty("_invokeOnStart");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.PropertyField(_wholeNumbers);
            DrawToggleWithValue(_hasMinValue, _minValue, "Min Value");
            DrawToggleWithValue(_hasMaxValue, _maxValue, "Max Value");
            EditorGUILayout.PropertyField(_deltaValue);
            EditorGUILayout.PropertyField(_value);
            EditorGUILayout.PropertyField(_direction);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_invokeOnStart);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_onValueChanged);
            DrawEventStringFormat();
            EditorGUILayout.PropertyField(_onValueChangedString);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawToggleWithValue(SerializedProperty toggleProp, SerializedProperty valueProp, string label)
        {
            var rect = EditorGUILayout.GetControlRect();
            var labelRect = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, rect.height);
            var toggleWidth = 16f;
            var toggleRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, toggleWidth, rect.height);
            var valueRect = new Rect(toggleRect.xMax + 2f, rect.y, rect.xMax - toggleRect.xMax - 2f, rect.height);

            EditorGUI.LabelField(labelRect, label);
            EditorGUI.PropertyField(toggleRect, toggleProp, GUIContent.none);
            using (new EditorGUI.DisabledScope(!toggleProp.boolValue))
            {
                EditorGUI.PropertyField(valueRect, valueProp, GUIContent.none);
            }
        }

        private void DrawEventStringFormat()
        {
            var rect = EditorGUILayout.GetControlRect();
            var labelRect = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, rect.height);
            var dropdownWidth = 100f;
            var textFieldRect = new Rect(
                rect.x + EditorGUIUtility.labelWidth,
                rect.y,
                rect.width - EditorGUIUtility.labelWidth - dropdownWidth - 2f,
                rect.height);
            var dropdownRect = new Rect(rect.xMax - dropdownWidth, rect.y, dropdownWidth, rect.height);

            EditorGUI.LabelField(labelRect, "Event String Format");

            EditorGUI.BeginChangeCheck();
            var newText = EditorGUI.TextField(textFieldRect, _eventStringFormat.stringValue);
            if (EditorGUI.EndChangeCheck())
            {
                _eventStringFormat.stringValue = newText;
            }

            var selectedIndex = EditorGUI.Popup(dropdownRect, 0, FormatPresets);
            if (selectedIndex > 0)
            {
                _eventStringFormat.stringValue = FormatPresets[selectedIndex];
                GUI.FocusControl(null);
            }
        }
    }
}
