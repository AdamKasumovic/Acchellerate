using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageSetter : MonoBehaviour
{
    public int lastState = -1;
    public Image[] images;
    // Start is called before the first frame update
    void Start()
    {
        images = GetComponentsInChildren<Image>();
    }

    // Update is called once per frame
    public void UpdateImage(int state, Sprite highImage, Sprite lowImage)
    {
        for (int i = 0; i < images.Length; i++)
        {
            if (i < state)
            {
                images[i].sprite = highImage;
            }
            else
            {
                images[i].sprite = lowImage;
            }
        }
        lastState = state;
    }
}
