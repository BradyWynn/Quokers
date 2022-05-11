using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;
public class RoomManager : MonoBehaviour
{
    PhotonView view;
    public Hashtable hash = new Hashtable();
    public int ctRoundsWon = 0;
    public int tRoundsWon = 0;
    private void OnEnable(){
        PhotonNetwork.NetworkingClient.EventReceived += OnRoundStartRecieve;
    }
    private void OnDisable(){
        PhotonNetwork.NetworkingClient.EventReceived -= OnRoundStartRecieve;
    }
    private void Start() {
        view = GetComponent<PhotonView>();

        hash.Add("CTWins", ctRoundsWon);
        hash.Add("TWins", tRoundsWon);

        PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
    }
    private void OnRoundStartRecieve(EventData photonEvent){
        // sending out event
        byte eventCode = photonEvent.Code;
        if (eventCode == 3 && view.IsMine){ // <= don't delete
            object[] data = (object[])photonEvent.CustomData;
            bool team = (bool)data[0];

            if(team == true){
                tRoundsWon++;
            }
            if(team == false){
                ctRoundsWon++;
            }
            Debug.Log("this has run once!");


            hash["CTWins"] = ctRoundsWon;
            hash["TWins"] = tRoundsWon;
            PhotonNetwork.CurrentRoom.CustomProperties["CTWins"] = ctRoundsWon;
            PhotonNetwork.CurrentRoom.CustomProperties["TWins"] = tRoundsWon;

            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }
    }
}
