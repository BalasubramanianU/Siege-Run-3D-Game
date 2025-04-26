using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class GateHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public Slider healthSlider;
    public float heightFraction = -2.0f;
    public float healthAnimationSpeed = 2.0f;
    public TMP_Text victoryText;
    private AudioSource[] audioSources;
    private AudioSource victorySound;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }

        Debug.Log("GateHealth initialized. Current health: " + victoryText);
        if (victoryText != null)
        {
            victoryText.gameObject.SetActive(false);
            PositionVictoryText();
        }

        audioSources = GetComponents<AudioSource>();
        victorySound = audioSources[0]; 
    }

    void PositionVictoryText()
    {
        RectTransform rectTransform = victoryText.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f); 
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f); 
        rectTransform.anchoredPosition = new Vector2(-400f, 140f);
    }

    void Update()
    {
        if (healthSlider != null)
        {
            Vector3 worldPos = transform.position + Vector3.up * (transform.localScale.y * heightFraction);
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            healthSlider.transform.position = screenPos;
            healthSlider.gameObject.SetActive(screenPos.z > 0);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        if (healthSlider != null)
        {
            StartCoroutine(AnimateHealthReduction(healthSlider.value, currentHealth));
        }

        if (currentHealth <= 0)
        {
            StartCoroutine(TriggerVictory());

        }
    }

    IEnumerator AnimateHealthReduction(float startValue, float targetValue)
    {
        float elapsed = 0;
        float duration = Mathf.Abs(startValue - targetValue) / (healthAnimationSpeed * maxHealth);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            healthSlider.value = Mathf.Lerp(startValue, targetValue, elapsed / duration);
            yield return null;
        }
        healthSlider.value = targetValue;
    }

    IEnumerator TriggerVictory()
    {
        Debug.Log("Victory achieved!");

        yield return new WaitForSeconds(0.5f); 

        GameObject[] healthBars = GameObject.FindGameObjectsWithTag("HealthBar");
        foreach (GameObject bar in healthBars)
        {
            bar.SetActive(false); 
        }

        if (victoryText != null)
        {
            victorySound.Play();
            victoryText.gameObject.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }
}
