using UnityEngine;

public class ScoringController : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Star"))
        {
            LevelManager.AddScore(1);
            Destroy(other.gameObject);
        }
    }
}
