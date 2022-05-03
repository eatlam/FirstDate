using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scale : MonoBehaviour
{
    private Vector3 targetMax;
    private Vector3 targetMin;
    private float speed;
    private bool canStart = false;
    private bool isScale;
    public void StartScale(float _minScale, float _maxScale, float _speed)
    {
        targetMax = new Vector3(_maxScale, _maxScale, _maxScale);
        targetMin = new Vector3(_minScale, _minScale, _minScale);
        speed = _speed;
        isScale = true;
        canStart = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(canStart)
            DoScale();
    }

    private void DoScale()
    {
        if (isScale)
        {
            if(transform.localScale.x < targetMax.x)
                transform.localScale = Vector3.MoveTowards(transform.localScale, targetMax, speed*Time.deltaTime);
            else
            {
                isScale = false;
            }
        }
        else if (!isScale)
        {
            if(transform.localScale.x > targetMin.x)
                transform.localScale = Vector3.MoveTowards(transform.localScale, targetMin, speed*Time.deltaTime);
            else
            {
                isScale = true;
            }
        }
    }
}
