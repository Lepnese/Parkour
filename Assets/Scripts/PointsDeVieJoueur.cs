using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsDeVieJoueur : MonoBehaviour
{
    const float VieInitialeJoueur = 100f;
    const float DommageParEnnemi = 40f;
    const float DommageTomberDansLeVide = 30f;
    float vieJoueur;
    // Start is called before the first frame update
    void Awake()
    {
        vieJoueur = VieInitialeJoueur;
    }

}
