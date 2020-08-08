using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public static class Persistence
{
    private const String SCOREBOARD_KEY = "scoreboard";

    public static Scoreboard LoadScoreboard()
    {
        if (!PlayerPrefs.HasKey(SCOREBOARD_KEY))
        {
            return new Scoreboard();
        }

        string json = PlayerPrefs.GetString(SCOREBOARD_KEY);
        Debug.Log("Loaded: " + json);
        return JsonConvert.DeserializeObject<Scoreboard>(json);
    }

    public static void SaveScoreboard(Scoreboard scoreboard)
    {
        string json = JsonConvert.SerializeObject(scoreboard);
        Debug.Log("Saving: " + json);
        PlayerPrefs.SetString(SCOREBOARD_KEY, json);
        PlayerPrefs.Save();
    }
}