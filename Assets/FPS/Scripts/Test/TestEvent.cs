using System.Collections;
using System.Collections.Generic;
using FPS.Scripts.Game;
using FPS.Scripts.Game.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

public class TestEvent : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // PASS
    public float timeScale = 1;
    [Button]
    public void Test()
    {
        var arg = Events.ChangeTimeScaleEvent;
        arg.timeScale = timeScale;
        EventManager.Broadcast(arg);
    }
}
