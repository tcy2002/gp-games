using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    private Animator _animator;
    private PolygonCollider2D _poly;
    public float toggleTime = 2;
    private bool _fireOn = true;
    private bool _triggerOn = true;
    private float _countTime;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _poly = GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _countTime += Time.deltaTime;

        if (_fireOn && _triggerOn)
        {
            if (_countTime > toggleTime)
            {
                _triggerOn = false;
                _poly.enabled = false;
                _fireOn = false;
                _animator.SetBool("On", false);
                _countTime = 0;
            }
        }
        else if (_fireOn && !_triggerOn)
        {
            if (_countTime > 0.2)
            {
                _triggerOn = true;
                _poly.enabled = true;
                _countTime = 0;
            }
        } 
        else if (!_fireOn && !_triggerOn)
        {
            if (_countTime > toggleTime)
            {
                _fireOn = true;
                _animator.SetBool("On", true);
                _countTime = 0;
            }
        }
    }
}
