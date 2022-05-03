using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    private void Start()
    {
        var meshRenderer = transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
        var mats = meshRenderer.materials;
        for (int i = 0; i < meshRenderer.materials.Length; i++)
        {
            if (i != 1)
            {
                mats[i] = null;
            }
            else
            {
                mats[i] = meshRenderer.materials[i];
            }
        }
        meshRenderer.materials = mats;
    }
}
