using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class ScoreUI : MonoBehaviour
{
    public Text redText;
    public Text blueText;

    void OnEnable()
    {
        ScoreManager.OnScoreChangedEvent += UpdateUI;
    }

    void OnDisable()
    {
        ScoreManager.OnScoreChangedEvent -= UpdateUI;
    }

    void UpdateUI()
    {
        if (ScoreManager.Instance != null)
        {
            redText.text = "Red: " + ScoreManager.Instance.blueScore;
            blueText.text = "Blue: " + ScoreManager.Instance.redScore;
        }
    }
}
