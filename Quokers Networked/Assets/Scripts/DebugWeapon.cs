using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class DebugWeapon : MonoBehaviour{
    public LayerMask hitMask;
    public int damage = 20;
    PhotonView view;
    public LineRenderer LineRenderer;
    public Transform TransformOne;
    public Transform TransformTwo;
    public float inBetweenShots = 0;
    public bool active; // we use a variable here instead of disabling the script because a disabled script cant reenable itself which means youd need to reference it in another script
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
    void Start(){
        view = GetComponent<PhotonView>();
        active = true;
        if(view.IsMine){ 
            // set the color of the line
            LineRenderer.startColor = Color.red;
            LineRenderer.endColor = Color.red;
    
            // set width of the renderer
            LineRenderer.startWidth = 0.3f;
            LineRenderer.endWidth = 0.3f;

            LineRenderer.SetPosition(0, new Vector3(0, 0, 0));
            LineRenderer.SetPosition(1, new Vector3(0, 0, 0));
        }
    }
    void Update(){
        // int layerMask = 7;
        if(view.IsMine && active == true){ 
            if (Input.GetMouseButton(0) && inBetweenShots > 2){
                RaycastHit hit;
                // rn raycast goes through walls, need check
                // also infinite distance is expensive to call so limit distance
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, hitMask))
                {
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                    // Debug.Log(hit.transform.name);

                    PhotonView pv = hit.transform.GetComponent<PhotonView>();
                    pv.RPC("Damage", RpcTarget.All, damage);
                }

                LineRenderer.SetPosition(0, transform.position);
                LineRenderer.SetPosition(1, transform.position + transform.forward * (20));
                inBetweenShots = 0;

            }
            if(inBetweenShots < 2)
                inBetweenShots = inBetweenShots + (10 * Time.deltaTime);
            // Debug.Log(inBetweenShots);
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

            Debug.Log(recievedparentname);
            Debug.Log(parentname);
            
            if(parentname == recievedparentname){
                active = false;
            }
        }
    }
    private void OnRoundStartRecieve(EventData photonEvent){
        // sending out event
        byte eventCode = photonEvent.Code;
        if (eventCode == 3){ // <= don't delete
            active = true;
        }
    }
}
