using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; 
    [Header("Settings")] 
    public GameObject[] sounds;
    public GameObject[] vibrates;
    public GameObject SettingsPanel;
    public AudioSource _generalAudio;
    private bool popUpActive = false;
    public bool isSound = false;
    [Header("Money Panel")] 
    public GameObject moneyPanel;
    public Image moneyImage;
    public TextMeshProUGUI moneyText;
    [Header("End Panel")] 
    public GameObject endPanel;
    public Text totalMoneyText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        isSound = PlayerDatabase.Instance.GetSound();
        moneyText.text = PlayerDatabase.Instance.GetGold().ToString();
    }

    #region Money
    
    public void GoldIncreased(int amount)
    {
        var currentMoney = int.Parse(moneyText.text);
        currentMoney += amount;
        moneyText.text = currentMoney.ToString();
    }

    public void GoldChanged()
    {
        moneyText.text = PlayerDatabase.Instance.GetGold().ToString();
    }

    #endregion
    
    #region Settings
    
    public void ResetLevel()
    {
        PlayerPrefs.DeleteAll();
        var m_Scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(m_Scene.name);
    }
    
    public void SettingsPanelActive(bool isActive)
    {
        if (PlayerDatabase.Instance.GetSound())
            _generalAudio.volume = 1f;
        PlayPopUp(isActive);
        SettingsPanel.SetActive(isActive);
        if(isActive)
            SetSettingsUI();
    }

    public void SetSound(int sound)
    {
        PlayerDatabase.Instance.ChangeSound(sound);
        SetSettingsUI();
    }
    
    public void SetVibrate(int vibrate)
    {
        PlayerDatabase.Instance.ChangeVibrate(vibrate);
        SetSettingsUI();
    }

    private void SetSettingsUI()
    {
        if (PlayerDatabase.Instance.GetSound()) // Sound On
        {
            _generalAudio.volume = 1f;
            sounds[0].SetActive(true);
            sounds[1].SetActive(false);
        }
        else
        {
            _generalAudio.volume = 0f;
            sounds[0].SetActive(false);
            sounds[1].SetActive(true);
        }
        
        if (PlayerDatabase.Instance.CanVibrate()) // Cibrate On
        {
            vibrates[0].SetActive(true);
            vibrates[1].SetActive(false);
        }
        else
        {
            vibrates[0].SetActive(false);
            vibrates[1].SetActive(true);
        }

        isSound = PlayerDatabase.Instance.GetSound();
    }
    
    public void PlayPopUp(bool isActive)
    {
        if(isActive == popUpActive) return;
        popUpActive = isActive;
        if (isActive)
        {
            if (!isSound) return;
            FeedbackManager.Instance.PopUpOpen.PlayFeedbacks();
        }
        else
        {
            if (!isSound) return;
            FeedbackManager.Instance.PopUpClose.PlayFeedbacks();
        }
    }

    #endregion
}
