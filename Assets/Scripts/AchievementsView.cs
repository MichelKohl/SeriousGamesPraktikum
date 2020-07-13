using System.Collections.Generic;
using UnityEngine;

public class AchievementsView : MonoBehaviour
{
    [SerializeField] private AchievementField prefab;
    [SerializeField] private Sprite bronzeTrophy;
    [SerializeField] private Sprite silverTrophy;
    [SerializeField] private Sprite goldTrophy;
    [SerializeField] private Sprite platinumTrophy;

    private bool changedToView = false;
    // Update is called once per frame
    void Update()
    {
        if (changedToView)
        {
            changedToView = false;

            GameManager gameManager = GameManager.INSTANCE;
            AchievementStorage storage = gameManager.profile.GetAchievements();

            foreach (string gameName in storage.isAchieved.Keys)
                foreach (string achievementName in storage.isAchieved[gameName].Keys)
                {
                    try
                    {
                        (TrophyType trophyType, string description, int _) = storage.info[gameName][achievementName];
                        switch (trophyType)
                        {
                            case TrophyType.Bronze:
                                _ = Instantiate(prefab, transform).Init(storage.isAchieved[gameName][achievementName],
                                    achievementName, description, gameName, bronzeTrophy);
                                break;
                            case TrophyType.Silver:
                                _ = Instantiate(prefab, transform).Init(storage.isAchieved[gameName][achievementName],
                                    achievementName, description, gameName, silverTrophy);
                                break;
                            case TrophyType.Gold:
                                _ = Instantiate(prefab, transform).Init(storage.isAchieved[gameName][achievementName],
                                    achievementName, description, gameName, goldTrophy);
                                break;
                            case TrophyType.Platinum:
                                _ = Instantiate(prefab, transform).Init(storage.isAchieved[gameName][achievementName],
                                    achievementName, description, gameName, platinumTrophy);
                                break;
                        }
                    }
                    catch (KeyNotFoundException)
                    {
                        continue;
                    }
                }
        }
    }

    public void ChangeToAchievementView()
    {
        changedToView = true;
    }
}
