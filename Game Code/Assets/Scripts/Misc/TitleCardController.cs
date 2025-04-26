using UnityEngine;

public class TitleCardController : MonoBehaviour
{
    public GameObject titleCardPanel;
    private GameObject[] healthBars;

    void Start()
    {
        healthBars = GameObject.FindGameObjectsWithTag("HealthBar");
        foreach (GameObject bar in healthBars)
        {
            bar.SetActive(false);
        }
    }

    public void HideTitleCard()
    {
        titleCardPanel.SetActive(false); 
        
        foreach (GameObject bar in healthBars)
        {
            bar.SetActive(true);
        }
    }
}
