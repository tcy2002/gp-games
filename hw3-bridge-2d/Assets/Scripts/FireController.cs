using System;
using UnityEngine;

public class FireController : MonoBehaviour
{
    public float force;
    
    private Animator _animator;
    private Rigidbody2D _body;
    private float _beginTime;
    
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _body = GetComponent<Rigidbody2D>();
        _beginTime = -1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (LevelManager.GetState() == 1 && Input.GetAxis("Vertical") < 0)
        {
            if (_beginTime < -0.5f)
            {
                _beginTime = Time.time;
            }
            else if (Time.time - _beginTime < 0.2f)
            {
                var angle = _body.rotation * Math.PI / 180;
                var forceCorrect = force * Time.deltaTime / 0.01f;
                _body.AddForce(new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * forceCorrect);
                _animator.SetBool("fireOn", true);
            }
            else
            {
                _animator.SetBool("fireOn", false);
            }
        }
        else
        {
            _animator.SetBool("fireOn", false);
        }
    }
}
