using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapGenerator : MonoBehaviour {
    NewBehaviourScript map;
    private Vector2 mousePos;
    Rect rect;
    int[, ] cellNum = new int[10, 10];

    void Awake()
    {
        map = GameObject.Find("Sphere").GetComponent<NewBehaviourScript>();
    }

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        
        if(Input.GetButtonDown("Fire1"))
        {
            mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
            for (int i = 0; i < map.mapSize; i++)
            {
                for (int j = 0; j < map.mapSize; j++)
                {
                    if (mousePos.x > i - .5f && mousePos.x < i + .5f &&
                        mousePos.y < j + .5f && mousePos.y > j - .5f)
                    {
                        Debug.DrawLine(new Vector3(i - .5f, j - .5f, 0), new Vector3(i + .5f, j - .5f, 0), Color.red, 1);
                        Debug.DrawLine(new Vector3(i - .5f, j - .5f, 0), new Vector3(i - .5f, j + .5f, 0), Color.red, 1);
                        Debug.DrawLine(new Vector3(i - .5f, j + .5f, 0), new Vector3(i + .5f, j + .5f, 0), Color.red, 1);
                        Debug.DrawLine(new Vector3(i + .5f, j + .5f, 0), new Vector3(i + .5f, j - .5f, 0), Color.red, 1);
                    }
                }
            }
        }  
    }

    
}
