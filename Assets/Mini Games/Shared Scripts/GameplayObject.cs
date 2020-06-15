using System;
using UnityEngine;

public class GameplayObject : MonoBehaviour
{
    protected MiniGameManager manager;
    /// <summary>
    /// Update methods that need to be implemented by the inheriting class
    /// </summary>
    protected virtual void DoUpdate(){}
    protected virtual void DoFixedUpdate(){}

    void Update()
    {
        try
        {
            if (manager.IsGameplay()) DoUpdate();
        }
        catch (Exception)
        {
            manager = GameObject.Find("Game Manager").GetComponent<MiniGameManager>();
        }
    }

    void FixedUpdate(){
        try
        {
            if (manager.IsGameplay()) DoFixedUpdate();
        }
        catch (Exception)
        {
            manager = GameObject.Find("Game Manager").GetComponent<MiniGameManager>();
        }
    }
}
