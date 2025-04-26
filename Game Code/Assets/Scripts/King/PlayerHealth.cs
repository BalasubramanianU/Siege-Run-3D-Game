using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    private Camera Camera;

    public Slider healthSlider;
    public TMP_Text gameOverText; 
    private AudioSource[] audioSources;
    private AudioSource defeatSound; 
    
    private Canvas canvas;
    
    public float healthAnimationSpeed = 2.0f;

    private Animator animator;

    public float regenDelay = 2f;          
    public float regenRate = 5f;
    private float lastDamageTime;   
    private float regenAccumulator = 0f;
    
    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        audioSources = GetComponents<AudioSource>();
        defeatSound = audioSources[1]; 
        
        if (healthSlider == null)
        {
            healthSlider = GameObject.Find("HealthSlider").GetComponent<Slider>();
            if (healthSlider == null)
            {
                Debug.LogError("Health Slider not found! Please assign it in the inspector.");
            }
        }

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
            PositionGameOverText();
        }
        
        canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        SetupHealthBar();
        
        Debug.Log("PlayerHealth initialized. Current health: " + currentHealth);
    }

    void PositionGameOverText()
    {
        RectTransform rectTransform = gameOverText.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f); 
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);     
        rectTransform.anchoredPosition = new Vector2(40f, 90f);     
    }
    
    void SetupHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
            
            Debug.Log("Health bar initialized. Min: " + healthSlider.minValue + 
                      ", Max: " + healthSlider.maxValue + 
                      ", Current: " + healthSlider.value);
            
            Image fillImage = healthSlider.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                fillImage.color = Color.blue;
            }
            
            Transform background = healthSlider.transform.Find("Background");
            if (background != null)
            {
                Image bgImage = background.GetComponent<Image>();
                if (bgImage != null)
                {
                    bgImage.color = Color.grey;
                }
            }
        }
        else
        {
            Debug.LogError("Health Slider not assigned!");
        }
    }
    
    void Update()
    {
        if (healthSlider != null)
        {
            Vector3 headPosition = transform.position + Vector3.up * 2.0f; // Adjust the 2.0f value to match your character's height
            
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

        if (Time.time >= lastDamageTime + regenDelay && currentHealth < maxHealth)
        {
            RegenerateHealth();
        }

    }
    
    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;
        
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);
        lastDamageTime = Time.time; 
        
        Debug.Log(gameObject.name + " took " + amount + " damage! Current Health: " + currentHealth);
        
        if (healthSlider != null)
        {
            StartCoroutine(AnimateHealthReduction(healthSlider.value, currentHealth));
        }
        
        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }

void RegenerateHealth()
{
    regenAccumulator += regenRate * Time.deltaTime;  

    if (regenAccumulator >= 1f)  
    {
        int healthToAdd = Mathf.FloorToInt(regenAccumulator); 
        regenAccumulator -= healthToAdd; 

        currentHealth += healthToAdd;
        currentHealth = Mathf.Min(currentHealth, maxHealth);  

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
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
    
    IEnumerator Die()
    {
        Debug.Log(gameObject.name + " died!");
        
        if (healthSlider != null && healthSlider.value > 0)
        {
            yield return StartCoroutine(AnimateHealthReduction(healthSlider.value, 0));
        }

        animator.SetBool("IsWalking", false);
        animator.SetBool("IsAttacking", false);
        animator.SetBool("IsDying", true);
        
        yield return new WaitForSeconds(0.5f);

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        while (!stateInfo.IsName("Death") || stateInfo.normalizedTime < 1.0f)
        {
            yield return null;
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        }

        GameObject[] healthBars = GameObject.FindGameObjectsWithTag("HealthBar");
        foreach (GameObject bar in healthBars)
        {
            bar.SetActive(false);
        }

        if (gameOverText != null)
        {
            defeatSound.Play();
            gameOverText.gameObject.SetActive(true);
        }

        Camera.main.transform.SetParent(null);

        Time.timeScale = 0f;
    }
}