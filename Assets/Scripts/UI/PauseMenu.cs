using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine.UI;
using System;

public class PauseMenu : MonoBehaviour
{
    // currently an instance of this exists for each player
    // this works in the case of say sensitivity however,
    // for global variables such as gravity you'd want one
    // player to change it and affect all players
    
    public static bool paused = false;
    // public CameraControl playercamera;
    public CharacterControllerr move;
    public RoundLogic master;
    public GameObject pauseUI;
    public InputField sensInput;
    public InputField speedInput;
    public InputField gravityInput;
    public InputField groundfricInput;
    public InputField airfricInput;
    public InputField jumpforceInput;
    public InputField nameInput;
    public float sens;
    public float speeding;
    public float gravityy;
    public float groundfriccc;
    public float airfricccccc;
    public float jumpforcee;
    public string playername;
    PhotonView view;
    public event Action <float> SensChangeEvent;
    public void Start(){
        view = GetComponent<PhotonView>();
    }

    void Update()
    {
        if(view.IsMine){ 
            // camera = FindObjectOfType<CameraControl>(); // calling this every frame is really bad please change
            // move = FindObjectOfType<CharacterControllerr>();
            master = FindObjectOfType<RoundLogic>();

            if(Input.GetKeyDown(KeyCode.Escape)){
                if(paused == true){
                    resume();
                }
                else{
                    pause();
                }
            }
        }
    }

    void resume(){
        if(view.IsMine){ 
            pauseUI.SetActive(false);
            Cursor.visible = false;
            paused = false;
        }
    }
    void pause(){
        if(view.IsMine){ 
            pauseUI.SetActive(true);
            Cursor.visible = true;
            paused = true;
        }
    }

    public void sensitivity(){
        if(view.IsMine){ 
            sens = float.Parse(sensInput.text); // float.Parse converts from String to float
            // camera.sens = sens;
            SensChangeEvent(sens);
        }
    }

    public void speed(){
        if(view.IsMine){ 
            speeding = float.Parse(speedInput.text);
            move.MAX_SPEED = speeding;
        }
    }

    public void gravity(){
        if(view.IsMine){ 
            gravityy = float.Parse(gravityInput.text);
            move.gravity = gravityy;
        }
    }
    public void groundfric(){
        if(view.IsMine){ 
            groundfriccc = float.Parse(groundfricInput.text);
            move.groundfric = groundfriccc;
        }
    }

    public void wallfricc(){
        if(view.IsMine){ 
            airfricccccc = float.Parse(airfricInput.text);
            move.wallfric = airfricccccc;
        }
    }

    public void jumpforce(){
        if(view.IsMine){ 
            jumpforcee = float.Parse(jumpforceInput.text);
            move.MAX_AIR_ACCEL = jumpforcee;
        }
    }

    public void setName(){
        if(view.IsMine){
            playername = nameInput.text;
            string currentname = (string)PhotonNetwork.LocalPlayer.CustomProperties["Name"];
            Debug.Log(PhotonNetwork.LocalPlayer);
            GameObject temp = GameObject.Find(currentname);
            GameObject temp2 = temp.transform.GetChild(0).gameObject;
            PhotonView view2 = temp2.GetComponent<PhotonView>();
            // Debug.Log(view2 + " and this " + currentname);
            // Debug.Log(view);

            view2.RPC("UpdateNames", RpcTarget.AllBuffered, playername, temp.name, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }
}
