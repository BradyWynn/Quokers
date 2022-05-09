using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class CharacterControllerr : MonoBehaviour
{
    public float MAX_SPEED = .1f;
    public float gravity = -9.18f;
    public float jumpHeight = 3f;
    public float groundDistance = 0.4f;
    public CharacterController controller;
    public GameObject camera;
    public Transform groundCheck;
    public LayerMask groundMask, wallMask;
    public Vector3 velocity;
    public Vector3 wishdir;
    public Vector3 vel = Vector3.zero;
    public bool prevScroll;
    public float fric;
    public float groundfric;
    public float wallfric;
    public float MAX_ACCEL;
    public float MAX_AIR_SPEED;
    public float MAX_AIR_ACCEL;
    public bool active = true;
    PhotonView view;
    // public GameObject menu;
    // public GameObject speed;
    public bool isGrounded, isWalled;
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
    private void Start(){
        view = GetComponent<PhotonView>();
        // if(view.IsMine == false){
        //     Destroy(menu);
        //     Destory(speed);
        // }
        // if(view.IsMine){
        //     PhotonNetwork.Instantiate(menu.name, new Vector3(0, 0, 0), Quaternion.identity);
        //     PhotonNetwork.Instantiate(speed.name, new Vector3(0, 0, 0), Quaternion.identity);
        // }
        Cursor.visible = false;
    }
    void Update(){   
        if(view.IsMine && active == true){  
            // debug
            Debug.DrawRay(transform.position, vel * 5, Color.green);
            Debug.DrawRay(transform.position, wishdir * 5, Color.blue);

            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            isWalled = Physics.CheckSphere(groundCheck.position, groundDistance, wallMask);

            // jump
            // if(Input.GetAxis("Mouse ScrollWheel") < 0 && isGrounded && prevScroll == true){
            if (Input.GetButton("Jump") && isGrounded){
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                isGrounded = false;
                prevScroll = false;
            }
            // if(Input.GetAxis("Mouse ScrollWheel") == 0){
            //     prevScroll = true;
            // }

            velocity.y += gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);
            
            // input
            // if (Input.GetKey("left shift") && isGrounded)
            //     speed = 20f;
            // else
            //     speed = 12f;

            // Vector3 x = Input.GetAxis("Horizontal") * transform.right;
            // Vector3 z = Input.GetAxis("Vertical") * transform.forward;

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

            if (isGrounded && velocity.y < 0)
                velocity.y = -2f;

            // friction
            if(isGrounded == true)
                fric = groundfric;
            if (isWalled == true)
                fric = wallfric;

            // move
            wishdir = (z + x).normalized; // wishdir is unit vector
            if(isGrounded)
                vel = accelerate(vel, wishdir, fric);
            else
                vel = airAccelerate(vel, wishdir);


            transform.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, camera.transform.eulerAngles.y, transform.eulerAngles.z));
            // transform.Rotate(new Vector3(0, camera.transform.y, 0)); // maybe use this instead of above 
                                                                     // because using built in functions is better than rewriting the rotation
            // if(vel.magnitude >= .2f) // speed cap (not really needed because map prevents certain speeds)
            //     vel = vel * (.2f/vel.magnitude);

            controller.Move(vel * Time.deltaTime);

            // Debug.Log(vel.magnitude);
            // Debug.Log(prevScroll);
        }
    }

        // a single update call goes
        // ground/wall check -> jump input -> jump displacement -> wasd input -> detects friction
        // -> wishdir normalization -> accelerate() + applies friction -> rotation displacement -> wasd displacement

     Vector3 accelerate(Vector3 vel, Vector3 wishdir, float fric){ 
        vel = vel * fric; // applies friciton (currently frame dependent and needs to be changed)(WHY DOES THIS BREAK???)
        float currentspeed = Vector3.Dot(vel, wishdir); // really should get around to writing documentation for this method
        float addspeed = MAX_SPEED - currentspeed;
        addspeed = Mathf.Max(Mathf.Min(addspeed, MAX_ACCEL * Time.deltaTime * 166), 0);
        return vel + addspeed * wishdir;
    }

    Vector3 airAccelerate(Vector3 vel, Vector3 wishdir){
        if(isWalled)
            vel = vel * wallfric; // applies wall friction while player is in air
        float currentspeed = Vector3.Dot(vel, wishdir);
        float addspeed = MAX_AIR_SPEED - currentspeed;
        addspeed = Mathf.Max(Mathf.Min(addspeed, MAX_AIR_ACCEL * Time.deltaTime * 166), 0); // maybe make an MAX_AIR_ACCEl variable independent of other
        return vel + addspeed * wishdir;
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
