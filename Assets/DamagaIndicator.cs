using UnityEngine;
using System.Collections;
public class DamagaIndicator : MonoBehaviour
{
    [SerializeField] private float lifeTimeBeforeFade;
    private float timer;
    public bool willDecay;
    private void Update()
    {
        if (!willDecay) return;
        timer+= Time.deltaTime;
        if(timer >= lifeTimeBeforeFade)
        {
            timer = 0;
            FadingHandler handler = GetComponent<FadingHandler>();
            StartCoroutine(handler.FadeAndDestroy());

        }
    }
    


}
