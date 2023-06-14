using System.Collections.Generic;
using UnityEngine;

namespace FPS.Scripts.Game.Data
{
    [CreateAssetMenu(fileName = nameof(SceneCollection), menuName = "GameData")]
    public class SceneCollection : ScriptableObject
    {
        public int             currentSceneIndex;
        public List<SceneData> SceneData;

        public string FirstSceneName => SceneData[0].sceneName;

        public string GetCurrentSceneName()
        {
            if (currentSceneIndex >= SceneData.Count && currentSceneIndex < 0)
            {
                Debug.LogWarning($"currentSceneIndex:{currentSceneIndex} out of range");
                return string.Empty;
            }

            return SceneData[currentSceneIndex].sceneName;
        }
        
        public string GetNextSceneName(bool updateCurrentSceneIndex = true)
        {
            var nextSceneIndex = currentSceneIndex + 1;
            if (nextSceneIndex >= SceneData.Count && currentSceneIndex < 0)
            {
                Debug.LogWarning($"nextSceneIndex:{nextSceneIndex} out of range");
                return string.Empty;
            }

            if (updateCurrentSceneIndex)
            {
                currentSceneIndex = nextSceneIndex;
            }

            return SceneData[currentSceneIndex].sceneName;
        }
        
    }
}