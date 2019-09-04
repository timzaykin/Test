using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField]
    private Image sampleFigureImage;
    [SerializeField]
    private Text playerName, score,gameTimer, startTimer, endGameMessage;

    public Sprite[] sampleSprites;

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        playerName.text = Settings.Instance.playerName;
        SetScoreValue(0);
    }
    

    public void SetActiveStartTimer(bool value) {
       if(startTimer.gameObject.activeInHierarchy != value) startTimer.gameObject.SetActive(value);
    }

    public void SetStartTimerValue(int value) {
        startTimer.text = value.ToString();
    }

    public void SetTimerValue(int value)
    {
        gameTimer.text = string.Format("Timer: {0}", value);
    }

    public void SetScoreValue(int value) {
        score.text = string.Format("Score: {0}", value);
    }

    public void SetSampleFigure(Figure figure, ElementColor color) {
        sampleFigureImage.sprite = sampleSprites[(int)figure];
        sampleFigureImage.color = Element.GetColor(color);
    }

    public void ShowEndGameMessage(string message) {
        endGameMessage.gameObject.SetActive(true);
        endGameMessage.text = message;
    }

    public void OnDisable()
    {
        if (Instance == this) Instance = null;
    }
}
