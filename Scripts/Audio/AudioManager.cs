using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {get;private set;}
    [SerializeField]private AudioSource BGM;
    [SerializeField]private AudioSource SFX;

    [SerializeField]private AudioClip ClickButtonSFX;
    [SerializeField]private AudioClip WinSFX;
    [SerializeField]private AudioClip LoseSFX;
    [SerializeField]private AudioClip ClickTileSFX;

    void Awake()
    {
        Instance = this;
    }

    private void Start() {
        if(GameData.SFX ==1)
            UnMuteSFX();
        else 
            MuteSFX();
        if(GameData.BGM ==1)
            UnMuteBGM();
        else 
            MuteBGM();
    }
    public void UnPauseBGM()
    {
        BGM.UnPause();
    }
    public void PauseBGM()
    {
        BGM.Pause();
    }

    public void MuteBGM()
    {
        BGM.mute = true;
        GameData.BGM = 0;
    }
    public void UnMuteBGM()
    {
        BGM.mute = false;
        GameData.BGM = 1;
    }
    public void MuteSFX()
    {
        SFX.mute = true;
        GameData.SFX = 0;
    }
    public void UnMuteSFX()
    {
        SFX.mute = false;
        GameData.SFX = 1;
    }

    public void PlayClickButtonSFX()
    {
        SFX.clip = ClickButtonSFX;
        SFX.Play();
    }
    public void PlayClickTileSFX()
    {
        SFX.clip = ClickTileSFX;
        SFX.Play();
    }
    public void PlayWinSFX()
    {
        SFX.clip = WinSFX;
        SFX.Play();
    }
    public void PlayLoseSFX()
    {
        SFX.clip = LoseSFX;
        SFX.Play();
    }


    
}
