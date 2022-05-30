using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BasketManager : MonoBehaviour
{

    public TMP_Text scoreText;
    int score = 0;

    void Start()
    {
        scoreText.text = "Votre score : " + scoreText.ToString();
    }

    public void OnScored()
    {
        score++;
        scoreText.text = "Votre score : " + score;
    }
}
