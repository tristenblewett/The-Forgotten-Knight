using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class IntroCutscene : MonoBehaviour
{
    public TextMeshProUGUI letterText;
    public string[] cutsceneLines;

    private float typingSpeed = 0.05f;

    private string nextSceneName = "Level_1";

    public AudioSource backgroundMusicAudio;
    public AudioSource campfireAudio;

    private int currentLineIndex = 0;
    private bool cutsceneEnded = false;

    private void Start()
    {
        backgroundMusicAudio.Play();
        campfireAudio.Play();
        letterText.text = "";
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        foreach(string line in cutsceneLines)
        {
            yield return StartCoroutine(TypeLines(line));
        }

        cutsceneEnded = true;
    }

    private IEnumerator TypeLines(string line)
    {
        string previousText = letterText.text;
        letterText.text = previousText;

        foreach(char letter in line)
        {
            letterText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        letterText.text += "\n";
    }
    private void Update()
    {
        if(cutsceneEnded && (Input.GetKeyDown(KeyCode.Space)))
        {
            EndCutScene();
        }
    }   

    private void EndCutScene()
    {
        StartCoroutine(FadeOutAudio(1f));
        Invoke("LoadNextScene", 1f);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator FadeOutAudio(float duration)
    {
        float startVolumeMusic = backgroundMusicAudio.volume;

        for(float t = 0; t< duration; t += Time.deltaTime)
        {
            backgroundMusicAudio.volume = Mathf.Lerp(startVolumeMusic, 0, t / duration);
            yield return null;
        }

        backgroundMusicAudio.volume = 0;

        float startVolumeCampfire = campfireAudio.volume;
        for(float i = 0; i < duration; i += Time.deltaTime)
        {
            campfireAudio.volume = Mathf.Lerp(startVolumeCampfire, 0, i / duration);
            yield return null;
        }
        campfireAudio.volume = 0;

    }

    private void OnDestroy()
    {
        campfireAudio.Stop();
        backgroundMusicAudio.Stop();
    }
}
