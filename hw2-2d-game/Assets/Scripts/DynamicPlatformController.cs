using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicPlatformController : MonoBehaviour
{
    public float speed = 1;
    public float toggleTime = 3;
    private float _countTime;
    private bool _right;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _countTime += Time.deltaTime;

        if (_right && transform.position.x < 3.5)
        { 
            transform.position += Vector3.right * (speed * Time.deltaTime);
        }
        else if (!_right && transform.position.x > -3.5)
        {
            transform.position += Vector3.left * (speed * Time.deltaTime);
        }

        if (_countTime > toggleTime || transform.position.x < -3.5 || transform.position.x > 3.5)
        {
            _right = !_right;
            _countTime = 0;
        }
    }
}
