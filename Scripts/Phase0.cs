using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Phase0 : MonoBehaviour
{
    public static Phase0 Instance;
    public GameObject[] FinalRemoves;
    public GameObject[] instantRemoves;
    public GameObject[] instantActives;
    public GameObject[] firstUIs;
    public GameObject AcceptUI;
    [Header("Curtain")]
    public Transform[] curtainSpawnPos;
    public Transform[] curtainMaxMovePos;
    public Transform[] curtainRemovePos;
    [Header("Models")] 
    public Transform[] modelSpawnPos;
    public Transform modelFirstMovePos;
    public Transform modelFinalMovePos;
    public Transform modelKillPos;
    public GameObject[] models;
    public GameObject currentModel;
    public Transform starLowSpawnPos;
    private int maxSpawnModel;
    public Transform lookRot;
    public Transform bubblePos;
    [Header("Days")] 
    public Text dayText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartPhase()
    {
        dayText.text = "Day " + PlayerDatabase.Instance.GetDay();
        for (int i = 0; i < instantActives.Length; i++)
        {
            instantActives[i].SetActive(true);
        }

        maxSpawnModel = GameManager.Instance.maxSpawnModel;
        models = new GameObject[maxSpawnModel];
        ModelsMove();
    }

    public void StartModels()
    {
        for (int i = 0; i < firstUIs.Length; i++)
        {
            firstUIs[i].SetActive((false));
        }
        //StartCoroutine(ModelsMove());
        StartCoroutine(ModelBegin());
    }

    IEnumerator ModelBegin()
    {
        
        var mainModel = models[models.Length - 1];
        currentModel = mainModel;
        mainModel.GetComponent<Agent>().StartMoving(
            modelFinalMovePos, GameManager.Instance.modelWalkSpeed, true, false,-1,-1, -1);
        GameManager.Instance.PlayHeelSound(true);
        
        yield return new WaitForSeconds(0.5f);
        
        var modelSecond = models[0];
        modelSecond.GetComponent<Agent>().StartMoving(
            modelFirstMovePos, GameManager.Instance.modelWalkSpeed, false, false,-1,-1, -1);
    }

    void ModelsMove()
    {
        for (int i = 0; i < maxSpawnModel; i++)
        {
            var modelNumber = RandomModel();
            var randomOutfit = RandomOutfit();
            var randomHair = RandomHair(modelNumber);
            var modelName = GameManager.Instance.modelNames[modelNumber];
            models[i] = Instantiate(Resources.Load<GameObject>(
                "Models/Woman/" + modelNumber + "/" + modelName + randomHair + "_Outfit" + randomOutfit));
            models[i].transform.position = modelSpawnPos[i].position;
            models[i].transform.rotation = modelSpawnPos[i].rotation;
            models[i].transform.localScale = modelSpawnPos[i].localScale;
            models[i].GetComponent<Agent>().model = modelNumber;
            models[i].GetComponent<Agent>().outfit = randomOutfit;
            models[i].GetComponent<Agent>().hair = randomHair;
            models[i].GetComponent<Agent>().PrepareSpawnedOnes(lookRot);
        }

    }
    
    public void MainReachedDestination()
    {
        AcceptUI.SetActive(true);
    }

    public int RandomModel()
    {
        if (!GameManager.Instance.isSameOutfit)
        {
            var randomModel = Random.Range(0, GameManager.Instance.modelsCount);
            return randomModel;
        }

        return GameManager.Instance.certainModel;
    }
    
    public int RandomOutfit()
    {
        if (!GameManager.Instance.isSameOutfit)
        {
            var randomOutfit = Random.Range(0, GameManager.Instance.outfitCount);
            randomOutfit += 1; // Because Outfit counter starts in 1
            return randomOutfit;
        }
        
        return GameManager.Instance.certainOutfit;
    }
    
    public int RandomHair(int isModel0)
    {
        if (!GameManager.Instance.isSameOutfit)
        {
            var randomHair = Random.Range(0, GameManager.Instance.hairCount);
            randomHair += 1; // Because Outfit counter starts in 1
            if (randomHair == 4 && isModel0 == 0)
                randomHair = Random.Range(1, 4);
            return randomHair;
        }
        
        return GameManager.Instance.certainOutfit;
    }
    
    public void Decline()
    {
        GameManager.Instance.Vibrate();
        AcceptUI.SetActive(false);
        StartCoroutine(Kill());
    }
    
    IEnumerator Kill()
    {
        var killModel = models[models.Length - 1];
        var newCurrent = models[0];
        killModel.GetComponent<Agent>().SadAnim();
        
        for (int i = 0; i < models.Length; i++)
        {
            models[i] = null;
        }
        models[models.Length - 1] = newCurrent;
        currentModel = newCurrent;
        
        yield return new WaitForSeconds(0.125f);
        GameManager.Instance.PlayDisappointSound(true);
        yield return new WaitForSeconds(1.375f);
        
        killModel.GetComponent<Agent>().StartMoving(
            modelKillPos, GameManager.Instance.modelWalkSpeed, false, true, -1,-1, -1);
        
        newCurrent.GetComponent<Agent>().StartMoving(
            modelFinalMovePos, GameManager.Instance.modelWalkSpeed, true, false, -1, -1, -1);
        GameManager.Instance.PlayHeelSound(true);
        
        yield return new WaitForSeconds(0.5f);
        
        var modelNumber = RandomModel();
        var randomOutfit = RandomOutfit();
        var randomHair = RandomHair(modelNumber);
        var modelName = GameManager.Instance.modelNames[modelNumber];
        models[0] = Instantiate(Resources.Load<GameObject>(
            "Models/Woman/" + modelNumber + "/" + modelName + randomHair + "_Outfit" + randomOutfit));
        models[0].transform.position = modelSpawnPos[0].position;
        models[0].transform.rotation = modelSpawnPos[0].rotation;
        models[0].transform.localScale = modelSpawnPos[0].localScale;
        models[0].GetComponent<Agent>().PrepareSpawnedOnes(lookRot);
        
        yield return new WaitForSeconds(0.5f);
        
        models[0].GetComponent<Agent>().StartMoving(
            modelFirstMovePos, GameManager.Instance.modelWalkSpeed, false, false, modelNumber, randomOutfit, randomHair);
    }

    public void Accept()
    {
        PlayerDatabase.Instance.SetModel(currentModel.GetComponent<Agent>().model);
        PlayerDatabase.Instance.SetHair(currentModel.GetComponent<Agent>().hair);
        currentModel.GetComponent<Agent>().HappyAnim();
        GameManager.Instance.Vibrate();
        StartCoroutine(NextPhase());
    }

    public void Skip6()
    {
        for (int i = 0; i < instantRemoves.Length; i++)
        {
            instantRemoves[i].SetActive(false);
        }
        var newPaintable = Instantiate(Resources.Load<GameObject>
            ("Models/Paintable/Abigail_Hairstyle1_Outfit" + (2)));
        Phase6.Instance.StartPhase(newPaintable);
    }
    

    IEnumerator NextPhase()
    {
        for (int i = 0; i < instantRemoves.Length; i++)
        {
            instantRemoves[i].SetActive(false);
        }

        var curtainOne = Instantiate(Resources.Load<GameObject>("Curtains/CurtainOne"));
        curtainOne.transform.position = curtainSpawnPos[0].position;
        var curtainTwo = Instantiate(Resources.Load<GameObject>("Curtains/CurtainTwo"));
        curtainTwo.transform.position = curtainSpawnPos[1].position;
        
        var distanceOne = Vector3.Distance(curtainOne.transform.position, curtainMaxMovePos[0].transform.position);
        var distanceTwo = Vector3.Distance(curtainTwo.transform.position, curtainMaxMovePos[1].transform.position);
        var animCurtainSpeed = GameManager.Instance.animCurtainSpeedP0;
        
        yield return new WaitForSeconds(0.1f);
        models[models.Length - 1].GetComponent<Agent>().DestroyBubble();
        GameManager.Instance.PlayYaySound(true);
        var starLow = Instantiate(Resources.Load<GameObject>("Prefabs/starLow"));
        starLow.transform.position = starLowSpawnPos.position;
        starLow.transform.rotation = starLowSpawnPos.rotation;
        starLow.transform.localScale = starLowSpawnPos.localScale;
        
        while (distanceOne > 0.01f && distanceTwo > 0.01f)
        {
            distanceOne = Vector3.Distance(curtainOne.transform.position, curtainMaxMovePos[0].position);
            distanceTwo = Vector3.Distance(curtainTwo.transform.position, curtainMaxMovePos[1].position);
            curtainOne.transform.position = Vector3.MoveTowards(curtainOne.transform.position,
                curtainMaxMovePos[0].position, (distanceOne/animCurtainSpeed)*Time.deltaTime);
            curtainTwo.transform.position = Vector3.MoveTowards(curtainTwo.transform.position,
                curtainMaxMovePos[1].position, (distanceTwo/animCurtainSpeed)*Time.deltaTime);
            yield return null;
        }
        Destroy(starLow);

        for (int i = 0; i < FinalRemoves.Length; i++)
        {
            FinalRemoves[i].SetActive(false);
        }
        for (int i = 0; i < models.Length; i++)
        {
            Destroy(models[i].gameObject);
        }
        
        distanceOne = Vector3.Distance(curtainOne.transform.position, curtainRemovePos[0].transform.position);
        distanceTwo = Vector3.Distance(curtainTwo.transform.position, curtainRemovePos[1].transform.position);
        Phase1.Instance.InstantActive();
        
        while (distanceOne > 0.1f && distanceTwo > 0.1f)
        {
            distanceOne = Vector3.Distance(curtainOne.transform.position, curtainRemovePos[0].position);
            distanceTwo = Vector3.Distance(curtainTwo.transform.position, curtainRemovePos[1].position);
            curtainOne.transform.position = Vector3.MoveTowards(curtainOne.transform.position,
                curtainRemovePos[0].position, (distanceOne/animCurtainSpeed)*Time.deltaTime);
            curtainTwo.transform.position = Vector3.MoveTowards(curtainTwo.transform.position,
                curtainRemovePos[1].position, (distanceTwo/animCurtainSpeed)*Time.deltaTime);
            yield return null;
        }
        
        Phase1.Instance.StartPhase();
        Destroy(curtainOne);
        Destroy(curtainTwo);
    }
}
