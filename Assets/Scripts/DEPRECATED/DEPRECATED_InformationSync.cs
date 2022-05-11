using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
public class DEPRECATED_InformationSync : MonoBehaviour {
    // <<THIS IS LEGACY CODE NOT USED IN THE CURRENT BUILD>>
    public List<string> playersCT = new List<string>();
    public List<string> aliveCT = new List<string>();
    public List<string> playersT = new List<string>();
    public List<string> aliveT = new List<string>();
    PhotonView view;
    public void Start(){
        view = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient == true)
        {
            gameObject.AddComponent<RoundLogic>();
        }
    }
    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnJoinedRecieve;
        PhotonNetwork.NetworkingClient.EventReceived += OnDeathRecieve;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnJoinedRecieve;
        PhotonNetwork.NetworkingClient.EventReceived += OnDeathRecieve;
    }

    private void OnJoinedRecieve(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == 1) // <= don't delete
        {
            object[] data = (object[])photonEvent.CustomData;
            bool teambool = (bool)data[2];
            string name = data[0].ToString();
            int photonViewRecievedNumber = (int)data[1];
            GameObject playername = GameObject.Find(name);
            playername.name = name + photonViewRecievedNumber;

            if(teambool == false){
                playersT.Add(playername.name);
            }
            if(teambool == true){
                playersCT.Add(playername.name);
            }

            // if(PhotonNetwork.IsMasterClient == true){
            //     object[] objectArrayCT = new object[] {playersCT.Count};
            //     object[] objectArrayT = new object[] {playersT.Count};

            //     for (int i = 0; i < playersCT.Count; i++){
            //         objectArrayCT[i] = playersCT[i];
            //     }
            //     for (int i = 0; i < playersT.Count; i++){
            //         objectArrayT[i] = playersT[i];
            //     }
            //     view.RPC("networksync",  RpcTarget.All, objectArrayCT as object, objectArrayT as object);
            // }
        }
    }
    private void OnDeathRecieve(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == 2){ // <= don't delete

            // processing obj list and adding to death lists
            object[] data = (object[])photonEvent.CustomData;
            string name = data[0].ToString();
            bool team = (bool)data[1];

            if(team == false)
                aliveT.Add(name);
            if(team == true)
                aliveCT.Add(name);
        }
    }
    [PunRPC]
    private void networksync(object[] ct, object[] t){
        List<string> ctlist = new List<string>();
        List<string> tlist = new List<string>();

        foreach(var cosa in ct){
            string thing = (string)cosa;
            ctlist.Add(thing);
        }
        foreach(var cosa in t){
            string thing = (string)cosa;
            tlist.Add(thing);
        }

        playersCT = ctlist;
        playersT = tlist;
        // Debug.Log("refriguator running");
    }
    public List<string> getplayersCT(){
        return playersCT;
    }
    public List<string> getplayersT(){
        return playersT;
    }
    public List<string> getaliveCT(){
        return aliveCT;
    }
    public List<string> getaliveT(){
        return aliveT;
    }
}