using System;
using System.Collections;
using System.Globalization;
using PaintIn3D;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.UI;
using VisCircle;
using Random = UnityEngine.Random;

public class Phase6 : MonoBehaviour
{
    public static Phase6 Instance;
    
    public GameObject[] finalActives;
    public GameObject[] instantActives;
    public GameObject[] instantRemoves;
    [Header("Paintable")] 
    public GameObject paintable;
    [Header("Model")]
    public GameObject model;
    public Transform modelSpawnPos;
    public int modelNumber;
    public int randomOutfit;
    public int randomHair;
    public Material matFinal;
    [Header("Man Model")]
    public Transform manSpawnPos;
    public Transform manMovePosFirst;
    public Transform manMovePosSecond;
    public Transform manMovePosFinal;
    private GameObject modelMan;
    [Header("Camera")]
    public Transform cameraMovePosFirst;
    public Transform cameraMovePosSecond;
    public Transform cameraMovePosFinal;
    public Camera cameraMain;
    [Header("Bubble")] 
    public Transform modelBubbleSpawnPos;
    public Transform manBubbleSpawnPos;
    [Header("Particle")] 
    public Transform particleStarSpawnPos;
    public Transform[] particleConfettiSpawnPos;
    public Transform particleConfettiShowerSpawnPos;
    public Transform particleLoveSpawnPos;
    [Header("Money Sender")] 
    public Button claimButton;
    public TextMeshProUGUI claimText;
    public Image moneySpawnPos;
    private int totalReward = 0;
    public Sprite newClaimSprite;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartPhase(GameObject _paintable)
    {
        paintable = _paintable;
        //paintable.SetActive(false);
        PlayerDatabase.Instance.SetDay(1);
        InstantRemove();
        InstantActive();
        SpawnModel();
        StartCoroutine(MoveToFinal());
    }
    
    public void InstantActive()
    {
        for (int i = 0; i < instantActives.Length; i++)
        {
            instantActives[i].SetActive(true);
        }
    }
    
    public void InstantRemove()
    {
        for (int i = 0; i < instantRemoves.Length; i++)
        {
            instantRemoves[i].SetActive(false);
        }
    }

    private void SpawnModel()
    {
        modelNumber = PlayerDatabase.Instance.GetModel();
        randomOutfit = PlayerDatabase.Instance.GetOutfit();
        randomHair = PlayerDatabase.Instance.GetHair();
        var modelName = GameManager.Instance.modelNames[modelNumber];
        model = Instantiate(Resources.Load<GameObject>(
            "Models/Woman/" + modelNumber + "/" + modelName + randomHair + "_Outfit" + randomOutfit));
        model.transform.position = modelSpawnPos.position;
        model.transform.rotation = modelSpawnPos.rotation;
        model.transform.localScale = modelSpawnPos.localScale;
        model.GetComponent<Agent>().SetFinalModelDetails();

        model.GetComponent<Agent>().AnimRun(false);
        model.GetComponent<NavMeshAgent>().enabled = false;
        
        paintable.transform.SetParent(model.transform);
        
        GameManager.Instance.PlayCompleteSound();
    }

    public void ChangeToFinalMat()
    {
        var meshRenderer = model.GetComponent<Agent>().TransparentableObject.GetComponent<SkinnedMeshRenderer>();
        var mats = meshRenderer.materials;
        var matLocations = paintable.GetComponent<Paintable>().partLocations;
        for (int i = 0; i < meshRenderer.materials.Length; i++)
        {
            for (int j = 0; j < matLocations.Length; j++)
            {
                if(i == matLocations[j])
                {
                    var partMesh = paintable.GetComponent<Paintable>().parts[j].GetComponent<MeshRenderer>();
                    for (int k = 0; k < partMesh.materials.Length; k++)
                    {
                        if(partMesh.materials[k] != null)
                        {
                            mats[i] = paintable.GetComponent<Paintable>().parts[j].GetComponent<MeshRenderer>().materials[k];
                        }
                    }
                }
            }
        }
        meshRenderer.materials = mats;
    }

    #region Move

    IEnumerator MoveToFinal()
    {
        ChangeToFinalMat();
        paintable.SetActive(false);
        model.GetComponent<Agent>().AnimRun(false);
        var manModelNumber = RandomManModel();
        var manRandomOutfit = RandomManOutfit();
        var manRandomHair = RandomManHair();
        modelMan = Instantiate(Resources.Load<GameObject>(
            "Models/Man/" + manModelNumber + "/Hair" + manRandomHair + "_Outfit" + manRandomOutfit));
        modelMan.transform.position = manSpawnPos.position;
        modelMan.transform.rotation = manSpawnPos.rotation;
        modelMan.transform.localScale = manSpawnPos.localScale;
        var particle = Instantiate(Resources.Load<GameObject>("Prefabs/particleStar"));
        particle.transform.position = particleStarSpawnPos.position;
        particle.transform.rotation = particleStarSpawnPos.rotation;
        particle.transform.localScale = particleStarSpawnPos.localScale;
        
        yield return new WaitForSeconds(1.25f);
        Destroy(particle);

        var distanceOne = Vector3.Distance(cameraMain.transform.position, cameraMovePosFirst.transform.position);
        var distanceOneR = Quaternion.Angle(cameraMain.transform.rotation, cameraMovePosFirst.transform.rotation);

        var animCamSpeed1 = GameManager.Instance.animCamSpeed1;

        while (distanceOne > 0.25f)
        {
            distanceOne = Vector3.Distance(cameraMain.transform.position, cameraMovePosFirst.position);
            distanceOneR = Quaternion.Angle(cameraMain.transform.rotation, cameraMovePosFirst.transform.rotation);
            
            cameraMain.transform.position = Vector3.MoveTowards(cameraMain.transform.position,
                cameraMovePosFirst.position, (distanceOne/animCamSpeed1)*Time.deltaTime);
            
            cameraMain.transform.rotation = Quaternion.Lerp(cameraMain.transform.rotation,
                cameraMovePosFirst.transform.rotation,(distanceOneR/animCamSpeed1)*Time.deltaTime);

            yield return null;
        }
        
        modelMan.GetComponent<ManAgent>().StartMoving(manMovePosFirst);
        
        var distanceOne2 = Vector3.Distance(cameraMain.transform.position, cameraMovePosSecond.transform.position);
        var distanceOneR2 = Quaternion.Angle(cameraMain.transform.rotation, cameraMovePosSecond.transform.rotation);

        var animCamSpeed2 = GameManager.Instance.animCamSpeed2;

        while (distanceOne2 > 0.01f)
        {
            distanceOne2 = Vector3.Distance(cameraMain.transform.position, cameraMovePosSecond.position);
            distanceOneR2 = Quaternion.Angle(cameraMain.transform.rotation, cameraMovePosSecond.transform.rotation);
            
            cameraMain.transform.position = Vector3.MoveTowards(cameraMain.transform.position,
                cameraMovePosSecond.position, (distanceOne2/animCamSpeed2)*Time.deltaTime);
            
            cameraMain.transform.rotation = Quaternion.Lerp(cameraMain.transform.rotation,
                cameraMovePosSecond.transform.rotation,(distanceOneR2/animCamSpeed2)*Time.deltaTime);

            yield return null;
        }
    }
    
    public void MainReachedDestination()
    {
        StartCoroutine(MoveToFinal2());
    }
    
    IEnumerator MoveToFinal2()
    {
        for (int i = 0; i < particleConfettiSpawnPos.Length; i++)
        {
            var particle = Instantiate(Resources.Load<GameObject>("Prefabs/particleConfetti"));
            particle.transform.position = particleConfettiSpawnPos[i].position;
            particle.transform.rotation = particleConfettiSpawnPos[i].rotation;
            particle.transform.localScale = particleConfettiSpawnPos[i].localScale;
            Destroy(particle,3.25f);
        }
        /*var particleShower = Instantiate(Resources.Load<GameObject>("Prefabs/particleConfettiShower"));
        particleShower.transform.position = particleConfettiShowerSpawnPos.position;
        particleShower.transform.rotation = particleConfettiShowerSpawnPos.rotation;
        particleShower.transform.localScale = particleConfettiShowerSpawnPos.localScale;*/
        model.GetComponent<Agent>().ShyAnim(true);
        yield return new WaitForSeconds(1f);
        model.GetComponent<Agent>().SpawnLastBubble();
        yield return new WaitForSeconds(1f);
        
        var particleLove = Instantiate(Resources.Load<GameObject>("Prefabs/particleLove"));
        particleLove.transform.position = particleLoveSpawnPos.position;
        particleLove.transform.rotation = particleLoveSpawnPos.rotation;
        particleLove.transform.localScale = particleLoveSpawnPos.localScale;
        
        yield return new WaitForSeconds(2f);
        var sz = particleLove.GetComponent<ParticleSystem>().sizeOverLifetime;
        //sz.enabled = false;
        
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0.0f, 0.25f);
        curve.AddKey(0.75f, 0.25f);

        sz.size = new ParticleSystem.MinMaxCurve(1.5f, curve);
        
        var rewardGold = Random.Range(10, 50);
        rewardGold *= 10;
        totalReward = rewardGold;
        claimText.text = "CLAIM " + totalReward;

        for (int i = 0; i < finalActives.Length; i++)
        {
            finalActives[i].SetActive(true);
        }
    }

    #endregion

    #region ManSpawn

    public int RandomManModel()
    {
        var randomModel = Random.Range(0, GameManager.Instance.manModelCount);
        return randomModel;
    }
    
    public int RandomManOutfit()
    {
        var randomOutfit = Random.Range(0, GameManager.Instance.manOutfitCount);
        randomOutfit += 1; // Because Outfit counter starts in 1
        return randomOutfit;
    }
    
    public int RandomManHair()
    {
        var randomHair = Random.Range(0, GameManager.Instance.manHairCount);
        randomHair += 1; // Because Outfit counter starts in 1
        return randomHair;
    }

    #endregion

    public void Replay()
    {
        StartCoroutine(SendMoney());
    }

    IEnumerator SendMoney()
    {
        claimButton.GetComponent<Animator>().enabled = false;
        claimButton.GetComponent<Button>().interactable = false;
        claimButton.GetComponent<Image>().sprite = newClaimSprite;
        PlayerDatabase.Instance.SetGold(totalReward);
        var increaseCount = GameManager.Instance.goldIncreaseCount;
        var speed = GameManager.Instance.sendSpeed;
        var delay = GameManager.Instance.durationBetweenSend;
        var spawnMoneyCount = totalReward / increaseCount;
        var removeGold = totalReward;
        for (int i = 0; i < spawnMoneyCount; i++)
        {
            var sendedMoney = Instantiate(Resources.Load<GameObject>("UI/moneySend"));
            sendedMoney.transform.SetParent(claimButton.transform.parent.transform);
            sendedMoney.GetComponent<RectTransform>().transform.position =
                moneySpawnPos.GetComponent<RectTransform>().transform.position;
            sendedMoney.transform.SetParent(UIManager.Instance.moneyPanel.transform);
            sendedMoney.GetComponent<MovingMoney>().Send(increaseCount, speed, i == spawnMoneyCount - 1);
            removeGold -= increaseCount;
            claimText.text = "CLAIM " + removeGold;
            GameManager.Instance.PlayMoneyCount();
            GameManager.Instance.MoneyIncreased(increaseCount);

            yield return new WaitForSeconds(delay);
        }

        yield return new WaitForSeconds(0.25f);
        InstantRemove();
        UIManager.Instance.totalMoneyText.text = totalReward.ToString();
        UIManager.Instance.endPanel.SetActive(true);
        GameManager.Instance.PlayCompleteSound();
    }

    public void ReplayGame()
    {
        GameManager.Instance.Replay();
    }
    
    
}
