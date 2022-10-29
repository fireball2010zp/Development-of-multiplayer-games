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

        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId)) // проверка, настроено ли свойство TitleId в настройках или нет
            PlayFabSettings.staticSettings.TitleId = _playFabTitle;

        var request = new LoginWithCustomIDRequest // запрос на создание нового юзера в PlayFab
        {
            CustomId = "TestUser",
            CreateAccount = true
        };

        _playFabLogIn.onClick.AddListener(() => LogInPlayFab(request));
        _photonConnect.onClick.AddListener(() => ClickOnPhoton());
    }

    private void LogInPlayFab(LoginWithCustomIDRequest request)
    {
        // запрос на обработку, если ли игрок в базе PlayFab
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

    // метод авторизации сцены в Photon
    private void Connect()
    {
        PhotonNetwork.AutomaticallySyncScene = true; // как только MasterClient вызывает загрузку следующей сцены методом PhotonNetwork.LoadLevel(), все подключенные к нему игроки автоматически делают то же самое. Таким образом все попадут в один игровой мир

        if (PhotonNetwork.IsConnected) // мы проверяем, подключены или нет, если подключены - присоединяемся, иначе инициируем подключение к серверу
        {
            PhotonNetwork.JoinRandomOrCreateRoom(roomName: $"Room N{Random.Range(0, 9999)}");
        }
        else
        {   //мы должны в первую очередь подключиться к Photon Online Server
            PhotonNetwork.ConnectUsingSettings(); // публичный статический метод класса PhotonNetwork, который обеспечивает соединение с сервером на основании тех настроек, которые заданы в PhotonServerSettings
            PhotonNetwork.GameVersion = PhotonNetwork.AppVersion; // чтобы игроки с разными версиями приложения не могли играть друг с другом, поскольку у них на клиенте может быть разный набор фичей
        }
    }

    // метод отключения от сцены в Photon
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
