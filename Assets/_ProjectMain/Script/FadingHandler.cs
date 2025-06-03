using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
public class FadingHandler : MonoBehaviour
{
    public float fadeDuration = 1f;
   
    public Image image;
    public SpriteRenderer spriteRenderer;
    public TMP_Text text;
    private Component target;
    private void Start()
    {
        if (spriteRenderer != null) target = spriteRenderer;
        if(image != null)target = image;
        if (text != null) target = text;

    }




    public void StartFadeOut()
    {

        if (spriteRenderer != null) target = spriteRenderer;
        if (image != null) target = image;

        if (target != null)
        {
            StartCoroutine(FadeAndDisable());
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
            StartCoroutine(FadeAndDisable());
        }
        else
        {
            Debug.LogWarning("No SpriteRenderer or Image found to fade.");
        }
    }

    public IEnumerator FadeAndDisable()
    {
        yield return FadeObject.Fade(fadeDuration,1, 0, target);
        gameObject.SetActive(false); // Disable this object when fade is done
    }
    public IEnumerator FadeAndDestroy()
    {
        yield return FadeObject.Fade(fadeDuration, 1, 0, target);
        Destroy(gameObject); // Disable this object when fade is done
    }
    private IEnumerator FadeIn()
    {
        yield return FadeObject.Fade(fadeDuration, 0,1, target);
     
    }
}

