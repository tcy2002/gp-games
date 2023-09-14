using UnityEngine;

public class Emitter : MonoBehaviour
{
    public Object bulletPrefab;

    private float _time;
    
    // Start is called before the first frame update
    void Start()
    {
        _time = 0.8f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.IsPlaying())
        {
            return;
        }
        
        _time -= Time.deltaTime;
        if (_time < 0)
        {
            _time = 0.8f;
            var pos = transform.position - transform.up * transform.localScale.y;
            var bullet = (GameObject)Instantiate(bulletPrefab);
            bullet.transform.position = pos;
            bullet.transform.forward = -transform.up;
        }
    }
}
