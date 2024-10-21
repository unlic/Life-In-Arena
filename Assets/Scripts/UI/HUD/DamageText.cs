using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageText;

    public float moveSpeed = 1.0f; 
    public float fadeDuration = 1.0f;

    
    private Color initialColor;
    private float elapsedTime = 0.0f;

    private void Start()
    {

    }

    private void Update()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        elapsedTime += Time.deltaTime;
        float alpha = Mathf.Lerp(initialColor.a, 0, elapsedTime / fadeDuration);
        damageText.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);

        if (elapsedTime >= fadeDuration)
        {
            Destroy(gameObject);
        }
    }

    public void SetDamageText(float damage)
    {
        initialColor = damageText.color;
        damageText.text = damage.ToString();
    }
}
