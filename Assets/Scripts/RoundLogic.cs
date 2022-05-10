using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

public class RoundLogic : MonoBehaviour
{
    PhotonView view;
    public void Start(){
        view = GetComponent<PhotonView>();
    }
    private void OnEnable(){
        PhotonNetwork.NetworkingClient.EventReceived += OnDeathRecieve;
    }
    private void OnDisable(){
        PhotonNetwork.NetworkingClient.EventReceived -= OnDeathRecieve;
    }
    private void OnDeathRecieve(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == 2){ // <= don't delete
            // processing obj list
            Debug.Log("this player has died");
            object[] data = (object[])photonEvent.CustomData;
            string name = data[0].ToString();
            bool team = (bool)data[1];

            int aliveCT = 0;
            int aliveT = 0;
            
            // loops over every player in the room and stores temp as the reference
            foreach(Player temp in PhotonNetwork.PlayerList){ 
                bool propteam = (bool)temp.CustomProperties["Team"];
                bool propalive = (bool)temp.CustomProperties["Alive"];

                // checks over every player and if they're alive increments their respective team
                if(propalive == true && propteam == true){
                    aliveCT++;
                }
                if(propalive == true && propteam == false){
                    aliveT++;
                }
            }
            // checks if there are any teams with 0 people alive and if there are then that means its time to start the next round
            if(aliveCT == 0){
                RoundStart();
            }
            if(aliveT == 0){
                RoundStart();
            }
        }
    }

    private void RoundStart(){
        // sending out event
        object[] content = new object[] {}; // even though theres no data to send still have to do this
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // ReceiverGroup.Others excludes clients (use All to include client)
        PhotonNetwork.RaiseEvent(3, content, raiseEventOptions, SendOptions.SendReliable);
    }
}
