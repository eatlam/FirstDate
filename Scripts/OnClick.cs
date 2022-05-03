using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class OnClick : MonoBehaviour
{
    public bool isTextureButton = false;
    public bool isColorButton = false;
    public bool isDressButton = false;
    private int color;

    public GameObject Choosen;
    public GameObject UnChoosen;
    public Image[] myTextures;

    public Button lockButton;

    public void AddTask (int _color)
    {
        color = _color;
        GetComponent<Button>().onClick.AddListener(TaskOnClick);
        lockButton.onClick.AddListener(BuyTaskOnClick);
    }

    void TaskOnClick()
    {
        if (isTextureButton)
        {
            GameManager.Instance.ChangeTexture(color);
        }
        else if (isColorButton)
        {
            GameManager.Instance.ChangeColor(color);
            GameManager.Instance.PlayButtonTouch();
        }
        else if (isDressButton)
        {
            GameManager.Instance.ChangeDress(color);
            Phase1.Instance.ChangeDress(color);
            GameManager.Instance.PlayButtonTouch();
        }
    }
    
    void BuyTaskOnClick()
    {
        if (isTextureButton)
        {
            GameManager.Instance.TryToBuyTexture(color);
        }
        else if (isColorButton)
        {
            GameManager.Instance.TryToBuySpray(color);
        }
        else if (isDressButton)
        {
            GameManager.Instance.TryToBuyDress(color);
        }
    }

    public void SetTextureOfMine(Sprite mySprite)
    {
        for (int i = 0; i < myTextures.Length; i++)
        {
            myTextures[i].sprite = mySprite;
        }
    }

    public Texture GetSprite()
    {
        return myTextures[0].sprite.texture;
    }

    public void Choose(bool isTrue)
    {
        if (isTrue)
        {
            Choosen.SetActive(true);
            UnChoosen.SetActive(false);
        }
        else
        {
            Choosen.SetActive(false);
            UnChoosen.SetActive(true);
        }
    }
}
