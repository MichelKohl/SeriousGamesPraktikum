using System;
using UnityEngine;

public class GameplayObject : MonoBehaviour
{
    [SerializeField] protected MiniGameManager manager;
    /// <summary>
    /// Update methods that need to be implemented by the inheriting class
    /// </summary>
    protected virtual void DoUpdate(){}
    protected virtual void DoFixedUpdate(){}

    void Update()
    {
       if (manager.IsGameplay()) DoUpdate();
    }

    void FixedUpdate(){
       if (manager.IsGameplay()) DoFixedUpdate();
    }
}
