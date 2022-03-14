using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private VoidEvent activate;
    
    private IEnumerator Start() {
        while (Time.timeSinceLevelLoad < 2.5f)
            yield return null;
        activate.Raise();
    }
}
