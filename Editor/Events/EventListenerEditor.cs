using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Utilities.General.Events.Core
{
    [CustomEditor(typeof(EventListenerBehaviour), true)]
    public class EventListenerEditor : Editor
    {
        private Type m_baseType =  typeof(IEventListener);
        public PropertyInfo EventPropertyInfo = null;
        private FieldInfo m_eventColorFieldInfo = null;
        
        
        private void OnEnable()
        {
            EventPropertyInfo = target.GetType()
                .GetProperty(EventEditorUtilities.Event_Property_Name, 
                EventEditorUtilities.Binding_Flags);
            
            m_eventColorFieldInfo = EventPropertyInfo.PropertyType.GetField(
                EventEditorUtilities.Color_Field_Name, EventEditorUtilities.Binding_Flags);
        }


        public override void OnInspectorGUI()
        {
            var @event = EventPropertyInfo.GetValue(target);
            if (@event is null)
            {
                base.OnInspectorGUI();
                return;
            }
            
            var eventColor = (Color)m_eventColorFieldInfo.GetValue(@event);
            var oldColor = GUI.color;
            GUI.color = eventColor;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.color = oldColor;
            base.OnInspectorGUI();
            EditorGUILayout.EndVertical();
        }
    }
}