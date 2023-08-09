using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ImageFadeIn : MonoBehaviour
{
    private Image image;
    private float duration = 0.5f;
    public bool doScale = true;
    public bool doAlpha = true;

    private Vector3 initialScale;
    private Vector3 targetScale;

    void Start()
    {
        image = GetComponent<Image>(); // Get the Image component
        if (image != null)
        {
            initialScale = image.transform.localScale * 10;
            targetScale = image.transform.localScale;

            if (doScale)
                image.transform.localScale = initialScale;

            if (doAlpha)
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0);

            StartCoroutine(AnimateImage());
        }
        else
        {
            Debug.LogError("No Image component found!");
        }
    }

    IEnumerator AnimateImage()
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            if (doScale)
                image.transform.localScale = Vector3.Lerp(initialScale, targetScale, t);

            if (doAlpha)
            {
                float alpha = Mathf.Lerp(0, 1, t);
                image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            }

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        if (doScale)
            image.transform.localScale = targetScale;

        if (doAlpha)
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
    }
}
