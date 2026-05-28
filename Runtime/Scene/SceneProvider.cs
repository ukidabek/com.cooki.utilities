using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace Utilities.SceneManagement
{
    [Serializable]
    public class SceneProvider
    {
        [SerializeField] private string m_guid = string.Empty;
        [SerializeField] private string m_path = string.Empty;
        [SerializeField] private string m_name = string.Empty;
        [SerializeField, Tooltip("If <b>true</b> scene will be loaded by full instead of name.")] private bool m_useScenePath = false;

        private string Path => m_useScenePath ? m_path : m_name;
        
        public Scene Scene
        {
            get
            {
                return m_useScenePath switch
                {
                    true => SceneManager.GetSceneByPath(m_path),
                    false => SceneManager.GetSceneByName(m_name)
                };
            }
        }

        public void LoadScene(LoadSceneMode mode = LoadSceneMode.Single) => SceneManager.LoadScene(Path, mode);

        public AsyncOperation LoadSceneAsync(LoadSceneMode mode = LoadSceneMode.Single) => SceneManager.LoadSceneAsync(Path, mode);

        public void UnloadScene() => SceneManager.UnloadSceneAsync(Path);

        public AsyncOperation UnloadSceneAsync() => SceneManager.UnloadSceneAsync(Path);

        public void SetSceneActive()
        {
            var scene = Scene;
            if(!scene.IsValid() || !scene.isLoaded) return; 
            SceneManager.SetActiveScene(Scene);
        }
    }
}