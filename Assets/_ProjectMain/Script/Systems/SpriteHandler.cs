using UnityEngine;
using UnityEngine.UI;

public class SpriteHandler : MonoBehaviour
{
    Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        //Color color = image.color;
        //color.a = image.sprite == null ? 0f : 1f;
        //image.color = color;
    }
    public void SetSprite(Sprite sprite)
    {
        image.sprite = sprite;  
        Color color = image.color;
        color.a = image.sprite == null ? 0f : 1f;
        image.color = color;

    }
}
