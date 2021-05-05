using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class Volume : MonoBehaviour
{
    void Start(){
        if(PlayerPrefs.HasKey("SFXVol") && gameObject.name == ("SFXSlider")){
            float temp = PlayerPrefs.GetFloat("SFXVol");
            gameObject.GetComponent<Slider>().value = temp;
            SetSFXLevel(temp);
        }
        if(PlayerPrefs.HasKey("MusicVol") && gameObject.name == ("MusicSlider")){
            float temp = PlayerPrefs.GetFloat("MusicVol");
            gameObject.GetComponent<Slider>().value = temp;
            SetMusicLevel(temp);
        }

    }
    [SerializeField] AudioMixer mixer;

    public void SetSFXLevel(float sliderVal){
        mixer.SetFloat("SFXVolume", Mathf.Log10(sliderVal) * 20);
        PlayerPrefs.SetFloat("SFXVol", sliderVal);
    }

    public void SetMusicLevel(float sliderVal){
        mixer.SetFloat("MusicVolume", Mathf.Log10(sliderVal) * 20);
        PlayerPrefs.SetFloat("MusicVol", sliderVal);
    }
}
