using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapGenerator : MonoBehaviour {
    NewBehaviourScript map;
    private Vector2 mousePos;
    Rect rect;

    void Awake()
    {
        map = GameObject.Find("Sphere").GetComponent<NewBehaviourScript>();
    }

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        //for (int i = 0; i < map.mapSize; i++)
        //{
        //    for (int j = 0; j < map.mapSize; j++)
        //    {
        //        Debug.DrawLine(new Vector3(i - .5f, j - .5f, 0), new Vector3(i + .5f, j - .5f, 0), Color.blue);
        //        Debug.DrawLine(new Vector3(i - .5f, j - .5f, 0), new Vector3(i - .5f, j + .5f, 0), Color.blue);
        //        Debug.DrawLine(new Vector3(i - .5f, j + .5f, 0), new Vector3(i + .5f, j + .5f, 0), Color.blue);
        //        Debug.DrawLine(new Vector3(i + .5f, j + .5f, 0), new Vector3(i + .5f, j - .5f, 0), Color.blue);
        //    }
        //}
        if (Input.GetButtonDown("Fire1"))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.name == "ground(Clone)")
                {
                    for (int i = 0; i < map.mapSize; i++)
                    {
                        for (int j = 0; j < map.mapSize; j++)
                        {
                            if (hit.point.x > i - .5f && hit.point.x < i + .5f &&
                                hit.point.y < j + .5f && hit.point.y > j - .5f)
                            {
                                if (map.graph[j, i] != null)
                                {
                                    Debug.DrawLine(new Vector3(i - .5f, j - .5f, 0), new Vector3(i + .5f, j - .5f, 0), Color.yellow, 1);
                                    Debug.DrawLine(new Vector3(i - .5f, j - .5f, 0), new Vector3(i - .5f, j + .5f, 0), Color.yellow, 1);
                                    Debug.DrawLine(new Vector3(i - .5f, j + .5f, 0), new Vector3(i + .5f, j + .5f, 0), Color.yellow, 1);
                                    Debug.DrawLine(new Vector3(i + .5f, j + .5f, 0), new Vector3(i + .5f, j - .5f, 0), Color.yellow, 1);
                                }
                                else
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
        }  
    }
}
