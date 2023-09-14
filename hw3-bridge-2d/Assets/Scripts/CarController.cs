using UnityEngine;

public class CarController : MonoBehaviour
{
    private Rigidbody2D _wheel;

    public float force;
    
    // Start is called before the first frame update
    void Start()
    {
        _wheel = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (LevelManager.GetState() == 1)
        {
            var input = Input.GetAxis("Horizontal");
            if (input > 0.5 || input < -0.5)
            {
                _wheel.AddTorque(-force * input);
            }

            var dest = LevelManager.GetDestination();
            if (transform.position.x >= dest.x && transform.position.y >= dest.y)
            {
                LevelManager.GameOver(true);
            }

            if (transform.position.y < -8)
            {
                LevelManager.GameOver(false);
            }
        }
    }
}
