using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lib
{
    public class RestartLevel : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.F12))
            {
                var scene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(scene.name);
            }
        }
    }
}