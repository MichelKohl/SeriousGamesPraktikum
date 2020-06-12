using UnityEngine;

public class GameplayObject : MonoBehaviour
{
    protected MiniGameManager manager;
    /// <summary>
    /// Update methods that need to be implemented by the inheriting class
    /// </summary>
    protected virtual void DoUpdate(){}
    protected virtual void DoFixedUpdate(){}

    void Start()
    {
        manager = GameObject.Find("Game Manager").GetComponent<MiniGameManager>();
    }

    void Update()
    {
        if (manager.IsGameplay()) DoUpdate();
    }

    void FixedUpdate(){
        if (manager.IsGameplay()) DoFixedUpdate();
    }
}
