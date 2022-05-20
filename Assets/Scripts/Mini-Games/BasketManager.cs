using UnityEngine;
using TMPro;

public class BasketManager : MonoBehaviour
{

    public TMP_Text scoreText;
    private int score;

    void Start() {
        scoreText.text = "Lancer un ballon!";
    }

    public void OnScored()
    {
        score++;
        scoreText.text = "Votre score : " + score;
    }
}
