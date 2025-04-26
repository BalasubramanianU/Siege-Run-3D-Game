using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameTimer : MonoBehaviour
{
    public float totalTime = 15f;               
    private float currentTime;
    
    public Slider healthSlider;
    public TMP_Text timerText; 
    public TMP_Text gameOverText;                   
    public GateHealth gateHealth; 
    private AudioSource[] audioSources;
    private AudioSource defeatSound;               

    public Vector2 screenOffset = new Vector2(-20f, -20f);

    void Start()
    {
        currentTime = totalTime;
        audioSources = GetComponents<AudioSource>();
        defeatSound = audioSources[0]; 

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
            PositionGameOverText();
        }

        PositionTimerText();
    }

    void PositionGameOverText()
    {
        RectTransform rectTransform = gameOverText.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = new Vector2(-110f, 140f);
    }

    void PositionTimerText()
    {
        RectTransform rectTransform = timerText.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(1, 1);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(1, 1);
        rectTransform.anchoredPosition = screenOffset;
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay();
        }
        else
        {
            if (gateHealth != null && gateHealth.IsAlive())
            {
                TriggerGameOver();
            }
        }
    }

    void UpdateTimerDisplay()
    {
        timerText.text = "Time: " + Mathf.CeilToInt(currentTime).ToString();
    }

    void TriggerGameOver()
    {
        Debug.Log("Game Over: Time ran out and gate still standing!", healthSlider);


        GameObject[] healthBars = GameObject.FindGameObjectsWithTag("HealthBar");
        foreach (GameObject bar in healthBars)
        {
            bar.SetActive(false);
        }

        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            if (gameOverText != null)
            {
                defeatSound.Play();
                gameOverText.gameObject.SetActive(true);
            }
        }

        Time.timeScale = 0f;
        this.enabled = false;
    }
}
