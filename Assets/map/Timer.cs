using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI timerText;

    [Header("Variables")]
    public float totalTime;
    private float currentTime;


    void Start()
    {
        currentTime = totalTime;
    }
    void Update()
    {
        // Mettre à jour le texte du chronomètre
        currentTime -= Time.deltaTime;
        if (currentTime < 0f)
            currentTime = 0f;
        
        UpdateTimeUI();
    }

    void UpdateTimeUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int secondes = Mathf.FloorToInt(currentTime % 60);
        string timeString = string.Format("{00:00}:{01:00}", minutes, secondes);
        timerText.text = timeString;

        if (currentTime > 15f)
            timerText.color = Color.green;
        if (currentTime < 15f && currentTime > 5f)
            timerText.color = new Color(1.0f, 0.5f, 0.0f);
        else if (currentTime < 5f)
            timerText.color = Color.red;
    }

}
