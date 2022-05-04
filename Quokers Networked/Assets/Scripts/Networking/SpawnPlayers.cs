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
    public GameObject roundlogicPrefab;
    public PhotonView view;
    public byte MoveUnitsToTargetPositionEventCode = 1;
    // public delegate void SpawnDelegate(string c);
    // public event SpawnDelegate spawnEvent;
    public void Awake() {
        view = GetComponent<PhotonView>();
        // if(PhotonNetwork.IsMasterClient == true)
        //     Instantiate(roundlogicPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        InstantiatePlayer();
    }
    public void InstantiatePlayer()
    {
        bool team;
        if(PhotonNetwork.CurrentRoom.PlayerCount % 2 == 0){
            team = true;
        }
        else{
            team = false;
        }

        instanobjCT = PhotonNetwork.Instantiate(ctPrefab.name, new Vector3(0, 0, 0), Quaternion.identity); // replacement for the code below so we don't need two prefabs

        // if(team == true)
        //     instanobjCT = PhotonNetwork.Instantiate(ctPrefab.name, new Vector3(0, 0, 0), Quaternion.identity);
        // if(team == false)
        //     instanobjCT = PhotonNetwork.Instantiate(tPrefab.name, new Vector3(0, 0, 0), Quaternion.identity);

        PhotonView view2 = instanobjCT.GetComponent<PhotonView>();
        int num = view2.ViewID;
        // instanobjCT.name = instanobjCT.name + PhotonNetwork.CurrentRoom.PlayerCount;

        // instanobjCT.name = instanobjCT.name + num + PhotonNetwork.CurrentRoom.PlayerCount;
        string name = instanobjCT.name;

        // instanobj.name = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        // int name = (int)PhotonNetwork.CurrentRoom.PlayerCount;
        object[] content = new object[] { name, num, team }; // Array contains the target position and the IDs of the selected units
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(MoveUnitsToTargetPositionEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    // problems
    // when master client kills someone it increases the correct team list by 2 instead of 1
    // when a non master client kills someone it adds 1 to both lists
}
