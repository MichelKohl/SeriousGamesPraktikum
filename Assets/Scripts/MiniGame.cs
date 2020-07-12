using System.Collections.Generic;
using UnityEngine;

public class MiniGame
{
    #region public member

    /// <summary>
    /// Name of the mini game
    /// </summary>
    public string name { get; set; }

    /// <summary>
    /// Name of the scene that the mini game plays in
    /// </summary>
    public string scene { get; set; }

    /// <summary>
    /// Thumbnail of the mini game  
    /// </summary>
    public Sprite thumbnail { get; set; }
    #endregion

    #region constructor

    /// <summary>
    /// Constructor of a mini game used to create data objects that hold
    /// enough information to load minigames on the fly
    /// </summary>
    /// <param name="name">Name of the mini game</param>
    /// <param name="scene">Name of the scene the mini game can be found in</param>
    /// <param name="thumbnail">Thumbnail used to show soem visuals of the game</param>
    public MiniGame(string name, string scene, Sprite thumbnail)
    {
        this.name = name;
        this.scene = scene;
        this.thumbnail = thumbnail;
    }

    #endregion
}