using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    //物品Prefabs
    public List<Object> blocks;

    //物品栏默认展示的物品Id
    public int[] blockIds = { -1, -1, -1, -1, -1, -1, -1, -1 };

    //物品栏参数
    private readonly Vector2 _initPos = new (-350, 0);
    public RawImage[] displays;
    public RawImage selectedBlock;
    public List<Texture> blockTexes;
    public Texture emptyTex;
    private int _selectedDisplayId = 0;
    private int _selectedBlockId = -1;
    private GameObject _hotBar;

    //区块参数
    public int[] chunkBlockIds = { 0, 1, 1, 1 };
    public Vector3Int chunkSize = new(16, 16, 4);

    //人物
    public GameObject player;

    //UI
    private int _state;
    public GameObject bagView;
    public GameObject homePanel;
    public GameObject gamePanel;
    public GameObject pausePanel;
    public GameObject optionsPanel;
    public GameObject shaderTestObject;
    
    //Shader
    public List<Shader> shaders;
    public List<Material> materials;
    public Texture ramp;
    public Texture[] hatches = new Texture[6];

    //后处理效果
    public MonoBehaviour bloom;
    public MonoBehaviour motionBlur;
    public MonoBehaviour fog;
    public MonoBehaviour depthBlur;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var mat in _instance.materials)
        {
            mat.shader = _instance.shaders[0];
        }

        _instance.bloom.enabled = false;
        _instance.motionBlur.enabled = false;
        _instance.fog.enabled = false;
        _instance.depthBlur.enabled = false;
        _instance.bagView.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            SelectBar();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause(true);
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                ToggleBag(true);
            }
        }

        if (_instance._selectedBlockId != -1)
        {
            UpdateSelectedBlock();
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

    void SelectBar()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && _selectedDisplayId != 0)
        {
            _instance._selectedDisplayId = 0;
            _instance._hotBar.transform.localPosition = _initPos;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && _selectedDisplayId != 1)
        {
            _instance._selectedDisplayId = 1;
            _instance._hotBar.transform.localPosition = _initPos + Vector2.right * 100;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && _selectedDisplayId != 2)
        {
            _instance._selectedDisplayId = 2;
            _instance._hotBar.transform.localPosition = _initPos + Vector2.right * 200;
        }

        if (Input.GetKeyDown(KeyCode.Alpha4) && _selectedDisplayId != 3)
        {
            _instance._selectedDisplayId = 3;
            _instance._hotBar.transform.localPosition = _initPos + Vector2.right * 300;
        }

        if (Input.GetKeyDown(KeyCode.Alpha5) && _selectedDisplayId != 4)
        {
            _instance._selectedDisplayId = 4;
            _instance._hotBar.transform.localPosition = _initPos + Vector2.right * 400;
        }

        if (Input.GetKeyDown(KeyCode.Alpha6) && _selectedDisplayId != 5)
        {
            _instance._selectedDisplayId = 5;
            _instance._hotBar.transform.localPosition = _initPos + Vector2.right * 500;
        }

        if (Input.GetKeyDown(KeyCode.Alpha7) && _selectedDisplayId != 6)
        {
            _instance._selectedDisplayId = 6;
            _instance._hotBar.transform.localPosition = _initPos + Vector2.right * 600;
        }

        if (Input.GetKeyDown(KeyCode.Alpha8) && _selectedDisplayId != 7)
        {
            _instance._selectedDisplayId = 7;
            _instance._hotBar.transform.localPosition = _initPos + Vector2.right * 700;
        }

        var scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0.05f)
        {
            _instance._selectedDisplayId--;
            if (_instance._selectedDisplayId < 0) _instance._selectedDisplayId = 7;
            _instance._hotBar.transform.localPosition = _initPos + Vector2.right * (_instance._selectedDisplayId * 100);
        }

        if (scroll < -0.05f)
        {
            _instance._selectedDisplayId = (_instance._selectedDisplayId + 1) % 8;
            _instance._hotBar.transform.localPosition = _initPos + Vector2.right * (_instance._selectedDisplayId * 100);
        }
    }

    public static void InstantiateBlock(Vector3 pos)
    {
        var id = _instance.blockIds[_instance._selectedDisplayId];
        if (id >= 0)
        {
            var obj = Instantiate(_instance.blocks[id], pos, Quaternion.identity);
            obj.GetComponent<MeshRenderer>().material.SetInt("_DisplayMode", 0);
        }
    }

    void InitGame()
    {
        _instance._selectedDisplayId = 0;
        _instance._hotBar = GameObject.Find("HotBar");
        _instance._hotBar.transform.localPosition = _instance._initPos;

        for (int i = 0; i < 8; i++)
        {
            var id = _instance.blockIds[i];
            _instance.displays[i].texture = id >= 0 ? _instance.blockTexes[id] : _instance.emptyTex;
        }

        var pos = Vector3.zero;
        SpawnChunks(pos);
    }

    private void SpawnChunks(Vector3 pos)
    {
        var chunk = new GameObject();
        chunk.transform.position = pos;

        for (var zi = 0; zi < _instance.chunkSize.z; zi++)
        {
            var id = _instance.chunkBlockIds[zi];
            for (var xi = 0; xi < _instance.chunkSize.x; xi++)
            {
                for (var yi = 0; yi < _instance.chunkSize.y; yi++)
                {
                    var block = (GameObject)Instantiate(_instance.blocks[id], chunk.transform, false);
                    block.transform.localPosition = new Vector3(xi, -zi, yi);
                }
            }
        }
    }

    public void StartGame()
    {
        _instance.homePanel.SetActive(false);
        _instance.gamePanel.SetActive(true);
        _instance.player.SetActive(true);
        InitGame();
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Restart()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void TogglePause(bool open)
    {
        _instance.gamePanel.SetActive(!open);
        _instance.pausePanel.SetActive(open);
        _instance.player.SetActive(!open);
        Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
        _instance._state = open ? 1 : 0;
    }

    public void ToggleBag(bool open)
    {
        if (_instance._state == 1)
        {
            return;
        }
        _instance.bagView.SetActive(open);
        Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
        _instance._state = open ? 2 : 0;
    }

    public void SelectBlock(int id)
    {
        if (id == -1)
        {
            return;
        }
        _instance.selectedBlock.texture = _instance.blockTexes[id];
        _instance._selectedBlockId = id;
    }

    private void UpdateSelectedBlock()
    {
        var mousePos = Input.mousePosition;
        _instance.selectedBlock.transform.position = mousePos;
    }

    public void UnselectBlock(int displayId)
    {
        if (_instance._state != 2 || _instance._selectedBlockId == -1)
        {
            return;
        }
        _instance.selectedBlock.texture = _instance.emptyTex;
        _instance.displays[displayId].texture = _instance.blockTexes[_instance._selectedBlockId];
        _instance._selectedDisplayId = displayId;
        _instance.blockIds[displayId] = _instance._selectedBlockId;
        _instance._hotBar.transform.localPosition = _initPos + Vector2.right * (_instance._selectedDisplayId * 100);
        _instance._selectedBlockId = -1;
    }
    
    public void ToggleOptions(bool open)
    {
        _instance.optionsPanel.SetActive(open);
        _instance.homePanel.SetActive(!open);
        _instance.shaderTestObject.SetActive(open);
    }

    public void ToggleShader(int index)
    {
        foreach (var mat in _instance.materials)
        {
            mat.shader = _instance.shaders[index];
            if (index == 1 || index == 2)
            {
                mat.SetTexture("_RampTex", _instance.ramp);
            }

            if (index == 2)
            {
                mat.SetFloat("_TranslationX", 0.2f);
                mat.SetFloat("_TranslationY", -0.6f);
                mat.SetFloat("_SplitX", 0);
                mat.SetFloat("_SplitY", 0.2f);
                mat.SetFloat("_SquareScale", 0.2f);
            }

            if (index == 3)
            {
                mat.SetFloat("_HatchFactor", 0.8f);
                for (var i = 0; i < 6; i++)
                {
                    mat.SetTexture("_Hatch" + i, _instance.hatches[i]);
                }
            }
        }
    }

    public static void ToggleBloom(bool value)
    {
        _instance.bloom.enabled = value;
    }

    public static void ToggleBlur(bool value)
    {
        _instance.motionBlur.enabled = value;
    }
    
    public static void ToggleFog(bool value)
    {
        _instance.fog.enabled = value;
    }
    
    public static void ToggleDeep(bool value)
    {
        _instance.depthBlur.enabled = value;
    }
}