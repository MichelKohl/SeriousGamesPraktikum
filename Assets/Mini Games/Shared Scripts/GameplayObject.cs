using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayObject : MonoBehaviour
{
    protected MiniGameManager manager;
    
    protected virtual void DoUpdate(){}

    void Start()
    {
      manager = GameObject.Find("Game Manager").GetComponent<MiniGameManager>();
    }

    void FixedUpdate(){
      if(manager.isGameplay()) DoUpdate();
    }
}
