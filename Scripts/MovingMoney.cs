using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingMoney : MonoBehaviour
{
    private Vector3 target_pos;
    private bool isAnimation = false;
    private Camera cam;
    private bool canGo = false;
    private float speed = 15f;
    private bool isLast = false;
    private int increaseCount;
    private RectTransform currentRect;
    float currentTime=0; // actual floting time 
    private float normalizedValue = 0.5f;
    
    public void Send(int _increaseCount,float _speed, bool _isLast)
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        currentRect = GetComponent<RectTransform>();
        target_pos = UIManager.Instance.moneyImage.GetComponent<RectTransform>().anchoredPosition;
        increaseCount = _increaseCount;
        isLast = _isLast;
        speed = _speed;
        isAnimation = true;
        canGo = true;
        StartCoroutine(LerpObject());
    }
    IEnumerator LerpObject(){ 
     
        while (currentTime <= speed) 
        { 
            currentTime += Time.deltaTime; 
            normalizedValue=currentTime/speed; // we normalize our time 
 
            currentRect.anchoredPosition=Vector3.Lerp(currentRect.anchoredPosition,target_pos, normalizedValue); 
            yield return null; 
        }
        Destroy(gameObject);
        
    }
}
