﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonStatus : MonoBehaviour {

    void OnGUI()
    {
        // Photonとの接続状況を表示する
        string status = "Photon: " + PhotonNetwork.connectionStateDetailed.ToString() + "\n";

        if (PhotonNetwork.inRoom)
        {
            status += "-------------------------------------------------------\n";
            status += "Room Name: " + PhotonNetwork.room.Name + "\n";
            status += "Player Num: " + PhotonNetwork.room.PlayerCount + "\n";
            status += "-------------------------------------------------------\n";
            status += "Player.Id: " + PhotonNetwork.player.ID + "\n";
            status += "IsMasterClient: " + PhotonNetwork.isMasterClient + "\n";
            status += "-------------------------------------------------------\n";
            status += "ResentReliableCmdsCounter : " + PhotonNetwork.ResentReliableCommands.ToString();
        }

        GUI.TextField(new Rect(10, 10, 220, 140), status);
    }
}
