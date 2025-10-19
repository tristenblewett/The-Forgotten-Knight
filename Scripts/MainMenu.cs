using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject player;

    public void StartNewGame()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        SceneManager.LoadScene("introcutscene");
        Debug.Log("starting New Game");
    }

    public void ContinueGame()
    {
        if (PlayerPrefs.HasKey("Scene"))
        {
            string sceneToLoad = PlayerPrefs.GetString("Scene");
            Debug.Log("Continuing saved game: " + sceneToLoad);
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("No saved game exists");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe to prevent multiple calls

        // Restore Player position
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            if (PlayerPrefs.HasKey("PlayerX"))
            {
                float x = PlayerPrefs.GetFloat("PlayerX");
                float y = PlayerPrefs.GetFloat("PlayerY");
                float z = PlayerPrefs.GetFloat("PlayerZ");
                player.transform.position = new Vector3(x, y, z);
                Debug.Log("Player position restored to: " + new Vector3(x, y, z));
            }
        }

        // Restore Camera position and rotation (if saved)
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            if (PlayerPrefs.HasKey("CameraX"))
            {
                float camX = PlayerPrefs.GetFloat("CameraX");
                float camY = PlayerPrefs.GetFloat("CameraY");
                float camZ = PlayerPrefs.GetFloat("CameraZ");
                mainCamera.transform.position = new Vector3(camX, camY, camZ);
            }

            if (PlayerPrefs.HasKey("CameraRotX"))
            {
                float rotX = PlayerPrefs.GetFloat("CameraRotX");
                float rotY = PlayerPrefs.GetFloat("CameraRotY");
                float rotZ = PlayerPrefs.GetFloat("CameraRotZ");
                mainCamera.transform.rotation = Quaternion.Euler(rotX, rotY, rotZ);
            }
        }
    }

    public void OpenSettings()
    {
        mainMenu.SetActive(false);

        settingsMenu.SetActive(true);
    }

    public void BackToMainMenu()
    {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game!");
    }
}
