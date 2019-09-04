using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : Singleton<Settings>
{

    public string playerName;
    public string hiScoreName;
    public int hiScore;

    public void Init() {

        if (PlayerPrefs.HasKey("Name"))
        {
            playerName = PlayerPrefs.GetString("Name");
        }
        else {
            playerName = "Player";
        }

        if (PlayerPrefs.HasKey("HiScoreName"))
        {
            hiScoreName = PlayerPrefs.GetString("HiScoreName");
        }
        else
        {
            hiScoreName = "Empty";
        }

        if (PlayerPrefs.HasKey("HiScore"))
        {
            hiScore = PlayerPrefs.GetInt("HiScore");
        }
        else
        {
            hiScore = 0;
        }
    } 

    public void SetPlayerName(string name) {
        playerName = name;
        PlayerPrefs.SetString("Name", playerName);
    }

    public void SetHiScore(int score) {
        hiScoreName = playerName;
        hiScore = score;
        PlayerPrefs.SetString("HiScoreName", hiScoreName);
        PlayerPrefs.SetInt("HiScore", hiScore);
    } 
}

