using UnityEngine;
using System.Collections;
using TMPro;

// attach to UI Text component (with the full text already there)

public class TutorialText : MonoBehaviour
{
	public TextMeshProUGUI text;
	public int originalContentLength;

	//public AK.Wwise.Event Speech;
	public bool finished = false;
	public int transparencyIndex = 0;
	public bool doTextScrolling = false;

	void Start()
	{
		text = GetComponent<TextMeshProUGUI>();
		originalContentLength = text.text.Length;
		if (!doTextScrolling)
			transparencyIndex = originalContentLength;
		//Debug.Log(ColorUtility.ToHtmlStringRGBA(text.color));
		text.text = text.text.Insert(transparencyIndex, "<color=#" + ColorUtility.ToHtmlStringRGB(text.color) + "00>") + "</color>";

		StartCoroutine("PlayText");
	}

    private void Update()
    {
        if (!finished && transparencyIndex == originalContentLength)
        {
			finished = true;
        }
    }

    IEnumerator PlayText()
	{
		while (transparencyIndex < originalContentLength)
		{ 
			//Speech.Post(gameObject);  Optionally play a sound here, Aidan
			++transparencyIndex;
			//Debug.Log(text.text[transparencyIndex + ("color=#" + ColorUtility.ToHtmlStringRGB(text.color) + "00>").Length]);
			int nextCharIndex = transparencyIndex + ("color=#" + ColorUtility.ToHtmlStringRGB(text.color) + "00>").Length;
			if (text.text[nextCharIndex] == '<')  // need to skip sprite tags
            {
				transparencyIndex += 25;  // Fixed length of sprite tags
            }
			text.text = text.text.Replace("<color=#" + ColorUtility.ToHtmlStringRGB(text.color) + "00>", "").Insert(transparencyIndex, "<color=#" + ColorUtility.ToHtmlStringRGB(text.color) + "00>");
			//yield return new WaitForSeconds(0.05f);
			yield return null;
		}
	}

}
