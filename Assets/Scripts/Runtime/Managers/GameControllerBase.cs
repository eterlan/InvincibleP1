using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Lib.Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lib
{
    public abstract class GameControllerBase : MonoBehaviour // SingletonMono<GameController> 
    {
        private static GameControllerBase instance;
        public static GameControllerBase Instance => instance;

#if CY_FRAMEWORK
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            InitAsync().Forget();
        }
#endif

        private static async UniTaskVoid InitAsync()
        {
            var persistentScene = await GetPersistentScene();
                
            // Setup instance field
            var allGO = persistentScene.GetRootGameObjects(); 
            for (var i = 0; i < persistentScene.rootCount; i++)
            {
                if (!allGO[i].TryGetComponent<GameControllerBase>(out var gameController)) 
                    continue;
                instance = gameController;
            }
            instance.OnAwake();
            Debug.Log($"{Instance} is loaded");
        }
        
        private static async UniTask<Scene> GetPersistentScene()
        {
            Scene persistentScene = default;
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.name == Const.PERSISTENT_SCENE_NAME)
                {
                    persistentScene = scene;
                    break;
                }
            }

            if (persistentScene == default)
            {
                await SceneManager.LoadSceneAsync(Const.PERSISTENT_SCENE_NAME, new LoadSceneParameters(LoadSceneMode.Additive));
                persistentScene = SceneManager.GetSceneByName(Const.PERSISTENT_SCENE_NAME);
            }

            return persistentScene;
        }
        
        /// <summary>
        /// 加载依赖于sceneLoader, 因此初始化放在所有管理器后面, 如果发现有管理器的OnAwake依赖于root, 要重新考虑这里该怎么处理
        /// </summary>
        [NonSerialized]
        public Root        root;

        [SerializeReference][ValidateInput(nameof(ValidateManagers), Const.VALIDATE_MANAGER_INFO)]
        public IManager[] managers;
        // public SceneLoader sceneLoader;
        // public SaveManager saveManager;

        private bool ValidateManagers()
        {
            return managers.Select(m => m.GetType()).Distinct().Count() == managers.Length;
        }

        protected void OnAwake()
        {
            for (var i = 0; i < managers.Length; i++)
            {
                managers[i].OnAwake();
            }
        }

        private void Update()
        {
            for (var i = 0; i < managers.Length; i++)
            {
                managers[i].OnUpdate();
            }            
        }

        public static TManager GetManager<TManager>() where TManager : class, IManager
        {
            for (var i = 0; i < Instance.managers.Length; i++)
            {
                if (Instance.managers[i] is TManager)
                {
                    return Instance.managers[i] as TManager;
                }
            }

            Debug.LogError($"找不到类型为{typeof(TManager).Name}的管理器, 请在{Instance}上挂载指定的管理器类型");
            return null;
        }
        
        public abstract class GameController<T> : GameControllerBase where T : GameController<T>
        {
            public static T InstanceT => (T)Instance;
        }
    }
}