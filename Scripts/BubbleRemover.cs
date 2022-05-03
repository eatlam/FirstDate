using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleRemover : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Paintables"))
        {
            GameManager.Instance.PlayWaterTouch();
        }
    }
}
