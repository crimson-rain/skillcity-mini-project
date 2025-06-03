using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
public static class FadeObject
{
    public static IEnumerator Fade(float duration,float startAlpha, float endAlpha, Component target)
    {
        if (target == null)
            yield break;

        Color color;

        if (target is SpriteRenderer spriteRenderer)
        {
            color = spriteRenderer.color;
        }
        else if (target is Image image)
        {
            color = image.color;
        }
        else if(target is TMP_Text text)
        {
            color = text.color;
        }
        else
        {
            Debug.LogWarning("FadeObject only supports SpriteRenderer or Image.");
            yield break;
        }

        //float startAlpha = color.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            Color newColor = new Color(color.r, color.g, color.b, alpha);

            if (target is SpriteRenderer sr)
                sr.color = newColor;
            else if (target is Image img)
                img.color = newColor;
            else if (target is TMP_Text text)
                text.color = newColor;

            yield return null;
        }

        // Ensure exact final alpha
        Color finalColor = new Color(color.r, color.g, color.b, endAlpha);
        if (target is SpriteRenderer finalSr)
            finalSr.color = finalColor;
        else if (target is Image finalImg)
            finalImg.color = finalColor;
        else if (target is TMP_Text text)
            text.color = finalColor;
    }
}
