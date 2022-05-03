using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class ManAgent : MonoBehaviour
{
    #region ManAgent

    public Transform targetPos;
    public float speed;
    public Animator animator;
    public GameObject speechBubble;
    
    public void StartMoving(Transform _targetPos)
    {
        targetPos = _targetPos;
        animator = GetComponent<Animator>();
        transform.LookAt(targetPos);
        AnimRun(true);
        GameManager.Instance.PlayFootstepSound(true);
        StartCoroutine(StartedMoving());
    }

    public IEnumerator StartedMoving()
    {
        var targetPos = Phase6.Instance.manMovePosFirst;
        var distanceOne = Vector3.Distance(transform.position, targetPos.position);
        var distanceOneR = Quaternion.Angle(transform.rotation, targetPos.rotation);

        var animCamSpeed1 = GameManager.Instance.manModelWalkSpeed;

        while (distanceOne > 0.25f)
        {
            distanceOne = Vector3.Distance(transform.position, targetPos.position);
            distanceOneR = Quaternion.Angle(transform.rotation, targetPos.transform.rotation);
            
            transform.position = Vector3.MoveTowards(transform.position,
                targetPos.position, (distanceOne/animCamSpeed1)*Time.deltaTime);
            
            transform.rotation = Quaternion.Lerp(transform.rotation,
                targetPos.transform.rotation,(distanceOneR/animCamSpeed1)*Time.deltaTime);

            yield return null;
        }
        
        GameManager.Instance.PlayFootstepSound(false);
        MainReachedDestination();
        yield return new WaitForSeconds(0.25f);
        GameManager.Instance.PlayManWowSound(true);
    }
    
    private void MainReachedDestination()
    {
        Phase6.Instance.MainReachedDestination();
        var mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        speechBubble = Instantiate(Resources.Load<GameObject>("Prefabs/speechBubble"));
        var bubblePos = Phase6.Instance.manBubbleSpawnPos;
        speechBubble.transform.position = bubblePos.transform.position;
        speechBubble.transform.rotation = bubblePos.transform.rotation;
        speechBubble.transform.localScale = bubblePos.transform.localScale;
        speechBubble.transform.LookAt(mainCam.transform);
        speechBubble.transform.rotation = Quaternion.LookRotation(mainCam.transform.forward);
        speechBubble.transform.GetChild(0).GetComponent<TextMeshPro>().text = randomText();
        speechBubble.transform.GetChild(0).GetComponent<TextMeshPro>().fontSizeMax = 72;
        AnimRun(false);
        AnimHappy();
    }

    private string randomText()
    {
        var i = Random.Range(0, GameManager.Instance.manSpeechText.Length);
        return GameManager.Instance.manSpeechText[i];
    }
    
    public void AnimRun(bool isTrue)
    {
        animator.SetBool("Run", isTrue);
    }
    
    public void AnimHappy()
    {
        animator.SetBool("Happy", true);
    }


    #endregion

}
