using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : MonoBehaviour {
    public string poolItemName = "Bullet";
    public float moveSpeed = 10;
    public bool isShoot;

    public GameObject bulletPos;
    NewBehaviourScript player;
    // Use this for initialization
    void Start () {
        isShoot = false;
        player = GameObject.Find("Sphere").GetComponent<NewBehaviourScript>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!isShoot)
        {
            transform.position = bulletPos.transform.position;
            transform.rotation = bulletPos.transform.rotation;
        }
        else
            transform.position += -transform.right * 1 * Time.deltaTime;

        if (Vector3.Distance(player.gameObject.transform.position, transform.position) > 3.0f)
        {
            transform.position = bulletPos.transform.position;
            isShoot = false;
        }
	}
}
