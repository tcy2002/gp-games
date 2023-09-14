using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTerrain : MonoBehaviour
{
    public Object terrainBlockPrefab;
    public Vector3 terrainSize;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateTerrain()
    {
        for (var zi = 0; zi < terrainSize.z; zi++)
        {
            for (var xi = 0; xi < terrainSize.x; xi++)
            {
                for (var yi = 0; yi < terrainSize.y; yi++)
                {
                    var block = (GameObject)Instantiate(terrainBlockPrefab, transform, false);
                    block.transform.localPosition = new Vector3(xi, -zi, yi);
                }
            }
        }
    }
}
