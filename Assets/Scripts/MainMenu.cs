using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private Button start, exit;
    [SerializeField]
    private InputField nameInput;
    [SerializeField]
    private Text hiScoreName, hiScore;

    public void Awake()
    {
        Settings.Instance.Init();
    }

    public void Start()
    {
        start.onClick.AddListener(delegate { StartGame();});
        exit.onClick.AddListener(delegate { Application.Quit();});
        nameInput.onValueChanged.AddListener(delegate { ChangeName(); });

        nameInput.text = Settings.Instance.playerName;
        hiScoreName.text = Settings.Instance.hiScoreName;
        hiScore.text = Settings.Instance.hiScore.ToString();
    }

    private void StartGame() {
        SceneManager.LoadScene("Main");
    }

    public void ChangeName()
    {
        Settings.Instance.SetPlayerName(nameInput.text);
    }
}
