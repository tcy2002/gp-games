using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //角色移动控制参数
    public float myGravity = -19.6f;
    public float moveSpeed = 5f;
    public float jumpSpeed = 8f;
    public float rotateSpeed = 3f;
    public Camera playerCamera;
    private float _mouseX, _mouseY;
    private float _horizontal, _vertical;
    private float _verticalSpeed;
    private bool _onGroundFlag;

    //光标对准的方块
    private GameObject _castCube;
    private Vector3 _hitNormal;

    private CharacterController _character;
    private LayerMask _blockLayer;

    void Start()
    {
        _character = GetComponent<CharacterController>();
        _blockLayer = LayerMask.GetMask("Ground");
    }

    void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            if (Input.GetMouseButtonDown(0))
            {
                DestroyBlock();
            }

            if (Input.GetMouseButtonDown(1))
            {
                NewBlock();
            }
            
            Jump();
            Move();
        }
    }

    void LateUpdate()
    {
        Cast();
        CameraFollow();
    }

    void FixedUpdate()
    {
        //旋转
        transform.rotation = Quaternion.Euler(0f, _mouseX, 0f);

        //移动
        var forward = transform.forward;
        var right = new Vector3(forward.z, 0, -forward.x); 
        var direction = (forward * _vertical + right * _horizontal).normalized;
        var speed = direction * moveSpeed;
        speed.y = _verticalSpeed;
        _character.Move(speed * Time.fixedDeltaTime);
    }

    void Move()
    {
        _mouseX += Input.GetAxis("Mouse X") * rotateSpeed;
        _mouseY -= Input.GetAxis("Mouse Y") * rotateSpeed;
        _mouseY = Mathf.Clamp(_mouseY, -90f, 90f);
        if (_character.isGrounded)
        {
             _horizontal = Input.GetAxisRaw("Horizontal");
             _vertical = Input.GetAxisRaw("Vertical");
        }
           
    }

    void Jump()
    {
        if (!_character.isGrounded)
        {
            if (_onGroundFlag)
            {
                _verticalSpeed = 0;
                _onGroundFlag = false;
            }
            _verticalSpeed += myGravity * Time.deltaTime;
        }
        else
        {
            if (!_onGroundFlag && _verticalSpeed < 0)
            {
                _onGroundFlag = true;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            { 
                _verticalSpeed = jumpSpeed;
                _onGroundFlag = false;
            }
        }
        
    }

    void CameraFollow()
    {
        playerCamera.transform.rotation = Quaternion.Euler(_mouseY, _mouseX, 0f);
        playerCamera.transform.position = transform.position + Vector3.up * 1.75f + transform.forward * 0.25f;
    }

    void Cast()
    {
        //摄像机位置与视角
        var pos = playerCamera.transform.position;
        var dir = playerCamera.transform.forward;
        
        //获得视线击中的方块并处理
        RaycastHit hit;
        if (Physics.Raycast(pos, dir, out hit, 6f) &&
            hit.collider.gameObject.CompareTag("Block"))
        {
            if (!_castCube)
            {
                _castCube = hit.collider.gameObject;
                _castCube.GetComponent<Renderer>().material.SetFloat("_EdgeWidth", 0.01f);
            }
            else if (hit.collider.gameObject != _castCube)
            {
                _castCube.GetComponent<Renderer>().material.SetFloat("_EdgeWidth", 0);
                _castCube = hit.collider.gameObject;
                _castCube.GetComponent<Renderer>().material.SetFloat("_EdgeWidth", 0.01f);
            }
            _hitNormal = hit.normal;
        }
        else if (_castCube)
        {
            _castCube.GetComponent<Renderer>().material.SetFloat("_EdgeWidth", 0);
            _castCube = null;
        }
    }

    void DestroyBlock()
    {
        //摧毁方块
        if (_castCube)
        {
            Destroy(_castCube);
        }
    }

    void NewBlock()
    {
        //搭建方块
        if (_castCube)
        {
            var pos = _castCube.transform.position + _hitNormal;
            GameManager.InstantiateBlock(pos);
        }
    }
}
