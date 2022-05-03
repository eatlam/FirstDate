using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class FeedbackManager : MonoBehaviour
{
    public static FeedbackManager Instance;
    [SerializeField] private MMFeedbacks _Vibrate;
    [SerializeField] private MMFeedbacks _VibrateHigher;
    [SerializeField] private MMFeedbacks _VibrateContinuous;
    [SerializeField] private MMFeedbacks _PopUpOpen;
    [SerializeField] private MMFeedbacks _PopUpClose;
    [SerializeField] private MMFeedbacks _Spray;
    [SerializeField] private MMFeedbacks _Clean;
    [SerializeField] private MMFeedbacks _WaterTouch;
    [SerializeField] private MMFeedbacks _TextureChange;
    [SerializeField] private MMFeedbacks _HeelSound;
    [SerializeField] private MMFeedbacks _DisappointSound;
    [SerializeField] private MMFeedbacks _YaySound;
    [SerializeField] private MMFeedbacks _ButtonTouch;
    [SerializeField] private MMFeedbacks _CompleteSound;
    [SerializeField] private MMFeedbacks _WaterCleanSound;
    [SerializeField] private MMFeedbacks _FootstepSound;
    [SerializeField] private MMFeedbacks _ManWowSound;
    [SerializeField] private MMFeedbacks _LessMoneyError;
    [SerializeField] private MMFeedbacks _CashOut;
    [SerializeField] private MMFeedbacks _MoneyCount;

    public MMFeedbacks Vibrate => _Vibrate;
    public MMFeedbacks VibrateHigher => _VibrateHigher;
    public MMFeedbacks VibrateContinuous => _VibrateContinuous;
    public MMFeedbacks PopUpOpen => _PopUpOpen;
    public MMFeedbacks PopUpClose => _PopUpClose;
    public MMFeedbacks Spray => _Spray;
    public MMFeedbacks Clean => _Clean;
    public MMFeedbacks WaterTouch => _WaterTouch;
    public MMFeedbacks TextureChange => _TextureChange;
    public MMFeedbacks HeelSound => _HeelSound;
    public MMFeedbacks DisappointSound => _DisappointSound;
    public MMFeedbacks YaySound => _YaySound;
    public MMFeedbacks ButtonTouch => _ButtonTouch;
    public MMFeedbacks CompleteSound => _CompleteSound;
    public MMFeedbacks WaterCleanSound => _WaterCleanSound;
    public MMFeedbacks FootstepSound => _FootstepSound;
    public MMFeedbacks ManWowSound => _ManWowSound;
    public MMFeedbacks LessMoneyError => _LessMoneyError;
    public MMFeedbacks CashOut => _CashOut;
    public MMFeedbacks MoneyCount => _MoneyCount;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
}