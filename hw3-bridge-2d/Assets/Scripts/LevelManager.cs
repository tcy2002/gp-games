using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private static LevelManager _instance;

    public int level;
    public int budgetLimit;
    public Vector2 tiling;
    public List<GameObject> mats;
    public List<float> freq;
    public List<int> cost;
    public List<GameObject> helpers;
    public GameObject joint;
    public float matMassPerLen;
    public float carMassPerWheel;
    public float maxRatio;
    public float maxLength;
    public Vector2 destination;

    //UI
    public TMP_Text levelText;
    public TMP_Text currentBudget;
    public TMP_Text bestBudget;
    public TMP_Text maxBudget;
    public TMP_Text budgetWin;
    public TMP_Text starWin;
    public TMP_Text scoreWin;
    public GameObject gamePanel;
    public GameObject gameOverPanel;
    public GameObject gameWinPanel;
    public GameObject pausePanel;
    
    //预算与计分系统
    private int _currentCost;
    private int _buildingCost;

    private int _score;
    private int _id;
    private int _type;
    private int _state;

    private AudioSource[] _audio;

    // Start is called before the first frame update
    void Start()
    {
        Physics2D.gravity = Vector2.zero;
        _instance.levelText.SetText("" + _instance.level);
        _instance.maxBudget.SetText("" + _instance.budgetLimit);
        var best = PlayerPrefs.GetInt(_instance.level + "-bestBudget");
        _instance.bestBudget.SetText("" + (best > 0 ? best : "-"));
        _instance._audio = GetComponents<AudioSource>();
        if (PlayerPrefs.GetInt("musicOn") == 0)
        {
            _instance._audio[0].Play();
        }
        if (PlayerPrefs.GetInt("meshOn") == 0)
        {
            _instance.helpers[0].SetActive(true);
        }
        Upload();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && _instance._state == 0)
        {
            //向场景中添加桥梁部件
            var obj = Instantiate(mats[_type], new Vector3(0, 0, 2), Quaternion.identity);
            obj.transform.localScale = new Vector3(0.2f, 0.2f, 1);
        }
    }

    void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        _instance = this;
    }
    
    public static void SelectMat(int t)
    {
        _instance._type = t;
    }

    public static void ToggleSim()
    {
        if (_instance._state == 0)
        {
            //结束搭建，开始操控小车通过桥梁
            _instance._state = 1;
            foreach (var p in _instance.helpers)
            {
                p.SetActive(false);
            }
            Physics2D.gravity = Vector2.down * 9.8f;
        }
        else if (_instance._state == 1)
        {
            //重新开始;
            Restart();
        }
    }

    public static void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static void BackToHome()
    {
        SceneManager.LoadScene("Home");
    }

    public static void TogglePause(bool active)
    {
        _instance.pausePanel.SetActive(active);
        _instance.gamePanel.SetActive(!active);
        _instance._state = active ? 2 : 0;
    }

    public static Vector2 GetTiling()
    {
        return _instance.tiling;
    }

    public static int GetState()
    {
        return _instance._state;
    }

    public static GameObject GetJoint()
    {
        return _instance.joint;
    }

    public static float GetMatMass()
    {
        return _instance.matMassPerLen;
    }

    public static float GetCarMass()
    {
        return _instance.carMassPerWheel;
    }

    public static float GetMaxRatio()
    {
        return _instance.maxRatio;
    }

    public static float GetMaxLength()
    {
        return _instance.maxLength;
    }

    public static int GetId()
    {
        _instance._id += 1;
        return _instance._id - 1;
    }

    public static int GetLevel()
    {
        return _instance.level;
    }

    public static float GetFreq(int type)
    {
        return _instance.freq[type];
    }

    public static void AddScore(int score)
    {
        _instance._score += score;
    }

    public static Vector2 GetDestination()
    {
        return _instance.destination;
    }

    public static void UpdateCost(int type, float len)
    {
        _instance._buildingCost = (int)(_instance.cost[type] * (len / 0.5f));
        var cost = _instance._currentCost + _instance._buildingCost;
        _instance.currentBudget.SetText("" + (_instance._currentCost + _instance._buildingCost));
        if (cost > _instance.budgetLimit)
        {
            _instance.currentBudget.color = Color.red;
        }
    }

    public static void FinishBuilding(bool valid)
    {
        if (valid)
        {
            _instance._currentCost += _instance._buildingCost;
            _instance._buildingCost = 0;
        }
        else
        {
            _instance._buildingCost = 0;
            _instance.currentBudget.SetText("" + _instance._currentCost);
        }
        _instance.currentBudget.color = Color.black;
    }

    public static void ReduceCost(int type, float len)
    {
        if (_instance._buildingCost == 0)
        {
            _instance._currentCost -= (int)(_instance.cost[type] * (len / 0.5f));
        }
        else
        {
            _instance._buildingCost = 0;
        }
        _instance.currentBudget.SetText("" + _instance._currentCost);
    }

    public static bool CostUnderLimit()
    {
        return (_instance._currentCost + _instance._buildingCost <= _instance.budgetLimit);
    }

    public static void GameOver(bool isWin)
    {
        _instance.gamePanel.SetActive(false);
        _instance._state = 2;
        if (isWin)
        {
            _instance.gameWinPanel.SetActive(true);
            if (PlayerPrefs.GetInt("musicOn") == 0)
            {
                _instance._audio[1].Play();
            }
            _instance.budgetWin.SetText("材料费用：" + _instance._currentCost);
            _instance.starWin.SetText("收集星数：" + _instance._score);
            var bestCost = PlayerPrefs.GetInt(_instance.level + "-bestBudget");
            if (bestCost == 0 || _instance._currentCost < bestCost)
            {
                PlayerPrefs.SetInt(_instance.level + "-bestBudget", _instance._currentCost);
            }
            var currentScore = (int)(1000.0 * _instance.budgetLimit / _instance._currentCost) + _instance._score * 1000;
            var bestScore = PlayerPrefs.GetInt(_instance.level + "-bestScore");
            if (bestScore == 0 || currentScore > bestScore)
            {
                PlayerPrefs.SetInt(_instance.level + "-bestScore", currentScore);
                _instance.scoreWin.SetText("最终得分：" + currentScore + "  新纪录！");
            }
            else
            {
                _instance.scoreWin.SetText("最终得分：" + currentScore + "  历史最佳：" + bestScore);
            }
        }
        else
        {
            if (PlayerPrefs.GetInt("musicOn") == 0)
            {
                _instance._audio[2].Play();
            }
            _instance.gameOverPanel.SetActive(true);
        }
    }

    void Upload()
    {
        //导入数据
        var num = PlayerPrefs.GetInt(_instance.level + "-num");
        for (var i = 0; i < num; i++)
        {
            var valid = PlayerPrefs.GetInt(_instance.level + "-valid" + i);
            if (valid == 0) continue;
            
            var x = PlayerPrefs.GetFloat(_instance.level + "-x" + i);
            var y = PlayerPrefs.GetFloat(_instance.level + "-y" + i);
            var dist = PlayerPrefs.GetFloat(_instance.level + "-dist" + i);
            var angle = PlayerPrefs.GetFloat(_instance.level + "-angle" + i);
            var type = PlayerPrefs.GetInt(_instance.level + "-type" + i);

            var obj = Instantiate(mats[type], new Vector3(x, y, type * 0.1f), Quaternion.Euler(0, 0, angle));
            var wid = 1.8f / tiling.y;
            obj.transform.localScale = new Vector3(dist, wid, 1);
            _instance._currentCost += (int)(_instance.cost[type] * (dist / 0.5f));
        }
        
        _instance.currentBudget.SetText("" + _instance._currentCost);
    }
}
