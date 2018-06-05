using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PhotonConnecter : Photon.MonoBehaviour {
    public Button ConPhotonButton;

    public Button CreateVWorldButton;

    public Button CreateVRoomVButton;

    public bool isDebuged = false;

    public Text Error;

    public string VWorldSceneName;

    public string VRoomSceneName;

    private string myEnv = "";

   void Awake()
    {
        ConPhotonButton.onClick.AddListener(ConnectPhoton);

        ConPhotonButton.onClick.AddListener(ConnectPhoton);
        CreateVWorldButton.onClick.AddListener(CreateAndJoinVWorld);
        CreateVRoomVButton.onClick.AddListener(CreateAndJoinVRoom);        
    }

    private void ConnectPhoton()
    {
        PhotonNetwork.ConnectUsingSettings("v1.0");
    }

    private void OnJoinedLobby()
    {
        Debug.Log("PhotonManager OnJoinedLobby");
        ConPhotonButton.interactable = false;
        CreateVWorldButton.interactable = true;
        CreateVRoomVButton.interactable = true;
    }

    public void CreateAndJoinVWorld()
    {        
       string userName = "VWorld";
        string userId = "001";
        ExitGames.Client.Photon.Hashtable customProp = new ExitGames.Client.Photon.Hashtable();
        customProp.Add("userName", userName); //ユーザ名
        customProp.Add("userId", userId); //ユーザID
        PhotonNetwork.SetPlayerCustomProperties(customProp);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.customRoomProperties = customProp;

        roomOptions.CustomRoomPropertiesForLobby = new string[] { "userName", "userId" };
        roomOptions.MaxPlayers = 10; //部屋の最大人数
        roomOptions.IsOpen = true; //入室許可する
        roomOptions.IsVisible = true; //ロビーから見えるようにする

        myEnv = "VWorld";
        PhotonNetwork.JoinOrCreateRoom(userId, roomOptions, null);
    }

    public void CreateAndJoinVRoom()
    {
        string userName = "VRoom";
        string userId = "002";
        ExitGames.Client.Photon.Hashtable customProp = new ExitGames.Client.Photon.Hashtable();
        customProp.Add("userName", userName); //ユーザ名
        customProp.Add("userId", userId); //ユーザID
        PhotonNetwork.SetPlayerCustomProperties(customProp);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.customRoomProperties = customProp;

        roomOptions.CustomRoomPropertiesForLobby = new string[] { "userName", "userId" };
        roomOptions.MaxPlayers = 10; //部屋の最大人数
        roomOptions.IsOpen = true; //入室許可する
        roomOptions.IsVisible = true; //ロビーから見えるようにする

        myEnv = "VRoom";
        PhotonNetwork.JoinOrCreateRoom(userId, roomOptions, null);        
    }
    
    private void OnJoinedRoom()
    {
        Debug.Log("PhotonManager OnJoinedRoom!");       
        SceneManager.LoadScene(GetNextScene(myEnv)); 
    }

    private void OnConnectionFail(DisconnectCause cause)
    {
        Debug.LogErrorFormat("Connection failed ; error code {0}", cause);
        Debug.LogErrorFormat("Recent command counter : {0}", PhotonNetwork.ResentReliableCommands.ToString());
        Debug.LogErrorFormat("PacketLossCountByCrc : {0}", PhotonNetwork.PacketLossByCrcCheck.ToString());
        Debug.LogErrorFormat("Ping ; {0}", PhotonNetwork.GetPing().ToString());
        Error.text = string.Format("Err: {0}", cause);

        CreateVWorldButton.interactable = false;
        CreateVRoomVButton.interactable = false;
        ConPhotonButton.interactable = true;
    }

    private string GetNextScene(string myEnv)
    {
        var activeSceneName = SceneManager.GetActiveScene().name;
        var index = activeSceneName.IndexOf("_");
        activeSceneName = activeSceneName.Substring(index, activeSceneName.Length - index);
        return myEnv + activeSceneName;
    }
}

		