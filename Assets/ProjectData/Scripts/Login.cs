using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;

public class Login : MonoBehaviourPunCallbacks//, IConnectionCallbacks, IInRoomCallbacks
{
    private const string PLAYFAB_TITLE = "D1D24";
    private const string GAME_VERSION = "dev";
    private const string AUTHENTIFICATION_KEY = "AUTHENTIFICATION_KEY";

    //private void Awake()
    //{
    //    PhotonNetwork.AddCallbackTarget(this);
    //}

    private struct Data
    {
        public bool needCreation;
        public string id;
    }

    void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
            PlayFabSettings.staticSettings.TitleId = PLAYFAB_TITLE;

        var needCreation = !PlayerPrefs.HasKey(AUTHENTIFICATION_KEY);
        var id = PlayerPrefs.GetString(AUTHENTIFICATION_KEY, Guid.NewGuid().ToString());
        var data = new Data { needCreation = needCreation, id = id };
        var request = new LoginWithCustomIDRequest
        {
            CustomId = id,
            CreateAccount = needCreation
        };
        PlayFabClientAPI.LoginWithCustomID(request, Success, Fail, data);
    }

    private void Success(LoginResult result)
    {
        PlayerPrefs.SetString(AUTHENTIFICATION_KEY, ((Data)result.CustomData).id);
        Debug.Log(result.PlayFabId);
        Debug.Log(((Data)result.CustomData).needCreation);
        Debug.Log(((Data)result.CustomData).id);
        Connect();

        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), SiccessInfo, Error);
    }

    private void Error(PlayFabError error)
    {
        Debug.LogError(error);
    }

    private void SiccessInfo(GetAccountInfoResult result)
    {
        Debug.Log(result.AccountInfo.PlayFabId);
    }

    private void Fail(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.LogError(errorMessage);
    }

    private void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomOrCreateRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = GAME_VERSION;
            PhotonNetwork.AutomaticallySyncScene = true;
        }
    }

    public override void OnConnected()
    {
        Debug.Log("OnConnected");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
        foreach (KeyValuePair<string, object> kvp in data)
            Debug.Log($"key: {kvp.Key}/value: {kvp.Value}");
    }

    public override void OnCustomAuthenticationFailed(string debugMessage)
    {
        Debug.Log(debugMessage);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName);
        Debug.Log(PhotonNetwork.CurrentRoom.Name);
        //throw new System.NotImplementedException();
    }
}
