using UnityEngine;
using UnityEngine.UI;

public class ToggleController : MonoBehaviour
{
    private Image _image;
    
    // Start is called before the first frame update
    void Start()
    {
        _image = transform.GetChild(0).GetComponent<Image>();
    }

    public void OnSelect()
    {
        _image.enabled = !_image.enabled;
    }
}
