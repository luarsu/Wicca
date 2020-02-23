using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demon : MonoBehaviour
{
    public Transform target;
    Rigidbody Rigidbody;
    public float orbitSpeed = 2f;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        Quaternion q = Quaternion.AngleAxis(orbitSpeed, transform.up);
        Rigidbody.MovePosition(q * (Rigidbody.transform.position - target.position) + target.position);
        Rigidbody.MoveRotation(Rigidbody.transform.rotation * q);
    }
}
