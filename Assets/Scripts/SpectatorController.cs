using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class SpectatorController : MonoBehaviour
{
    public float MAX_AIR_SPEED = 1;
    public GameObject camera;
    public Vector3 wishdir;
    public Vector3 vel = Vector3.zero;
    public CharacterController controller;
    public float fric;
    public float groundfric, wallfric = .8f;
    public float MAX_AIR_ACCEL;
    PhotonView view;
    public bool active = false;
    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnDeathRecieve;
        PhotonNetwork.NetworkingClient.EventReceived += OnRoundStartRecieve;
    }
    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnDeathRecieve;
        PhotonNetwork.NetworkingClient.EventReceived -= OnRoundStartRecieve;
    }
    public void Start() {
        view = GetComponent<PhotonView>();
    }
    void Update(){   
        if(view.IsMine && active == true){  
            Vector3 z = Vector3.zero;
            Vector3 x = Vector3.zero;

            if(Input.GetKey(KeyCode.W))
                z = transform.forward;
            if(Input.GetKey(KeyCode.S))
                z = -transform.forward;
            if(Input.GetKey(KeyCode.D))
                x = transform.right;
            if(Input.GetKey(KeyCode.A)) 
                x = -transform.right;

            // move
            wishdir = (z + x).normalized; // wishdir is unit vector

            transform.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, camera.transform.eulerAngles.y, transform.eulerAngles.z));
            transform.position = transform.position + (wishdir/2);

        }
    }
    private void OnDeathRecieve(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == 2){ // <= don't delete
            // processing obj list
            object[] data = (object[])photonEvent.CustomData;
            string recievedname = data[0].ToString();

            GameObject recievedgameObject = GameObject.Find(recievedname);
            string recievedparentname = recievedgameObject.transform.root.gameObject.name;
            string parentname = transform.root.gameObject.name;

            if(parentname == recievedparentname){
                active = true;
            }
        }
    }
    private void OnRoundStartRecieve(EventData photonEvent){
        // sending out event
        byte eventCode = photonEvent.Code;
        if (eventCode == 3){ // <= don't delete
            active = false;
        }
    }
}
