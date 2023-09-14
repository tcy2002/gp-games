using UnityEngine;

public class RotController : MonoBehaviour
{
    public float rotSpeed = 2;
    public bool rot;

    private Quaternion _initAngle;

    public void SetRot(bool value)
    {
        rot = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        _initAngle = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (rot)
        {
            transform.Rotate(Vector3.up, rotSpeed * Time.deltaTime);
        }
        else
        {
            transform.rotation = _initAngle;
        }
    }
}
