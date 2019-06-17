using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isMine : MonoBehaviour
{
    PhotonView PV;
    public GameObject[] children;
    public GameObject enemy;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponentInChildren<PhotonView>();

        
    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine)
        {
            foreach (GameObject go in children)
            {
                go.SetActive(false);
            }
            //enemy.SetActive(true);
        }
        else
        {
            foreach (GameObject go in children)
            {
                go.SetActive(true);
            }
           // enemy.SetActive(false);
        }
    }
}
