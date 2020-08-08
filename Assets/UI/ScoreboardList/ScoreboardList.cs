using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using TMPro;

public class ScoreboardList : MonoBehaviour
{
    public GameObject rowPrefab;

    void Start()
    {
        foreach (var level in Persistence.LoadScoreboard().completedLevels.OrderByDescending(l => l.level))
        {
            AddRow(level.level, level.accuracy, level.date);
        }
    }

    public void AddRow(int level, float accuracy, DateTime date)
    {
        var row = Instantiate(rowPrefab);
        row.transform.SetParent(gameObject.transform, false);

        row.transform.GetChild(0).GetComponent<TMP_Text>().SetText(level.ToString());
        row.transform.GetChild(1).GetComponent<TMP_Text>().SetText(accuracy.ToString("P1"));
        row.transform.GetChild(2).GetComponent<TMP_Text>().SetText(date.ToLocalTime().ToString("g"));
    }
}
