using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VRoomPhoton : MonoBehaviour {
    static private VRoomPhoton _instance;
    static public VRoomPhoton Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<VRoomPhoton>();
            }
            return _instance;
        }
    }

    public Button LeaveButton;

	private void Awake()
	{
		LeaveButton.onClick.AddListener(LeaveRoom);
	}
    private void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    private void OnLeftRoom()
    {
        LeaveButton.interactable = false;
        SceneManager.LoadScene(GetLobbyScene());
    }


    private void OnConnectionFail(DisconnectCause cause)
    {
        Debug.LogErrorFormat("Connection failed ; error code {0}", cause);
        Debug.LogErrorFormat("Recent command counter : {0}", PhotonNetwork.ResentReliableCommands.ToString());
        Debug.LogErrorFormat("PacketLossCountByCrc : {0}", PhotonNetwork.PacketLossByCrcCheck.ToString());
        Debug.LogErrorFormat("Ping ; {0}", PhotonNetwork.GetPing().ToString());
    }

    private void OnDisconnectedFromPhoton()
    {
        Debug.Log("Disconnected from Photon");
        SceneManager.LoadScene(GetLobbyScene());
    }


	private string GetLobbyScene()
    {
        var activeSceneName = SceneManager.GetActiveScene().name;
        var index = activeSceneName.IndexOf("_");
        activeSceneName = activeSceneName.Substring(index, activeSceneName.Length - index);
        return "VLobby" + activeSceneName;
    }
}
