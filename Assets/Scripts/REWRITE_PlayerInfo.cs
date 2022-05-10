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
    public bool alreadyRan;
    public Material ctskin, tskin;
    private void OnEnable(){
        PhotonNetwork.NetworkingClient.EventReceived += OnJoinedRecieve;
        PhotonNetwork.NetworkingClient.EventReceived += OnRoundStartRecieve;
    }
    private void OnDisable(){
        PhotonNetwork.NetworkingClient.EventReceived -= OnJoinedRecieve;
        PhotonNetwork.NetworkingClient.EventReceived -= OnRoundStartRecieve;
    }
    private void Start(){
        view = GetComponent<PhotonView>();
        health = 100; // required to prevent dying upon spawning for some reason
        alive = true;
        alreadyRan = false;
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
        }
    }
    private void OnJoinedRecieve(EventData photonEvent) // method that only runs once when this local player joins the game
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == 1 && view.IsMine && alreadyRan == false) // <= don't delete 
        {
            // unpacking event data
            object[] data = (object[])photonEvent.CustomData;
            name = (string)data[0];
            int recievedview = (int)data[1];
            team = (bool)data[2];
            name = name + recievedview;
            
            // assigning custom properties to hash table
            PhotonNetwork.LocalPlayer.NickName = name;
            hash.Add("Name", name);
            hash.Add("Team", team);
            hash.Add("Health", health);
            hash.Add("Alive", alive);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash); // propegates changes to hash table to all clients

            // syncs names and skins across all clients
            view.RPC("UpdateNames", RpcTarget.AllBuffered, name); // note that these calls are Buffered meaning the server will remember that they were called and give them to new clients who join
            view.RPC("UpdateSkins", RpcTarget.AllBuffered, team);

            // ExitGames.Client.Photon.Hashtable nameProperty = new ExitGames.Client.Photon.Hashtable() {{"Name", name}}; // this is alternative way of doing what the using statement above does while preserving the defualt hashtable datatype
            // PhotonNetwork.NetworkingClient.EventReceived -= OnJoinedRecieve; // removes us from the OnJoinedRecieve event so if someone else joins we don't run this code again
            alreadyRan = true;
        }
    }
    private void OnRoundStartRecieve(EventData photonEvent){
        // sending out event
        byte eventCode = photonEvent.Code;
        if (eventCode == 3){ // <= don't delete
            alive = true;
            hash["Alive"] = alive;
            PhotonNetwork.LocalPlayer.CustomProperties["Alive"] = alive;

            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

            if(team == true){
                transform.position = new Vector3(-1.2f, 8.3f, -39.9f); // teleporting better than deleting and reinstantiating(citation needed)
            }
            if(team == false){
                transform.position = new Vector3(21.2f, 8.3f, 37.6f);
            }
        }
    }
    [PunRPC]
    private void Damage(int damage){
        if(health > 0 && view.IsMine){
            health = health - damage;

            hash["Health"] = health;
            PhotonNetwork.LocalPlayer.CustomProperties["Health"] = health; // this is more efficient than assigning to local hash then updating the entire table because it only updates the health value
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }
    private void OnDeathSend(){
        // sending out event
        object[] content = new object[] { name, team}; // Array contains the target position and the IDs of the selected units
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(2, content, raiseEventOptions, SendOptions.SendReliable);
    }

    [PunRPC]
    private void UpdateSkins(bool team){
        // code to find the player mesh and renderer
        GameObject capsulegameobject = transform.GetChild(1).gameObject;
        Renderer Objectrenderer = capsulegameobject.GetComponent<Renderer>();
        Debug.Log(Objectrenderer);
        
        // updates material according to team
        if (team == true){
            Objectrenderer.material = ctskin;
        }
        if(team == false){
            Objectrenderer.material = tskin;
        }
    }
    [PunRPC]
    private void UpdateNames(string name){
        transform.parent.name = name; // tranform.root.name should also work here and might be better practice(?)

        hash["Name"] = name;
        PhotonNetwork.LocalPlayer.CustomProperties["Name"] = name;

        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    // good resources
    //https://forum.photonengine.com/discussion/9937/example-for-custom-properties
    //https://doc-api.photonengine.com/en/pun/v2/class_photon_1_1_realtime_1_1_player.html
}
