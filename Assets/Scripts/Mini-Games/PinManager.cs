using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PinManager : MonoBehaviour
{

    public TMP_Text scoreText;
    int knockedPins = 0;
    Pin pin;

    // Use this for initialization
    void Start()
    {

        scoreText.text = "Votre score: " + scoreText.ToString();
        pin = GetComponent<Pin>();
    }
    //private void Update()
    //{
    //    time += Time.deltaTime;
    //}

    public void OnKnocked()
    {
        knockedPins++;

            if (knockedPins == 10)
            {
                Debug.Log("Bien joué");

                scoreText.text = "Bien joué";
            }
            //if (pin.time <= 2f)
            //scoreText.text = "Strike!!!";
            //faudrait checker les 10 pins pis prendre la valeur max pis la valeur min
            else
                scoreText.text = "Votre score" + knockedPins;
    }
}
