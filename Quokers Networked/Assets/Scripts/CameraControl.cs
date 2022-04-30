using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraControl : MonoBehaviour
{
    public float mouseX, mouseY;
    public Camera cam;
    PhotonView view;
    public float sens = 3;
    public GameObject menu;
    public GameObject speed;

    public GameObject collider;
    private void Start() {
        view = GetComponent<PhotonView>();
        if(!view.IsMine){
            Destroy(cam);
        }
        Application.targetFrameRate = 1000;
    }
    void Update()
    {
        if(view.IsMine){ 
            mouseX += Input.GetAxis("Mouse X") * sens;
            mouseY += Input.GetAxis("Mouse Y") * sens;

            // mouseX = Mathf.Clamp(mouseX, -90f, 90f);
            mouseY = Mathf.Clamp(mouseY, -90f, 90f);  

            transform.rotation = Quaternion.Euler(-mouseY, mouseX, 0f);
            transform.position = collider.transform.position + new Vector3(0, .833f ,0.135f);
        }
    }
}
