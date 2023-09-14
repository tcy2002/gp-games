using System;
using System.Collections;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public float speed = 1;
    private GameObject _topLine;
    
    // Start is called before the first frame update
    void Start()
    {
        _topLine = GameObject.Find("TopLine");
    }

    // Update is called once per frame
    void Update()
    {
        int level = GameManager.GetLevel();
        Move(speed + (level - 1) * 0.5f);
    }

    void Move(float spd)
    {
        transform.position += Vector3.up * (spd * Time.deltaTime);
        if (transform.position.y > _topLine.transform.position.y)
        {
            Destroy(gameObject);
        }
    }
}
