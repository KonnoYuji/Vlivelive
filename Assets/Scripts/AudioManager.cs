using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    static private AudioManager _instance;

    static public AudioManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<AudioManager>();
            }
            return _instance;
        }
    }

    [SerializeField]
    private AudioClip defaultBGM, cheers;

    [SerializeField]
    private bool isOnBGM = false;

    [SerializeField]
    private AudioSource[] bgmPlayers;

    [SerializeField]
    private AudioSource[] cheerPlayers;

    private float bgmSoundVolume = 1.0f;

    public float BgmSoundVolume
    {
        get
        {
            return bgmSoundVolume;
        }
        set
        {
            bgmSoundVolume = value;
            ChangeBGMSoundVolume();
        }
    }
   
    private float cheerSoundVolume = 1.0f;

    public float CheerSoundVolume
    {
        get
        {
            return cheerSoundVolume;
        }
        set
        {
            cheerSoundVolume = value;
            ChangeCheerSoundVolume();
        }        
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<AudioManager>();
        }
        
        if(isOnBGM)
        {
            PlayDefaultBGM();
        }        
    }

    public void PlaySound(AudioSource source)
    {
        if(source.clip == null)
        {
            source.Play();
        }
        else
        {
            Debug.Log("No AudioClip");
        }
    }

    public void PlayCheerSound()
    {
        for(int i=0; i<cheerPlayers.Length; i++)
        {
            cheerPlayers[i].clip = cheers;
            cheerPlayers[i].Play();       
        }
    }

    private void PlayDefaultBGM()
    {
        for(int i=0; i<bgmPlayers.Length; i++)
        {
            bgmPlayers[i].clip = defaultBGM;
            bgmPlayers[i].Play();
        }
    }

    private void ChangeCheerSoundVolume()
    {

    }

    private void ChangeBGMSoundVolume()
    {

    }
}
