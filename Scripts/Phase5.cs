using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using PaintIn3D;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Phase5 : MonoBehaviour
{
    public static Phase5 Instance;
    public GameObject[] FinalRemoves;
    public GameObject[] instantRemoves;
    public GameObject[] instantActives;
    public Transform textureSpawnPos;
    public GameObject texturer;
    [Header("Paintable")]
    public GameObject paintable;
    public Transform paintableFirstMovePos;
    private int isFirst;
    private int currentColor = -1;
    [Header("UI")] 
    public GameObject texturePanel;
    public GameObject[] textureButtons;
    [Header("Particle")] 
    public Transform particleStarSpawnPos;
    public bool isParticle = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    #region Start

    private void InstantiateButtons()
    {
        var buttonCount = Textures.Instance.texturePalets.Length;
        var textures = Textures.Instance.texturePalets;
        var defaultCount = GameManager.Instance.defaultTextureCount;
        var expandX = GameManager.Instance.expandX * 2;
        var expandWidth = GameManager.Instance.expandWidth * 2;
        
        textureButtons = new GameObject[buttonCount];
        
        for (int i = 0; i < buttonCount; i++)
        {
            var insButton = Instantiate(Resources.Load<GameObject>("Prefabs/textureButton"));
            insButton.transform.SetParent(texturePanel.transform);
            //insButton.name = i.ToString();
            textureButtons[i] = insButton;
            if (i > defaultCount - 1)
            {
                var position = texturePanel.transform.position;
                position = new Vector3(position.x + expandX,
                    position.y, position.z);
                texturePanel.transform.position = position;
                var sizeDelta = texturePanel.GetComponent<RectTransform>().sizeDelta;
                texturePanel.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeDelta.x + expandWidth, sizeDelta.y);
            }
            insButton.GetComponent<OnClick>().AddTask(i);
            insButton.GetComponent<OnClick>().SetTextureOfMine(textures[i]);
            if (GameManager.Instance.ShouldOpenTexture(i))
            {
                insButton.GetComponent<OnClick>().lockButton.gameObject.SetActive(false);
            }
            else
            {
                insButton.GetComponent<OnClick>().lockButton.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                    GameManager.Instance.texturePrices[i].ToString();
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
        StartCoroutine(StartPhase5());
    }

    IEnumerator StartPhase5()
    {
        yield return new WaitForSeconds(0.5f);
        InstantActive();
        InstantiateButtons();
    }

    #endregion

    #region Others
    
    public void ChangeTexture(int color)
    {
        if(color == currentColor) return;
        currentColor = color;
        if (isFirst == 0)
        {
            Save();
        }
        else
        {
            ReDo();
            UnDo();
        }

        isFirst++;
            
        var decal = texturer.GetComponent<P3dPaintDecal>();
        decal.Texture = textureButtons[color].GetComponent<OnClick>().GetSprite();
        decal.HandleHitPoint(false,10,100,100,textureSpawnPos.position,Quaternion.identity);
        CorrectButtons(color);
    }

    public void Save()
    {
        var paintableScript = paintable.GetComponent<Paintable>();
        for (int i = 0; i < paintableScript.parts.Length; i++)
        {
            var paintexture = paintableScript.parts[i].GetComponent<P3dPaintableTexture>();
            paintexture.StoreState();
        }
    }

    public void UnDo()
    {
        var paintableScript = paintable.GetComponent<Paintable>();
        for (int i = 0; i < paintableScript.parts.Length; i++)
        {
            var paintexture = paintableScript.parts[i].GetComponent<P3dPaintableTexture>();
            paintexture.Undo();
        }
    }
    
    public void ReDo()
    {
        var paintableScript = paintable.GetComponent<Paintable>();
        for (int i = 0; i < paintableScript.parts.Length; i++)
        {
            var paintexture = paintableScript.parts[i].GetComponent<P3dPaintableTexture>();
            paintexture.Redo();
        }
    }

    public void CorrectButtons(int color)
    {
        for (int i = 0; i < textureButtons.Length; i++)
        {
            textureButtons[i].GetComponent<OnClick>().Choose(false);
            textureButtons[i].transform.localScale = Vector3.one;
        }
        textureButtons[color].GetComponent<OnClick>().Choose(true);
        textureButtons[color].transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
    }

    #endregion

    #region End

    public void Phase5Completed()
    {
        GameManager.Instance.VibrateContinuous(false);
        StartCoroutine(Phase5Ended());
    }

    IEnumerator Phase5Ended()
    {
        InstantRemoves();
        if (isParticle)
        {
            
            var particle = Instantiate(Resources.Load<GameObject>("Prefabs/particleStar"));
            particle.transform.position = particleStarSpawnPos.position;
            particle.transform.rotation = particleStarSpawnPos.rotation;
            particle.transform.localScale = particleStarSpawnPos.localScale;
            Destroy(particle, 1.5f);
        }
        yield return new WaitForSeconds(1f);
        Phase6.Instance.StartPhase(paintable);
        yield return new WaitForSeconds(0.5f);
    }

    #endregion

}
