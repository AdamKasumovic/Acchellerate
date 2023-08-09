using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackFade : MonoBehaviour
{
    [Tooltip("Time it takes to fade in seconds.")]
    public float fadeSpeed = 1;
    public Image image;
    private float lerper;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lerper += Time.deltaTime;
        lerper = Mathf.Min(fadeSpeed, lerper);
        if (lerper == fadeSpeed)
            image.enabled = false;  // image is blocking UI buttons
        else
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(1, 0, lerper / fadeSpeed));
    }

    private void OnDisable()
    {
        lerper = 0;
        image.enabled = true;
    }
}
