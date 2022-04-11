using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PinManager : MonoBehaviour
{

    public TMP_Text scoreText;
    int knockedPins = 0;


    // Use this for initialization
    void Start()
    {

        scoreText.text = "You Scored: " + scoreText.ToString();

    }

    public void OnKnocked()
    {
        knockedPins++;

            if (knockedPins == 10)
            {
                Debug.Log("Strike");

                scoreText.text = "Strike";
            }
            else
                scoreText.text = "You scored" + knockedPins;
    }
}
