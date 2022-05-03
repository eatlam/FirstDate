using System;
using System.Collections;
using PaintIn3D;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Phase1 : MonoBehaviour
{
    public static Phase1 Instance;
    public GameObject[] FinalRemoves;
    public GameObject[] instantRemoves;
    public GameObject[] instantActives;
    public GameObject[] selectButtons;
    public Sprite[] selectSprites;
    [Header("Paintable")]
    public GameObject paintable;
    public GameObject floor;
    public Transform movePosLeftPaintable;
    public Transform movePosMidPaintable;
    public Transform movePosRightPaintable;
    public Transform movePosLeftFloor;
    public Transform movePosMidFloor;
    public Transform movePosRightFloor;
    public Transform killPosMidFloor;
    [Header("Values")] 
    private int currentDress = 0;
    private bool isProccessing = false;
    public GameObject dressPanel;
    public GameObject[] dressButtons;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    

    #region Start
    public void InstantActive()
    {
        paintable = Instantiate(Resources.Load<GameObject>("Models/Paintable/Abigail_Hairstyle1_Outfit" + 4));
        paintable.transform.position = movePosMidPaintable.position;
        paintable.transform.rotation = movePosMidPaintable.rotation;
        paintable.transform.localScale = movePosMidPaintable.localScale;
        PowerUpAnim(paintable, true);
        floor = Instantiate(Resources.Load<GameObject>("Models/Floor"));
        floor.transform.position = movePosMidFloor.position;
        floor.transform.rotation = movePosMidFloor.rotation;
        currentDress = 3;
    }

    public void StartPhase()
    {
        for (int i = 0; i < instantActives.Length; i++)
        {
            instantActives[i].SetActive(true);
        }
        InstantiateButtons();
        CorrectButtons(3);
    }

    #endregion

    #region End

    public void Phase1Completed()
    {
        GameManager.Instance.VibrateHigher();
        PlayerDatabase.Instance.SetOutfit(currentDress + 1);
        StartCoroutine(Phase1Ended());
    }

    IEnumerator Phase1Ended()
    {
        PowerUpAnim(paintable, false);
        for (int i = 0; i < instantRemoves.Length; i++)
        {
            instantRemoves[i].SetActive(false);
        }
        var distanceOne = Vector3.Distance(floor.transform.position, killPosMidFloor.transform.position);
        var targetScale = Vector3.Distance(floor.transform.localScale, killPosMidFloor.transform.localScale);

        var floorKillTimer = GameManager.Instance.animKillSpeed;

        while (distanceOne > 0.1f)
        {
            distanceOne = Vector3.Distance(floor.transform.position, killPosMidFloor.position);
            floor.transform.position = Vector3.MoveTowards(floor.transform.position,
                killPosMidFloor.position, (distanceOne/floorKillTimer)*Time.deltaTime);
            
            
            targetScale = Vector3.Distance(floor.transform.localScale, killPosMidFloor.localScale);
            floor.transform.localScale = Vector3.MoveTowards(floor.transform.localScale,
                killPosMidFloor.localScale, (targetScale/floorKillTimer)*Time.deltaTime);
            
            yield return null;
        }
        
        Destroy(floor);
        
        yield return new WaitForSeconds(0.1f);
        PlayerPrefs.SetInt("bubble_active", 0);
        Phase2.Instance.StartPhase(paintable);
    }

    #endregion

    #region Move

    public void MoveLeft(GameObject RightOne, GameObject RightFloor)
    {
        if(isProccessing) return;
        isProccessing = true;
        StartCoroutine(MovingLeft(RightOne,RightFloor));
    }

    IEnumerator MovingLeft(GameObject RightOne, GameObject RightFloor)
    {
        var distanceMid = Vector3.Distance(paintable.transform.position, movePosLeftPaintable.transform.position);
        var distanceMidFloor = Vector3.Distance(floor.transform.position, movePosLeftFloor.transform.position);
        var distanceRight = Vector3.Distance(RightOne.transform.position, movePosMidPaintable.transform.position);
        var distanceRightFloor = Vector3.Distance(RightFloor.transform.position, movePosMidFloor.transform.position);

        var animMoveSpeed = GameManager.Instance.animMoveSpeed;
        
        yield return new WaitForSeconds(0.05f);

        while (distanceMid > 0.01f)
        {
            distanceMid = Vector3.Distance(paintable.transform.position, movePosLeftPaintable.transform.position);
            distanceMidFloor = Vector3.Distance(floor.transform.position, movePosLeftFloor.transform.position);
            distanceRight = Vector3.Distance(RightOne.transform.position, movePosMidPaintable.transform.position);
            distanceRightFloor = Vector3.Distance(RightFloor.transform.position, movePosMidFloor.transform.position);
            
            paintable.transform.position = Vector3.MoveTowards(paintable.transform.position,
                movePosLeftPaintable.position, (distanceMid/animMoveSpeed)*Time.deltaTime);
            floor.transform.position = Vector3.MoveTowards(floor.transform.position,
                movePosLeftFloor.position, (distanceMidFloor/animMoveSpeed)*Time.deltaTime);
            RightOne.transform.position = Vector3.MoveTowards(RightOne.transform.position,
                movePosMidPaintable.position, (distanceRight/animMoveSpeed)*Time.deltaTime);
            RightFloor.transform.position = Vector3.MoveTowards(RightFloor.transform.position,
                movePosMidFloor.position, (distanceRightFloor/animMoveSpeed)*Time.deltaTime);
            
            yield return null;
        }

        var destroyable = paintable;
        var destroyableFloor = floor;
        paintable = RightOne;
        floor = RightFloor;
        Destroy(destroyable);
        Destroy(destroyableFloor);
        PowerUpAnim(paintable, true);

        isProccessing = false;
    }
    
    public void MoveRight(GameObject LeftOne, GameObject LeftFloor)
    {
        if(isProccessing) return;
        isProccessing = true;
        StartCoroutine(MovingRight(LeftOne,LeftFloor));
    }

    IEnumerator MovingRight(GameObject LeftOne, GameObject LeftFloor)
    {
        var distanceMid = Vector3.Distance(paintable.transform.position, movePosRightPaintable.transform.position);
        var distanceMidFloor = Vector3.Distance(floor.transform.position, movePosRightFloor.transform.position);
        var distanceLeft = Vector3.Distance(LeftOne.transform.position, movePosMidPaintable.transform.position);
        var distanceLeftFloor = Vector3.Distance(LeftFloor.transform.position, movePosMidFloor.transform.position);
        
        var animMoveSpeed = GameManager.Instance.animMoveSpeed;
        yield return new WaitForSeconds(0.05f);

        while (distanceMid > 0.01f)
        {
            distanceMid = Vector3.Distance(paintable.transform.position, movePosRightPaintable.transform.position);
            distanceMidFloor = Vector3.Distance(floor.transform.position, movePosRightFloor.transform.position);
            distanceLeft = Vector3.Distance(LeftOne.transform.position, movePosMidPaintable.transform.position);
            distanceLeftFloor = Vector3.Distance(LeftFloor.transform.position, movePosMidFloor.transform.position);
            
            paintable.transform.position = Vector3.MoveTowards(paintable.transform.position,
                movePosRightPaintable.position, (distanceMid/animMoveSpeed)*Time.deltaTime);
            floor.transform.position = Vector3.MoveTowards(floor.transform.position,
                movePosRightFloor.position, (distanceMidFloor/animMoveSpeed)*Time.deltaTime);
            LeftOne.transform.position = Vector3.MoveTowards(LeftOne.transform.position,
                movePosMidPaintable.position, (distanceLeft/animMoveSpeed)*Time.deltaTime);
            LeftFloor.transform.position = Vector3.MoveTowards(LeftFloor.transform.position,
                movePosMidFloor.position, (distanceLeftFloor/animMoveSpeed)*Time.deltaTime);
            
            yield return null;
        }

        var destroyable = paintable;
        var destroyableFloor = floor;
        paintable = LeftOne;
        floor = LeftFloor;
        Destroy(destroyable);
        Destroy(destroyableFloor);
        PowerUpAnim(paintable,true);

        isProccessing = false;
    }

    #endregion
    
    #region ChangeDress

    public void ChangeDress(int _dress)
    {
        if(_dress>GameManager.Instance.totalPaintables) return;
        if(_dress == currentDress) return;
        if(isProccessing) return;
        
        if (_dress > currentDress)
        {
            var newPaintable = Instantiate(Resources.Load<GameObject>
                    ("Models/Paintable/Abigail_Hairstyle1_Outfit" + (_dress + 1)));

            newPaintable.transform.position = movePosRightPaintable.position;
            newPaintable.transform.rotation = movePosRightPaintable.rotation;
            newPaintable.transform.localScale = movePosRightPaintable.localScale;
            var newFloor = Instantiate(Resources.Load<GameObject>("Models/Floor"));
            newFloor.transform.position = movePosRightFloor.position;
            newFloor.transform.rotation = movePosRightFloor.rotation;
            
            MoveLeft(newPaintable, newFloor);
        }
        else
        {
            var newPaintable = Instantiate(Resources.Load<GameObject>
                ("Models/Paintable/Abigail_Hairstyle1_Outfit" + (_dress + 1)));

            newPaintable.transform.position = movePosLeftPaintable.position;
            newPaintable.transform.rotation = movePosLeftPaintable.rotation;
            newPaintable.transform.localScale = movePosLeftPaintable.localScale;
            var newFloor = Instantiate(Resources.Load<GameObject>("Models/Floor"));
            newFloor.transform.position = movePosLeftFloor.position;
            newFloor.transform.rotation = movePosLeftFloor.rotation;
            
            MoveRight(newPaintable, newFloor);
        }
        
        GameManager.Instance.Vibrate();
        currentDress = _dress;
    }

    #endregion

    public void PowerUpAnim(GameObject go, bool isTrue)
    {
        var currentScale = paintable.transform.localScale.x;
        go.GetComponent<Scale>().StartScale(currentScale - 3,currentScale +GameManager.Instance.maxScaleOutfit,
            GameManager.Instance.scaleSpeed);
        go.GetComponent<Scale>().enabled = isTrue;
    }
    
    public void CorrectButtons(int dress)
    {
        for (int i = 0; i < dressButtons.Length; i++)
        {
            dressButtons[i].GetComponent<OnClick>().Choose(false);
            dressButtons[i].transform.localScale = new Vector3(1f, 1f, 1f);
        }
        dressButtons[dress].GetComponent<OnClick>().Choose(true);
        dressButtons[dress].transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
    }
    
    private void InstantiateButtons()
    {
        var buttonCount = Dresses.Instance.dressPalets.Length;
        
        dressButtons = new GameObject[buttonCount];
        
        InstantiateAButton(3);
        InstantiateAButton(1);
        InstantiateAButton(2);
        InstantiateAButton(4);
        InstantiateAButton(5);
        InstantiateAButton(0);
    }

    private void InstantiateAButton(int i)
    {
        var textures = Dresses.Instance.dressPalets;
        var defaultCount = GameManager.Instance.defaultTextureCount;
        var expandX = GameManager.Instance.expandX;
        var expandWidth = GameManager.Instance.expandWidth;
        var insButton = Instantiate(Resources.Load<GameObject>("Prefabs/dressButton"));
        insButton.transform.SetParent(dressPanel.transform);
        //insButton.name = i.ToString();
        dressButtons[i] = insButton;
        if (i > defaultCount - 1)
        {
            var position = dressPanel.transform.position;
            position = new Vector3(position.x + expandX,
                position.y, position.z);
            dressPanel.transform.position = position;
            var sizeDelta = dressPanel.GetComponent<RectTransform>().sizeDelta;
            dressPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeDelta.x + expandWidth, sizeDelta.y);
        }
        insButton.GetComponent<OnClick>().AddTask(i);
        insButton.GetComponent<OnClick>().SetTextureOfMine(textures[i]);
        if (GameManager.Instance.ShouldOpenDress(i))
        {
            insButton.GetComponent<OnClick>().lockButton.gameObject.SetActive(false);
        }
        else
        {
            insButton.GetComponent<OnClick>().lockButton.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                GameManager.Instance.dressPrices[i].ToString();
        }
    }
    
    

}
