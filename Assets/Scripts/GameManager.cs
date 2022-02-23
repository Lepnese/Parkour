using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static Action GameStart; 
    public static float startTime = 5f;

    void Update() {
        if (Time.timeSinceLevelLoad < startTime) return;
        GameStart();
    }
}
