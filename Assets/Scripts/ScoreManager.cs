using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ScoreManager : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnRedScoreChanged))]
    public int redScore = 0;

    [SyncVar(hook = nameof(OnBlueScoreChanged))]
    public int blueScore = 0;

    public static ScoreManager Instance;

    public delegate void ScoreChanged();
    public static event ScoreChanged OnScoreChangedEvent;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [Server]
    public void AddScore(Team team)
    {
        if (team == Team.Red) redScore++;
        else if (team == Team.Blue) blueScore++;
    }

    void OnRedScoreChanged(int oldScore, int newScore)
    {
        OnScoreChangedEvent?.Invoke();
    }

    void OnBlueScoreChanged(int oldScore, int newScore)
    {
        OnScoreChangedEvent?.Invoke();
    }
}
