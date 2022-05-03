using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rack : MonoBehaviour
{
    public List<GameObject> rackPos;

    public void SetRacks()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).CompareTag("Rack"))
            {
                rackPos.Add(transform.GetChild(i).gameObject);
            }
        }
    }
}
