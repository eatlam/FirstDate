using System;
using System.Collections;
using System.Collections.Generic;
using PaintIn3D;
using UnityEngine;

public class Paintable : MonoBehaviour
{
    [Header("Phase 1")]
    public GameObject[] parts;
    public int[] partLocations = { 0, 0 };
    public bool isStarted = false;
    public float[] currentRatios;
    public float finishRatio = 0f;
    public int FinalMat = 0;
    public float currentRatio = 0f;
    public P3dChangeCounter[] p3dChangeCounters;

    private void Start()
    {
        p3dChangeCounters = new P3dChangeCounter[parts.Length];
        for (int i = 0; i < parts.Length; i++)
        {
            p3dChangeCounters[i] = parts[i].GetComponent<P3dChangeCounter>();
        }

        currentRatios = new float[parts.Length];
        finishRatio = (GameManager.Instance.finishRatio / 100f) * parts.Length;
    }

    public void StartPaint()
    {
        isStarted = true;
    }

    private void Update()
    {
        if (isStarted)
        {
            float totalRat = 0f;
            for (int i = 0; i < parts.Length; i++)
            {
                currentRatios[i] = p3dChangeCounters[i].Ratio;
                totalRat += currentRatios[i];
            }
            currentRatio = totalRat;
            var totalFinish = (100f * parts.Length) - (finishRatio * 100f);
            Phase2.Instance.Phase2CompleteRatio(currentRatio * 100f, totalFinish, currentRatios.Length);
            if (totalRat <= finishRatio)
            {
                isStarted = false;
                Phase2.Instance.Phase2CompleteRatio(101f, finishRatio, currentRatios.Length);
                Phase2.Instance.Phase2Completed();
            }
        }
    }

    public void SetFinalMats()
    {
        for (int i = 0; i < parts.Length; i++)
        {
            var curMat = parts[i].GetComponent<MeshRenderer>().materials;
            for (int j = 0; j < curMat.Length; j++)
            {
                var matToChange = curMat[j];
                matToChange.SetTexture("_BumpMap",null);
                matToChange.SetTexture("_MetallicGlossMap",null);
                matToChange.SetTexture("_EmissionMap",null);
                
            }
            GameManager.Instance.ChangeColor(parts[i],Color.white);

        }
    }
}
