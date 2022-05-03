using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Textures : MonoBehaviour
{
    public static Textures Instance;
    public Sprite[] texturePalets;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
