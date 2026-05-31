using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
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

        private GUIContent m_plusIcon;
        private GUIContent m_minusIcon;
        private GUIContent m_confirmIcon; // NEW

        private FieldInfo m_keysFieldInfo;

        private List<(bool visible, Key key, bool renaming, string pendingName)> m_keysFoldout = null;

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
            m_plusIcon    = EditorGUIUtility.IconContent("Toolbar Plus");
            m_minusIcon   = EditorGUIUtility.IconContent("Toolbar Minus");
            m_confirmIcon = EditorGUIUtility.IconContent("FilterSelectedOnly"); // NEW

            var type = target.GetType();
            m_keysFieldInfo = type.GetField("m_keys", BindingFlags.NonPublic | BindingFlags.Instance);

            GenerateKeyFoldout();
        }

        private void GenerateKeyFoldout()
        {
            var keys = m_keysFieldInfo.GetValue(target) as List<Key>;
            // CHANGED: tuple now includes renaming=false and pendingName=key.name
            m_keysFoldout = keys
                .Select(key => (visible: false, key, renaming: false, pendingName: key.name))
                .ToList();
        }

        private void OverrideKeys()
        {
            var keys = m_keysFieldInfo.GetValue(target) as List<Key>;
            keys.Clear();
            keys.AddRange(m_keysFoldout.Select(f => f.key));
        }

        private void CommitRename(int index)
        {
            var info = m_keysFoldout[index];
            if (string.IsNullOrWhiteSpace(info.pendingName) || info.pendingName == info.key.name)
            {
                m_keysFoldout[index] = (info.visible, info.key, false, info.key.name);
                return;
            }

            Undo.RecordObject(info.key, "Rename Key");
            info.key.name = info.pendingName;

            EditorUtility.SetDirty(target);
            EditorUtility.SetDirty(info.key);
            AssetDatabase.SaveAssets();

            m_keysFoldout[index] = (info.visible, info.key, false, info.pendingName);
        }

        public override void OnInspectorGUI()
        {
            var box = GUI.skin.box;
            EditorGUI.indentLevel++;

            for (var i = 0; i < m_keysFoldout.Count; i++)
            {
                var info = m_keysFoldout[i];

                EditorGUILayout.BeginVertical(box);
                EditorGUILayout.BeginHorizontal();

                if (info.renaming)
                {
                    var foldoutRect = GUILayoutUtility.GetRect(0, 16, GUILayout.ExpandWidth(false));   
                    info.visible = EditorGUI.Foldout(foldoutRect, info.visible, string.Empty);
                    
                    GUI.SetNextControlName($"RenameField_{i}");
                    var newName = EditorGUILayout.TextField(info.pendingName, GUILayout.ExpandWidth(true));
                    info = m_keysFoldout[i] = (info.visible, info.key, true, newName);
                    
                    if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
                    {
                        CommitRename(i);
                        GUIUtility.keyboardControl = 0;
                        break;
                    }

                    if (GUILayout.Button(m_confirmIcon, GUILayout.Width(50)))
                    {
                        CommitRename(i);
                        break;
                    }
                }
                else
                {
                    info.visible = EditorGUILayout.Foldout(info.visible, info.key.name);
                    
                    if (GUILayout.Button("✎", GUILayout.Width(28)))
                    {
                        m_keysFoldout[i] = (info.visible, info.key, true, info.key.name);
                        EditorGUI.FocusTextInControl($"RenameField_{i}");
                        break;
                    }
                }

                if (GUILayout.Button(m_minusIcon, GUILayout.Width(50)))
                {
                    m_keysFoldout.RemoveAt(i);
                    OverrideKeys();
                    AssetDatabase.RemoveObjectFromAsset(info.key);
                    AssetDatabase.SaveAssets();
                    break;
                }
                EditorGUILayout.EndHorizontal();

                if (info.visible)
                {
                    var editor = CreateEditor(info.key);
                    editor.OnInspectorGUI();
                }

                EditorGUILayout.EndVertical();

                if (i < m_keysFoldout.Count)
                {
                    var current = m_keysFoldout[i];
                    m_keysFoldout[i] = (info.visible, current.key, current.renaming, current.pendingName);
                }
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
                m_keysFoldout.Add((false, instance, false, m_newKeyName));
                OverrideKeys();
                serializedObject.ApplyModifiedProperties();
                AssetDatabase.AddObjectToAsset(instance, target);
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
                m_newKeyName = string.Empty;
            }
            GUI.enabled = oldEnabled;
            EditorGUILayout.EndHorizontal();
        }
    }
}
