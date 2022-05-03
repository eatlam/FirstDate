using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spray : MonoBehaviour
{
    /*public Material upgradeCircle;
    private bool isUpgradePanelOpened = false;
    private bool isStay = false;
    private float stayCounter;
    private float stayChanger;

    private void Start()
    {
        ResetDuration();
    }

    private void Update()
    {
        if(isUpgradePanelOpened) return;
        if (isStay)
        {
            if (stayCounter > 0)
            {
                stayCounter -= Time.deltaTime;
                var visible = (stayCounter * 180f);
                upgradeCircle.SetFloat("_Arc2", visible);
            }
            else
            {
                upgradeCircle.SetFloat("_Arc2", 0f);
                isUpgradePanelOpened = true;
                UIManager.instance.UpgradePanelActive(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerMain"))
        {
            StartDuration();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerMain"))
        {
            StartDuration();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerMain"))
        {
            ResetDuration();
        }
    }

    private void StartDuration()
    {
        isStay = true;
    }

    private void ResetDuration()
    {
        upgradeCircle.SetFloat("_Arc2", 360f);
        isUpgradePanelOpened = false;
        isStay = false;
        stayCounter = GameManager.instance.upgradeShowerDuration;
    }*/
}
