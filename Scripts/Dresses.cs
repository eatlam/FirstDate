using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dresses : MonoBehaviour
{
    public static Dresses Instance;
    public Sprite[] dressPalets;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
