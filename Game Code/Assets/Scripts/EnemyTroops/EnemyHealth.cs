using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 20;
    private int currentHealth;
    public Slider healthSlider;
    public float heightOffset = 2.0f;


    void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
    {
        healthSlider.minValue = 0;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
    }
    }

    void Update()
{
    if (healthSlider != null)
    {
        Vector3 worldPosition = transform.position + Vector3.up * heightOffset;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        healthSlider.transform.position = screenPosition;

        healthSlider.gameObject.SetActive(screenPosition.z > 0);
    }
}


    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

    if (healthSlider != null)
    {
        healthSlider.value = currentHealth;
    }
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }


}
