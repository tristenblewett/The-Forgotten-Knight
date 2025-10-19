using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraFader : MonoBehaviour
{
    [SerializeField] private SpriteRenderer fadeOverlay;
    [SerializeField] private float fadeDuration = 2f;

    private void Awake()
    {
        if(fadeOverlay != null)
        {
            fadeOverlay.color = new Color(0f, 0f, 0f, 0f);
        }
    }

    public IEnumerator FadeOut()
    {
        if(fadeOverlay == null)
        {
            yield break;
        }

        float timer = 0f;
        while(timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            fadeOverlay.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
        fadeOverlay.color = new Color(0f, 0f, 0f, 1f);
    }

    public IEnumerator FadeIn()
    {
        if (fadeOverlay == null)
        {
            yield break;
        }

        float timer = 0f;
        while(timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            fadeOverlay.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
        fadeOverlay.color = new Color(0f, 0f, 0f, 0f);
    }
}
