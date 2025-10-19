using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public TextMeshProUGUI pauseTitle;
    public TextMeshProUGUI settingTitle;

    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Toggle fullscreenToggle;

    private bool isPaused = false;
    private Coroutine flashCorountine;
    private Coroutine flashSettingCorountine;

    public Transform playerTransform;

    public AudioMixer audioMixer;


    private void Start()
    {
        float mastersavedVolume = PlayerPrefs.GetFloat("Master", 1f);
        masterVolumeSlider.value = mastersavedVolume;
        MasterVolume(mastersavedVolume);
        masterVolumeSlider.onValueChanged.AddListener(MasterVolume);

        float musicsavedVolume = PlayerPrefs.GetFloat("MUsic", 1f);
        musicVolumeSlider.value = musicsavedVolume;
        MusicVolume(musicsavedVolume);
        musicVolumeSlider.onValueChanged.AddListener(MusicVolume);

        float SFXsavedVolume = PlayerPrefs.GetFloat("SFX", 1f);
        sfxVolumeSlider.value = SFXsavedVolume;
        SFXVolume(SFXsavedVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SFXVolume);
    }
    private void Update()
    {
        //Debug.Log("Escape key pressed. Current pause state: " + isPaused);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(settingsMenu.activeSelf)
            {
                BackToPauseMenu();
            }
            else if(isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false;
        isPaused = false;
        StopFlashingTitle();
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        pauseTitle.gameObject.SetActive(true);
        Time.timeScale = 0f; // pauses the game
        AudioListener.pause = true;
        isPaused = true;
        StartFlashingTitle(pauseTitle, ref flashCorountine);
        
    }

    private void StartFlashingTitle(TextMeshProUGUI title, ref Coroutine flashCoroutine)
    {
        if(flashCoroutine == null)
        {
            flashCoroutine = StartCoroutine(FlashPauseTitle(title));
        }
    }

    private void StopFlashingTitle()
    {
        if(flashCorountine != null)
        {
            StopCoroutine(flashCorountine);
            flashCorountine = null;
            pauseTitle.alpha = 1f;
            
        }

        if (flashSettingCorountine != null)
        {
            StopCoroutine(flashSettingCorountine);
            flashSettingCorountine = null; // Clear the reference
            settingTitle.alpha = 1f; // Ensure the settings title is fully visible
        }
    }

    private IEnumerator FlashPauseTitle(TextMeshProUGUI title)
    {
        Color originalColor = title.color; // Store the original color
        float flashDuration = 0.5f; // Duration of each flash

        while (true)
        {
            // Fade out
            for (float t = 0; t < flashDuration; t += Time.unscaledDeltaTime)
            {
                float normalizedTime = t / flashDuration;
                title.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1f, 0f, normalizedTime));
                yield return null;
            }

            // Fade in
            for (float t = 0; t < flashDuration; t += Time.unscaledDeltaTime)
            {
                float normalizedTime = t / flashDuration;
                title.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(0f, 1f, normalizedTime));
                yield return null;
            }
        }
    }    

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("mainmenu");
    }

    public void BackToPauseMenu()
    {
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(true);
        StartFlashingTitle(pauseTitle, ref flashCorountine);
    }

    public void GoToSettings()
    {
        settingsMenu.SetActive(true);
        pauseMenu.SetActive(false);
        StartFlashingTitle(settingTitle, ref flashSettingCorountine);
    }

    public void SaveGame()
    {
        PlayerPrefs.SetFloat("PlayerX", playerTransform.position.x);
        PlayerPrefs.SetFloat("PlayerY", playerTransform.position.y);
        PlayerPrefs.SetFloat("PlayerZ", playerTransform.position.z);

        PlayerPrefs.SetString("Scene", SceneManager.GetActiveScene().name);

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            PlayerPrefs.SetFloat("CameraX", mainCamera.transform.position.x);
            PlayerPrefs.SetFloat("CameraY", mainCamera.transform.position.y);
            PlayerPrefs.SetFloat("CameraZ", mainCamera.transform.position.z);

            // Save camera rotation
            PlayerPrefs.SetFloat("CameraRotX", mainCamera.transform.rotation.eulerAngles.x);
            PlayerPrefs.SetFloat("CameraRotY", mainCamera.transform.rotation.eulerAngles.y);
            PlayerPrefs.SetFloat("CameraRotZ", mainCamera.transform.rotation.eulerAngles.z);
        }

        PlayerPrefs.Save();

        Debug.Log("Game Saved!");
    }

    public void MasterVolume(float volume)
    {
        if (volume < 0.001f)
        {
            audioMixer.SetFloat("Master", -80f);
        }
        else
        {
            audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        }

        PlayerPrefs.SetFloat("Master", volume);
    }

    public void MusicVolume(float volume)
    {
        if (volume < 0.001f)
        {
            audioMixer.SetFloat("Music", -80f);
        }
        else
        {
            audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
        }

        PlayerPrefs.SetFloat("Music", volume);
    }

    public void SFXVolume(float volume)
    {
        if (volume < 0.001f)
        {
            audioMixer.SetFloat("SFX", -80f);
        }
        else
        {
            audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        }

        PlayerPrefs.SetFloat("SFX", volume);
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        Debug.Log("Full Screen mode: " + (isFullscreen ? "Enabled" : "Disabled"));
    }

    /*
    public void LoadGame()
    {
        if(PlayerPrefs.HasKey("PlayerX"))
        {
            float x = PlayerPrefs.GetFloat("PlayerX");
            float y = PlayerPrefs.GetFloat("PlayerY");
            float z = PlayerPrefs.GetFloat("PlayerZ");
            PlayerPrefs.SetString("Scene", SceneManager.GetActiveScene().name);

            playerTransform.position = new Vector3(x, y, z);

            string sceneName = PlayerPrefs.GetString("Scene");
            SceneManager.LoadScene(sceneName);

            Debug.Log("Game is Loaded");
        }
        else
        {
            Debug.LogWarning("No Saved Game was found");
        }
    }
    */
}
