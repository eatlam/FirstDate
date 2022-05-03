using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using PaintIn3D;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Phase4 : MonoBehaviour
{
    public static Phase4 Instance;
    public GameObject[] FinalRemoves;
    public GameObject[] instantRemoves;
    public GameObject[] instantActives;
    public GameObject spray;
    [Header("Paintable")]
    public GameObject paintable;
    public Transform paintableFirstMovePos;
    [Header("UI")] 
    public GameObject colorPanel;
    public GameObject[] colorButtons;
    private List<GameObject> instantiatedButtons;
    public GameObject[] SMLbuttons;
    public Sprite[] SMLSprites;
    [Header("Particle")] 
    public Transform particleStarSpawnPos;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    #region Button
    
    private bool canVibrate = true;

    public void onButtonDown()
    {
        spray.GetComponent<P3dToggleParticles>().enabled = true;
        spray.GetComponent<Moves>().SetMove(true,true);
        spray.transform.GetChild(0).gameObject.SetActive(true);
        if (canVibrate)
        {
            canVibrate = false;
            GameManager.Instance.VibrateContinuous(true);
        }
    }
    
    public void onButtonUp()
    {
        spray.GetComponent<P3dToggleParticles>().enabled = false;
        spray.GetComponent<Moves>().SetMove(false,true);
        spray.transform.GetChild(0).gameObject.SetActive(false);
        spray.transform.GetChild(1).gameObject.SetActive(false);
        canVibrate = true;
        GameManager.Instance.VibrateContinuous(false);
    }

    public void ChangeSMLs(int choosen)
    {
        for (int i = 0; i < SMLbuttons.Length; i++)
        {
            SMLbuttons[i].GetComponent<Image>().sprite = SMLSprites[0];
            SMLbuttons[i].transform.localScale = Vector3.one;
        }
        SMLbuttons[choosen].GetComponent<Image>().sprite = SMLSprites[1];
        SMLbuttons[choosen].transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
    }

    #endregion
    
    #region Start
    
    private void InstantiateButtons()
    {
        var buttonCount = Colors.Instance.colorPalets.Length;
        var colors = Colors.Instance.colorPalets;
        var defaultCount = GameManager.Instance.defaultTextureCount;
        var expandX = GameManager.Instance.expandX;
        var expandWidth = GameManager.Instance.expandWidth;
        
        instantiatedButtons = new List<GameObject>();
        colorButtons = new GameObject[buttonCount];
        
        for (int i = 0; i < buttonCount; i++)
        {
            var insButton = Instantiate(Resources.Load<GameObject>("Prefabs/colorButton"));
            insButton.transform.SetParent(colorPanel.transform);
            //insButton.name = i.ToString();
            insButton.transform.GetChild(0).GetComponent<Image>().color = colors[i];
            insButton.GetComponent<P3dColor>().Color = colors[i];
            colorButtons[i] = insButton;
            if (i > defaultCount - 1)
            {
                var position = colorPanel.transform.position;
                position = new Vector3(position.x + expandX,
                    position.y, position.z);
                colorPanel.transform.position = position;
                var sizeDelta = colorPanel.GetComponent<RectTransform>().sizeDelta;
                colorPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeDelta.x + expandWidth, sizeDelta.y);
            }
            insButton.GetComponent<OnClick>().AddTask(i);
            if (GameManager.Instance.ShouldOpenSpray(i))
            {
                insButton.GetComponent<OnClick>().lockButton.gameObject.SetActive(false);
            }
            else
            {
                insButton.GetComponent<OnClick>().lockButton.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                    GameManager.Instance.sprayPrices[i].ToString();
            }
        }
    }
    
    public void InstantActive()
    {
        for (int i = 0; i < instantActives.Length; i++)
        {
            instantActives[i].SetActive(true);
        }
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
        StartCoroutine(StartPhase4());
    }

    IEnumerator StartPhase4()
    {
        var distanceOne = Vector3.Distance(paintable.transform.position, paintableFirstMovePos.transform.position);
        var distanceOneR = Quaternion.Angle(paintable.transform.rotation, paintableFirstMovePos.transform.rotation);
        var distanceOneS = Vector3.Distance(paintable.transform.localScale, paintableFirstMovePos.transform.localScale);

        var paintableFirstMovePosTimer = GameManager.Instance.paintableFirstMovePosTime4;

        while (distanceOne > 0.2f)
        {
            distanceOne = Vector3.Distance(paintable.transform.position, paintableFirstMovePos.position);
            distanceOneR = Quaternion.Angle(paintable.transform.rotation, paintableFirstMovePos.transform.rotation);
            distanceOneS = Vector3.Distance(paintable.transform.localScale, paintableFirstMovePos.transform.localScale);
            
            paintable.transform.localScale = Vector3.MoveTowards(paintable.transform.localScale,
                paintableFirstMovePos.localScale, (distanceOneS/paintableFirstMovePosTimer)*Time.deltaTime);
            
            paintable.transform.position = Vector3.MoveTowards(paintable.transform.position,
                paintableFirstMovePos.position, (distanceOne/paintableFirstMovePosTimer)*Time.deltaTime);
            
            paintable.transform.rotation = Quaternion.Lerp(paintable.transform.rotation,
                paintableFirstMovePos.transform.rotation,(distanceOneR/paintableFirstMovePosTimer)*Time.deltaTime);
            yield return null;
        }
        
        GameManager.Instance.ChangePaletSize(1);
        InstantActive();
        InstantiateButtons();
        
        GameManager.Instance.ChangeColor(0);
    }

    #endregion

    #region End

    public void Phase4Completed()
    {
        GameManager.Instance.PlayClean(false);
        GameManager.Instance.PlaySpray(false);
        GameManager.Instance.VibrateContinuous(false);
        StartCoroutine(Phase4Ended());
    }

    IEnumerator Phase4Ended()
    {
        InstantRemoves();
        
        var particle = Instantiate(Resources.Load<GameObject>("Prefabs/particleStar"));
        particle.transform.position = particleStarSpawnPos.position;
        particle.transform.rotation = particleStarSpawnPos.rotation;
        particle.transform.localScale = particleStarSpawnPos.localScale;
        yield return new WaitForSeconds(0.25f);
        Phase5.Instance.StartPhase(paintable);
        yield return new WaitForSeconds(1.25f);
        Destroy(particle);
    }

    #endregion

}
