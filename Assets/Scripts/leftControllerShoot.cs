using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class leftControllerShoot : MonoBehaviour
{
    public GameObject prefab;
    public float spawnSpeed = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            GameObject spawnedBall = Instantiate(prefab, transform.position, Quaternion.identity); // Quaternion.identity means no rotation
            Rigidbody spawnedBallRB = spawnedBall.GetComponent<Rigidbody>();
            spawnedBallRB.linearVelocity = transform.forward * spawnSpeed;
        }
    }
}
