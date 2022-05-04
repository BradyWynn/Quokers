using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

public class RoundLogic : MonoBehaviour
{
    // player joins => added to list (how do we store player? GameObject? reference to script?)
    // prompt player on which team to join
    // onJoin wait until start of next round to instaniate player? auto spectate random person
    // round ends when every player of a certain team is dead (does having multiple lists for teams make sense?)
    // spawn every player depending on team bool (probably predefine a range of positions and perform a random function inbetween)
    PhotonView view;
    public List<string> playersCT = new List<string>();
    public List<string> aliveCT = new List<string>();
    public List<string> playersT = new List<string>();
    public List<string> aliveT = new List<string>();
    public InformationSync infosyncref;
    public void Start(){
        view = GetComponent<PhotonView>();
        infosyncref = GetComponent<InformationSync>();
        playersCT = infosyncref.getplayersCT();
        playersT = infosyncref.getplayersT();
        aliveCT = infosyncref.getaliveCT();
        aliveT = infosyncref.getaliveT();
    }
    private void OnEnable()
    {
        // PhotonNetwork.NetworkingClient.EventReceived += OnJoinedRecieve;
        PhotonNetwork.NetworkingClient.EventReceived += OnDeathRecieve;
        // PhotonNetwork.NetworkingClient.EventReceived += SyncPlayersRecieve;
    }

    private void OnDisable()
    {
        // PhotonNetwork.NetworkingClient.EventReceived -= OnJoinedRecieve;
        PhotonNetwork.NetworkingClient.EventReceived += OnDeathRecieve;
        // PhotonNetwork.NetworkingClient.EventReceived += SyncPlayersRecieve;
    }

    // private void OnJoinedRecieve(EventData photonEvent)
    // {
    //     byte eventCode = photonEvent.Code;
    //     if (eventCode == 1) // <= don't delete
    //     {
    //         object[] data = (object[])photonEvent.CustomData;
    //         bool teambool = (bool)data[2];
    //         string name = data[0].ToString();
    //         int photonViewRecievedNumber = (int)data[1];
    //         GameObject playername = GameObject.Find(name);
    //         playername.name = name + photonViewRecievedNumber;
    //         if(teambool == false)
    //             playersT.Add(playername.name);
    //         if(teambool == true)
    //             playersCT.Add(playername.name);

    //         //PhotonView photonViewRecieved = PhotonView.Find(photonViewRecievedNumber); 

    //         // if(PhotonNetwork.IsMasterClient == true)
    //         //     view.RPC("SyncPlayers", RpcTarget.All, playersCT, playersT);
    //         // SyncPlayersSend();
    //     }
    // }

    private void OnDeathRecieve(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == 2){ // <= don't delete

            // processing obj list and adding to death lists
            Debug.Log("this player has died");
            object[] data = (object[])photonEvent.CustomData;
            string name = data[0].ToString();
            bool team = (bool)data[1];

            playersCT = infosyncref.getplayersCT();
            playersT = infosyncref.getplayersT();
            aliveCT = infosyncref.getaliveCT();
            aliveT = infosyncref.getaliveT();

            // if(team == false)
            //     aliveT.Add(false);
            // if(team == true)
            //     aliveCT.Add(false);

            // checking if all players on one team are dead
            // if they are RoundStart event is called
            // if(PhotonNetwork.IsMasterClient == true){ // this if statement is redundant since only master has RoundLogic(i think??)
                if(playersCT.Count == aliveCT.Count){
                    Debug.Log(playersCT.Count + " " + aliveCT.Count);
                    RoundStart();
                }
                if(playersT.Count == aliveT.Count){
                    Debug.Log(playersT.Count + " " + aliveT.Count);
                    RoundStart();
                // }
            }
        }
    }

    private void RoundStart(){
        object[] content = new object[] {}; // even though theres no data to send still have to do this
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // ReceiverGroup.Others excludes clients (use All to include client)
        PhotonNetwork.RaiseEvent(3, content, raiseEventOptions, SendOptions.SendReliable);

        aliveCT.Clear();
        aliveT.Clear();
    }

    public void settingthedesriednameoftheplayer(string newName, string oldName){
        // Debug.Log(newName + oldName);
        GameObject temp;
        temp = GameObject.Find(oldName);
        temp.name = newName;
    }
    // [PunRPC]
    // private void RPC_getplayersCT(){
    //     getplayersCT();
    // }
    // [PunRPC]
    // private void RPC_getplayersT(){
    //     getplayersT();
    // }

    // private void SyncPlayersSend(){
    //     object[] content = new object[] { playersCT, playersT }; // Array contains the target position and the IDs of the selected units
    //     RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others }; // You would have to set the Receivers to All in order to receive this event on the local client as well
    //     PhotonNetwork.RaiseEvent(4, content, raiseEventOptions, SendOptions.SendReliable);
    // }

    // private void SyncPlayersRecieve(EventData photonEvent){
    //     byte eventCode = photonEvent.Code;
    //     if (eventCode == 4){ // <= don't delete
    //         object[] data = (object[])photonEvent.CustomData;
    //         List<string> playersCTrecieved = (List<string>)data[0];
    //         playersCT = playersCTrecieved;
    //         List<string> playersTrecieved = (List<string>)data[1];
    //         playersT = playersTrecieved;
    //     }
    // }
}
