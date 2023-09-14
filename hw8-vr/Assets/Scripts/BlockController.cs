using UnityEngine;
using UnityEngine.EventSystems;

public class BlockController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnPointEnter() {
        GetComponent<Renderer>().material.SetFloat("_EdgeWidth", 0.01f);
    }
    
    public void OnPointExit() {
        GetComponent<Renderer>().material.SetFloat("_EdgeWidth", 0.0f);
    }
    
    public void OnPointClick()
    {
        var pos = transform.position;
        pos.y = 0;
        PlayerController.SetVelocity(pos);
    }
}
