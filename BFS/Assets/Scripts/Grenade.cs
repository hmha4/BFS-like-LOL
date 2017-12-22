using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{

    public GameObject grenadePos;
    NewBehaviourScript player;
    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Sphere").GetComponent<NewBehaviourScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.isGrenade)
        {
            //
        }
        else
        {
            transform.position = grenadePos.transform.position;
            transform.rotation = grenadePos.transform.rotation;
        }

        //if (transform.position.y <= 0)
        //    transform.position = grenadePos.transform.position;
    }
}
