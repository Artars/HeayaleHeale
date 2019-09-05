using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MuteButton : MonoBehaviour
{
    public AudioMixer audioMixer;
    public string audioVariableName = "MasterVolume";
    public UnityEngine.UI.Image image;
    public Sprite playingIcon;
    public Sprite notPlayingIcon;

    public void OnEnable()
    {
        int playAudio = PlayerPrefs.GetInt("PlayAudio", 1);

        float volume = (playAudio == 1) ? 0 : -80;

        audioMixer.SetFloat(audioVariableName, volume);

        if(image != null)
        {
            image.sprite = (playAudio == 1) ? playingIcon : notPlayingIcon;
        }
    }

    public void OnClick()
    {
        int previousState = PlayerPrefs.GetInt("PlayAudio", 1);
        int playAudio = (previousState == 1) ? 0 : 1;
        PlayerPrefs.SetInt("PlayAudio", playAudio);

        float volume = (playAudio == 1) ? 0 : -80;
        audioMixer.SetFloat(audioVariableName, volume);

        if(image != null)
        {
            image.sprite = (playAudio == 1) ? playingIcon : notPlayingIcon;
        }
        
    }
}
