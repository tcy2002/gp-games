using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    
    //UI
    public GameObject homePanel;
    public GameObject menu;
    public GameObject about;
    public GameObject options;

    public Toggle meshToggle;
    public Toggle forceToggle;
    public Toggle musicToggle;

    private bool _meshOn = true;
    private bool _forceOn = true;
    private bool _musicOn = true;
    private AudioSource _audio;

    void Start()
    {
        _instance._audio = GetComponent<AudioSource>();
        if (PlayerPrefs.GetInt("meshOn") == 1)
        {
            _instance.meshToggle.isOn = false;
            _instance._meshOn = false;
        }
        if (PlayerPrefs.GetInt("forceOn") == 1)
        {
            _instance.forceToggle.isOn = false;
            _instance._forceOn = false;
        }
        if (PlayerPrefs.GetInt("musicOn") == 0)
        {
            _instance._audio.Play();
        }
        else
        {
            _instance.musicToggle.isOn = false;
            _instance._musicOn = false;
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

    public static void ToggleMenu(bool open)
    {
        _instance.menu.SetActive(open);
        _instance.homePanel.SetActive(!open);
    }
    
    public static void ToggleAbout(bool open)
    {
        _instance.about.SetActive(open);
        _instance.homePanel.SetActive(!open);
    }
    
    public static void ToggleOptions(bool open)
    {
        _instance.options.SetActive(open);
        _instance.homePanel.SetActive(!open);
    }

    public static void EnterLevel(int level)
    {
        SceneManager.LoadScene("Level" + level);
    }

    public static void ToggleMesh()
    {
        _instance._meshOn = !_instance._meshOn;
        PlayerPrefs.SetInt("meshOn", _instance._meshOn ? 0: 1);
    }
    
    public static void ToggleForce()
    {
        _instance._forceOn = !_instance._forceOn;
        PlayerPrefs.SetInt("forceOn", _instance._forceOn ? 0: 1);
    }
    
    public static void ToggleMusic()
    {
        _instance._musicOn = !_instance._musicOn;
        if (_instance._musicOn) _instance._audio.Play();
        else _instance._audio.Pause();
        PlayerPrefs.SetInt("musicOn", _instance._musicOn ? 0: 1);
    }
}
