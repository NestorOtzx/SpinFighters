using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAudio : MonoBehaviour
{
    private void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null && audioSource.clip != null)
        {
            Destroy(gameObject, audioSource.clip.length);
        }
    }
}
