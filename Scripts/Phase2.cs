using System;
using System.Collections;
using PaintIn3D;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Phase2 : MonoBehaviour
{
    public static Phase2 Instance;
    public GameObject[] FinalRemoves;
    public GameObject[] instantRemoves;
    public GameObject[] instantActives;
    public Image CompleteRatioImage;
    public float total = 0;
    [Header("Paintable")]
    public GameObject paintable;
    public Transform paintableSpawnPos;
    public Transform paintableFirstMovePos;
    public Transform paintableLastMovePos;
    [Header("Pillow")]
    public GameObject pillow;
    public Transform pillowSpawnPos;
    public Transform pillowLastMovePos;
    public float changeableRatio = 0f;
    [Header("Particle")] 
    public Transform particleStarSpawnPos;
    [Header("Tutorial Common")] 
    public TextMeshProUGUI _swipeText;
    public GameObject TutorialPanel;
    private bool isActives = true;
    [Header("Tutorial / Horizontal")]
    public GameObject SwipePanelHorizontal;
    public GameObject[] handsHorizontal;
    public GameObject handsHorizontalParent;
    private bool _moveHandHorizontal = true;
    private bool isRight = true;
    private float speed = 2f;

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
    }

    public void StartPhase(GameObject _paintable)
    {
        paintable = _paintable;
        StartCoroutine(StartPhase2());
    }

    IEnumerator StartPhase2()
    {
        var distanceOne = Vector3.Distance(paintable.transform.position, paintableFirstMovePos.transform.position);
        var distanceOneR = Quaternion.Angle(paintable.transform.rotation, paintableFirstMovePos.transform.rotation);
        var distanceOneS = Vector3.Distance(paintable.transform.localScale, paintableFirstMovePos.transform.localScale);
        //paintable.transform.localScale = paintableFirstMovePos.transform.localScale;

        var paintableFirstMovePosTimer = GameManager.Instance.paintableFirstMovePosTime;

        while (distanceOne > 0.1f)
        {
            distanceOne = Vector3.Distance(paintable.transform.position, paintableFirstMovePos.position);
            distanceOneR = Quaternion.Angle(paintable.transform.rotation, paintableFirstMovePos.transform.rotation);
            distanceOneS = Vector3.Distance(paintable.transform.localScale, paintableFirstMovePos.transform.localScale);
            
            paintable.transform.position = Vector3.MoveTowards(paintable.transform.position,
                paintableFirstMovePos.position, (distanceOne/paintableFirstMovePosTimer)*Time.deltaTime);
            
            paintable.transform.rotation = Quaternion.Lerp(paintable.transform.rotation,
                paintableFirstMovePos.transform.rotation,(distanceOneR/paintableFirstMovePosTimer)*Time.deltaTime);
            
            paintable.transform.localScale = Vector3.MoveTowards(paintable.transform.localScale,
                paintableFirstMovePos.localScale, (distanceOneS/paintableFirstMovePosTimer)*Time.deltaTime);
            yield return null;
        }
        
        distanceOne = Vector3.Distance(paintable.transform.position, paintableLastMovePos.transform.position);
        distanceOneR = Quaternion.Angle(paintable.transform.rotation, paintableLastMovePos.transform.rotation);
        
        pillow = Instantiate(Resources.Load<GameObject>("Models/Pillow_0"));
        pillow.transform.position = pillowSpawnPos.position;
        pillow.transform.rotation = pillowSpawnPos.rotation;
        
        var distanceOneP = Vector3.Distance(pillow.transform.position, pillowLastMovePos.transform.position);
        var distanceOnePR = Quaternion.Angle(pillow.transform.rotation, pillowLastMovePos.transform.rotation);

        var paintableLastMovePosTimer = GameManager.Instance.paintableLastMovePosTime;
        var pillowLastMovePosTimer = GameManager.Instance.pillowLastMovePosTime;

        while (distanceOne > 0.05f)
        {
            distanceOne = Vector3.Distance(paintable.transform.position, paintableLastMovePos.position);
            distanceOneR = Quaternion.Angle(paintable.transform.rotation, paintableLastMovePos.transform.rotation);
            
            paintable.transform.position = Vector3.MoveTowards(paintable.transform.position,
                paintableLastMovePos.position, (distanceOne/paintableLastMovePosTimer)*Time.deltaTime);
            paintable.transform.rotation = Quaternion.Lerp(paintable.transform.rotation,
                paintableLastMovePos.transform.rotation,(distanceOneR/paintableLastMovePosTimer)*Time.deltaTime);
            
            distanceOneP = Vector3.Distance(pillow.transform.position, pillowLastMovePos.position);
            distanceOnePR = Quaternion.Angle(pillow.transform.rotation, pillowLastMovePos.transform.rotation);
            
            pillow.transform.position = Vector3.MoveTowards(pillow.transform.position,
                pillowLastMovePos.position, (distanceOneP/pillowLastMovePosTimer)*Time.deltaTime);
            pillow.transform.rotation = Quaternion.Lerp(pillow.transform.rotation,
                pillowLastMovePos.transform.rotation,(distanceOnePR/pillowLastMovePosTimer)*Time.deltaTime);
            yield return null;
        }
        
        //ratioParent.SetActive(true);
        paintable.GetComponent<Paintable>().StartPaint();
        BubbleCleanActive();
        InstantActive();
    }

    #endregion

    #region Others

    public void Phase2CompleteRatio(float _ratio, float finishRatio, int currentLength)
    {
        if (changeableRatio != _ratio)
        {
            changeableRatio = _ratio;
        }
        //TODO Might activate again percentage of bubble
        if (_ratio != 101f)
        {
            // 180 finish R ratio 200, 2 total 200
            total = ((100 * currentLength) - _ratio) * (100f / finishRatio); // 200
            // ratio 0'ken dest 30 ratio
            CompleteRatioImage.fillAmount = total / 100f;
            //CompleteRatioText.text = "%" + totalRatio.ToString("0");
        }
        else
        {
            GameManager.Instance.PlayClean(false);
            CompleteRatioImage.fillAmount = 1;
            //CompleteRatioText.text = "%100%";
        }
    }
    
    public GameObject[] bubbleClean;
    public GameObject bubbleCleaner;
    public GameObject vrManager;
    
    public void BubbleCleanActive()
    {
        for (int i = 0; i < bubbleClean.Length; i++)
        {
            bubbleClean[i].SetActive(true);
        }
    }

    #endregion

    #region End

    public void Phase2Completed()
    {
        GameManager.Instance.PlayClean(false);
        GameManager.Instance.PlaySpray(false);
        GameManager.Instance.PlayCompleteSound();
        StartCoroutine(Phase2Ended());
    }

    IEnumerator Phase2Ended()
    {
        var particle = Instantiate(Resources.Load<GameObject>("Prefabs/particleStar"));
        particle.transform.position = particleStarSpawnPos.position;
        particle.transform.rotation = particleStarSpawnPos.rotation;
        particle.transform.localScale = particleStarSpawnPos.localScale;
        for (int i = 0; i < instantRemoves.Length; i++)
        {
            instantRemoves[i].SetActive(false);
        }
        GameManager.Instance.VibrateContinuous(false);
        yield return new WaitForSeconds(1.5f);
        Destroy(particle);
        PlayerPrefs.SetInt("bubble_active", 1);
        Destroy(paintable.GetComponent<P3dChangeCounter>());
        pillow.SetActive(false);
        Phase3.Instance.StartPhase(paintable);
    }

    #endregion
    
    #region Button

    private bool canVibrate = true;
    
    public void onButtonDown()
    {
        bubbleCleaner.GetComponent<P3dToggleParticles>().enabled = true;
        bubbleCleaner.transform.GetChild(0).gameObject.SetActive(true);
        bubbleCleaner.GetComponent<Moves>().SetMove(true, false);
        if (canVibrate)
        {
            canVibrate = false;
            GameManager.Instance.VibrateContinuous(true);
        }

        SetHorizontalTutorial(false, false);
    }
    
    public void onButtonUp()
    {
        bubbleCleaner.GetComponent<P3dToggleParticles>().enabled = false;
        bubbleCleaner.GetComponent<Moves>().SetMove(false, false);
        canVibrate = true;
        GameManager.Instance.VibrateContinuous(false);
        SetHorizontalTutorial(true, true);
    }

    #endregion

    #region Tutorial

    #region Tutorial

    public void SetHorizontalTutorial(bool _isActive,bool _isRight)
    {
        TutorialPanel.SetActive(_isActive);
        isActives = _isActive;
        return;
        if (isActives)
        {
            handsHorizontalParent.SetActive(true);
            SwipePanelHorizontal.SetActive(true);
            _moveHandHorizontal = true;
            isRight = _isRight;
        }

        _swipeText.text = _isRight ? "SWIPE RIGHT" : "SWIPE LEFT";
    }

    private void Update()
    {
        if(!isActives) return;
        if (_moveHandHorizontal)
        {
            if (isRight)
            {
                if (Math.Abs(handsHorizontal[0].transform.position.x - handsHorizontal[1].transform.position.x) > 105)
                {
                    speed =Time.deltaTime;
                    handsHorizontal[0].transform.position = Vector3.Lerp(handsHorizontal[0].transform.position, handsHorizontal[1].transform.position, speed);
                }
                else
                {
                    isRight = false;
                }
            }
            else
            {
                if (Math.Abs(handsHorizontal[0].transform.position.x - handsHorizontal[2].transform.position.x) > 105)
                {
                    speed =Time.deltaTime;
                    handsHorizontal[0].transform.position = Vector3.Lerp(handsHorizontal[0].transform.position, handsHorizontal[2].transform.position, speed);
                }
                else
                {
                    isRight = true;
                }
            }
        }
    }

    #endregion

    #endregion

}
