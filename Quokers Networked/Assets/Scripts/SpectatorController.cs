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
    public void Start() {
        view = GetComponent<PhotonView>();
    }
    void Update(){   
        if(view.IsMine){  
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
}
