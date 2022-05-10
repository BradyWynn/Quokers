using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine.UI;

public class Billboard : MonoBehaviour
{
    public Camera maincam;
    public Text nametag;
    public void Update(){
        if(maincam == null){
            maincam = Camera.current;
        }

        // nametag.text = (string)PhotonNetwork.LocalPlayer.CustomProperties["Name"];
        nametag.text = transform.root.name;

        transform.position = transform.parent.position + new Vector3(0, 1.41f, 0);
        transform.LookAt(transform.position + maincam.transform.rotation * Vector3.forward, maincam.transform.rotation * Vector3.up);
    //     // transform.LookAt(transform.position + camera.rotation * Vector3.forward);
    }
}
