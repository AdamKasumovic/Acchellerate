using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jukebox : MonoBehaviour
{
    public AudioSource[] music;
    // Start is called before the first frame update
    void Awake()
    {
        int absoluteBanger = Random.Range(0, music.Length);
        music[absoluteBanger].Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
