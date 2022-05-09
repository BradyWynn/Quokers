using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DEPRECATED_Movement : MonoBehaviour
{
    // <<THIS IS LEGACY CODE NOT USED IN THE CURRENT BUILD>>
    public Vector3 accelerationX;
    public Vector3 accelerationY;
    public Vector3 wishdir;
    public float gravity = 0.005f;
    public GameObject camera;
    public bool isgrounded;
    public Vector3 accelerationZ;
    public float speed = .4f;
    public Vector3 vel = Vector3.zero;
    public float fric;
    public float groundfric = .75f;
    public float airfric = 1;
    public float jumpforce = .1f;
    public float var2;
    PhotonView view;
    private void Start() {
        view = GetComponent<PhotonView>();
    }

    void FixedUpdate(){
        if(view.IsMine){  
            // final term in lerp is host fast vel approaches wishdir
            // vel = Vector3.Lerp(vel, wishdir * (vel.magnitude + wishdir.magnitude), .1f);
            // vel = vel * (vel.magnitude);

            // othervel = transform.localPosition - lastframe;
            // lastframe = transform.localPosition;

            Debug.DrawRay(transform.position, vel * 20, Color.green);
            Debug.DrawRay(transform.position, wishdir * 20, Color.blue);

            // input
            accelerationZ = Vector3.zero;
            accelerationX = Vector3.zero;

            if(Input.GetKey(KeyCode.W))
                accelerationZ = transform.forward;
            if(Input.GetKey(KeyCode.S))
                accelerationZ = -transform.forward;
            if(Input.GetKey(KeyCode.D))
                accelerationX = transform.right;
            if(Input.GetKey(KeyCode.A)) 
                accelerationX = -transform.right;

            // jump/friction code
            if(isgrounded == false){
                accelerationY = accelerationY - new Vector3(0, gravity, 0);
                fric = airfric;
            }
            else{
                accelerationY = Vector3.zero;
                fric = groundfric;
            }
            // if(Input.GetKeyDown(KeyCode.Space) && isgrounded == true) // manual jump
            // if(Input.GetKey(KeyCode.Space) && isgrounded == true) // auto jump
            if((Input.GetAxis("Mouse ScrollWheel") < 0f) && isgrounded == true)
                accelerationY = transform.up * jumpforce;
                
            // wishdir = (accelerationX + accelerationZ).normalized * speed;
            wishdir = (accelerationX + accelerationZ).normalized; // in quake wishdir is unit vector
            vel = accelerate(vel, wishdir);
            vel = vel * (fric);
            // vel = vel * (Mathf.Clamp(vel.magnitude, 0, 1));

            Debug.Log(vel.magnitude); // speed

            //position/rotation updates
            transform.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, camera.transform.eulerAngles.y, transform.eulerAngles.z));
            // transform.position = transform.position + (vel * 5) + accelerationY;
            transform.position = transform.position + vel + accelerationY;
        }
    }
    Vector3 accelerate(Vector3 vel, Vector3 wishdir){ 
        float currentspeed = Vector3.Dot(vel, wishdir);
        float addspeed = speed - currentspeed;
        addspeed = Mathf.Max(Mathf.Min(addspeed, var2), 0);
        return vel + wishdir * addspeed;
    }

    void OnCollisionEnter(Collision theCollision){
    if (theCollision.gameObject.name == "floor")
        isgrounded = true;
    }
    void OnCollisionExit(Collision theCollision){
    if (theCollision.gameObject.name == "floor")
        isgrounded = false;  
    }
}
