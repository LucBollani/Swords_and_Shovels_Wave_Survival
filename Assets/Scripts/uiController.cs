using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class uiController : MonoBehaviour
{
    public TextMeshProUGUI wave;
    public TextMeshProUGUI completed;
    public TextMeshProUGUI unlockable;
    public TextMeshProUGUI dead;
    public TextMeshProUGUI youRock;
    public TextMeshProUGUI thanks;

    public GameObject start;
    public GameObject retry;
    public GameObject reset;

    private float waveTimer;
    private float completedTimer;

    private void Start()
    {
        clearDisplay();
        waveTimer = Time.time;
        completedTimer = Time.time;
    }

    private void Update()
    {
        if (Time.time > waveTimer)
        {
            wave.enabled = false;
        }
        if (Time.time > completedTimer)
        {
            completed.enabled = false;
            unlockable.enabled = false;
        }
    }

    public void displayWave(string text, float duration)
    {
        wave.text = text;
        waveTimer = Time.time + duration;
        wave.enabled = true;
    }

    public void displayCompleted(string text, float duration)
    {
        unlockable.text = text;
        completedTimer = Time.time + duration;
        unlockable.enabled = true;
        completed.enabled = true;
    }

    public void displayStart()
    {
        start.SetActive(true);
    }

    public void displayDeath()
    {
        retry.SetActive(true);
        reset.SetActive(true);
        dead.enabled = true;
    }

    public void displayWin()
    {
        youRock.enabled = true;
        thanks.enabled = true;
    }

    public void clearDisplay()
    {
        start.SetActive(false);
        retry.SetActive(false);
        reset.SetActive(false);
        dead.enabled = false;
        youRock.enabled = false;
        thanks.enabled = false;
    }
}
