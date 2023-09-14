using UnityEngine;

public class PointController : MonoBehaviour
{
    private float _angle;

    // Update is called once per frame
    void Update()
    {
        //随时间旋转
        _angle += Time.deltaTime * 30;
        if (_angle > 360)
        {
            _angle -= 360;
        }
        transform.rotation = Quaternion.Euler(0, 0, _angle);
    }
}
