using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabController : MonoBehaviour
{
    void Start()
    {
        // Call login
        PlayFabAuthService.Instance.Authenticate(Authtypes.Silent);
    }

    private void PlayFabAuthService_OnLoginSuccess(LoginResult success)
    {
        Debug.Log("Login Success!!");


    }

    private void OnEnable()
    {
        // Add login success event
        PlayFabAuthService.OnLoginSuccess += PlayFabAuthService_OnLoginSuccess;
    }

    private void OnDisable()
    {
        // Remove login success event
        PlayFabAuthService.OnLoginSuccess -= PlayFabAuthService_OnLoginSuccess;
    }
}
