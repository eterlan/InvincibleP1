using System;
using FPS.Scripts.Game;
using FPS.Scripts.Game.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Unity.FPS.UI
{
    public class LoadSceneButton : MonoBehaviour
    {
        // public string          SceneName = "";
        public LoadSceneType   LoadSceneType;
        public SceneCollection SceneCollection;

        private void Start()
        {
            if (LoadSceneType == LoadSceneType.Menu)
            {
                
            }
        }

        void Update()
        {
            if (EventSystem.current.currentSelectedGameObject == gameObject
                && Input.GetButtonDown(GameConstants.k_ButtonNameSubmit))
            {
                LoadTargetScene();
            }
        }

        public void LoadTargetScene()
        {
            var sceneName = GetSceneName(LoadSceneType);
            SceneManager.LoadScene(sceneName);
        }

        public string GetSceneName(LoadSceneType loadSceneType) =>
            loadSceneType switch{
                LoadSceneType.Lose => SceneCollection.GetCurrentSceneName(),
                LoadSceneType.Win  => SceneCollection.GetNextSceneName(),
                LoadSceneType.Menu => SceneCollection.FirstSceneName,
                _                  => throw new ArgumentOutOfRangeException(nameof(loadSceneType), loadSceneType, null)
            };
    }

    public enum LoadSceneType
    {
        Menu,
        Lose,
        Win
    }
}