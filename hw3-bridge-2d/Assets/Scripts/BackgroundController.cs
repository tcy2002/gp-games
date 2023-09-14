using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public List<GameObject> level1;
    public List<GameObject> level2;
    public float speedLevel1;
    public float speedLevel2;

    // Update is called once per frame
    void Update()
    {
        foreach (var o in level1)
        {
            var pos = o.transform.position;
            var newPos = new Vector3(pos.x - Time.deltaTime * speedLevel1, pos.y, pos.z);
            if (newPos.x < -14.0f)
            {
                newPos.x += 27.0f;
            }
            o.transform.position = newPos;
        }
        
        foreach (var o in level2)
        {
            var pos = o.transform.position;
            var newPos = new Vector3(pos.x - Time.deltaTime * speedLevel2, pos.y, pos.z);
            if (newPos.x < -14.0f)
            {
                newPos.x += 27.0f;
            }
            o.transform.position = newPos;
        }
    }
}
