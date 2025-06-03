using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TransitionScreen : MonoBehaviour
{ //fade in
    public CanvasGroup canvasGroup;
    public float fadeSpeed = 2f;
    public float holdDuration = 0.5f;

    private void Awake()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 0;
    }

    public void LoadSceneWithTransition(string sceneName)
    {
        StartCoroutine(FadeAndLoadScene(sceneName));
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {

        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }


        yield return new WaitForSeconds(holdDuration);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }


    }
}
