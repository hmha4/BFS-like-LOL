using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {
    Vector2 mousePos;
    NewBehaviourScript map;
    bool isTree;
    bool isRock;
    bool isWater;
    bool isWaterHorizontal;
    
  
    // Use this for initialization
    void Start () {
        map = GameObject.Find("Sphere").GetComponent<NewBehaviourScript>();
        isTree = false;
        isRock = false;
        isWater = false;
        isWaterHorizontal = false;
        
        //cameraRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(!map.isMoving)
        { 
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKey(KeyCode.A))
                {
                    transform.RotateAround(map.transform.position, Vector3.forward, 40 * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.D))
                    transform.RotateAround(map.transform.position, Vector3.back, 40 * Time.deltaTime);
            }
        }

        if (Input.GetButtonDown("Fire2"))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (isTree)
                {
                    if (hit.collider.gameObject.name == "Tree(Clone)")
                    {
                        int numX = (int)hit.collider.gameObject.transform.position.x;
                        int numY = (int)hit.collider.gameObject.transform.position.y;
                        Destroy(hit.collider.gameObject);

                        map.map[numY, numX] = 0;

                        PlayerPrefs.DeleteKey("Tree" + map.ReturnCellNumber(numY, numX));
                    }
                }
                if (isRock)
                {
                    if (hit.collider.gameObject.name == "rock(Clone)")
                    {
                        int numX = (int)hit.collider.gameObject.transform.position.x;
                        int numY = (int)hit.collider.gameObject.transform.position.y;
                        Destroy(hit.collider.gameObject);

                        map.groundGraph[numY, numX].gameObject.GetComponent<GrooundManager>().unmovable = false;
                        map.groundGraph[numY, numX + 1].gameObject.GetComponent<GrooundManager>().unmovable = false;
                        map.groundGraph[numY + 1, numX].gameObject.GetComponent<GrooundManager>().unmovable = false;
                        map.groundGraph[numY + 1, numX + 1].gameObject.GetComponent<GrooundManager>().unmovable = false;

                        PlayerPrefs.DeleteKey("Rock" + map.ReturnCellNumber(numX, numY));
                    }
                }
                if (isWater)
                {
                    if (hit.collider.gameObject.name == "Water(Clone)")
                    {
                        if (isWaterHorizontal)
                        {
                            int numX = (int)hit.collider.gameObject.transform.position.x;
                            int numY = (int)hit.collider.gameObject.transform.position.y;

                            map.groundGraph[numY, numX].gameObject.GetComponent<GrooundManager>().unmovable = false;
                            map.groundGraph[numY, numX + 1].gameObject.GetComponent<GrooundManager>().unmovable = false;

                            Destroy(hit.collider.gameObject);

                            PlayerPrefs.DeleteKey("Water1" + map.ReturnCellNumber(numY, numX));
                        }
                        if (!isWaterHorizontal)
                        {
                            int numX = (int)hit.collider.gameObject.transform.position.x;
                            int numY = (int)hit.collider.gameObject.transform.position.y;

                            map.groundGraph[numY, numX].gameObject.GetComponent<GrooundManager>().unmovable = false;
                            map.groundGraph[numY + 1, numX].gameObject.GetComponent<GrooundManager>().unmovable = false;

                            Destroy(hit.collider.gameObject);

                            PlayerPrefs.DeleteKey("Water2" + map.ReturnCellNumber(numY, numX));
                        }
                    }
                }
                if (hit.collider.gameObject.name == "ground(Clone)")
                {
                    if (isTree)
                    {
                        for (int i = 0; i < map.mapSize; i++)
                        {
                            for (int j = 0; j < map.mapSize; j++)
                            {
                                if (hit.point.x > i - .5f && hit.point.x < i + .5f &&
                                    hit.point.y < j + .5f && hit.point.y > j - .5f)
                                {
                                    if (map.graph[j, i] == null)
                                    {
                                        GameObject instance = Instantiate(map.quad);
                                        instance.transform.position = new Vector3(i, j, -0.5f);
                                        map.graph[j, i] = instance.transform;

                                        PlayerPrefs.SetInt("Tree" + map.ReturnCellNumber(j, i), map.ReturnCellNumber(j, i));
                                    }
                                }
                            }
                        }
                    }
                    if (isRock)
                    {
                        for (int i = 0; i < map.mapSize; i += 2)
                        {
                            for (int j = 0; j < map.mapSize; j += 2)
                            {
                                if (hit.point.x > i - .5f && hit.point.x < i + 1.5f &&
                                    hit.point.y < j + 1.5f && hit.point.y > j - .5f)
                                {
                                    if (map.graph[j, i] == null && map.graph[j, i + 1] == null &&
                                       map.graph[j + 1, i] == null && map.graph[j + 1, i + 1] == null)
                                    {
                                        GameObject rockInst = Instantiate(map.rock);
                                        rockInst.transform.position = new Vector3(i + 0.5f, j + 0.5f, 0.5f);

                                        map.rockGraph[i, j] = rockInst.transform;

                                        PlayerPrefs.SetInt("Rock" + map.ReturnCellNumber(i, j), map.ReturnCellNumber(i, j));
                                    }
                                    
                                }
                            }
                        }
                    }
                    if (isWater)
                    {
                        if (isWaterHorizontal)
                        {
                            for (int i = 0; i < map.mapSize; i++)
                            {
                                for (int j = 0; j < map.mapSize; j += 2)
                                {
                                    if (hit.point.x > j - .5f && hit.point.x < j + 1.5f &&
                                        hit.point.y < i + .5f && hit.point.y > i - .5f)
                                    {
                                        if (map.graph[i, j] == null && map.graph[i, j + 1] == null)
                                        {
                                            GameObject waterInst = Instantiate(map.water);
                                            waterInst.transform.position = new Vector3(j + 0.5f, i, -0.01f);

                                            map.waterGraph1[i, j] = waterInst.transform;

                                            PlayerPrefs.SetInt("Water1" + map.ReturnCellNumber(i, j), map.ReturnCellNumber(i, j));
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < map.mapSize; i += 2)
                            {
                                for (int j = 0; j < map.mapSize; j++)
                                {
                                    if (hit.point.x > j - .5f && hit.point.x < j + .5f &&
                                        hit.point.y < i + 1.5f && hit.point.y > i - .5f)
                                    {
                                        if (map.graph[i, j] == null && map.graph[i + 1, j] == null)
                                        {
                                            GameObject waterInst = Instantiate(map.water);
                                            waterInst.transform.position = new Vector3(j, i + 0.5f, -0.01f);
                                            waterInst.transform.Rotate(0, 90, 0);

                                            map.waterGraph2[i, j] = waterInst.transform;

                                            PlayerPrefs.SetInt("Water2" + map.ReturnCellNumber(i, j), map.ReturnCellNumber(i, j));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }    
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            isTree = true;
            isRock = false;
            isWater = false;
            isWaterHorizontal = false;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            isTree = false;
            isRock = true;
            isWater = false;
            isWaterHorizontal = false;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            isTree = false;
            isRock = false;
            isWater = true;

            if (!isWaterHorizontal)
                isWaterHorizontal = true;
            else
                isWaterHorizontal = false;
        }
        PlayerPrefs.Save();
    }
}

