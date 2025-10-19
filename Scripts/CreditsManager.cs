using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class CreditsManager : MonoBehaviour
{
    [SerializeField] private RectTransform creditsText;
    [SerializeField] private Image fadePanel;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject princess;
    [SerializeField] private GameObject tomb;
    [SerializeField] private float scrollSpeed = 24f;
    [SerializeField] private float fadeSpeed = 3f;
    [SerializeField] private AudioSource music;

    private string returntoMainMenu = "mainmenu";
    [SerializeField] private float delayBeforeMainmenu = 3f;

    private float startY;
    private float endY;

    private void Start()
    {
        startY = creditsText.anchoredPosition.y;
        endY = startY + 4760f;
        StartCoroutine(PlayCredits());
    }

    private IEnumerator PlayCredits()
    {
        yield return StartCoroutine(Fade(1, 0));

        if (music) music.Play();

        while(creditsText.anchoredPosition.y < endY)
        {
            creditsText.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(delayBeforeMainmenu);

        if(music)
        {
            while(music.volume > 0.01f)
            {
                music.volume -= Time.deltaTime / 2f;
                yield return null;
            }
        }

        yield return StartCoroutine(Fade(0, 1));

        SceneManager.LoadScene(returntoMainMenu, LoadSceneMode.Single);
        Resources.UnloadUnusedAssets();
    }

    private IEnumerator Fade(float from, float to)
    {
        Color c = fadePanel.color;
        float t = 0f;
        while(t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            c.a = Mathf.Lerp(from, to, t);
            fadePanel.color = c;
            yield return null;
        }    
    }
}
