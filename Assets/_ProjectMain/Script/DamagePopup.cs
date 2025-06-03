using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    private float disappearTimer;
    private Color textColor;
    private Vector3 moveVector;

    private const float DISAPPEAR_TIMER_MAX = 0.8f;
    private const float FADE_DURATION = 0.5f;
    private const float MOVE_SPEED = 1.0f;
    private const float MAX_RANDOM_OFFSET = 0.5f;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        if (textMesh == null)
        {

            textMesh = GetComponentInChildren<TextMeshProUGUI>();
        }

        if (textMesh == null)
        {
            Debug.LogError("DamagePopup: TextMeshProUGUI component not found on this GameObject or its children!");
            enabled = false;
        }
    }

    public void Setup(float damageAmount, bool isCritical = false)
    {
        if (textMesh == null) return;

        textMesh.SetText(damageAmount.ToString("F0"));

        if (isCritical)
        {
            textMesh.fontSize *= 1.5f;
            textColor = Color.yellow;
        }
        else
        {
            textColor = Color.red;
        }
        textMesh.color = textColor;
        disappearTimer = DISAPPEAR_TIMER_MAX + FADE_DURATION;

        moveVector = new Vector3(Random.Range(-MAX_RANDOM_OFFSET, MAX_RANDOM_OFFSET), 1, 0).normalized * MOVE_SPEED;


    }

    private void Update()
    {
        if (textMesh == null) return;

        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 8f * Time.deltaTime; 

        disappearTimer -= Time.deltaTime;

        if (disappearTimer <= FADE_DURATION)
        {
            // Start fading
            float alpha = Mathf.Clamp01(disappearTimer / FADE_DURATION);
            textColor.a = alpha;
            textMesh.color = textColor;

            if (disappearTimer <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public static DamagePopup Create(Vector3 position, float damageAmount, Transform parentCanvas, GameObject popupPrefab, bool isCritical = false)
    {
        if (popupPrefab == null)
        {
            Debug.LogError("Popup Prefab is not assigned!");
            return null;
        }
        if (parentCanvas == null)
        {
            Debug.LogError("Parent Canvas for popup is not assigned!");
            return null;
        }

        GameObject popupInstance = Instantiate(popupPrefab, position, Quaternion.identity, parentCanvas);
        DamagePopup damagePopup = popupInstance.GetComponent<DamagePopup>();
        if (damagePopup != null)
        {
            damagePopup.Setup(damageAmount, isCritical);
        }
        else
        {
            Debug.LogError("DamagePopup script not found on the instantiated prefab!");
            TextMeshProUGUI tmp = popupInstance.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) tmp.SetText(damageAmount.ToString("F0"));
            Destroy(popupInstance, DISAPPEAR_TIMER_MAX + FADE_DURATION);
        }
        return damagePopup;
    }
}