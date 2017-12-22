using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrooundManager : MonoBehaviour {
    public bool unmovable;

    // Use this for initialization
    void Start () {
        unmovable = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("rock"))
        {
            unmovable = true;
        }

        if(other.gameObject.CompareTag("water"))
        {
            unmovable = true;
        }
    }
    

}
