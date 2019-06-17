using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class enemypos : MonoBehaviour
{
    public GameObject location;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!location)
        {
            transform.position = location.transform.position;
        }
        
    }
}
