using System;
using UnityEngine;
using TMPro;

public class PinManager : MonoBehaviour
{

    [SerializeField] private TMP_Text scoreText;
    
    private Pin[] pins;
    private int knockedPins;

    private void Awake() {
        pins = GetComponentsInChildren<Pin>();
        print(pins.Length);
    }

    void Start() {
        UpdateText();
    }

    private void UpdateText() {
        // if (knockedPins == 10)
        //     scoreText.text = "Bien joué!";
        // else
        //     scoreText.text = $"Votre score : {knockedPins}";
    }

    public void ResetPins() {
        foreach (var pin in pins) {
            pin.Reset();
        }

        knockedPins = 0;
        UpdateText();
    }
 
    public void OnKnocked()
    {
        knockedPins++;
        UpdateText();
    }
}
