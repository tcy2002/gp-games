using System.Collections.Generic;
using UnityEngine;

public class SpawnRewards : MonoBehaviour
{
    public Vector2 radius;
    public float height;
    public int count;
    public List<Object> rewards;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RandomRewards()
    {
        var size = rewards.Count;

        for (var i = 0; i < count; i++)
        {
            var n = Random.Range(0, size * 3);
            var index = 0;
            if (n <= size / 3)
            {
                index = 0;
            }
            else if (n < size)
            {
                index = 1;
            }
            else
            {
                index = 2;
            }
            
            var pos = Vector3.zero;
            pos.x = Mathf.Round(Random.Range(-radius.y, radius.y) - 0.5f) + 0.5f;
            pos.z = Mathf.Round(Random.Range(-radius.y, radius.y) - 0.5f) + 0.5f;
            if (Mathf.Abs(pos.x) < radius.x && Mathf.Abs(pos.z) < radius.x)
            {
                i--;
                continue;
            }
            pos.y = height;
            var reward = (GameObject)Instantiate(rewards[index], transform, false);
            reward.transform.localPosition = pos;
        }
        
    }
}
