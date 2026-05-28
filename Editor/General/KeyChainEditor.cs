using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;

namespace Utilities.General
{
    [CustomEditor(typeof(KeyChain)), InitializeOnLoad]
    public class KeyChainEditor : Editor
    {
        private static string[] m_typesNames = Array.Empty<string>();
        private static Type[] m_types = Array.Empty<Type>();
        
        private int m_selectedTypeIndex = 0;
        private string m_newKeyName = string.Empty;
        
        private List<Key> m_keys = new List<Key>(10);
        
        private  GUIContent m_plusIcon;
        private  GUIContent m_minusIcon;
        
        private FieldInfo m_keysFieldInfo;

        private List<(bool visible, Key key)> m_keysFoldout = null;
        
        static KeyChainEditor()
        {
            var keyType = typeof(Key);
            var keyTypes = TypeCache.GetTypesDerivedFrom(keyType);
            var types = Array.Empty<(string name, Type type)>()
                .Append((name: keyType.Name, type: keyType))
                .Concat(keyTypes.Select(type => (name: type.Name, type)));

            m_typesNames = types.Select(info => info.name).ToArray();
            m_types = types.Select(info => info.type).ToArray();
            
        }

        private void OnEnable()
        {
            m_plusIcon = EditorGUIUtility.IconContent("Toolbar Plus");
            m_minusIcon = EditorGUIUtility.IconContent("Toolbar Minus");
            
            var type = target.GetType();
            m_keysFieldInfo = type.GetField("m_keys", BindingFlags.NonPublic |  BindingFlags.Instance);
            
            GenerateKeyFoldout();
        }

        private void GenerateKeyFoldout()
        {
            var keys = m_keysFieldInfo.GetValue(target) as List<Key>;
            m_keysFoldout = keys.Select(key => (visible: false, key)).ToList();
        }

        private void OverrideKeys()
        {
            var keys = m_keysFieldInfo.GetValue(target) as List<Key>;
            keys.Clear();
            keys.AddRange(m_keysFoldout.Select(foldout => foldout.key));
        }

        public override void OnInspectorGUI()
        {
            var box = GUI.skin.box;
            EditorGUI.indentLevel++;

            for (int i = 0; i < m_keysFoldout.Count; i++)
            {
                var info = m_keysFoldout[i];

                EditorGUILayout.BeginVertical(box);

                EditorGUILayout.BeginHorizontal();
                
                info.visible = EditorGUILayout.Foldout(m_keysFoldout[i].visible, info.key.name);
                // EditorGUI.BeginChangeCheck();
                // info.key.name = EditorGUILayout.TextField(info.key.name);
                // if(EditorGUI.EndChangeCheck())
                //     AssetDatabase.SaveAssetIfDirty(this);
                if (GUILayout.Button(m_minusIcon, GUILayout.Width(50)))
                {
                    m_keysFoldout.RemoveAt(i);
                    OverrideKeys();
                    AssetDatabase.RemoveObjectFromAsset(info.key);
                    AssetDatabase.SaveAssetIfDirty(this);
                    AssetDatabase.SaveAssets();
                    break;
                }

                EditorGUILayout.EndHorizontal();

                if (m_keysFoldout[i].visible)
                {
                    var editor = CreateEditor(info.key);
                    editor.OnInspectorGUI();
                }

                EditorGUILayout.EndVertical();

                m_keysFoldout[i] = info;
            }
            EditorGUI.indentLevel--;
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("New key name:", GUILayout.Width(100));
            m_newKeyName = EditorGUILayout.TextField(m_newKeyName);
            m_selectedTypeIndex = EditorGUILayout.Popup(GUIContent.none, m_selectedTypeIndex, m_typesNames, GUILayout.Width(100));
            var oldEnabled = GUI.enabled;
            GUI.enabled = !string.IsNullOrEmpty(m_newKeyName);
            if (GUILayout.Button(m_plusIcon, GUILayout.Width(50)))
            {
                var instance = CreateInstance(m_types[m_selectedTypeIndex]) as Key;
                instance.name = m_newKeyName;
                m_keysFoldout.Add((false, instance));
                OverrideKeys();
                serializedObject.ApplyModifiedProperties();
                AssetDatabase.AddObjectToAsset(instance, target);
                AssetDatabase.SaveAssetIfDirty(this);
                AssetDatabase.SaveAssets();
            }
            GUI.enabled = oldEnabled;
            EditorGUILayout.EndHorizontal();
        }
    }
}