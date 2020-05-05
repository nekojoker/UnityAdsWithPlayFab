using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabController : MonoBehaviour
{
    public static string PlacementId { get; private set; } = "";
    public static string RewardId { get; private set; } = "";
    public static int? PlacementViewsRemaining { get; private set; } = null;
    public static double? PlacementViewsResetMinutes { get; private set; } = null;

    public delegate void RewardFinishedEvent();
    public static event RewardFinishedEvent OnRewardFinished;

    public static PlayFabController Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Call login
        PlayFabAuthService.Instance.Authenticate(Authtypes.Silent);
    }

    public void GetAdPlacements(string gameId, string placementName)
    {
        PlayFabClientAPI.GetAdPlacements(new GetAdPlacementsRequest { AppId = gameId }
        , result =>
        {
            var placement = result.AdPlacements.Find(x => x.PlacementName == placementName);
            PlacementId = placement.PlacementId;
            RewardId = placement.RewardId;
            PlacementViewsRemaining = placement.PlacementViewsRemaining;
            PlacementViewsResetMinutes = placement.PlacementViewsResetMinutes;

            Debug.Log("GetAdPlacements Success!!");
            Debug.Log("PlacementName:" + placement.PlacementName);
            Debug.Log("RewardName:" + placement.RewardName);
            Debug.Log("PlacementViewsRemaining:" + placement.PlacementViewsRemaining);
            Debug.Log("PlacementViewsResetMinutes:" + placement.PlacementViewsResetMinutes);
        }, error =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public void ReportAdActivity(AdActivity activity)
    {
        PlayFabClientAPI.ReportAdActivity(new ReportAdActivityRequest { PlacementId = PlacementId, RewardId = RewardId, Activity = activity }
        , result =>
        {
            if (activity == AdActivity.End)
            {
                RewardAdActivity();
            }
        }, error =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public void RewardAdActivity()
    {
        PlayFabClientAPI.RewardAdActivity(new RewardAdActivityRequest { PlacementId = PlacementId, RewardId = RewardId }
        , result =>
        {
            Debug.Log("GrantedVirtualCurrencies:" + result.RewardResults.GrantedVirtualCurrencies["GD"]);
            OnRewardFinished?.Invoke();
        }, error =>
        {
            if (error.Error == PlayFabErrorCode.AllAdPlacementViewsAlreadyConsumed)
            {
                Debug.Log("Run GetAdPlacements again.");
            }
            Debug.Log(error.GenerateErrorReport());
        });
    }
}
