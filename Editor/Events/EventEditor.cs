using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities.General.Events.Core
{
    [CustomEditor(typeof(Event<>), true)]
    public class EventEditor : Editor
    {
        private Type m_objetType = typeof(Object);
        private FieldInfo m_colorFieldInfo = null;
        private MethodInfo m_invokeMethodInfo = null;
        private Type[] m_genericArguments = null;
        
        private IEnumerable m_enumerable;
        
        private Vector2 m_scrollPosition;
        
        private void OnEnable()
        {
            var type = target.GetType();
            var baseType = type.BaseType;
            m_genericArguments = type == typeof(VoidEvent) ? Array.Empty<Type>() : baseType.GetGenericArguments();

            var bindingFlags = EventEditorUtilities.Binding_Flags;
            m_invokeMethodInfo = m_genericArguments.Any() ?
                baseType.GetMethod(EventEditorUtilities.Invoke_Method_Name, m_genericArguments):
                type.GetMethod(EventEditorUtilities.Invoke_Method_Name, EventEditorUtilities.Binding_Flags);
            
            var listenersFieldInfo = baseType.GetField(EventEditorUtilities.Listeners_Field_Name, bindingFlags); 
            var listeners = listenersFieldInfo.GetValue(target);
            if (listeners is IEnumerable enumerable)
                m_enumerable = enumerable;
            
            m_colorFieldInfo = baseType.GetField(EventEditorUtilities.Color_Field_Name, bindingFlags);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var oldColor = GUI.color;
            GUI.color = (Color)m_colorFieldInfo.GetValue(target);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.color = oldColor;
            if (GUILayout.Button(EventEditorUtilities.Invoke_Method_Name) && EditorApplication.isPlaying)
            {
                var parameters = m_invokeMethodInfo.GetParameters()
                    .Select(info => info.ParameterType)
                    .Select(Activator.CreateInstance)
                    .ToArray();
                m_invokeMethodInfo.Invoke(target, parameters);
            }
                
            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            GUILayout.Label("Subscribed listeners:");
            m_scrollPosition =  EditorGUILayout.BeginScrollView(m_scrollPosition,GUILayout.MaxHeight(400));
            var oldEnabled = GUI.enabled;
            GUI.enabled = false;
            foreach (var listener in m_enumerable)
            {
                if (listener is not Object current)
                {
                    EditorGUILayout.LabelField($"{listener.GetType().Name}");
                    continue;
                }
#if UNITY_6000_1_OR_NEWER
                EditorGUILayout.ObjectField(current, m_objetType, allowSceneObjects: true);
#else
                EditorGUILayout.ObjectField(current, m_objetType);
#endif
                GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            }
            GUI.enabled = oldEnabled;
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }
}