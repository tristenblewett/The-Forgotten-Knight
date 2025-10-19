using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider volumeSlider;
    

    private void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("Master", 1f);
        volumeSlider.value = savedVolume;

        SetVolume(savedVolume);
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    // Update is called once per frame
    public void SetVolume(float volume)
    {
        if(volume == 0)
        {
            audioMixer.SetFloat("Master", -80f);
        }
        else
        {
            audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        }

        PlayerPrefs.SetFloat("Master", volume);
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        Debug.Log("Full Screen mode: " + (isFullscreen ? "Enabled" : "Disabled"));
    }

}
