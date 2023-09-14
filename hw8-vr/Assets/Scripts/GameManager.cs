using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    
    public Material skybox;
    public List<Material> materials;
    public Shader normalShader;
    public Shader hatchShader;
    public Texture[] hatches = new Texture[6];
    public TMP_Text scoreText;
    public TMP_Text timeText;
    public TMP_Text gameOverScoreText;
    public GameObject gameOverPanel;
    public GameObject gamePanel;
    public GameObject scoreCanvas;

    private int _score;
    private float _time;
    private bool _playing;

    public void Pause()
    {
        _instance._playing = !_instance._playing;
    }

    public static bool IsPlaying()
    {
        return _instance._playing;
    }

    // Start is called before the first frame update
    void Start()
    {
        RenderSettings.skybox = skybox;
        _score = 0;
        _time = 60;
        SelectShader(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_playing)
        {
            return;
        }
        
        _time -= Time.deltaTime;
        timeText.text = "" + (int)(_time + 1);
        if (_time < 0)
        {
            GameOver();
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

    public void Restart()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public static void GameOver()
    {
        _instance._playing = false;
        _instance.gamePanel.SetActive(false);
        _instance.scoreCanvas.SetActive(false);
        _instance.gameOverPanel.SetActive(true);
        _instance.gameOverScoreText.text = "游戏结束  最终得分：" + _instance._score;
        var blocks = GameObject.Find("Terrain");
        var rewards = GameObject.Find("Rewards");
        Destroy(blocks);
        Destroy(rewards);
    }

    public void SelectShader(int type)
    {
        foreach (var mat in _instance.materials)
        {
            if (type == 0)
            {
                mat.shader = _instance.normalShader;
            }
            else
            {
                mat.shader = _instance.hatchShader;
                mat.SetFloat("_HatchFactor", 0.8f);
                for (var i = 0; i < 6; i++)
                {
                    mat.SetTexture("_Hatch" + i, _instance.hatches[i]);
                }
            }
        }
    }
    
    public static void AddScore(int score)
    {
        _instance._score += score;
        _instance.scoreText.text = "" + _instance._score;
    }
}
