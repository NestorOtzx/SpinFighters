using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioClip [] backgroundClips;

    public GameObject [] audioEffects;

    public AudioSource backgroundAudioSource;

    int currentBackgroundMusic = -1;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded+= OnSceneLoaded;
        }else{
            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded-= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        if (scene.name == Utilities.SceneNames.MainMenu.ToString() ||
            scene.name == Utilities.SceneNames.JoinMatch.ToString() ||
            scene.name == Utilities.SceneNames.SinglePlayer.ToString() ||
            scene.name == Utilities.SceneNames.Credits.ToString() ||
            scene.name == Utilities.SceneNames.MatchMaking.ToString()){ //menu song

            SetBackgroundAudio(0);
        }else if (scene.name == Utilities.SceneNames.ScoresScreen.ToString()){ //end game screen
            SetBackgroundAudio(1);
        }else{ //levels
            if (scene.name == Utilities.SceneNames.Beach.ToString())
            {
                SetBackgroundAudio(2);
            }else if (scene.name == Utilities.SceneNames.Volcano.ToString())
            {
                SetBackgroundAudio(3);
            }else if (scene.name == Utilities.SceneNames.Mountain.ToString())
            {
                SetBackgroundAudio(4);
            }
        }
    }

    public void SetBackgroundAudio(int audioID)
    {
        if (currentBackgroundMusic != audioID)
        {
            currentBackgroundMusic = audioID;
            backgroundAudioSource.clip = backgroundClips[audioID];
            backgroundAudioSource.Play();
        }
    }

    public void PlayAudio(int audioEffect)
    {
        Instantiate(audioEffects[audioEffect], transform);
    }

    public void PlayAudio(int audioEffect, Vector3 position)
    {
        Instantiate(audioEffects[audioEffect], position, Quaternion.identity, transform);
    }
}
