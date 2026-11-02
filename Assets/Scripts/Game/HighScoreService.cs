using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ScoreEntry
{
    public int score;
    public string characterName;
    public int kills;
    public float timeSurvived;
    public string date;
}

[Serializable]
public class ScoreTable
{
    public List<ScoreEntry> entries = new List<ScoreEntry>();
}

public static class HighScoreService
{
    private const string KEY = "DWG_HighScores";
    private const int MAX_ENTRIES = 5;

    public static ScoreTable Load()
    {
        string json = PlayerPrefs.GetString(KEY, "");
        if (string.IsNullOrEmpty(json)) return new ScoreTable();
        return JsonUtility.FromJson<ScoreTable>(json) ?? new ScoreTable();
    }

    public static int BestScore
    {
        get
        {
            ScoreTable table = Load();
            return table.entries.Count > 0 ? table.entries[0].score : 0;
        }
    }

    public static void Submit(ScoreEntry entry)
    {
        if (entry.score <= 0) return;

        ScoreTable table = Load();
        table.entries.Add(entry);
        table.entries.Sort((a, b) => b.score.CompareTo(a.score));
        if (table.entries.Count > MAX_ENTRIES)
            table.entries.RemoveRange(MAX_ENTRIES, table.entries.Count - MAX_ENTRIES);

        PlayerPrefs.SetString(KEY, JsonUtility.ToJson(table));
        PlayerPrefs.Save();
    }
}