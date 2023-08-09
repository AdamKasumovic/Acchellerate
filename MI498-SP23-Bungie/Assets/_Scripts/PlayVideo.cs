using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PlayVideo : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public VideoClip[] clips;
    public void PlayVid(int i)
    {
        videoPlayer.gameObject.SetActive(true);
        StartCoroutine(PlayVidCR(i));
    }
    public void PlayTornadoVidIfApplicable()
    {
        videoPlayer.gameObject.SetActive(true);
        if(UpgradeUnlocks.groundPoundUnlockNum > 0)
        {
            StartCoroutine(PlayVidCR(5));
        }
        else
        {
            StartCoroutine(PlayVidCR(0));
        }
    }
    public void EndVid()
    {
        videoPlayer.Stop();
        videoPlayer.gameObject.SetActive(false);
    }
    public IEnumerator PlayVidCR(int i)
    {
        if (videoPlayer == null) // || string.IsNullOrEmpty(videoUrl))
            yield break;
        videoPlayer.clip = clips[i];
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
            yield return new WaitForSeconds(.1f);
        videoPlayer.Play();
    }
}