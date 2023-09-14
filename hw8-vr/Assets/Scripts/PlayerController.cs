using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float drag;
    public float gravity = 9.79f;
    
    private Camera _vrCamera;
    private GameObject _playerBody;
    private GameObject _head;
    private GameObject _rightArm;
    private static float _speed;
    private static Vector3 _currentPos;
    private static Vector3 _velocity;
    private static Vector3 _direction;
    private CharacterController _characterController;
    
    // Start is called before the first frame update
    void Start()
    {
        _vrCamera = GetComponentInChildren<Camera>();
        _playerBody = transform.Find("PlayerBody").gameObject;
        _head = _playerBody.transform.Find("Head").gameObject;
        _rightArm = _playerBody.transform.Find("RightArm").gameObject;
        _characterController = GetComponent<CharacterController>();
        _velocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        // 根据摄像机的朝向，旋转玩家的朝向
        var cameraForward = _vrCamera.transform.forward;
        cameraForward.y = 0;
        _playerBody.transform.forward = cameraForward.normalized;
        _playerBody.transform.localPosition = cameraForward.normalized * -0.2f;

        // 根据摄像机的朝向，旋转玩家的手臂和头部
        var cameraRotation = _vrCamera.transform.localRotation.eulerAngles;
        cameraRotation.y = 0;
        cameraRotation.z = 0;
        _head.transform.localRotation = Quaternion.Euler(cameraRotation);
        cameraRotation.x -= 90;
        _rightArm.transform.localRotation = Quaternion.Euler(cameraRotation);
        
        if (!_characterController.enabled)
        {
            return;
        }
        
        _characterController.center = _playerBody.transform.localPosition + Vector3.down * 0.8f;
    }

    void FixedUpdate()
    {
        if (!_characterController.enabled)
        {
            return;
        }
        
        if (transform.position.y < -10)
        {
            GameManager.GameOver();
            return;
        }
        
        //move at speed of _velocity of one time step
        _characterController.Move(_velocity * Time.fixedDeltaTime);

        //calculate _velocity in the next time step
        _currentPos = transform.position;
        _speed = speed;
        if (new Vector2(_velocity.x, _velocity.z).magnitude > 0.01f)
        {
            _velocity -= _direction * (drag * Time.fixedDeltaTime);
        }
        _velocity += Vector3.down * (gravity * Time.fixedDeltaTime);
    }

    public static void SetVelocity(Vector3 dest)
    {
        if (!GameManager.IsPlaying())
        {
            return;
        }
        
        _currentPos.y = 0;
        _direction = (dest - _currentPos).normalized;
        _velocity = _speed * _direction;
    }
}
