using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class Interact : MonoBehaviour
{
    public GameObject GunPrefab;
    public LayerMask hitMask;
    public LayerMask gunMask;
    public LineRenderer LineRenderer;
    public Transform TransformOne;
    public Transform TransformTwo;
    public PhotonView view;
    public Gun Gun;
    public bool active;
    public float inBetweenShots = 0;
    public int fireDistance;
    public MeshFilter filtermesh;
    public CameraControl playercamera;
    public Camera currentcamera;
    public float localsens;
    public RaycastHit hit;
    private void OnEnable()
    {
        FindObjectOfType<PauseMenu>().SensChangeEvent += SensChange;
    }
    private void OnDisable()
    {
        FindObjectOfType<PauseMenu>().SensChangeEvent -= SensChange;
    }
    void Start() {
        view = GetComponent<PhotonView>();
        active = true;
        filtermesh = GetComponent<MeshFilter>();
        filtermesh.mesh = Gun.gunMesh;
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
        localsens = playercamera.sens;
        // if(Camera.current != null){
        //     currentcamera = Camera.current;
        // }
    }
    void Update(){
        // setting current camera in start just returns null cause its too early so need to do this in Update but pretty sure theres a better way to do this
        if(currentcamera == null){
            currentcamera = Camera.current;
        }

        // checking player inputs and running methods accordingly
        if(view.IsMine && active == true){  // this code is begging for a state machine, should probably do that
            if(Input.GetMouseButton(0) && Gun == null && Physics.Raycast(transform.parent.position, transform.TransformDirection(Vector3.forward), out hit, 100, gunMask)){
                view.RPC("PickupGun", RpcTarget.All, view.ViewID, hit.transform.name);
            }
            if(Gun != null){
                if(Input.GetKeyDown(KeyCode.E) && Gun != null){
                view.RPC("ThrowGun", RpcTarget.All, view.ViewID); // ThrowGun can be run then Gun becomes null and checks below throw an error
                }
                if(Input.GetMouseButton(0) && inBetweenShots > (1 + (60/Gun.fireRate)) && Gun.isAutomatic == true && Gun != null){
                fire();
                }
                if (Input.GetMouseButtonDown(0) && inBetweenShots > (1 + (60/Gun.fireRate)) && Gun.isAutomatic == false && Gun != null){
                    fire();
                }
                if(Input.GetMouseButton(0) && inBetweenShots > (1 + (60/Gun.fireRate)) && Gun.isAutomatic == true){
                fire();
                }
                if (Input.GetMouseButtonDown(0) && inBetweenShots > (1 + (60/Gun.fireRate)) && Gun.isAutomatic == false){
                    fire();
                }
                if(inBetweenShots < (1 + (60/Gun.fireRate))){
                    inBetweenShots = inBetweenShots + (10 * Time.deltaTime);
                }
                if(Gun.isSniper == true){ // checking this every frame doesn't really makes sense for sure a better way to do this
                    if(Input.GetMouseButton(1)){
                    currentcamera.fieldOfView = 25;
                    playercamera.sens = localsens * .25f;
                    }
                    else{
                    currentcamera.fieldOfView = 100;
                    playercamera.sens = localsens;
                    }
                }
            }
        }
    }
    void fire(){
        if(view.IsMine){
            RaycastHit hit;
        // rn raycast goes through walls, need check
        if (Physics.Raycast(transform.parent.position, transform.TransformDirection(Vector3.forward), out hit, Gun.fireDistance, hitMask))
        {
            Debug.DrawRay(transform.parent.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            // Debug.Log(hit.transform.name);

            // all of this code is exceedingly bad
            // ensures that you can't kill yourself 
            // alternatively i could just turn off your own player model which would be alot cleaner and smarter
            PhotonView pv = hit.transform.GetComponent<PhotonView>();
            PhotonView pvparentparent = hit.transform.parent.GetComponent<PhotonView>();
            GameObject parenttransform = transform.root.gameObject;
            PhotonView pvparent = parenttransform.GetComponent<PhotonView>();
            if(pvparentparent.ViewID != pvparent.ViewID){
                pv.RPC("Damage", RpcTarget.All, Gun.damage);
                Debug.Log(pvparentparent.ViewID);
                Debug.Log(pvparent.ViewID);
            }
        }

        LineRenderer.SetPosition(0, transform.parent.position);
        LineRenderer.SetPosition(1, transform.parent.position + transform.forward * (20));
        inBetweenShots = 0;
        }
    }
    public void SensChange(float sens){
        localsens = sens;
    }
    [PunRPC]
    private void ThrowGun(int recieveview){ // this method and the one below not being buffered means that there can be some desync when a new player joins theres a weapon dropped but I don't think buffering them is good practice either(?)
        PhotonView currentview = GetComponent<PhotonView>();
        if(currentview.ViewID == recieveview){
            MeshFilter localfiltermesh = GetComponent<MeshFilter>();
            localfiltermesh.mesh = null; // start method is local so filtermesh never gets assigned on other clients and this fails
            GameObject newGun = Instantiate(GunPrefab, transform.position, new Quaternion(2, 5, 2, 8));
            DroppedGun Scriptablegunobject = newGun.GetComponent<DroppedGun>();
            MeshFilter renderermesh = newGun.GetComponent<MeshFilter>();

            Scriptablegunobject.Gun = Gun;
            renderermesh.mesh = Gun.gunMesh;

            Gun = null;
        }
    }
    [PunRPC]
    private void PickupGun(int recieveview, string hitname){
        PhotonView currentview = GetComponent<PhotonView>();
        if(currentview.ViewID == recieveview){
            MeshFilter localfiltermesh = GetComponent<MeshFilter>(); // calling GetComponent like this every time its run is pretty bad
            // GameObject gameobjecthitted = hit.transform.gameObject;// not really sure what I can do though, refer to comment in method above
            GameObject gameobjecthitted = GameObject.Find(hitname);
            DroppedGun hitgo = gameobjecthitted.GetComponent<DroppedGun>();
            Gun = hitgo.Gun;
            localfiltermesh.mesh = Gun.gunMesh;
            Destroy(gameobjecthitted);
        }
    }
}
