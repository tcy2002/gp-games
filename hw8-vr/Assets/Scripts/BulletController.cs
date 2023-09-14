using System;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed;
    public float lastingTime;
    public float gravity;

    private Vector3 _velocity;
    
    // Start is called before the first frame update
    void Start()
    {
        _velocity = transform.forward * speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.IsPlaying())
        {
            return;
        }
        
        DetectCollision();
        lastingTime -= Time.deltaTime;
        if (lastingTime < 0)
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        if (!GameManager.IsPlaying())
        {
            return;
        }
        
        //move at speed of _velocity of one time step
        transform.position += _velocity * Time.fixedDeltaTime;
        
        //calculate _velocity in the next time step
        _velocity.y -= gravity * Time.fixedDeltaTime;

        if (transform.position.x > -8 && transform.position.x < 8 &&
            transform.position.z > -8 && transform.position.z < 8 &&
            transform.position.y < -1.7f)
        {
            _velocity.y = -_velocity.y;
        }
    }

    private void DetectCollision()
    {
        //检查子弹是否碰撞到方块
        var colliders = Physics.OverlapSphere(transform.position, transform.localScale.x / 2);
        foreach (var collider in colliders)
        {
            if (collider.gameObject.CompareTag("Reward"))
            {
                if (collider.gameObject.name.StartsWith("WoolRed"))
                {
                    GameManager.AddScore(10);
                }
                else if (collider.gameObject.name.StartsWith("WoolBlue"))
                {
                    GameManager.AddScore(3);
                }
                else
                {
                    GameManager.AddScore(1);
                }
                Destroy(gameObject);
                Destroy(collider.gameObject);
                return;
            }
        }
    }
}
