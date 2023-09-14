using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject car;
    public GameObject background1;
    public GameObject background2;
    public float bg1Speed;
    public float bg2Speed;

    private Vector3 _camOffset;
    private Vector3 _bg1Offset;
    private Vector3 _bg2Offset;
    
    // Start is called before the first frame update
    void Start()
    {
        _camOffset = transform.position;
        _bg1Offset = background1.transform.position;
        _bg2Offset = background2.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var state = LevelManager.GetState();
        if (state == 1)
        {
            if (car)
            {
                var pos = car.transform.position;
                transform.position = new Vector3(pos.x, 0, 0) + _camOffset;
                background1.transform.position = new Vector3(pos.x * bg1Speed, 0, 0) + _bg1Offset;
                background2.transform.position = new Vector3(pos.x * bg2Speed, 0, 0) + _bg2Offset;
            }
        }
        else if (state == 0)
        {
            transform.position = _camOffset;
            background1.transform.position = _bg1Offset;
            background2.transform.position = _bg2Offset;
        }
    }
}
