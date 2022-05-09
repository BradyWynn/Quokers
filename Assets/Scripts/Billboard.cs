using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform camera;
    private void Start() {
        // camera = Camera.main.transform;
    }
    public void Update(){
        transform.LookAt(transform.position + camera.rotation * Vector3.forward, camera.rotation * Vector3.up);
        // transform.LookAt(transform.position + camera.rotation * Vector3.forward);
    }
}
