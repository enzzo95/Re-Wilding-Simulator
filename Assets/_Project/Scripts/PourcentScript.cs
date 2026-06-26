using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PourcentScript : MonoBehaviour
{
    [Header("Objects")]
    public GridGenerator generator;
    public TextMeshProUGUI percentageText;

    [Header("Config")]
    public float updateInterval = 0.2f;
    private float nextUpdateTime = 0f;

    [Header("UI")]
    public Image loadingBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextUpdateTime)
        {
            nextUpdateTime = Time.time + updateInterval;

            float currentPercentage = generator.GetWildingPercentage();

            percentageText.text = $"{currentPercentage.ToString("F1")}%";
            loadingBar.fillAmount = currentPercentage / 100f;
        }
    }
}
