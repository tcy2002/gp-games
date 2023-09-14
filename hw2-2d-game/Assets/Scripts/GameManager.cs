using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public TMP_Text timeScore;
    public TMP_Text fruitScore;
    public TMP_Text final;
    public TMP_Text level;
    public GameObject homePanel;
    public GameObject helpPanel;
    public GameObject gamePanel;
    public GameObject gameOverPanel;
    private int _score;
    private int _countTime;
    private int _gameLevel;
    public int levelTime = 20;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        _countTime = (int)Time.timeSinceLevelLoad;
        SetTime();
    }

    void LateUpdate()
    {
        _gameLevel = _countTime / levelTime + 1;
        if (_countTime % levelTime == 0)
        {
            level.SetText("Level " + _gameLevel);
        } 
        else if (_countTime % levelTime == 2)
        {
            level.SetText("");
        }
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        _instance = this;
    }

    private void SetTime()
    {
        int second = _countTime % 60;
        int minute = (_countTime / 60) % 60;
        int hour = _countTime / 3600;
        string timeStr = minute.ToString("00") + second.ToString(":00");
        if (hour != 0) timeStr = hour.ToString("0:") + timeStr;
        timeScore.SetText(timeStr);
    }

    public static int GetLevel()
    {
        return _instance._gameLevel;
    }

    public static void ToggleHelp()
    {
        _instance.homePanel.SetActive(false);
        _instance.helpPanel.SetActive(true);
    }

    public static void DestroyHelp()
    {
        _instance.homePanel.SetActive(true);
        _instance.helpPanel.SetActive(false);
    }

    public static void IncreaseScore(int s)
    {
        _instance._score += s;
        _instance.fruitScore.SetText("Score: " + _instance._score);
    }

    public static void StartGame()
    {
        _instance.homePanel.SetActive(false);
        _instance.gamePanel.SetActive(true);
        Time.timeScale = 1;
    }
    
    public static void GameOver(bool dead)
    {
        if (dead)
        {
            
            _instance.gameOverPanel.SetActive(true);
            _instance.gamePanel.SetActive(false);
            int score = OldBest();
            _instance.final.SetText("Time: " + _instance.timeScore.text + 
                                    "\nScore: " + _instance._score +
                                    (_instance._score > score ? "\nNew best!" : "\nBest: " + score));
            //记录最好成绩
            if (_instance._score > score)
            {
                NewBest(_instance._score);
            }
            Time.timeScale = 0;
        }
    }

    private static int OldBest()
    {
        if (!File.Exists(@".\best"))
        {
            return 0;
        }
        StreamReader reader = new StreamReader(@".\best");
        string bestStr = reader.ReadLine();
        if (bestStr == null)
        {
            reader.Close();
            return 0;
        }
        reader.Close();
        return int.Parse(bestStr);
    }

    private static void NewBest(int score)
    {
        StreamWriter writer = new StreamWriter(@".\best");
        writer.Write(score);
        writer.Close();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
