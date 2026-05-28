using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Event = UnityEngine.Event;

namespace Utilities.General
{
#if UNITY_2023_1_OR_NEWER
    [CustomPropertyDrawer(typeof(ReferenceAttribute))]
#endif  
    public class ReferenceAttributePropertyDrawer : PropertyDrawer
    {
        private TypeProvider m_typeProvider = null;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (m_typeProvider == null)
            {
                m_typeProvider = ScriptableObject.CreateInstance<TypeProvider>();
                m_typeProvider.GenerateTreeEntries(fieldInfo.FieldType);
            }
            
            var propertyHeight = EditorGUI.GetPropertyHeight(property, label, true);

            if (property.managedReferenceValue == null) return propertyHeight;
            
            propertyHeight = EditorGUI.GetPropertyHeight(property, label, property.isExpanded);
            propertyHeight += base.GetPropertyHeight(property, label);

            return propertyHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            var controlPosition = EditorGUI.PrefixLabel(position, label);

            if (property.managedReferenceValue != null)
            {
                var typeName = property.managedReferenceValue.GetType().Name;
                var propertyLabel = new GUIContent(typeName);
                EditorGUIUtility.labelWidth = controlPosition.width * .4f;
                EditorGUI.indentLevel = 1;
                var isExpanded = property.isExpanded;
                if (!isExpanded)
                    controlPosition.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(controlPosition, property, propertyLabel, isExpanded);
                controlPosition.y += EditorGUI.GetPropertyHeight(property, label);
                controlPosition.height = EditorGUIUtility.singleLineHeight;
            }

            EditorGUI.indentLevel = 0;
            controlPosition.width /= 2f;
            
            if (GUI.Button(controlPosition, "Select Type"))
            {
                var mousePosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                var context = new SearchWindowContext(mousePosition);
                m_typeProvider.Property = property;
                SearchWindow.Open(context, m_typeProvider);
            }
            
            controlPosition.x += controlPosition.width;
            if (GUI.Button(controlPosition, "Clear"))
            {
                property.managedReferenceValue = null;
                ReferencePropertyDroverHelper.SaveAndReserialize(property.serializedObject);
            }
            EditorGUI.EndProperty();
        }
    }
}