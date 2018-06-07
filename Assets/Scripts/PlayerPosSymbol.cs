using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PlayerPosSymbol : Photon.MonoBehaviour {

	[SerializeField]
	private PhotonView myView;

	public bool isOccupied = false;

	public int playerId;

	private void Awake()
	{
		if(myView == null)
		{
			myView = GetComponent<PhotonView>();
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.isWriting)
        {
            stream.SendNext(isOccupied);
			stream.SendNext(playerId);
        }
        else
        {
            this.isOccupied = (bool)stream.ReceiveNext();
			this.playerId = (int)stream.ReceiveNext();
        }
    }

	public void ChaneOwnerShip(PhotonPlayer nextPlayer)
	{
		myView.TransferOwnership(nextPlayer.ID);
	}	

	public void WriteActiveState(bool nextState)
	{
		if(myView.isMine)
		{
			this.isOccupied = nextState;			
		}
	}

	public void WritePlayerId(int playerId)
	{
		if(myView.isMine)
		{
			this.playerId = playerId;
		}
	}	
}
