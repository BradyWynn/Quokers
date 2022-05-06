using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class PlayerInformation : MonoBehaviour
{
    public PhotonView view;
    public GameObject GameManager;
    public InformationSync infosyncref;
    public CharacterControllerr movementscript;
    public CharacterController charactercontrol;
    public SpectatorController spectatormove;
    public DebugWeapon weaponscript;
    public GameObject camera;
    public GameObject mayoichild;
    public MeshRenderer mesh;
    public int health = 100;
    public GameObject UI;
    public bool side; // false is t side, true is ct
    public bool alive;
    public Material ctskin;
    public Material tskin;
    bool ran = false;
    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnRoundStart;
        PhotonNetwork.NetworkingClient.EventReceived += OnJoinedRecieve;
    }
    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnRoundStart;
        PhotonNetwork.NetworkingClient.EventReceived -= OnJoinedRecieve;
    }
    private void Start() {
        view = GetComponent<PhotonView>();
        charactercontrol = GetComponent<CharacterController>();
        movementscript = GetComponent<CharacterControllerr>();
        spectatormove = GetComponent<SpectatorController>();
        mesh = mayoichild.GetComponent<MeshRenderer>();
        weaponscript = camera.GetComponent<DebugWeapon>();
        GameManager = GameObject.Find("GameManager");
        infosyncref = GameManager.GetComponent<InformationSync>();
        // deleting shit so UI works
        if(!view.IsMine){
            Destroy(UI);
        }
    }

    public void Update(){
        if(health == 0 && view.IsMine){
            health = 100;
            OnDeathSend();
            view.RPC("componentsync", RpcTarget.All);
            // charactercontrol.enabled = false;
        }
    }

    [PunRPC]
    public void damage(int damage){

        if(health > 0)
            health = health - damage;
    }
    private void OnDeathSend(){
        // what happens to the player
        // sending out event
        string name = transform.parent.name;
        bool team = side;
        object[] content = new object[] { name, team }; // Array contains the target position and the IDs of the selected units
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(2, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private void OnRoundStart(EventData photonEvent){
        // sending out event
        byte eventCode = photonEvent.Code;
        if (eventCode == 3){ // <= don't delete
        movementscript.enabled = true; 
        spectatormove.enabled = false;
        mesh.enabled = true;
        weaponscript.enabled = true;
            if(side == true)
                transform.position = new Vector3(-1.2f, 8.3f, -39.9f); // change to delete/instaniate player instead of teleport
                Debug.Log("-1.2f, 8.3f, -39.9f");
            if(side == false)
                transform.position = new Vector3(21.2f, 8.3f, 37.6f); // change to delete/instaniate player instead of teleport
                Debug.Log("21.2f, 8.3f, 37.6f");
        }

        // what happens to the player
        // movementscript.enabled = true; 
        // spectatormove.enabled = false;
        // mesh.enabled = true;
    }
    private void OnJoinedRecieve(EventData photonEvent) // method that only runs once when this local player joins the game
    {
        byte eventCode = photonEvent.Code;
        string parentname = transform.parent.name;
        if (eventCode == 1) // <= don't delete (had a check for PhotonNetwork.IsMasterClient for some reason)
        {
            if(view.IsMine){
                view.RPC("namesync",  RpcTarget.All, parentname);
            }
            if(!ran){
                ran = true;
                object[] data = (object[])photonEvent.CustomData;
                side = (bool)data[2];
                name = (string)data[0];

                // GameObject capsule = playername.transform.Find("Capsule").gameObject;
                // PhotonView photonViewRecieved = capsule.GetComponent<PhotonView>();

                if(view.IsMine){ // this code essentially makes it so that it only runs on the local client
                    view.RPC("skinsyncothers",  RpcTarget.All, side);
                    if (PhotonNetwork.IsMasterClient == false){
                        skinsynclocal();
                    }
                }
            }
        }
            
            

            // if(view.IsMine){
            //     setotherplayersteamcolors();
            // }

            // PhotonNetwork.NetworkingClient.EventReceived -= OnJoinedRecieve; // removes us from the OnJoinedRecieve event so if someone else joins we don't run this code again
    }
    [PunRPC]
    private void componentsync(){
        movementscript.enabled = false; 
        spectatormove.enabled = true;
        mesh.enabled = false;
        weaponscript.enabled = false;
    }
    [PunRPC]
    private void skinsyncothers(bool side){
        GameObject capsuletwopointo = transform.Find("GameObject").gameObject;
        Renderer Object = capsuletwopointo.GetComponent<Renderer>();
        Debug.Log("a player has joined!");
        if(side == false){
            Object.material = ctskin;
        }
        if(side == true){
            Object.material = tskin;
        }
    }
    public void skinsynclocal(){
        List<string> playersCT = infosyncref.getplayersCT();
        // Debug.Log(playersCT[1]);
        List<string> playersT = infosyncref.getplayersT();
        // Debug.Log(playersT[1]);


        foreach(var player in playersCT){
            string name = (string)player;
            GameObject playerobject = GameObject.Find(name);
            // GameObject capsulegameobject = playerobject.transform.transform.Find("GameObject");
            GameObject capsulegameobject = playerobject.transform.GetChild(0).GetChild(1).gameObject;
            Debug.Log(capsulegameobject);
            Renderer Objectrenderer = capsulegameobject.GetComponent<Renderer>();
            Objectrenderer.material = ctskin;
        }
        foreach(var player in playersT){
            string name = (string)player;
            GameObject playerobject = GameObject.Find(name);
            // GameObject capsulegameobject = playerobject.transform.transform.Find("GameObject").gameObject;
            GameObject capsulegameobject = playerobject.transform.GetChild(0).GetChild(1).gameObject;
            Debug.Log(capsulegameobject);
            Renderer Objectrenderer = capsulegameobject.GetComponent<Renderer>();
            Objectrenderer.material = tskin;
        } // pretty sure this code is working but needs more testing
    } // one problem is that names are not synced across clients and so if it looks for the master client on another client then
    // it will not find it and then throw an error and the skin will not be updated

    [PunRPC]
    private void namesync(string name){ // need a better general solution for syncing variables instead of individual methods????
        transform.parent.name = name;
        Debug.Log(name);
    }
    // public void setotherplayersteamcolors(){
    //     view.RPC("RPC_getplayersCT",  RpcTarget.All, view);
    // }

    //RPC is equivalent to running method on all clients with the same photon view id
    //RaiseEvent is equivalent to an Event but is sent out to all clients (usually)

    // player one joins on local client, OnJoinedRecieve runs but skinsync doesn't do anything because of RpcTarget.Others
    // another player joins on second client, both player 1 and player two run OnJoinedRecieve on their client sending out two RPC 
    // that are both recieved by the first client, this updates the first clients skin on their screen but not on the second because
    // they do no recieve the call, 
}
