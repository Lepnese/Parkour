using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPin : MonoBehaviour
{
    //[SerializeField] private VoidEvent OnPressedEvent;
    Pin pin;
    Vector3 PositionAtStart;
    void Start()
    {
        pin = GetComponent<Pin>();
        PositionAtStart = pin.transform.position;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pin.transform.position = PositionAtStart;
        }
    }
}
