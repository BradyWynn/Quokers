using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
public class SpawnPlayers : MonoBehaviour
{
    public GameObject ctPrefab;
    GameObject instanobjCT;
    public PhotonView view;
    public void Awake() {
        view = GetComponent<PhotonView>();
        InstantiatePlayer();
    }
    public void InstantiatePlayer()
    {
        bool team;

        // this just gets the number of players in a room and does mod two to deterministically assign teams
        if(PhotonNetwork.CurrentRoom.PlayerCount % 2 == 0){
            team = true;
        }
        else{
            team = false;
        }

        instanobjCT = PhotonNetwork.Instantiate(ctPrefab.name, new Vector3(0, 0, 0), Quaternion.identity);

        PhotonView view2 = instanobjCT.GetComponent<PhotonView>();
        int num = view2.ViewID;
        string name = instanobjCT.name;

        object[] content = new object[] { name, num, team }; // obj array contains the target position and the IDs of the selected units
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(1, content, raiseEventOptions, SendOptions.SendReliable);
    }
}
