using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using TMPro;

public class vel : MonoBehaviour
{
    public TextMeshProUGUI velocitytextbox;
    public CharacterControllerr cc;
    PhotonView view;
    public TextMeshProUGUI Scoreboard;
    private void OnEnable(){
        PhotonNetwork.NetworkingClient.EventReceived += OnRoundStartRecieve;
    }
    private void OnDisable(){
        PhotonNetwork.NetworkingClient.EventReceived -= OnRoundStartRecieve;
    }
    void Start(){
        view = GetComponent<PhotonView>();
        // if(view.IsMine){ 
        //     textbox = GetComponent<Text>();
        // }
    }

    void Update(){
        if(view.IsMine){ 
            // cc = FindObjectOfType<CharacterControllerr>();
            Vector3 speed = cc.vel;
            velocitytextbox.text = "" + speed.magnitude;
            UpdateScoreboard();
        }
    }
    private void OnRoundStartRecieve(EventData photonEvent){
        // sending out event
        byte eventCode = photonEvent.Code;
        if (eventCode == 3 && view.IsMine){ // <= don't delete
            UpdateScoreboard();
        }
    }
    public void UpdateScoreboard(){
        Scoreboard.text = PhotonNetwork.CurrentRoom.CustomProperties["CTWins"] + " - " + PhotonNetwork.CurrentRoom.CustomProperties["TWins"];
    }
}
