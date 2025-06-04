using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class TransitionScreen : MonoBehaviour
{
    [Header("UI")]
    public CanvasGroup canvasGroup;
    public float fadeSpeed = 2f;       // Speed of fade in/out
    public float holdDuration = 0.5f;  // Black screen length

    private void Awake()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 0f; 
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


        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

       
    }

    // Fade in and then fade out
    private IEnumerator FadeOnlyIn()
    {
        // Fade to black
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        // black screen
        yield return new WaitForSeconds(holdDuration);

   
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }

    // fadeout manually
    public IEnumerator FadeOut()
    {
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }
}
