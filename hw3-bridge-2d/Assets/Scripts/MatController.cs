using System;
using Unity.VisualScripting;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class MatController : MonoBehaviour
{
    private Vector2 _gridSize;
    private Vector2 _tiling;
    private Vector2 _cGridSize;
    private bool _valid;
    private Vector2 _startPos;
    private Vector2 _endPos;
    private LayerMask _maskObs;
    private LayerMask _maskChk;
    private Rigidbody2D _start;
    private Rigidbody2D _end;
    private float _initLen;
    private SpringJoint2D _joint;
    private ParticleSystem _particle;
    private MeshRenderer _mesh;
    private MeshRenderer _meshAssist;
    private Collider2D _collider;
    private bool _death;
    private float _freq;
    private int _type;
    private int _id;
    private AudioSource[] _audio;
    private bool _remove;

    // Start is called before the first frame update
    void Start()
    {
        _tiling = LevelManager.GetTiling();
        _gridSize.x = Screen.width / 16.0f;
        _gridSize.y = Screen.height / 9.0f;
        _cGridSize = new Vector2(8.0f, 4.5f) / _tiling;
        _maskObs = LayerMask.GetMask("Obstacle");
        _maskChk = LayerMask.GetMask("Joint");
        _particle = GetComponentInChildren<ParticleSystem>();
        _mesh = GetComponent<MeshRenderer>();
        _meshAssist = transform.GetChild(0).GetComponent<MeshRenderer>();
        _meshAssist.enabled = false;
        _collider = GetComponent<Collider2D>();
        _audio = GetComponents<AudioSource>();

        if (CompareTag("Road")) _type = 0;
        else if (CompareTag("Wood")) _type = 1;
        else _type = 2;
        
        InitMat();
    }

    // Update is called once per frame
    void Update()
    {
        if (_death)
        {
            if (!_particle.isPlaying && !_audio[0].isPlaying)
            {
                Destroy(gameObject);
            }
            return;
        }
        
        if (!_valid)
        {
            AdjustMat();
            
            if (Input.GetMouseButtonUp(0))
            {
                _valid = true;
                if (!CheckValid())
                {
                    LevelManager.FinishBuilding(false);
                    Destroy(gameObject);
                    return;
                }
                LevelManager.FinishBuilding(true);
                _initLen = Vector2.Distance(_startPos, _endPos);
                if (AddJoints())
                {
                    Download();
                    if (PlayerPrefs.GetInt("musicOn") == 0)
                    {
                        _audio[1].Play();
                    }
                }
                else
                {
                    Destroy(gameObject);
                    PlayerPrefs.SetInt(LevelManager.GetLevel() + "-valid" + _id, 0);
                    LevelManager.ReduceCost(_type, Vector2.Distance(_startPos, _endPos));
                }
            }
        }

        var state = LevelManager.GetState();
        if (state != 0)
        {
            if (!_start || !_end)
            {
                Destroy(gameObject);
                return;
            }
            Reshape();
            CheckLength();
        }
        else
        {
            if (Input.GetMouseButtonDown(1))
            {
                _remove = true;
            }
            if (Input.GetMouseButtonUp(1))
            {
                _remove = false;
            }
            if (_remove)
            {
                Remove();
            }
        }

        if (_meshAssist)
        {
            if (state == 1 && !_meshAssist.enabled && PlayerPrefs.GetInt("forceOn") == 0)
            {
                _meshAssist.enabled = true;
            } 
            if (state != 1 && _meshAssist.enabled)
            {
                _meshAssist.enabled = false;
            }   
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!_start || !_end) return;
        
        //计算碰撞力
        var startPos = _start.transform.position;
        var endPos = _end.transform.position;
        var len = Vector2.Distance(startPos, endPos);
        var vel = col.relativeVelocity;
        vel.x = 0;
        var mass = LevelManager.GetMatMass() * (len / 0.5f);
        var otherMass = LevelManager.GetCarMass();
        var rate = 2 * otherMass / (otherMass + mass);
        var force = otherMass * Math.Abs(Physics2D.gravity.y) * rate * vel;
        
        //计算碰撞点到两端的比例
        var point = col.GetContact(0).point;
        var left = Vector2.Distance(point, startPos);
        var right = Vector2.Distance(point, endPos);
        var lr = right / (left + right);
        var rr = 1 - lr;
        
        //将碰撞力施加在两个端点处
        _start.AddForce(force * lr);
        _end.AddForce(force * rr);
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (!_start || !_end) return;
        
        //计算重力
        var otherMass = LevelManager.GetCarMass();
        var force = otherMass * Physics2D.gravity;
        
        //计算碰撞点到两端的比例
        var point = col.GetContact(0).point;
        var left = Vector2.Distance(point, _start.transform.position);
        var right = Vector2.Distance(point, _end.transform.position);
        var lr = right / (left + right);
        var rr = 1 - lr;
        
        //将重力施加在两个端点处
        _start.AddForce(force * lr);
        _end.AddForce(force * rr);
    }

    void InitMat()
    {
        //初始化当前桥梁部件的参数
        var pos = transform.position;
        if (pos.z < 1)
        {
            //这是导入的部件
            _valid = true;
            _initLen = transform.localScale.x;
            var angle = transform.rotation.eulerAngles.z * (Math.PI / 180);
            var halfDist = _initLen / 2;
            var sin = (float)Math.Sin(angle) * halfDist;
            var cos = (float)Math.Cos(angle) * halfDist;
            _startPos = new Vector2(pos.x - cos, pos.y - sin);
            _endPos = new Vector2(pos.x + cos, pos.y + sin);
            AddJoints();
            Download();
        }
        else
        {
            //这是新建的部件
            var p = Input.mousePosition / _gridSize - new Vector2(8.0f, 4.5f);
            var pr = Vector2Int.RoundToInt(p / _cGridSize);
            _startPos = new Vector2(pr.x, pr.y) * _cGridSize;
            transform.position = _startPos;
        }
    }

    void AdjustMat()
    {
        //根据鼠标位置动态调整桥梁部件的参数
        var p = Input.mousePosition / _gridSize - new Vector2(8.0f, 4.5f);
        p = _startPos + Vector2.ClampMagnitude(p - _startPos, LevelManager.GetMaxLength());
        var pi = Vector2Int.RoundToInt(p / _cGridSize);
        _endPos = new Vector2(pi.x, pi.y) * _cGridSize;
        var dist = Vector2.Distance(_startPos, _endPos);
        LevelManager.UpdateCost(_type, dist);
        var angle = Vector2.SignedAngle(Vector2.right, _endPos - _startPos);
        Vector3 pos = (_startPos + _endPos) / 2;
        pos.z = _type * 0.1f;

        transform.rotation = Quaternion.Euler(0, 0, angle);
        transform.position = pos;
        var wid = 1.8f / _tiling.y;
        transform.localScale = new Vector3(dist > 0.1f ? dist : wid, wid, 1);
    }

    bool CheckValid()
    {
        //检查部件是否达到最小长度限制
        if (transform.localScale.x < 0.25f || !LevelManager.CostUnderLimit())
        {
            return false;
        }
        
        //检查部件的位置是否合法（不能穿过障碍物）
        var len = Vector3.Distance(_startPos, _endPos);
        var n = (int)Math.Round(len / 0.5f) + 1;
        for (var i = 0; i <= n; i++)
        {
            var pos = Vector3.Lerp(_startPos, _endPos, 1.0f * i / n);
            if ((i == 0 || i == n) && Physics2D.OverlapCircle(pos, 0.1f, _maskChk))
            {
                continue;
            }
            if (Physics2D.OverlapCircle(pos, 0.1f, _maskObs))
            {
                return false;
            }
        }

        return true;
    }

    bool AddJoints()
    {
        //获取相邻部件的锚点
        var collider1 = Physics2D.OverlapCircle(_startPos, 0.1f, _maskChk);
        var collider2 = Physics2D.OverlapCircle(_endPos, 0.1f, _maskChk);
        Rigidbody2D body;
        
        //根据不同情况分别创建铰链
        if (collider1 && !collider2)
        {
            var other = Instantiate(LevelManager.GetJoint(), _endPos, Quaternion.identity);
            _joint = other.AddComponent<SpringJoint2D>(); 
            body = collider1.GetComponent<Rigidbody2D>();
            _start = body;
            _end = other.GetComponent<Rigidbody2D>();
        }
        else if (!collider1 && collider2)
        {
            var other = Instantiate(LevelManager.GetJoint(), _startPos, Quaternion.identity);
            _joint = other.AddComponent<SpringJoint2D>();
            body = collider2.GetComponent<Rigidbody2D>();
            _start = other.GetComponent<Rigidbody2D>();
            _end = body;
        }
        else if (!collider1 && !collider2)
        {
            var point1 = Instantiate(LevelManager.GetJoint(), _startPos, Quaternion.identity);
            var point2 = Instantiate(LevelManager.GetJoint(), _endPos, Quaternion.identity);
            _joint = point1.AddComponent<SpringJoint2D>();
            body = point2.GetComponent<Rigidbody2D>();
            _start = point1.GetComponent<Rigidbody2D>();
            _end = body;
        }
        else
        {
            //检查是否有部件重合
            var body1 = collider1.GetComponent<Rigidbody2D>();
            body = collider2.GetComponent<Rigidbody2D>();
            var joint1 = collider1.GetComponents<SpringJoint2D>();
            var joint2 = collider2.GetComponents<SpringJoint2D>();
            
            foreach (var j in joint1)
            {
                if (j.connectedBody == body)
                {
                    return false;
                }
            }
            foreach (var j in joint2)
            {
                if (j.connectedBody == body1)
                {
                    return false;
                }
            }
            
            _joint = collider1.AddComponent<SpringJoint2D>();
            _start = body1;
            _end = body;
        }
        
        _joint.connectedBody = body;
        _joint.frequency = LevelManager.GetFreq(_type);
        _joint.autoConfigureDistance = true;
        return true;
    }

    void Reshape()
    {
        if (_death)
        {
            return;
        }
        
        //模拟过程中动态更改部件的几何参数
        Vector2 startPos = _start.transform.position;
        Vector2 endPos = _end.transform.position;
        var dist = Vector2.Distance(startPos, endPos);
        var angle = Vector2.SignedAngle(Vector2.right, endPos - startPos);
        Vector3 pos = (endPos + startPos) / 2;
        pos.z = _type * 0.1f;
        
        transform.rotation = Quaternion.Euler(0, 0, angle);
        transform.position = pos;
        var wid = 1.8f / _tiling.y;
        transform.localScale = new Vector3(dist, wid, 1);
    }

    void Remove()
    {
        //删除当前部件
        var p = Input.mousePosition / _gridSize - new Vector2(8.0f, 4.5f);
        if (_collider.OverlapPoint(p))
        {
            var level = LevelManager.GetLevel();
            PlayerPrefs.SetInt(level + "-valid" + _id, 0);
            LevelManager.ReduceCost(_type, Vector2.Distance(_startPos, _endPos));
            _death = true;
            Destroy(_mesh);
            Destroy(_meshAssist);
            Destroy(_joint);
            Destroy(_collider);
            if (PlayerPrefs.GetInt("musicOn") == 0)
            {
                _audio[0].Play();
            }
        }
    }

    void CheckLength()
    {
        //更新参考线的颜色深度
        var len = Vector2.Distance(_start.transform.position, _end.transform.position);
        var maxRatio = LevelManager.GetMaxRatio();
        var ratio = Math.Abs(len / _initLen - 1);
        _meshAssist.material.color = new Color(0, 0, 1, ratio / maxRatio);

        //当长度超出弹性限度时，销毁部件
        if (ratio > maxRatio)
        {
            if (!_death && !_particle.isPlaying)
            {
                _death = true;
                Destroy(_mesh);
                Destroy(_meshAssist);
                Destroy(_joint);
                Destroy(_collider);
                _particle.Play();
                if (PlayerPrefs.GetInt("musicOn") == 0)
                {
                    _audio[0].Play();
                }
            }
        }
    }

    void Download()
    {
        //保存数据
        _id = LevelManager.GetId();
        var level = LevelManager.GetLevel();
        PlayerPrefs.SetInt(level + "-num", _id + 1);

        var p = transform.position;
        PlayerPrefs.SetFloat(level + "-x" + _id, p.x);
        PlayerPrefs.SetFloat(level + "-y" + _id, p.y);
        PlayerPrefs.SetFloat(level + "-dist" + _id, transform.localScale.x);
        PlayerPrefs.SetFloat(level + "-angle" + _id, transform.rotation.eulerAngles.z);
        PlayerPrefs.SetInt(level + "-type" + _id, _type);
        PlayerPrefs.SetInt(level + "-valid" + _id, 1);
    }
}
