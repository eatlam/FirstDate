using System;
using System.Collections;
using System.Globalization;
using LPWAsset;
using PaintIn3D;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Phase3 : MonoBehaviour
{
    public static Phase3 Instance;
    public GameObject[] FinalRemoves;
    public GameObject[] instantRemoves;
    public GameObject[] instantActives;
    [Header("Paintable")]
    public GameObject paintable;
    public Transform paintableFirstMovePos;
    [Header("Pillow")]
    public GameObject bath;
    public Transform bathSpawnPos;
    public Transform bathLastMovePos;
    [Header("Rack")] 
    public GameObject rack;
    public Transform rackSpawnPos;
    public Transform rackFirstMovePos;
    public Transform rackLastMovePos;
    [Header("Swipe Up UI")] 
    public GameObject SwipeUp;
    [Header("Bath")]
    public Transform killBathPos;
    private GameObject[] water;
    [Header("Particle")] 
    public Transform particleStarSpawnPos;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    #region Start
    
    public void InstantActive()
    {
        for (int i = 0; i < instantActives.Length; i++)
        {
            instantActives[i].SetActive(true);
        }
        SwipeUp.SetActive(true);
    }
    
    public void InstantRemoves()
    {
        for (int i = 0; i < instantActives.Length; i++)
        {
            instantRemoves[i].SetActive(false);
        }
        
    }

    public void StartPhase(GameObject _paintable)
    {
        paintable = _paintable;
        StartCoroutine(StartPhase3());
    }

    IEnumerator StartPhase3()
    {
        var distanceOne = Vector3.Distance(paintable.transform.position, paintableFirstMovePos.transform.position);
        var distanceOneR = Quaternion.Angle(paintable.transform.rotation, paintableFirstMovePos.transform.rotation);

        bath = Instantiate(Resources.Load<GameObject>("Others/Bath"));
        bath.transform.position = bathSpawnPos.position;
        bath.transform.rotation = bathSpawnPos.rotation;
        bath.transform.localScale = bathSpawnPos.localScale;
        
        var distanceOneBath = Vector3.Distance(bath.transform.localScale, bathLastMovePos.transform.localScale);

        var paintableFirstMovePosTime3 = GameManager.Instance.paintableFirstMovePosTime3;
        var bathLastMovePosTime3 = GameManager.Instance.bathLastMovePosTime3;

        water = new GameObject[2];
        for (int j = 0; j < bath.transform.childCount; j++)
        {
            if (bath.transform.GetChild(j).CompareTag("Water"))
            {
                water[0] = bath.transform.GetChild(j).gameObject;
            }
            else if (bath.transform.GetChild(j).CompareTag("Water2"))
            {
                water[1] = bath.transform.GetChild(j).gameObject;
            }
        }
        
        water[0].SetActive(true);
        water[1].SetActive(false);

        while (distanceOne > 0.1f)
        {
            distanceOne = Vector3.Distance(paintable.transform.position, paintableFirstMovePos.position);
            distanceOneR = Quaternion.Angle(paintable.transform.rotation, paintableFirstMovePos.transform.rotation);
            paintable.transform.position = Vector3.MoveTowards(paintable.transform.position,
                paintableFirstMovePos.position, (distanceOne/paintableFirstMovePosTime3)*Time.deltaTime);
            
            paintable.transform.rotation = Quaternion.Lerp(paintable.transform.rotation,
                paintableFirstMovePos.transform.rotation,(distanceOneR/paintableFirstMovePosTime3)*Time.deltaTime);
            
            distanceOneBath = Vector3.Distance(bath.transform.localScale, bathLastMovePos.transform.localScale);
            bath.transform.localScale = Vector3.MoveTowards(bath.transform.localScale,
                bathLastMovePos.localScale, (distanceOneBath/bathLastMovePosTime3)*Time.deltaTime);
            
            yield return null;
        }
        
        InstantActive();
    }

    #endregion

    #region Others

    public void BeginCleaning()
    {
        StartCoroutine(BeganCleaning());
    }

    IEnumerator BeganCleaning()
    {
        SwipeUp.SetActive(false);
        rack = Instantiate(Resources.Load<GameObject>("Others/Rack"));
        rack.transform.position = rackSpawnPos.position;
        rack.transform.rotation = rackSpawnPos.rotation;
        rack.GetComponent<Rack>().SetRacks();
        var targetRack = rack.GetComponent<Rack>().rackPos[PlayerDatabase.Instance.GetOutfit() - 1];
        paintable.transform.SetParent(targetRack.transform);
        paintable.transform.position = targetRack.transform.position;
        paintable.transform.rotation = targetRack.transform.rotation;
        
        var distanceOne = Vector3.Distance(rack.transform.position, rackFirstMovePos.transform.position);
        var distanceOneR = Quaternion.Angle(rack.transform.rotation, rackFirstMovePos.transform.rotation);

        var rackFirstMovePosTimer = GameManager.Instance.rackFirstMovePosTime;
        
        bool canWaterMove = true;
        var waterLimit = distanceOne / 3f;

        yield return new WaitForSeconds(0.1f);

        while (distanceOne > 0.1f)
        {
            distanceOne = Vector3.Distance(rack.transform.position, rackFirstMovePos.position);
            distanceOneR = Quaternion.Angle(rack.transform.rotation, rackFirstMovePos.transform.rotation);
            rack.transform.position = Vector3.MoveTowards(rack.transform.position,
                rackFirstMovePos.position, (distanceOne/rackFirstMovePosTimer)*Time.deltaTime);
            
            rack.transform.rotation = Quaternion.Lerp(rack.transform.rotation,
                rackFirstMovePos.transform.rotation,(distanceOneR/rackFirstMovePosTimer)*Time.deltaTime);
            
            
            if (canWaterMove)
            {
                if (distanceOne < waterLimit)
                {
                    canWaterMove = false;
                    GameManager.Instance.PlayWaterCleanSound(true);
                    water[0].SetActive(false);
                    water[1].SetActive(true);
                }
            }

            yield return null;
        }

        var animMoveMax3 = GameManager.Instance.animMoveMax;
        var animCounter = GameManager.Instance.animCount;
        var animSpeed3 = GameManager.Instance.animSpeed3;

        int i = 0;
        var midTarget = rack.transform.position;
        var leftTarget = rack.transform.position - animMoveMax3;
        var rightTarget = rack.transform.position + animMoveMax3;
        
        var distanceLeft = Vector3.Distance(rack.transform.position, leftTarget);
        var distanceRight = Vector3.Distance(rack.transform.position, rightTarget);
        
        
        bool isLeft = true;
        while (i < animCounter)
        {
            if (isLeft)
            {
                distanceLeft = Vector3.Distance(rack.transform.position, leftTarget);
                if (distanceLeft > 0.01f)
                {
                    rack.transform.position = Vector3.MoveTowards(rack.transform.position,
                        leftTarget, (distanceLeft/animSpeed3)*Time.deltaTime);
                }
                else
                {
                    i++;
                    isLeft = false;
                }
            }
            else
            {
                distanceRight = Vector3.Distance(rack.transform.position, rightTarget);
                if (distanceRight > 0.01f)
                {
                    rack.transform.position = Vector3.MoveTowards(rack.transform.position,
                        rightTarget, (distanceRight/animSpeed3)*Time.deltaTime);
                }
                else
                {
                    i++;
                    isLeft = true;
                }
            }
            
            
            yield return null;
        }

        var distanceMid = Vector3.Distance(rack.transform.position, midTarget);
        var midSpeed = animSpeed3 / 2f;

        bool canStopSound = true;
        var soundLimit = distanceMid / 2f;
        
        while (distanceMid > 0.1f)
        {
            distanceMid = Vector3.Distance(rack.transform.position, midTarget);
            rack.transform.position = Vector3.MoveTowards(rack.transform.position,
                midTarget, (distanceMid/midSpeed)*Time.deltaTime);

            if (canStopSound)
            {
                if (distanceMid < soundLimit)
                {
                    canStopSound = false;
                    GameManager.Instance.PlayWaterCleanSound(false);
                    GameManager.Instance.PlayWaterTouch();
                }
            }
            yield return null;
        }
        
        Phase2.Instance.paintable.GetComponent<Paintable>().SetFinalMats();

        var distanceLast = Vector3.Distance(rack.transform.position, rackLastMovePos.transform.position);
        var distanceLastR = Quaternion.Angle(rack.transform.rotation, rackLastMovePos.transform.rotation);

        var rackLastMovePosTimer = GameManager.Instance.rackLastMovePosTime;
        
        while (distanceLast > 0.1f)
        {
            distanceLast = Vector3.Distance(rack.transform.position, rackLastMovePos.position);
            distanceLastR = Quaternion.Angle(rack.transform.rotation, rackLastMovePos.transform.rotation);
            rack.transform.position = Vector3.MoveTowards(rack.transform.position,
                rackLastMovePos.position, (distanceLast/rackLastMovePosTimer)*Time.deltaTime);
            
            rack.transform.rotation = Quaternion.Lerp(rack.transform.rotation,
                rackLastMovePos.transform.rotation,(distanceLastR/rackLastMovePosTimer)*Time.deltaTime);

            yield return null;
        }
        
        //TODO: Shinng Stars
        yield return new WaitForSeconds(0.1f);
        GameManager.Instance.PlayCompleteSound();
        InstantRemoves();
        yield return new WaitForSeconds(0.25f);
        
        paintable.transform.SetParent(null);
        Destroy(rack);
        
        var targetScaleB = Vector3.Distance(bath.transform.localScale, killBathPos.transform.localScale);

        var floorKillTimer = GameManager.Instance.animKillSpeed;
        
        var particle = Instantiate(Resources.Load<GameObject>("Prefabs/particleStar"));
        particle.transform.position = particleStarSpawnPos.position;
        particle.transform.rotation = particleStarSpawnPos.rotation;
        particle.transform.localScale = particleStarSpawnPos.localScale;

        while (targetScaleB > 0.01f)
        {
            targetScaleB = Vector3.Distance(bath.transform.localScale, killBathPos.localScale);
            bath.transform.localScale = Vector3.MoveTowards(bath.transform.localScale,
                killBathPos.localScale, (targetScaleB/floorKillTimer)*Time.deltaTime);
            
            yield return null;
        }

        Destroy(bath);
        Destroy(particle);
        yield return new WaitForSeconds(0.1f);
        Phase3Completed();
        Phase4.Instance.StartPhase(paintable);
    }

    #endregion

    #region End

    public void Phase3Completed()
    {
        GameManager.Instance.PlayClean(false);
        GameManager.Instance.PlaySpray(false);
        StartCoroutine(Phase3Ended());
    }

    IEnumerator Phase3Ended()
    {
        PlayerPrefs.SetInt("bubble_active", 1);
        yield return new WaitForSeconds(1f);
    }

    #endregion

}
