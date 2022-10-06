#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ReadOnlyRenameFieldAttribute : PropertyAttribute
{
    public string Name { get; }

    public ReadOnlyRenameFieldAttribute(string name) => Name = name;

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ReadOnlyRenameFieldAttribute))]
    class FieldNameDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(true);
            string[] path = property.propertyPath.Split('.');
            bool isArray = path.Length > 1 && path[1] == "Array";

            if (!isArray && attribute is ReadOnlyRenameFieldAttribute fieldName)
                label.text = fieldName.Name;

            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndDisabledGroup();
        }
    }
#endif
}
