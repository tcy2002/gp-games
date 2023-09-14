using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<GameObject> platforms;
    public List<GameObject> traps;
    public List<GameObject> fruits;

    public float spawnTime = 2.7f;
    private float _countTime;
    private bool _spike;
    private bool _platform;
    private int _platformIndex;
    private int _trapIndex;

    // Start is called before the first frame update
    void Start()
    {
        _countTime = spawnTime;
    }

    // Update is called once per frame
    void Update()
    {
        _countTime += Time.deltaTime;
        int level = GameManager.GetLevel();
        float mySpawnTime = spawnTime / (1 + (level - 1) * 0.5f);

        if (_countTime >= mySpawnTime)
        {
            //最多连续一层陷阱
            if (Random.Range(0, 3) < 1 && !_spike)
            {
                _trapIndex = SpawnTrap(Random.Range(-3.5f, 3.5f), _trapIndex);
                _spike = true;
            }
            else
            {
                float offset = Random.Range(-3.5f, 3.5f);
                _platformIndex = SpawnPlatform(offset, _platformIndex);
                if (Random.Range(0, 3) < 2)
                {
                    float offset1 = offset > 0 ? offset - 3.5f : offset + 3.5f;
                    if (Random.Range(0, 3) < 2)
                    {
                        _trapIndex = SpawnTrap(offset1, _trapIndex);
                    }
                    else
                    {
                        _platformIndex = SpawnPlatform(offset1, _platformIndex);
                    }
                }
                _spike = false;
            }
            _countTime = 0;
        }
    }

    int SpawnPlatform(float offset, int last)
    {
        Vector3 spawnPosition = transform.position;
        spawnPosition.x = offset;

        int index;
        //保证静止平台的刷新几率
        if (!_platform)
        {
            index = platforms.Count - 1;
            _platform = true;
        }
        else
        {
            do
            {
                index = Random.Range(0, platforms.Count);
            } while (index == last);
            _platform = (index == platforms.Count - 1);
        }
        
        spawnPosition.z = 0.4f + 0.05f * index;
        GameObject go = Instantiate(platforms[index], spawnPosition, Quaternion.identity);
        go.transform.SetParent(transform);

        if (Random.Range(0, 3) < 2)
        {
            int index1 = Random.Range(0, 18);
            if (index1 < 10) index1 = 0;
            else if (index1 < 15) index1 = 1;
            else if (index1 < 17) index1 = 2;
            else index1 = 3;
            GameObject fu = Instantiate(fruits[index1], spawnPosition + (Vector3.up * 0.7f), Quaternion.identity);
            fu.transform.SetParent(go.transform);
        }

        return index;
    }

    int SpawnTrap(float offset, int last)
    {
        Vector3 spawnPosition = transform.position;
        spawnPosition.x = offset;

        int index;
        do
        {
            index = Random.Range(0, platforms.Count);
        } while (index == last);

        spawnPosition.z = 0.2f + 0.05f * index;
        GameObject go = Instantiate(traps[index], spawnPosition, Quaternion.identity);
        
        go.transform.SetParent(transform);
        return index;
    }
}
