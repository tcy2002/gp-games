using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMove : MonoBehaviour
{
    public float speed = 1;
    private Material _material;
    private Vector2 _movement;
    
    // Start is called before the first frame update
    void Start()
    {
        _material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        int level = GameManager.GetLevel();
        MoveTexture(speed + (level - 1) * 0.5f);
    }

    void MoveTexture(float spd)
    {
        _movement -= Vector2.up * (Time.deltaTime * spd);
        _material.mainTextureOffset = _movement;
    }
}