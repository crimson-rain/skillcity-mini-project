using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class TransitionScreen : MonoBehaviour
{
    [Header("UI")]
    public CanvasGroup canvasGroup;
    public float fadeSpeed = 2f;       // Speed of fade in/out
    public float holdDuration = 0.5f;  // Time to stay fully black

    private void Awake()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 0f; // Start fully transparent
    }

    private void Start()
    {
        FadeInOnly();
    }

    // Fade in, then load a new scene
    public void LoadSceneWithTransition(string sceneName)
    {
        StartCoroutine(FadeAndLoadScene(sceneName));
    }

    // Only fade in and out — no scene loading (e.g. at start)
    public void FadeInOnly()
    {
        StartCoroutine(FadeOnlyIn());
    }

    // Fade in + load scene
    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        // Fade to black
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        // Hold black screen briefly
        yield return new WaitForSeconds(holdDuration);

        // Load scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // (Optional) Fade back out after load — only works if this object is DontDestroyOnLoad
        // StartCoroutine(FadeOut());
    }

    // Fade in and then fade out (no scene change)
    private IEnumerator FadeOnlyIn()
    {
        // Fade to black
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        // Hold black screen briefly
        yield return new WaitForSeconds(holdDuration);

        // Fade back to transparent
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }

    // (Optional) You can call this if you ever want to fade out manually
    public IEnumerator FadeOut()
    {
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }
}
