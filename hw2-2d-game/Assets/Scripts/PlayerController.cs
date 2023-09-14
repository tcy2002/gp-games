using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb;
    public float speed = 5;
    private float _jumpSpeed;
    private Vector3 _scale;
    private Animator _animator;
    public float checkRadius;
    public LayerMask platformMask;
    public GameObject checkPoint;
    public bool isOnGround;
    private bool _hit;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _scale = transform.localScale;
        _animator = GetComponent<Animator>();
        _jumpSpeed = speed * 1.3f;
    }

    // Update is called once per frame
    void Update()
    {
        isOnGround = Physics2D.OverlapCircle(checkPoint.transform.position, checkRadius, platformMask);
        _animator.SetBool("isOnGround", isOnGround);
        Move();
    }

    void Move()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");
        
        _rb.velocity = new Vector2(xInput * speed,  _rb.velocity.y);
        if (_hit && isOnGround && yInput > 0)
        {
            _rb.velocity += Vector2.up * _jumpSpeed;
            _hit = false;
        }
        
        if (xInput != 0)
        {
            transform.localScale = new Vector3(xInput * _scale.x, _scale.y, _scale.z);
        }

        _animator.SetFloat("speed", Mathf.Abs(_rb.velocity.x));
    }

    void Die()
    {
        GameManager.GameOver(true);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(checkPoint.transform.position, checkRadius);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //积分系统
        if (col.CompareTag("Spike"))
        {
            Die();
        }
        else if (col.CompareTag("Apple"))
        {
            GameManager.IncreaseScore(1);
            Destroy(col.gameObject);
        }
        else if (col.CompareTag("Banana"))
        {
            GameManager.IncreaseScore(2);
            Destroy(col.gameObject);
        }
        else if (col.CompareTag("Orange"))
        {
            GameManager.IncreaseScore(5);
            Destroy(col.gameObject);
        }
        else if (col.CompareTag("Strawberry"))
        {
            GameManager.IncreaseScore(10);
            Destroy(col.gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        _hit = true;
        if (col.gameObject.CompareTag("Spike"))
        {
            Die();
        }
    }
}
