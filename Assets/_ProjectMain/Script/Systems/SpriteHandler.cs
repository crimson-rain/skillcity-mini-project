using UnityEngine;
using UnityEngine.UI;

public class SpriteHandler : MonoBehaviour
{
    [SerializeField] private Image image;

    private void Awake()
    {
        if (image == null)
        {
            Debug.LogError("SpriteHandler: Image not assigned in Inspector! " + gameObject.name);
        }
    }

    public void SetSprite(Sprite sprite)
    {
        if (image == null) return;

        image.sprite = sprite;
        Color color = image.color;
        color.a = image.sprite == null ? 0f : 1f;
        image.color = color;
    }
}

