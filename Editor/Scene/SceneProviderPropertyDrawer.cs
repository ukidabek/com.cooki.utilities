using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utilities.SceneManagement
{
    [CustomPropertyDrawer(typeof(SceneProvider))]
    public class SceneProviderPropertyDrawer : PropertyDrawer
    {
        private SceneAsset m_sceneAsset = null;

        private SerializedProperty m_guidProperty = null;
        private SerializedProperty m_nameProperty = null;
        private SerializedProperty m_pathProperty = null;
        private SerializedProperty m_useScenePathProperty = null;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            m_guidProperty = property.FindPropertyRelative("m_guid");
            m_nameProperty = property.FindPropertyRelative("m_name");
            m_pathProperty = property.FindPropertyRelative("m_path");
            m_useScenePathProperty = property.FindPropertyRelative("m_useScenePath");
            
            return base.GetPropertyHeight(property, label) * 3;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var currentGuid = m_guidProperty.stringValue;
            var currentPath = m_pathProperty.stringValue;
            if (!string.IsNullOrEmpty(currentGuid))
            {
                currentPath = AssetDatabase.GUIDToAssetPath(currentGuid);
                m_sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(currentPath);
            }

            var objectFieldPosition = position;
            objectFieldPosition.height = EditorGUIUtility.singleLineHeight;
            m_sceneAsset = (SceneAsset)EditorGUI.ObjectField(objectFieldPosition, label, m_sceneAsset, typeof(SceneAsset), false);

            var path = AssetDatabase.GetAssetPath(m_sceneAsset);
            var guid = AssetDatabase.AssetPathToGUID(path);

            if (m_sceneAsset == null)
            {
                m_nameProperty.stringValue = m_guidProperty.stringValue = m_pathProperty.stringValue = string.Empty;
                ApplyChanges(property);
                return;
            }

            if (currentGuid != guid)
            {
                m_nameProperty.stringValue = m_sceneAsset.name;
                m_guidProperty.stringValue = guid;
                m_pathProperty.stringValue = path;
                
                ApplyChanges(property);
            }

            var buildIndexProperty = -1;
            var sceneCount = SceneManager.sceneCountInBuildSettings;
            for (var i = 0; i < sceneCount; i++)
            {
                var scene = EditorBuildSettings.scenes[i];
                if (scene.path != path) continue;
                buildIndexProperty = i;
                break;
            }
            
            var useScenePathPropertyPosition = position;
            useScenePathPropertyPosition.y += EditorGUIUtility.singleLineHeight;
            useScenePathPropertyPosition.height = EditorGUIUtility.singleLineHeight;
            
            EditorGUI.PropertyField(useScenePathPropertyPosition, m_useScenePathProperty);

            objectFieldPosition.x += EditorGUI.indentLevel * 14f;
            objectFieldPosition.y += EditorGUIUtility.singleLineHeight * 2;
            objectFieldPosition.width = EditorGUIUtility.labelWidth;

            var isBuildIndexValid = buildIndexProperty >= 0;
            var labelText = isBuildIndexValid ? "Scene is added to build setting." : "Scene is not in build settings.";
            var buttonText = isBuildIndexValid ? "Remove" : "Add";

            var buttonPosition = position;
            buttonPosition.x += objectFieldPosition.width;
            buttonPosition.y = objectFieldPosition.y;
            buttonPosition.height = EditorGUIUtility.singleLineHeight;
            buttonPosition.width = position.width - objectFieldPosition.width;

            var oldColor = GUI.color;
            GUI.color = isBuildIndexValid ? Color.green : Color.red;
            var leftStyle = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.MiddleLeft
            };
            // leftStyle.padding = new RectOffset(8, 8, 8, 8);
            GUI.Box(objectFieldPosition, labelText, leftStyle);
            GUI.color = oldColor;
            
            if (!GUI.Button(buttonPosition, buttonText)) return;

            var currentScene = new EditorBuildSettingsScene(currentPath, true);
            
            EditorBuildSettings.scenes = isBuildIndexValid switch
            {
                true => EditorBuildSettings.scenes.Where(scene => scene.CompareTo(currentScene) != 0).ToArray(),
                false => EditorBuildSettings.scenes.Concat(new[] { currentScene }).ToArray()
            };
        }

        private static void ApplyChanges(SerializedProperty property)
        {
            var serializedObject = property.serializedObject;
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(serializedObject.targetObject);
        }
    }
}