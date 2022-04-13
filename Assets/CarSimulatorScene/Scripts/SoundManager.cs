using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public int musicOn = 1;
    public int soundOn = 1;

    public AudioSource bgMusic;
    public AudioSource button;
    public AudioSource Win;
    public AudioSource Lose;
    AudioSource mainAudioSource;
   
    private void Awake()
    {
        if(Instance == null)
        Instance = this;
        mainAudioSource = bgMusic;
        //if (PlayerPrefs.HasKey(StringConstants.SoundOn))
        //{
        //    musicOn = PlayerPrefs.GetInt(StringConstants.MusicOn);
        //    soundOn = PlayerPrefs.GetInt(StringConstants.SoundOn);
        //}
        //else
        //{
        //    PlayerPrefs.SetInt(StringConstants.SoundOn, 1);
        //    PlayerPrefs.SetInt(StringConstants.MusicOn, 1);
        //    musicOn = PlayerPrefs.GetInt(StringConstants.MusicOn);
        //    soundOn = PlayerPrefs.GetInt(StringConstants.SoundOn);
        //}
        if (PlayerPrefs.GetInt(StringConstants.MusicOn) == 1)
        {//on
            Play_BgMusic();
        }
        else
        {
           Stop_BgMusic();
        }

    }
    private void Start()
    {
        if (PlayerPrefs.GetInt(StringConstants.SoundOn) == 1)
        {
            RCC_Settings.Instance.audioMixer.audioMixer.SetFloat("volume", 0f);
        }
        else
        {
            RCC_Settings.Instance.audioMixer.audioMixer.SetFloat("volume", -80f);
        }
    }

    public void Play_BgMusic()
    {
        //print(soundOn);
        if (bgMusic.clip != null) // && PlayerPrefs.GetInt(StringConstants.MusicOn) == 0)
        {
            print("in bg ");
            mainAudioSource.clip = bgMusic.clip;
            mainAudioSource.Play();
        }

    }
    public void Play_WinMusic()
    {
        if (Win.clip != null && PlayerPrefs.GetInt(StringConstants.SoundOn) == 1)
        {
            print(PlayerPrefs.GetInt(StringConstants.SoundOn));
            mainAudioSource.clip = Win.clip;
            mainAudioSource.Play();
        }

    }
    public void play_Lose()
    {
        if (Lose.clip != null && soundOn == 1)
        {
            mainAudioSource.clip = Lose.clip;
            mainAudioSource.Play();
        }

    }
    public void Stop_BgMusic()
    {
        if (bgMusic.clip != null && musicOn == 1)
        {
            mainAudioSource.clip = bgMusic.clip;
            mainAudioSource.Stop();
        }

    }
    public void Stop_WinMusic()
    {
        if (Win.clip != null && soundOn == 1)
        {
            mainAudioSource.clip = Win.clip;
            Win.Stop();
        }

    }
    public void Stop_Lose()
    {
        if (Lose.clip != null && soundOn == 1)
        {
            mainAudioSource.clip = Lose.clip;
            Lose.Stop();
        }

    }
    public void PlayButtonSound()
    {
        button.PlayOneShot(button.clip);
    }
}
