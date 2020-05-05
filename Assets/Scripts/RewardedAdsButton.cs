using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using PlayFab.ClientModels;

[RequireComponent(typeof(Button))]
public class RewardedAdsButton : MonoBehaviour, IUnityAdsListener
{

#if UNITY_IOS
    private string gameId = "1486550";
#elif UNITY_ANDROID
    private string gameId = "1486550";
#endif

    [SerializeField] Button reloadButton;
    Button myButton;
    Text buttonText;
    string myPlacementId = "rewardedVideo";
    string placementName = "TestReward2";

    void Start()
    {
        myButton = GetComponent<Button>();
        buttonText = GetComponentInChildren<Text>();

        // Map the ShowRewardedVideo function to the button’s click listener:
        if (myButton) myButton.onClick.AddListener(ShowRewardedVideo);
        if (reloadButton) reloadButton.onClick.AddListener(PlayFabController_OnRewardFinished);

        // Initialize the Ads listener and service:
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, true);

    }

    private void Update()
    {
        // Set interactivity to be dependent on the Placement’s status:

        if (PlayFabController.PlacementViewsRemaining == null
            || PlayFabController.PlacementViewsResetMinutes == null
            || PlayFabController.PlacementViewsRemaining > 0
            || PlayFabController.PlacementViewsResetMinutes <= 0)
        {
            buttonText.text = "Get Reward!!";
            myButton.interactable = Advertisement.IsReady(myPlacementId);

        }
        else
        {
            buttonText.text = string.Format("Next : {0} minutes", PlayFabController.PlacementViewsResetMinutes.ToString());
            myButton.interactable = false;
        }
    }

    // Implement a function for showing a rewarded video ad:
    void ShowRewardedVideo()
    {
        Advertisement.Show(myPlacementId);
    }

    // Implement IUnityAdsListener interface methods:
    public void OnUnityAdsReady(string placementId)
    {
        // If the ready Placement is rewarded, activate the button: 
        if (placementId == myPlacementId)
        {
            myButton.interactable = true;
        }
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished)
        {
            Debug.Log("Reward Finish!!");
            // Reward the user for watching the ad to completion.
            PlayFabController.Instance.ReportAdActivity(AdActivity.End);
        }
        else if (showResult == ShowResult.Skipped)
        {
            Debug.Log("Reward Skipped...");
            // Do not reward the user for skipping the ad.
            PlayFabController.Instance.ReportAdActivity(AdActivity.Closed);
        }
        else if (showResult == ShowResult.Failed)
        {
            Debug.Log("Reward Failed...");
            PlayFabController.Instance.ReportAdActivity(AdActivity.Closed);
        }
    }

    public void OnUnityAdsDidError(string message)
    {
        // Log the error.
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        Debug.Log("Reward Start!!");
        // Optional actions to take when the end-users triggers an ad.
        PlayFabController.Instance.ReportAdActivity(AdActivity.Start);
    }

    private void PlayFabAuthService_OnLoginSuccess(LoginResult success)
    {
        Debug.Log("Login Success!!");
        PlayFabController.Instance.GetAdPlacements(gameId, placementName);
    }

    private void PlayFabController_OnRewardFinished()
    {
        // Get the latest placement every time you reward
        PlayFabController.Instance.GetAdPlacements(gameId, placementName);
    }

    private void OnEnable()
    {
        // Add login success event
        PlayFabAuthService.OnLoginSuccess += PlayFabAuthService_OnLoginSuccess;
        PlayFabController.OnRewardFinished += PlayFabController_OnRewardFinished;
    }

    private void OnDisable()
    {
        // Remove login success event
        PlayFabAuthService.OnLoginSuccess -= PlayFabAuthService_OnLoginSuccess;
        PlayFabController.OnRewardFinished -= PlayFabController_OnRewardFinished;
    }
}