using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatButton : MonoBehaviour
{
    [SerializeField] StoryManager storyManager;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Stat stat;


    private DCPlayer player;
    

    private void Update()
    {
        if (player == null)
            player = storyManager.GetPlayerCharacter();
        if (player == null) return;
        else
        {
            (string name, int value) = player.GetStat(stat);
            text.text = $"{name}\n{value}";
        }
    }

    public void IncreaseStat()
    {
        player.IncreaseStat(stat);
    }
}
