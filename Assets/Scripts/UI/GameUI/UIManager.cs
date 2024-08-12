using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public LevelSO LevelSO;
    public Slider TimeRemainSlider;

    //pause
    public GameObject PauseScene;

    //end
    public GameObject EndMenuScene;
    public TextMeshProUGUI GameStateText;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI CoinText;


    //buff
    public TextMeshProUGUI ShuffleBuffText;
    public TextMeshProUGUI HintBuffText;
    public TextMeshProUGUI AddTimeBuffText;
    public TextMeshProUGUI GameStreakText;

    private float timeRemain;
    private bool updateEndMenu;

    // Start is called before the first frame update
    void Start()
    {
        InitializeBuffUI();
        updateEndMenu = false;
        UpdateGameStreak();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimeRemain();
        UpdateEndMenu();
        UpdateBuffUI();

        //Debug.Log(GameData.Coins);
    }


    //time slider
    public void UpdateTimeRemain()
    {
        timeRemain = GameManager.Instance.GetTimeRemain();
        TimeRemainSlider.value = timeRemain / GameData.TimePlay; 
    }

    public void PlayClickButtonSFX()
    {
        AudioManager.Instance.PlayClickButtonSFX();
    }
    public void PlayClickTileSFX()
    {
        AudioManager.Instance.PlayClickTileSFX();
    }
    public void PlayWinSFX()
    {
        AudioManager.Instance.PlayWinSFX();
    }
    public void PlayLoseSFX()
    {
        AudioManager.Instance.PlayLoseSFX();
    }
    public void PauseBGM()
    {
        AudioManager.Instance.PauseBGM();
    }
    public void UnPauseBGM()
    {
        AudioManager.Instance.UnPauseBGM();
    }

    #region pause continue home
    public void PauseButton()
    {
        Time.timeScale = 0f;
        PauseScene.SetActive(true);
    }
    public void ContinueButton()
    {
        Time.timeScale = 1f;
        PauseScene.SetActive(false);
    }

    public void HomeButton()
    {
        Time.timeScale = 1f;
        SceneTransition.Instance.ChangeHomeScene();
    }
    public void Replay()
    {
        SceneTransition.Instance.ChangeGameScene();
    }

    #endregion

    #region EndMenu
    public void UpdateEndMenu()
    {
        if(GameManager.Instance.CheckGameState!=0 && !updateEndMenu)
        {
            EndMenuScene.SetActive(true);
            PauseBGM();
            SetGameState();
            SetScoreText();
            SetCoinText();
            SaveSystem.SaveGameData();
            updateEndMenu = true;
        }
    }
    public void SetGameState()
    {
        if(GameManager.Instance.CheckGameState==1) 
        {
            GameStateText.text = "You win!";
            if(GameData.GameMode == GameMode.Classic) GameData.GameStreak++;
            if(GameData.GameStreak > GameData.HighestGameStreak) GameData.HighestGameStreak = GameData.GameStreak;
        }
        else 
        {
            GameStateText.text = "You lose!";
            if(GameData.GameMode == GameMode.Classic) GameData.GameStreak=0;
        }
    }
    public void SetScoreText()
    {
        ScoreText.text = GameManager.Instance.ScoreCalculation().ToString();
    }
    public void SetCoinText()
    {
        CoinText.text = GameManager.Instance.CoinCalculation().ToString();
        GameData.Coins += GameManager.Instance.CoinCalculation();
    }


    #endregion

    #region buff
    public void InitializeBuffUI()
    {
        TimeRemainSlider.value = TimeRemainSlider.maxValue;
        ShuffleBuffText.text = LevelSO.ShuffleBuffBase.ToString();
        HintBuffText.text = LevelSO.HintBuffBase.ToString();
        AddTimeBuffText.text = LevelSO.AddTimeBuffBase.ToString();
    }
    public void UpdateBuffUI()
    {
        UpdateShuffleBuffUI();
        UpdateHintBuffUI();
        UpdateAddTimeBuffUI();
    }
    public void UpdateShuffleBuffUI()
    {
        ShuffleBuffText.text = GameData.ShuffleBuff.ToString();
    }
    public void UpdateHintBuffUI()
    {
        HintBuffText.text =GameData.HintBuff.ToString();
    }
    public void UpdateAddTimeBuffUI()
    {
        AddTimeBuffText.text = GameData.AddTimeBuff.ToString();
    }
    #endregion


    public void UpdateGameStreak()
    {
        if(GameData.GameMode == GameMode.Classic)
            GameStreakText.text = $"Current Game Streak: {GameData.GameStreak}";
        else
            GameStreakText.text = "";
    }




}
