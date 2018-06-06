using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PhotonManager : Photon.MonoBehaviour {

    static private PhotonManager _instance;
    static public PhotonManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<PhotonManager>();
            }
            return _instance;
        }
    }

    public RoomInfo[] rooms;
    public UnityAction leaveEvent;

    private GameObject myPlayer;

    private int playerNumOneFrameBefore = 0;

    public delegate void Messenger(string roomName);

    public Messenger foundVWorldRoom;

    public Messenger foundVRoomRoom;

    public Messenger foundError;

    public UnityAction joinedRoomCallback;

    private string currentRoomId;

    private RoomOptions currentRoomOptions;    

    // Use this for initialization
    void Awake()
    {
        var others = FindObjectsOfType<PhotonManager>();
        if(others.Length > 1)
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this);        
        PhotonNetwork.networkingPeer.QuickResendAttempts = 4;    
        PhotonNetwork.CrcCheckEnabled = true;
        PhotonNetwork.MaxResendsBeforeDisconnect = 10;              
    }

    private void Update()
    {
        if (!PhotonNetwork.connected)
        {
            return;
        }        

        if(!PhotonNetwork.inRoom || playerNumOneFrameBefore == PhotonNetwork.room.PlayerCount)
        {
            return;
        }

        if(playerNumOneFrameBefore > PhotonNetwork.room.PlayerCount && PhotonNetwork.isMasterClient)
        {            
            RemoveConFailedClient();
        }

        playerNumOneFrameBefore = PhotonNetwork.room.PlayerCount;

    }
    public void ConnectPhoton()
    {
        PhotonNetwork.ConnectUsingSettings("v1.0");
    }

    private void OnJoinedLobby()
    {
        Debug.Log("PhotonManager OnJoinedLobby");        
    }

    public void CreateVRoom()
    {        
        string userName = "YujiKonno";
        currentRoomId = "VRoom";

        ExitGames.Client.Photon.Hashtable customProp = new ExitGames.Client.Photon.Hashtable();
        customProp.Add("userName", userName); //ユーザ名
        customProp.Add("userId", currentRoomId); //ユーザID
        PhotonNetwork.SetPlayerCustomProperties(customProp);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.customRoomProperties = customProp;

        roomOptions.CustomRoomPropertiesForLobby = new string[] { "userName", "userId" };
        roomOptions.MaxPlayers = 10; //部屋の最大人数
        roomOptions.IsOpen = true; //入室許可する
        roomOptions.IsVisible = true; //ロビーから見えるようにする
    
        currentRoomOptions = roomOptions;
                
        SceneManager.activeSceneChanged += CreateRoom;
        SceneManager.LoadScene(GetRoomScene(currentRoomId));        
    }
    
    public void CreateVWorld()
    {        
        string userName = "YujiKonno";
        currentRoomId = "VWorld";
        ExitGames.Client.Photon.Hashtable customProp = new ExitGames.Client.Photon.Hashtable();
        customProp.Add("userName", userName); //ユーザ名
        customProp.Add("userId", currentRoomId); //ユーザID
        PhotonNetwork.SetPlayerCustomProperties(customProp);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.customRoomProperties = customProp;

        roomOptions.CustomRoomPropertiesForLobby = new string[] { "userName", "userId" };
        roomOptions.MaxPlayers = 10; //部屋の最大人数
        roomOptions.IsOpen = true; //入室許可する
        roomOptions.IsVisible = true; //ロビーから見えるようにする

        currentRoomOptions = roomOptions;

        SceneManager.activeSceneChanged += CreateRoom;
        SceneManager.LoadScene(GetRoomScene(currentRoomId));                                 
    }    

    private void OnJoinedRoom()
    {
        Debug.Log("PhotonManager OnJoinedRoom!");       
        if(joinedRoomCallback != null)
        {
            joinedRoomCallback();
        }        
    }
    
    public void InstantiateMyChar(string prefabName, Vector3 playerPos, UnityAction callback)
    {
        myPlayer = PhotonNetwork.Instantiate(prefabName, playerPos, Quaternion.identity, 0);
        var controller = myPlayer.GetComponent<BaseVCharacter>();
        if(controller != null)
        {
            controller.isMyPlayer = true;
        }

        if(callback != null)
        {
            callback();
        }
    }

    private void OnReceivedRoomListUpdate()
    {
        Debug.Log("Called OnReceivedRoomList");
        AwakeExistedRoom();    
    }

    private void AwakeExistedRoom()
    {
        //ルーム一覧を取る
        rooms= PhotonNetwork.GetRoomList();

        if (rooms.Length == 0)
        {
            Debug.Log("ルームが一つもありません");
        }
        else
        {
            //ルームが1件以上ある時ループでRoomInfo情報をログ出力
            for (int i = 0; i < rooms.Length; i++)
            {
                if(rooms[i].Name.Contains("VRoom"))
                {
                    foundVRoomRoom(rooms[i].Name);
                }
                else if(rooms[i].Name.Contains("VWorld"))
                {
                    foundVWorldRoom(rooms[i].Name);
                }                
            }            
        }
    }

    public void CallJoinRoom(string RoomId)
    {
        if(RoomId.Length == 0)
        {
            return;
        }

        currentRoomId = RoomId;
        SceneManager.activeSceneChanged += JoinRoom;
        SceneManager.LoadScene(GetRoomScene(currentRoomId));
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    private void OnLeftRoom()
    {
#if VR_COMMON        
        if (offset.activeSelf)
        {
            offset.SetActive(false);
        }
#endif
        SceneManager.LoadScene(GetLobbyScene());
    }

    private void OnPhotonJoinRoomFailed()
    {
        Debug.Log("PhotonManager: ルーム入室に失敗");
        SceneManager.LoadScene(GetLobbyScene());
    }

    private void OnPhotonCreateRoomFailed()
    {
        Debug.Log("PhotonManager: ルーム作成に失敗");
        SceneManager.LoadScene(GetLobbyScene());
    }

    private void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        Debug.LogErrorFormat("Connection failed To Photon; error code {0}", cause);
        
        if(foundError != null)
        {
            foundError(cause.ToString());
        }        
    }

    private void OnConnectionFail(DisconnectCause cause)
    {
        Debug.LogErrorFormat("Connection failed ; error code {0}", cause);
        Debug.LogErrorFormat("Recent command counter : {0}", PhotonNetwork.ResentReliableCommands.ToString());
        Debug.LogErrorFormat("PacketLossCountByCrc : {0}", PhotonNetwork.PacketLossByCrcCheck.ToString());
        Debug.LogErrorFormat("Ping ; {0}", PhotonNetwork.GetPing().ToString());

        playerNumOneFrameBefore = 0;

        if(foundError != null)
        {
            foundError(cause.ToString());
        }

        if (leaveEvent != null)
        {
            leaveEvent();
        }

#if VR_COMMON        
        if(offset.activeSelf)
        {
            offset.SetActive(false);
        }        
#endif
    }

    private void OnDisconnectedFromPhoton()
    {
        Debug.Log("Disconnected from Photon");
        SceneManager.LoadScene(GetLobbyScene());
    }

    private void RemoveConFailedClient()
    {
        var charControllers = FindObjectsOfType<BaseVCharacter>();

        if(charControllers == null || charControllers.Length == 0)
        {
            Debug.Log("No CharController");
            return;
        }

        for(int i = 0; i<charControllers.Length; i++)
        {
            var isMine = charControllers[i].GetComponent<PhotonView>().isMine;
            if(!charControllers[i].isMyPlayer && isMine)
            {
                PhotonNetwork.Destroy(charControllers[i].gameObject);
            }
        }
    }

    private string GetLobbyScene()
    {
        var activeSceneName = SceneManager.GetActiveScene().name;
        var index = activeSceneName.IndexOf("_");
        activeSceneName = activeSceneName.Substring(index, activeSceneName.Length - index);
        return "VLobby" + activeSceneName;
    }

    private string GetRoomScene(string currentRoomId)
    {
        var activeSceneName = SceneManager.GetActiveScene().name;
        var index = activeSceneName.IndexOf("_");
        activeSceneName = activeSceneName.Substring(index, activeSceneName.Length - index);
        return currentRoomId + activeSceneName;
    }

    public void DisconnectFromPhoton()
    {
        if(PhotonNetwork.connected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    private void CreateRoom(Scene prevScene, Scene nextScene)
    {
        if(!nextScene.name.Contains("Lobby"))
        {
            if(!PhotonNetwork.CreateRoom(currentRoomId, currentRoomOptions, null))
            {
                Debug.Log("Failed Create Room");
                SceneManager.activeSceneChanged -= CreateRoom;
                SceneManager.LoadScene(GetLobbyScene());
                return;
            }
            
            SceneManager.activeSceneChanged -= CreateRoom;
        }
    }
    private void JoinRoom(Scene prevScene, Scene nextScene)
    {
        if(!nextScene.name.Contains("Lobby"))
        {
            if(!PhotonNetwork.JoinRoom(currentRoomId))
            {
                Debug.Log("Failed to Join Room");
                SceneManager.activeSceneChanged -= JoinRoom;
                SceneManager.LoadScene(GetLobbyScene());
                return;
            }

            SceneManager.activeSceneChanged -= JoinRoom;
        }        
    }
    
/***************************Delete After developed VR Side*********************************************/
    // public void InstantiateMyChar(int charNum)
// {
//         Vector3 initialPosition = new Vector3(0.0f, 0.0f, 0.0f);
//         PlayerStyle myPlayerStyle;

//         switch (charNum)
//         {
//             case (int)PlayerStyle.Main:

//                 myPlayerStyle = PlayerStyle.Main;
//                 if(mainChar != null)
//                 {
//                     myPlayer = PhotonNetwork.Instantiate(mainChar.name, InitialPosManager.Instance.MyCharOffset(myPlayerStyle), mainChar.transform.rotation, 0);
//                     initialPosition = myPlayer.transform.position;
//                     myPlayer.GetComponent<MainCharController>().isMyPlayer = true;
//                 }               
//                 break;

//             case (int)PlayerStyle.Vip:

//                 myPlayerStyle = PlayerStyle.Vip;
//                 if(vipChar != null)
//                 {
//                     myPlayer = PhotonNetwork.Instantiate(vipChar.name, InitialPosManager.Instance.MyCharOffset(myPlayerStyle), vipChar.transform.rotation, 0);
//                     initialPosition = myPlayer.transform.position;
//                     myPlayer.GetComponent<MainCharController>().isMyPlayer = true;
//                 }        
//                 break;

//             case (int)PlayerStyle.Audience:

//                 myPlayerStyle = PlayerStyle.Audience;
//                 if(audienceChar != null)
//                 {
//                     myPlayer = PhotonNetwork.Instantiate(audienceChar.name, InitialPosManager.Instance.MyCharOffset(myPlayerStyle), audienceChar.transform.rotation, 0);
//                     initialPosition = myPlayer.transform.position;

//                 }
//                 break;
//         }

// #if VR_COMMON
//         Camera.main.transform.parent.transform.position = initialPosition + InitialPosManager.Instance.initialYAxisOffsetOfCamera;
// #else
//         Camera.main.transform.parent.transform.position = initialPosition;
// #endif          
//         playerSelection.SetActive(false);

// #if VR_COMMON
//         offset.SetActive(true);
// #endif
//    }
/***************************Delete After developed VR Side*********************************************/
}
