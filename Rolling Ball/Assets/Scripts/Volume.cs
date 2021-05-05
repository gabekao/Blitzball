using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class Volume : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;

    public void SetSFXLevel(float sliderVal){
        mixer.SetFloat("SFXVolume", Mathf.Log10(sliderVal) * 20);
    }

    public void SetMusicLevel(float sliderVal){
        mixer.SetFloat("MusicVolume", Mathf.Log10(sliderVal) * 20);
    }
}
