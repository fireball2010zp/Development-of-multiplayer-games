using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using UnityEngine;

public class Authorization : MonoBehaviourPunCallbacks
{
    [SerializeField] private string _playFabTitle;
    
    [SerializeField] private Button _playFabLogIn;
    [SerializeField] private Text _playFabConnectInfo;

    [SerializeField] private Button _photonConnect;
    [SerializeField] private Text _photonConnectInfo;
    [SerializeField] private Text _photonConnectTextOnButton;

    void Start()
    {
        _photonConnect.enabled = false;

        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId)) // ��������, ��������� �� �������� TitleId � ���������� ��� ���
            PlayFabSettings.staticSettings.TitleId = _playFabTitle;

        var request = new LoginWithCustomIDRequest // ������ �� �������� ������ ����� � PlayFab
        {
            CustomId = "TestUser",
            CreateAccount = true
        };

        _playFabLogIn.onClick.AddListener(() => LogInPlayFab(request));
        _photonConnect.onClick.AddListener(() => ClickOnPhoton());
    }

    private void LogInPlayFab(LoginWithCustomIDRequest request)
    {
        // ������ �� ���������, ���� �� ����� � ���� PlayFab
        PlayFabClientAPI.LoginWithCustomID(request,
            result =>
            {
                Debug.Log(result.PlayFabId);
                PhotonNetwork.AuthValues = new AuthenticationValues(result.PlayFabId);
                PhotonNetwork.NickName = result.PlayFabId;

                _playFabConnectInfo.text = "Playfab is connected!";
                _playFabConnectInfo.color = Color.green;
                _playFabLogIn.enabled = false;
                _photonConnect.enabled = true;
            },
            error =>
            {   
                Debug.LogError(error);
                _playFabConnectInfo.text = "Playfab isn't connected!";
                _playFabConnectInfo.color = Color.red;
            });

    }

    private void ClickOnPhoton()
    {
        if (PhotonNetwork.IsConnected)
        {
            Disconnect();
            _photonConnectInfo.text = "Photon is disconnected!";
            _photonConnectInfo.color = Color.red;
            _photonConnectTextOnButton.text = "Connect to Photon";
        }
        else
        {
            Connect();
            _photonConnectInfo.text = "Photon is connected!";
            _photonConnectInfo.color = Color.green;
            _photonConnectTextOnButton.text = "Disconnect Photon";
        }
    }

    // ����� ����������� ����� � Photon
    private void Connect()
    {
        PhotonNetwork.AutomaticallySyncScene = true; // ��� ������ MasterClient �������� �������� ��������� ����� ������� PhotonNetwork.LoadLevel(), ��� ������������ � ���� ������ ������������� ������ �� �� �����. ����� ������� ��� ������� � ���� ������� ���

        if (PhotonNetwork.IsConnected) // �� ���������, ���������� ��� ���, ���� ���������� - ��������������, ����� ���������� ����������� � �������
        {
            PhotonNetwork.JoinRandomOrCreateRoom(roomName: $"Room N{Random.Range(0, 9999)}");
        }
        else
        {   //�� ������ � ������ ������� ������������ � Photon Online Server
            PhotonNetwork.ConnectUsingSettings(); // ��������� ����������� ����� ������ PhotonNetwork, ������� ������������ ���������� � �������� �� ��������� ��� ��������, ������� ������ � PhotonServerSettings
            PhotonNetwork.GameVersion = PhotonNetwork.AppVersion; // ����� ������ � ������� �������� ���������� �� ����� ������ ���� � ������, ��������� � ��� �� ������� ����� ���� ������ ����� �����
        }
    }

    // ����� ���������� �� ����� � Photon
    private void Disconnect()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        PhotonNetwork.Disconnect();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("OnConnectedToMaster");
        if (!PhotonNetwork.InRoom)
            PhotonNetwork.JoinRandomOrCreateRoom(roomName: $"Room N{Random.Range(0, 9999)}");
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("OnCreatedRoom");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log($"OnJoinedRoom {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Debug.Log("OnLeftRoom");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("OnDisconnected");
    }
}
