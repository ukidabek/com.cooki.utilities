using UnityEditor;
using UnityEditor.SceneManagement;

namespace Utilities.General
{
    public static class ReferencePropertyDroverHelper
    {
        public const float Margin =
#if UNITY_6000
            14f;
#else
            0f;
#endif

        public static void SaveAndReserialize(SerializedObject serializedObject, bool change = true)
        {
            if(!change) return;
         
            serializedObject.ApplyModifiedProperties();
            serializedObject.UpdateIfRequiredOrScript();
            
            AssetDatabase.SaveAssetIfDirty(serializedObject.targetObject);
            var stage = PrefabStageUtility.GetCurrentPrefabStage();
            var path = stage != null ? stage.assetPath : AssetDatabase.GetAssetPath(serializedObject.targetObject);
            AssetDatabase.ForceReserializeAssets(new[] { path });
        }
    }
}