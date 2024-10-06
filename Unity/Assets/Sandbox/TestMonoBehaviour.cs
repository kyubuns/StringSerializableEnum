using System;
using UnityEditor;
using UnityEngine;

namespace Sandbox
{
    public class TestMonoBehaviour : MonoBehaviour
    {
        [SerializeField] private StringSerializableTestEnum testEnum;

        public void Start()
        {
            Debug.Log($"Value is {testEnum.Value}");
        }
    }

    public enum TestEnum
    {
        Zero,
        One,
        Two,
    }
}

// Generated

namespace Sandbox
{
    [Serializable]
    public class StringSerializableTestEnum
    {
        [SerializeField] private string stringValue;
        private TestEnum? _cached;

        public TestEnum Value
        {
            get
            {
                if (_cached == null)
                {
                    _cached = stringValue switch
                    {
                        "Zero" => TestEnum.Zero,
                        "One" => TestEnum.One,
                        "Two" => TestEnum.Two,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }

                return _cached.Value;
            }
            set
            {
                _cached = value;
                stringValue = value switch
                {
                    TestEnum.Zero => "Zero",
                    TestEnum.One => "One",
                    TestEnum.Two => "Two",
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
    }

    [CustomPropertyDrawer(typeof(StringSerializableTestEnum))]
    public class StringSerializableTestEnumDrawer : PropertyDrawer
    {
        private static readonly string[] Values = { "Zero", "One", "Two" };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using var propertyScope = new EditorGUI.PropertyScope(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var p = property.FindPropertyRelative("stringValue");
            var currentValue = p.stringValue;
            var selectedIndex = Array.FindIndex(Values, x => x == currentValue);
            if (selectedIndex == -1)
            {
                var warningWidth = EditorGUIUtility.singleLineHeight;
                var popupPosition = new Rect(position.x, position.y, position.width - warningWidth - EditorGUIUtility.standardVerticalSpacing, position.height);
                var warningPosition = new Rect(position.x + position.width - warningWidth, position.y, position.width, position.height);
                p.stringValue = Values[EditorGUI.Popup(popupPosition, 0, Values)];
                EditorGUI.LabelField(warningPosition, new GUIContent("", EditorGUIUtility.IconContent("console.warnicon.sml").image, $"Invalid Value: {currentValue}"));
            }
            else
            {
                p.stringValue = Values[EditorGUI.Popup(position, selectedIndex, Values)];
            }
        }
    }
}