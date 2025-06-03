using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class FadingHandler : MonoBehaviour
{
    public float fadeDuration = 1f;
   
    public Image image;
    public SpriteRenderer spriteRenderer;
    private Component target;
    private void Start()
    {
        if (spriteRenderer != null) target = spriteRenderer;
        if(image != null)target = image;

    }




    public void StartFadeOut()
    {

        if (spriteRenderer != null) target = spriteRenderer;
        if (image != null) target = image;

        if (target != null)
        {
            StartCoroutine(FadeAndDisable(target));
        }
        else
        {
            Debug.LogWarning("No SpriteRenderer or Image found to fade.");
        }
    }
    public void StartFadeIn()
    {
        if (target != null)
        {
            StartCoroutine(FadeAndDisable(target));
        }
        else
        {
            Debug.LogWarning("No SpriteRenderer or Image found to fade.");
        }
    }

    private IEnumerator FadeAndDisable(Component target)
    {
        yield return FadeObject.Fade(fadeDuration,1, 0, target);
        gameObject.SetActive(false); // Disable this object when fade is done
    }
    private IEnumerator FadeIn(Component target)
    {
        yield return FadeObject.Fade(fadeDuration, 0,1, target);
     
    }
}

