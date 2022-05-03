using System.Runtime.InteropServices;
using CW.Common;
using UnityEngine.EventSystems;
using PaintIn3D;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool isSkipTo6 = false;
    [Header("Phase 0 Model")]
    public GameObject model;
    public string[] modelNames = { "", "", "" };
    public int modelsCount = 3;
    public int outfitCount = 6;
    public int hairCount = 6;
    public int maxSpawnModel = 2;
    public float modelWalkSpeed = 4f;
    public bool isSameOutfit = false;
    public int[] paintableMaterialNumber = { 0, 0, 0, 0, 0, 0 };
    public int[] transparentableObjectNumber = { 0, 0, 0 };
    public string[] speechText = { "", "", "" };
    public string[] speechTextLast = { "", "", "" };
    public string[] manSpeechText = { "", "", "" };
    public int manModelsCount = 1;
    public int manOutfitCount = 6;
    public int manHairCount = 6;
    [HideInInspector]
    public int certainOutfit = 1;
    [HideInInspector]
    public int certainHair = 1;
    [HideInInspector]
    public int certainModel = 0;
    [Header("Phase 0 (Accept/Decline)")] 
    public float animCurtainSpeedP0 = 1f;
    [Header("Phase 1 (Outfit Choose)")] 
    public float animMoveSpeed = 0.1f;
    public int totalPaintables = 3;
    public float maxScaleOutfit = 5;
    public float scaleSpeed = 5f;
    public float animKillSpeed = 0.1f;
    [Header("Phase 2 (Bubbles)")] 
    public float finishRatio = 80;
    public float paintableFirstMovePosTime = 0.5f;
    public float paintableLastMovePosTime = 2f;
    public float pillowLastMovePosTime = 2f;
    public bool isBubble = true;
    [Header("Phase 3 (Cleaning pool)")] 
    public float paintableFirstMovePosTime3 = 1f;
    public float bathLastMovePosTime3 = 1f;
    public float rackFirstMovePosTime = 1f;
    public float rackLastMovePosTime = 1f;
    public Vector3 animMoveMax = new Vector3(0.1f, 0f, 0f);
    public int animCount = 6;
    public float animSpeed3 = 0.5f;
    [Header("Phase 4 (Color Paint)")] 
    public float paintableFirstMovePosTime4 = 1f;
    public float[] paletSize = { 0.2f, 0.4f, 0.6f };
    [Header("Phase 5 (Texture Paint)")] 
    public int defaultTextureCount = 6;
    public float expandX = 90f;
    public float expandWidth = 180f;
    [Header("Phase 6 (Final Date)")] 
    public int manModelCount = 1;
    public float manModelWalkSpeed = 4f;
    public float animCamSpeed1 = 1f;
    public float animCamSpeed2 = 1f;
    public float animCamSpeed3 = 1f;
    [Header("Prices")]
    public int[] dressPrices = { 0, 0, 0 };
    public int[] texturePrices = { 0, 0, 0 };
    public int[] sprayPrices = { 0, 0, 0 };
    [Header("Money Sender")]
    public int goldIncreaseCount = 10;
    public float sendSpeed = 15f;
    public float durationBetweenSend = 0.1f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        if (!isSkipTo6)
            Phase0.Instance.StartPhase();
        else
        {
            Phase0.Instance.Skip6();
        }

        Safe();
    }

    #region Money

    public void MoneyIncreased(int money)
    {
        UIManager.Instance.GoldIncreased(money);
    }

    #endregion

    #region ChangeStuff

    public void ChangeColor(GameObject paintable, Color color)
    {
        paintable.GetComponent<P3dPaintableTexture>().Replace(null, color);
    }
    
    public void ChangeColor(int color)
    {
        Vibrate();
        var colorButtons = Phase4.Instance.colorButtons;
        Phase4.Instance.spray.transform.GetChild(0).GetComponent<P3dPaintSphere>().Color =
            colorButtons[color].GetComponent<P3dColor>().Color;
        Phase4.Instance.spray.GetComponent<MeshRenderer>().materials[1].color = 
            Phase4.Instance.colorButtons[color].GetComponent<P3dColor>().Color;
        for (int i = 0; i < colorButtons.Length; i++)
        {
            colorButtons[i].transform.GetChild(2).gameObject.SetActive(false);
        }
        colorButtons[color].transform.GetChild(2).gameObject.SetActive(true);
        for (int i = 0; i < Phase4.Instance.colorButtons.Length; i++)
        {
            Phase4.Instance.colorButtons[i].transform.localScale = Vector3.one;
        }

        Phase4.Instance.colorButtons[color].transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
    }

    public void ChangePaletSize(int size)
    {
        Vibrate();
        Phase4.Instance.spray.transform.GetChild(0).GetComponent<P3dPaintSphere>().Scale =
            new Vector3(paletSize[size], paletSize[size], paletSize[size]);
        Phase4.Instance.ChangeSMLs(size);
    }
    
    public void ChangeTexture(int texture)
    {
        PlayTextureChange();
        Phase5.Instance.ChangeTexture(texture);
    }
    
    public void ChangeDress(int dress)
    {
        PlayTextureChange();
        Phase1.Instance.CorrectButtons(dress);
    }

    #endregion

    #region BuyThings

    public bool ShouldOpenDress(int dress)
    {
        if (dressPrices[dress] == 0 || PlayerDatabase.Instance.IsDressActive(dress))
            return true;
        else
        {
            return false;
        }
    }

    public void TryToBuyDress(int dress)
    {
        if (PlayerDatabase.Instance.GetGold() >= dressPrices[dress])
        {
            PlayerDatabase.Instance.SetGold(-dressPrices[dress]);
            PlayerDatabase.Instance.SetDressActive(dress);
            Phase1.Instance.dressButtons[dress].GetComponent<OnClick>().lockButton.gameObject.SetActive(false);
            UIManager.Instance.GoldChanged();
            ChangeDress(dress);
            Phase1.Instance.ChangeDress(dress);
            PlayCashOut();
        }
        else
        {
            FeedbackManager.Instance.LessMoneyError.PlayFeedbacks();
        }
    }
    
    public bool ShouldOpenTexture(int texture)
    {
        if (texturePrices[texture] == 0 || PlayerDatabase.Instance.IsTextureActive(texture))
            return true;
        else
        {
            return false;
        }
    }

    public void TryToBuyTexture(int texture)
    {
        if (PlayerDatabase.Instance.GetGold() >= texturePrices[texture])
        {
            PlayerDatabase.Instance.SetGold(-texturePrices[texture]);
            PlayerDatabase.Instance.SetTextureActive(texture);
            Phase5.Instance.textureButtons[texture].GetComponent<OnClick>().lockButton.gameObject.SetActive(false);
            UIManager.Instance.GoldChanged();
            ChangeTexture(texture);
            PlayCashOut();
        }
        else
        {
            FeedbackManager.Instance.LessMoneyError.PlayFeedbacks();
        }
    }
    
    public bool ShouldOpenSpray(int spray)
    {
        if (sprayPrices[spray] == 0 || PlayerDatabase.Instance.IsSprayActive(spray))
            return true;
        else
        {
            return false;
        }
    }

    public void TryToBuySpray(int spray)
    {
        if (PlayerDatabase.Instance.GetGold() >= sprayPrices[spray])
        {
            PlayerDatabase.Instance.SetGold(-sprayPrices[spray]);
            PlayerDatabase.Instance.SetSprayActive(spray);
            Phase4.Instance.colorButtons[spray].GetComponent<OnClick>().lockButton.gameObject.SetActive(false);
            UIManager.Instance.GoldChanged();
            ChangeColor(spray);
            PlayCashOut();
        }
        else
        {
            FeedbackManager.Instance.LessMoneyError.PlayFeedbacks();
        }
    }

    #endregion

    #region Settings

    public void PlaySpray(bool isPlay)
    {
        if(!UIManager.Instance.isSound) return;

        if (isPlay)
            FeedbackManager.Instance.Spray.PlayFeedbacks();
        else
        {
            FeedbackManager.Instance.Spray.StopFeedbacks();
        }
    }
    
    public void PlayClean(bool isPlay)
    {
        if(!UIManager.Instance.isSound) return;

        if (isPlay)
            FeedbackManager.Instance.Clean.PlayFeedbacks();
        else
            FeedbackManager.Instance.Clean.StopFeedbacks();
    }
    
    public void PlayWaterTouch()
    {
        if(PlayerDatabase.Instance.CanVibrate())
            Vibrate();
        if(!UIManager.Instance.isSound) return;

        FeedbackManager.Instance.WaterTouch.PlayFeedbacks();
    }
    
    public void PlayTextureChange()
    {
        if(PlayerDatabase.Instance.CanVibrate())
            Vibrate();
        if(!UIManager.Instance.isSound) return;

        FeedbackManager.Instance.TextureChange.PlayFeedbacks();
    }
    
    public void PlayHeelSound(bool isPlay)
    {
        if(!UIManager.Instance.isSound) return;

        if (isPlay)
            FeedbackManager.Instance.HeelSound.PlayFeedbacks();
        else
        {
            FeedbackManager.Instance.HeelSound.StopFeedbacks();
        }
    }
    public void PlayDisappointSound(bool isPlay)
    {
        if(!UIManager.Instance.isSound) return;

        if (isPlay)
            FeedbackManager.Instance.DisappointSound.PlayFeedbacks();
        else
        {
            FeedbackManager.Instance.DisappointSound.StopFeedbacks();
        }
    }
    
    public void PlayYaySound(bool isPlay)
    {
        if(!UIManager.Instance.isSound) return;

        if (isPlay)
            FeedbackManager.Instance.YaySound.PlayFeedbacks();
        else
        {
            FeedbackManager.Instance.YaySound.StopFeedbacks();
        }
    }
    
    
    public void PlayButtonTouch()
    {
        if(!UIManager.Instance.isSound) return;

        FeedbackManager.Instance.ButtonTouch.PlayFeedbacks();
    }
    
    public void PlayCompleteSound()
    {
        if(!UIManager.Instance.isSound) return;

        FeedbackManager.Instance.CompleteSound.PlayFeedbacks();
    }
    
    public void PlayWaterCleanSound(bool isPlay)
    {
        if(!UIManager.Instance.isSound) return;

        if (isPlay)
            FeedbackManager.Instance.WaterCleanSound.PlayFeedbacks();
        else
        {
            FeedbackManager.Instance.WaterCleanSound.StopFeedbacks();
        }
    }
    public void PlayFootstepSound(bool isPlay)
    {
        if(!UIManager.Instance.isSound) return;

        if (isPlay)
            FeedbackManager.Instance.FootstepSound.PlayFeedbacks();
        else
        {
            FeedbackManager.Instance.FootstepSound.StopFeedbacks();
        }
    }
    public void PlayManWowSound(bool isPlay)
    {
        if(!UIManager.Instance.isSound) return;

        if (isPlay)
            FeedbackManager.Instance.ManWowSound.PlayFeedbacks();
        else
        {
            FeedbackManager.Instance.ManWowSound.StopFeedbacks();
        }
    }
    
    public void PlayCashOut()
    {
        if(!UIManager.Instance.isSound) return;

        FeedbackManager.Instance.CashOut.PlayFeedbacks();
    }
    
    public void PlayMoneyCount()
    {
        if(!UIManager.Instance.isSound) return;

        FeedbackManager.Instance.MoneyCount.PlayFeedbacks();
    }

    public void Vibrate()
    {
        if (PlayerDatabase.Instance.CanVibrate())
            FeedbackManager.Instance.Vibrate.PlayFeedbacks();
    }
    
    public void VibrateHigher()
    {
        if (PlayerDatabase.Instance.CanVibrate())
            FeedbackManager.Instance.VibrateHigher.PlayFeedbacks();
    }
    
    public void VibrateContinuous(bool isActive)
    {
        if (!PlayerDatabase.Instance.CanVibrate()) return;
        if(isActive)
            FeedbackManager.Instance.VibrateContinuous.PlayFeedbacks();
        else
        {
            FeedbackManager.Instance.VibrateContinuous.StopFeedbacks();
        }
    }

    #endregion

    #region AddRemoveComponents

    
    public void AddPaintableComponents(GameObject _paintable)
    {
        _paintable.AddComponent<MeshCollider>();
        _paintable.AddComponent<P3dPaintable>();
        _paintable.AddComponent<P3dMaterialCloner>();
        _paintable.AddComponent<P3dPaintableTexture>();
        _paintable.AddComponent<P3dChangeCounter>();
        _paintable.AddComponent<Paintable>();
    }

    #endregion

    public void Replay()
    {
        SceneManager.LoadScene(0);
    }

    private void Safe()
    {
        if (dressPrices.Length != Dresses.Instance.dressPalets.Length)
        {
            var copyList = dressPrices;
            dressPrices = new int[Dresses.Instance.dressPalets.Length];
            for (int i = 0; i < dressPrices.Length; i++)
            {
                if (i <= copyList.Length)
                {
                    dressPrices[i] = copyList[i];
                }
                else
                {
                    dressPrices[i] = copyList[copyList.Length - 1];
                }
            }
        }
        if (texturePrices.Length != Textures.Instance.texturePalets.Length)
        {
            var copyList = texturePrices;
            texturePrices = new int[Textures.Instance.texturePalets.Length];
            for (int i = 0; i < texturePrices.Length; i++)
            {
                if (i <= copyList.Length)
                {
                    texturePrices[i] = copyList[i];
                }
                else
                {
                    texturePrices[i] = copyList[copyList.Length - 1];
                }
            }
        }
        if (sprayPrices.Length != Colors.Instance.colorPalets.Length)
        {
            var copyList = sprayPrices;
            sprayPrices = new int[Colors.Instance.colorPalets.Length];
            for (int i = 0; i < sprayPrices.Length; i++)
            {
                if (i <= copyList.Length)
                {
                    sprayPrices[i] = copyList[i];
                }
                else
                {
                    sprayPrices[i] = copyList[copyList.Length - 1];
                }
            }
        }
    }


}
