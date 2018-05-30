using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonStatus : MonoBehaviour {

    void OnGUI()
    {
        string status = "";

        status += "Ping : " + PhotonNetwork.GetPing() + "\n";

        status += "-------------------------------------------------------\n";

        if (PhotonNetwork.inRoom)
        {
            status += "Room Name: " + PhotonNetwork.room.Name + "\n";
            status += "Player Num: " + PhotonNetwork.room.PlayerCount + "\n";
            status += "-------------------------------------------------------\n";
            status += "Player.Id: " + PhotonNetwork.player.ID + "\n";
            status += "IsMasterClient: " + PhotonNetwork.isMasterClient + "\n";
            status += "-------------------------------------------------------\n";
            status += "PacketLossByCrcCheck : " + PhotonNetwork.PacketLossByCrcCheck.ToString();            
        }

        GUI.TextField(new Rect(10, 10, 220, 150), status);
    }
}
