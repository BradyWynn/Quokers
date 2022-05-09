using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable; // this prevents unity from getting confused at there being two hashtable datatypes

public class REWRITE_PlayerInfo : MonoBehaviour
{
    public PhotonView view;
    public GameObject UI;
    public Hashtable hash = new Hashtable();
    public string name;
    public bool team; // false is t side, true is ct
    public int health;
    public bool alive;
    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnJoinedRecieve;
        PhotonNetwork.NetworkingClient.EventReceived += OnRoundStartRecieve;
    }
    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnJoinedRecieve;
        PhotonNetwork.NetworkingClient.EventReceived -= OnRoundStartRecieve;
    }
    private void Start() {
        view = GetComponent<PhotonView>();
        // player = PhotonNetwork.LocalPlayer.Get(PhotonNetwork.CurrentRoom.PlayerCount);
        // mesh = mayoichild.GetComponent<MeshRenderer>();
        health = 100; // required to prevent dying upon spawning for some reason
        alive = true;
        if(!view.IsMine){ // deletes all UI not on local client
            Destroy(UI);
        }
    }
    private void Update(){
        if(health == 0 && view.IsMine){
            alive = false;
            hash["Alive"] = alive;
            PhotonNetwork.LocalPlayer.CustomProperties["Alive"] = alive;

            health = 100;
            hash["Health"] = health;
            PhotonNetwork.LocalPlayer.CustomProperties["Health"] = health;

            PhotonNetwork.LocalPlayer.SetCustomProperties(hash); // THIS IS REQUIRED!!!!!!

            OnDeathSend();
            // view.RPC("componentsync", RpcTarget.All);
            // charactercontrol.enabled = false;
        }
        // if(view.IsMine){
        //     foreach (Player temp in PhotonNetwork.PlayerList){
        //         // Debug.Log((string)temp.CustomProperties["Name"]);    
        //         // Debug.Log(temp);
        //         if(player.IsLocal){
        //             Debug.Log("this player is local" + temp + player);
        //         } 
        //         else{
        //             Debug.Log("this player is not local" + temp + player);
        //         }
        //     }
        // }
    }
    private void OnJoinedRecieve(EventData photonEvent) // method that only runs once when this local player joins the game
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == 1 && view.IsMine) // <= don't delete 
        {
            // unpacking event data
            object[] data = (object[])photonEvent.CustomData;
            name = (string)data[0];
            int recievedview = (int)data[1];
            team = (bool)data[2];
            name = name + recievedview;
            
            PhotonNetwork.LocalPlayer.NickName = name;
            hash.Add("Name", name);
            hash.Add("Team", team);
            hash.Add("Health", health);
            hash.Add("Alive", alive);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash); // propegates changes to hash table to all clients

            // ExitGames.Client.Photon.Hashtable nameProperty = new ExitGames.Client.Photon.Hashtable() {{"Name", name}}; // this is alternative way of doing what the using statement above does while preserving the defualt hashtable datatype
            PhotonNetwork.NetworkingClient.EventReceived -= OnJoinedRecieve; // removes us from the OnJoinedRecieve event so if someone else joins we don't run this code again
        }
    }
    private void OnRoundStartRecieve(EventData photonEvent){
        // sending out event
        byte eventCode = photonEvent.Code;
        if (eventCode == 3){ // <= don't delete
            alive = true;
            hash["Alive"] = alive;
            PhotonNetwork.LocalPlayer.CustomProperties["Alive"] = alive;

            if(team == true)
                transform.position = new Vector3(-1.2f, 8.3f, -39.9f); // change to delete/instaniate player instead of teleport
                // Debug.Log("-1.2f, 8.3f, -39.9f");
            if(team == false)
                transform.position = new Vector3(21.2f, 8.3f, 37.6f); // change to delete/instaniate player instead of teleport
                // Debug.Log("21.2f, 8.3f, 37.6f");
        }
    }
    [PunRPC]
    private void Damage(int damage){
        if(health > 0 && view.IsMine){
            health = health - damage;
            hash["Health"] = health;
            PhotonNetwork.LocalPlayer.CustomProperties["Health"] = health; // this is more efficient than assigning to local hash then updating the entire table because it only updates the health value
        }
    }
    private void OnDeathSend(){
        // sending out event
        object[] content = new object[] { name, team}; // Array contains the target position and the IDs of the selected units
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(2, content, raiseEventOptions, SendOptions.SendReliable);
    }
    // [PunRPC]
    // private void componentsync(){
    //     movementscript.enabled = false; 
    //     spectatormove.enabled = true;
    //     mesh.enabled = false;
    //     weaponscript.enabled = false;
    // }

    // good resource https://forum.photonengine.com/discussion/9937/example-for-custom-properties
    // not sure how to deal with local hashtable and variable syncing???
    // local hashtable and varialbes should always be synced
    // but should you assign values directly to the hashtable and then update the variable from the hashtalbe
    // or assign directly to variable and then update hashtable?
    // or you could go without the variable completely and everytime you want to use health access it from the hashtable
    // however this is probably not a good idea for performance reasons (hashtable look up is O(n) vs just accesing a variable)
}
