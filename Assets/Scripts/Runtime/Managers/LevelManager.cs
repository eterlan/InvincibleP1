using System;
using Cysharp.Threading.Tasks;
using DevLocker.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lib.Managers
{
    /// <summary>
    /// 场景管理器应该第一个加载
    /// </summary>
    [Serializable]
    public class LevelManager : IManager// : Singleton<SceneManager>
    {
        public       int     currentLevelIndex;
        public       SceneReference[] levels; 
        [NonSerialized]
        public Scene       persistentScene;

        public async UniTask LoadNextLevel()
        {
            await LoadLevel(currentLevelIndex + 1);
        }

        public async UniTask LoadLevel(int nextLevelIndex)
        {
            // 临时
            if (nextLevelIndex >= levels.Length)
            {
                nextLevelIndex = 0;
            }
            
            if (nextLevelIndex < 0 || nextLevelIndex >= levels.Length)
            {
                Debug.LogWarning($"不存在的Level index: {nextLevelIndex}");
                return;
            }

            // Unload Current Level and Load Next Level.
            var currentLevel = levels[currentLevelIndex];
            await SceneManager.UnloadSceneAsync(currentLevel.SceneName);
            var nextLevel = levels[nextLevelIndex];

            await SceneManager.LoadSceneAsync(nextLevel.SceneName, LoadSceneMode.Additive);
            var levelScene = SceneManager.GetSceneByName(nextLevel.SceneName);
            currentLevelIndex = nextLevelIndex;
            Debug.Log($"{levelScene}已加载");
            
            SetSceneRoot(levelScene);
            SetUIRoot(persistentScene);
        }

        public void OnAwake()
        {
            // Find Level scene
            SetCurrentLevel();

            void SetCurrentLevel()
            {
                // Check if current level exist
                var currentLevelSceneName = string.Empty;
                for (var i = 0; i < SceneManager.sceneCount; i++)
                {
                    var scene = SceneManager.GetSceneAt(i);
                    if (scene.name.Contains(Const.LEVEL_NAME, StringComparison.OrdinalIgnoreCase))
                    {
                        currentLevelSceneName = scene.name;
                        break;
                    }
                }

                if (currentLevelSceneName.IsNullOrEmpty())
                {
                    Debug.Log("当前没有关卡在加载状态");
                    return;
                }
            
                // Cache current level
                currentLevelIndex = -1;
                for (var i = 0; i < levels.Length; i++)
                {
                    if (levels[i].SceneName == currentLevelSceneName)
                    {
                        currentLevelIndex = i;
                        break;
                    } 
                }
                if (currentLevelIndex == -1)
                {
                    Debug.LogError($"关卡未被添加进{this}->Levels中, 请添加");
                }
                SetSceneRoot(SceneManager.GetSceneByName(currentLevelSceneName));
                
                // var levelScene  = levels[currentLevelIndex].LoadedScene;
                // SceneManager.SetActiveScene(levelScene);

                persistentScene = SceneManager.GetSceneByName(Const.PERSISTENT_SCENE_NAME);
                SetUIRoot(persistentScene);
            }
        }
        
        public static void SetSceneRoot(Scene scene)
        {
            // Set Root
            if (!scene.FindRootTransformInScene(Const.ROOT_NAME, out var root))
            {
                Debug.LogError($"关卡{scene.name}没有找到根节点GameObject, 请添加一个并命名为{Const.ROOT_NAME}");
                return;
            }

            GameControllerBase.Instance.root = root.GetComponent<Root>();
        }
        
        private static void SetUIRoot(Scene scene)
        {
            if (!scene.FindRootTransformInScene(Const.UI_ROOT_NAME, out var uiRoot))
                return;
            
            GameControllerBase.Instance.root.uiRoot = (RectTransform)uiRoot;
        }

        public void OnUpdate()
        {
            if (Input.GetKeyUp(KeyCode.F12))
            {
                LoadLevel(currentLevelIndex).Forget();
            }
        }
    }
}