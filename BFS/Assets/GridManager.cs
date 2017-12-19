using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {
    Vector2 mousePos;
    NewBehaviourScript map;
	// Use this for initialization
	void Start () {
        map = GameObject.Find("Sphere").GetComponent<NewBehaviourScript>();
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
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
                                if (map.graph[j, i].gameObject.activeSelf == false)
                                    map.graph[j, i].gameObject.SetActive(true);
                                else
                                    map.graph[j, i].gameObject.SetActive(false);
                            }
                        }
                    }
                }
            }    
        }
    }
}

//mousePos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));

//for (int i = 0; i < map.mapSize; i++)
//{
//    for (int j = 0; j < map.mapSize; j++)
//    {
//        if (mousePos.x > i - .5f && mousePos.x < i + .5f &&
//            mousePos.y < j + .5f && mousePos.y > j - .5f)
//        {
//            if(map.graph[j, i].gameObject.activeSelf == false)
//                map.graph[j, i].gameObject.SetActive(true);
//            else
//                map.graph[j, i].gameObject.SetActive(false);
//        }
//    }
//}
