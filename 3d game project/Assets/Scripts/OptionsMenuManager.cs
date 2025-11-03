using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsMenuManager : MonoBehaviour
{
    [Header("Audio Controls")]
    public AudioMixer mainMixer;
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        // Load existing values from Audio Mixer and set slider positions
        float musicVol;
        float sfxVol;

        mainMixer.GetFloat("MusicVolume", out musicVol);
        mainMixer.GetFloat("SFXVolume", out sfxVol);

        // Convert decibel values to linear slider scale (0–1)
        musicSlider.value = Mathf.Pow(10, musicVol / 20);
        sfxSlider.value = Mathf.Pow(10, sfxVol / 20);
    }

    // Called when Music slider is moved
    public void SetMusicVolume(float value)
    {
        mainMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
    }

    // Called when SFX slider is moved
    public void SetSFXVolume(float value)
    {
        mainMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
    }

    // Called by the "Back" button
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}