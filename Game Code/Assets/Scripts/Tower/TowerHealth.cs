using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TowerHealth : MonoBehaviour
{
    public int maxHealth = 50;
    public int currentHealth;
    
    public Slider healthSlider;
    public float heightOffset = 2.0f;
    
    public float healthAnimationSpeed = 2.0f;
    
    void Start()
    {
        currentHealth = maxHealth;
        
        PositionHealthBar();
        
        SetupHealthBar();
        
        Debug.Log("TowerHealth initialized. Current health: " + currentHealth);
    }
    
    void SetupHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
            
            Image fillImage = healthSlider.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                fillImage.color = Color.red;
            }
        }
        else
        {
            Debug.LogWarning("Health Slider not assigned for " + gameObject.name);
        }
    }
    
    void PositionHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.transform.position = transform.position + Vector3.up * heightOffset;
            
            healthSlider.transform.rotation = Camera.main.transform.rotation;
        }
    }

        void Update()
        {
        if (healthSlider != null)
        {
            Vector3 headPosition = transform.position + Vector3.up * 2.0f; 
            
            Vector3 screenPos = Camera.main.WorldToScreenPoint(headPosition);
            
            RectTransform healthBarRect = healthSlider.GetComponent<RectTransform>();
            if (healthBarRect != null)
            {
                Canvas canvas = healthBarRect.GetComponentInParent<Canvas>();
                if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    healthBarRect.position = screenPos;
                }
                else if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        canvas.GetComponent<RectTransform>(),
                        screenPos,
                        canvas.worldCamera,
                        out Vector2 localPoint);
                    
                    healthBarRect.localPosition = localPoint;
                }
                
                healthBarRect.gameObject.SetActive(screenPos.z > 0);
            }
        }
    }
    
    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;
        
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);
        
        Debug.Log(gameObject.name + " took " + amount + " damage! Current Health: " + currentHealth);
        
        if (healthSlider != null)
        {
            StartCoroutine(AnimateHealthReduction(healthSlider.value, currentHealth));
        }
        
        if (currentHealth <= 0)
        {
            DestroyTower();
        }
    }
    
    IEnumerator AnimateHealthReduction(float startValue, float targetValue)
    {
        float elapsedTime = 0;
        float duration = Mathf.Abs(startValue - targetValue) / (healthAnimationSpeed * maxHealth);
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / duration);
            
            healthSlider.value = Mathf.Lerp(startValue, targetValue, normalizedTime);
            yield return null;
        }
        
        healthSlider.value = targetValue;
    }
    
    void DestroyTower()
    {
        Debug.Log(gameObject.name + " destroyed!");
        
        if (healthSlider != null)
        {
            healthSlider.gameObject.SetActive(false);
        }
        
        Destroy(gameObject, 0.1f);
    }
}