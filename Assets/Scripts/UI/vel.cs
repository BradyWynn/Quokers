using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class vel : MonoBehaviour
{
    public Text textbox;
    public CharacterControllerr cc;
    PhotonView view;

    void Start(){
        view = GetComponent<PhotonView>();
        if(view.IsMine){ 
            textbox = GetComponent<Text>();
        }
    }

    void Update(){
        if(view.IsMine){ 
            // cc = FindObjectOfType<CharacterControllerr>();
            Vector3 speed = cc.vel;
            textbox.text = "" + speed.magnitude;
        }
    }
}
