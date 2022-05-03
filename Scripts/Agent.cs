using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Agent : MonoBehaviour
{
    #region Agent

    public Transform targetPos;
    public float speed;
    public bool isMain;
    public bool isStarted = false;
    public NavMeshAgent navMeshAgent;
    public Animator animator;
    public bool isKill = false;
    public int model = -1;
    public int outfit = -1;
    public int hair = -1;
    private Transform lookRot;
    public GameObject speechBubble;
    
    public void StartMoving(Transform _targetPos, float _speed, bool _isMain
        , bool _isKill, int _model, int _outfit, int _hair)
    {
        if (_model != -1 && _outfit != -1 && _hair != -1)
        {
            model = _model;
            outfit = _outfit;
            hair = _hair;
            SetFinalModelDetails();
        }
        navMeshAgent = GetComponent<NavMeshAgent>();
        targetPos = _targetPos;
        speed = _speed;
        isMain = _isMain;
        isKill = _isKill;
        navMeshAgent.speed = speed;
        SetAnimator(true);
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(targetPos.position);
        StartCoroutine(IsStarted());
    }

    public void PrepareSpawnedOnes(Transform _lookRot)
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        lookRot = _lookRot;
        transform.LookAt(lookRot);
        navMeshAgent.updateUpAxis = false;
        navMeshAgent.radius = 0.1f;
        navMeshAgent.height = 1.25f;
    }

    public void SetFinalModelDetails()
    {
        SetTransparentableObject();
        SetPaintableMat();
    }

    IEnumerator IsStarted()
    {
        yield return new WaitForSeconds(0.5f);
        isStarted = true;
    }
    
    private void Update()
    {
        if(!isStarted) return;
        if (!isKill)
        {
            navMeshAgent.updateUpAxis = false;
            transform.LookAt(lookRot.transform);
        }

        if (navMeshAgent.remainingDistance < 0.75f)
        {
            isStarted = false;
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
            AnimRun(false);
            if (isKill)
            {
                Destroy(gameObject);
            }
            else if (isMain)
            {
                MainReachedDestination();
            }
        }
    }

    public void SadAnim()
    {
        animator.SetBool("Upset", true);
        DestroyBubble();
        StartCoroutine(disActiveUpset());
    }

    public void DestroyBubble()
    {
        Destroy(speechBubble);
    }

    IEnumerator disActiveUpset()
    {
        yield return new WaitForSeconds(1f);
        animator.SetBool("Upset",false);
    }
    public void HappyAnim()
    {
        animator.SetBool("Happy", true);
        StartCoroutine(disActiveHappy());
    }
    
    IEnumerator disActiveHappy()
    {
        yield return new WaitForSeconds(1f);
        animator.SetBool("Happy",false);
    }
    
    public void ShyAnim(bool isTrue)
    {
        animator.SetBool("Shy", isTrue);
    }

    public void SetAnimator(bool isRun)
    {
        animator = GetComponent<Animator>();
        AnimRun(isRun);
    }

    public void CloseAnimator()
    {
        animator = GetComponent<Animator>();
        animator.enabled = false;
    }

    private void MainReachedDestination()
    {
        Phase0.Instance.MainReachedDestination();
        var mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        speechBubble = Instantiate(Resources.Load<GameObject>("Prefabs/speechBubble"));
        var bubblePos = Phase0.Instance.bubblePos;
        speechBubble.transform.position = bubblePos.transform.position;
        /*var speechText = speechBubble.transform.GetChild(0).GetComponent<RectTransform>().position;
        speechBubble.transform.GetChild(0).GetComponent<RectTransform>().position = new Vector3(speechText.x,
            0.96f, speechText.z);*/
        speechBubble.transform.rotation = bubblePos.transform.rotation;
        speechBubble.transform.localScale = bubblePos.transform.localScale;
        speechBubble.transform.LookAt(mainCam.transform);
        speechBubble.transform.rotation = Quaternion.LookRotation(mainCam.transform.forward);
        speechBubble.transform.GetChild(0).GetComponent<TextMeshPro>().text = randomText();
    }

    private string randomText()
    {
        var i = Random.Range(0, GameManager.Instance.speechText.Length);
        return GameManager.Instance.speechText[i];
    }

    public void AnimRun(bool isTrue)
    {
        animator = GetComponent<Animator>();
        animator.enabled = true;
        animator.SetBool("Run", isTrue);
    }
    
    public void AnimDance(bool isTrue)
    {
        animator = GetComponent<Animator>();
        animator.enabled = true;
        animator.SetBool("Dance", isTrue);
    }

    public void SpawnLastBubble()
    {
        var mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        speechBubble = Instantiate(Resources.Load<GameObject>("Prefabs/speechBubble"));
        var bubblePos = Phase6.Instance.modelBubbleSpawnPos;
        speechBubble.transform.position = bubblePos.transform.position;
        speechBubble.transform.rotation = bubblePos.transform.rotation;
        speechBubble.transform.localScale = bubblePos.transform.localScale;
        speechBubble.transform.LookAt(mainCam.transform);
        speechBubble.transform.rotation = Quaternion.LookRotation(mainCam.transform.forward);
        speechBubble.transform.GetChild(0).GetComponent<TextMeshPro>().text = randomTextLast();
        speechBubble.transform.GetChild(0).GetComponent<TextMeshPro>().fontSizeMax = 72;
    }

    private string randomTextLast()
    {
        var i = Random.Range(0, GameManager.Instance.speechTextLast.Length);
        return GameManager.Instance.speechTextLast[i];
    }

    #endregion

    #region Paintable

    public int PaintableMaterial;
    public GameObject TransparentableObject;

    public void SetPaintableMat()
    {
        var outfitFinal = PlayerDatabase.Instance.GetOutfit();
        PaintableMaterial = GameManager.Instance.paintableMaterialNumber[outfitFinal - 1];
    }

    public void SetTransparentableObject()
    {
        var modelFinal = PlayerDatabase.Instance.GetModel();
        TransparentableObject = transform.GetChild(GameManager.Instance.transparentableObjectNumber[modelFinal]).gameObject;
    }
    
    public Material GetMatAgent(int number)
    {
        var meshOfTrObj = TransparentableObject.GetComponent<SkinnedMeshRenderer>();
        return meshOfTrObj.materials[number];
    }

    #endregion

}
