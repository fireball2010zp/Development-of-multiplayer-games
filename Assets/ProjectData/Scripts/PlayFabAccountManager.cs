using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class PlayFabAccountManager : MonoBehaviour
{
    [SerializeField] private Text _titleLabel;
    private void Start()
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(),
        OnGetAccountSuccess, OnFailure);
    }
    private void OnGetAccountSuccess(GetAccountInfoResult result)
    {
        _titleLabel.text = $"Welcome back, Player ID {result.AccountInfo.PlayFabId}\n\n" +
            $"Name: {result.AccountInfo.Username ?? ""}\n" +
            $"Email: {result.AccountInfo.PrivateInfo.Email ?? ""}\n" +
            $"Last login: {result.AccountInfo.TitleInfo.LastLogin.ToString() ?? ""}\n";
    }
    private void OnFailure(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.LogError($"Something went wrong: {errorMessage}");
        _titleLabel.text = $"Something went wrong: {errorMessage}";
    }
}

