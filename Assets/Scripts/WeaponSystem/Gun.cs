using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "Quokers/Gun", order = 0)]
public class Gun : ScriptableObject {
    public string gunName;
    public int damage;
    public int fireRate;
    public int fireDistance;
    public Mesh gunMesh;
    public bool isAutomatic;
    public bool isSniper;

}
