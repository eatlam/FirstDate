using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDatabase : MonoBehaviour
{
    public static PlayerDatabase Instance;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    #region Player
    
    public int GetModel()
    {
        return PlayerPrefs.GetInt("current_model");
    }

    public void SetModel(int change)
    {
        PlayerPrefs.SetInt("current_model", change);
    }
    
    public int GetOutfit()
    {
        return PlayerPrefs.GetInt("current_outfit");
    }

    public void SetOutfit(int change)
    {
        PlayerPrefs.SetInt("current_outfit", change);
    }
    public int GetHair()
    {
        return PlayerPrefs.GetInt("current_hair");
    }

    public void SetHair(int change)
    {
        PlayerPrefs.SetInt("current_hair", change);
    }
    
    public int GetGold()
    {
        return PlayerPrefs.GetInt("current_gold_firstdate");
    }

    public void SetGold(int change)
    {
        PlayerPrefs.SetInt("current_gold_firstdate", PlayerPrefs.GetInt("current_gold_firstdate") + change);
    }
    
    public bool IsDressActive(int dress)
    {
        return PlayerPrefs.GetInt("model_dress_" + dress) == 1;
    }

    public void SetDressActive(int dress)
    {
        PlayerPrefs.SetInt("model_dress_" + dress, 1);
    }
    
    public bool IsTextureActive(int texture)
    {
        return PlayerPrefs.GetInt("model_texture_" + texture) == 1;
    }

    public void SetTextureActive(int texture)
    {
        PlayerPrefs.SetInt("model_texture_" + texture, 1);
    }
    
    public bool IsSprayActive(int spray)
    {
        return PlayerPrefs.GetInt("model_spray_" + spray) == 1;
    }

    public void SetSprayActive(int spray)
    {
        PlayerPrefs.SetInt("model_spray_" + spray, 1);
    }
    
    public int GetDay()
    {
        return PlayerPrefs.GetInt("current_day");
    }

    public void SetDay(int change)
    {
        PlayerPrefs.SetInt("current_day", PlayerPrefs.GetInt("current_day") +change);
    }

    #endregion

    #region Settings
    
    public bool CanVibrate()
    {
        return PlayerPrefs.GetInt("_vibrate_") == 0;
    }

    public void ChangeVibrate(int vibrate)
    {
        PlayerPrefs.SetInt("_vibrate_",vibrate);
    }
    
    public bool GetSound()
    {
        return PlayerPrefs.GetInt("_sound_") == 0;
    }

    public void ChangeSound(int sound)
    {
        PlayerPrefs.SetInt("_sound_",sound);
    }
    #endregion

}
