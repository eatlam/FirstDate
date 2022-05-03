using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moves : MonoBehaviour
{
    private float mZCoord;
    private Vector3 mOffset;
    public bool canMove = false;
    
    
    private Touch touch;
    private float speedModifier;
    void Start()
    {
        speedModifier = 0.001f;
    }
    public void SetMove(bool isTrue, bool isFast)
    {
        canMove = isTrue;
        if(!isFast)
            speedModifier = 0.001f;
        else
        {
            speedModifier = 0.001f;
        }
    }
    void Update()
    {
        if(!canMove) return;

#if UNITY_EDITOR
        EditorTouch();
#endif
        
        MobileTouch();
    }
 
    Vector3 GetMouseAsWorldPoint()
    {
        // Pixel coordinates of mouse (x,y)
        Vector3 mousePoint = Input.mousePosition;
 
        // z coordinate of game object on screen
        mousePoint.z = mZCoord;
 
        // Convert it to world points
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    void EditorTouch()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
            // Store offset = gameobject world pos - mouse world pos
            mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
        }

        if (Input.GetMouseButton(0))
        {
            transform.position = new Vector3(GetMouseAsWorldPoint().x + mOffset.x, GetMouseAsWorldPoint().y + mOffset.y,
                transform.position.z);
            //transform.position = GetMouseAsWorldPoint() + mOffset;
        }
    }

    void MobileTouch()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                transform.position = new Vector3(transform.position.x + touch.deltaPosition.x * speedModifier,
                    transform.position.y + touch.deltaPosition.y * speedModifier,
                    transform.position.z);
            }
        }
    }
}
