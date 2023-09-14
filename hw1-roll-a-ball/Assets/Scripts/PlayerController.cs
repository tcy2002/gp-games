using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public TMP_Text countText;
    private int count;
    
    // Start is called before the first frame update
    void Start()
    {
        count = 0;
        setCountText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        
        GetComponent<Rigidbody>().AddForce(movement * (speed * Time.deltaTime));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PickUp")
        {
            other.gameObject.SetActive(false);
            count++;
            if (count < 8)
            {
                setCountText();
            }
            else
            {
                gameOver();
            }
        }
    }

    private void setCountText()
    {
        countText.text = "Count: " + count;
    }

    private void gameOver()
    {
        countText.text = "You win!";
    }
}
