using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Save : MonoBehaviour {
    NewBehaviourScript map;
    // Use this for initialization
    void Start () {
        map = GameObject.Find("Sphere").GetComponent<NewBehaviourScript>();

        for(int i = 0; i < map.cell; i++)
        {
            if (PlayerPrefs.HasKey("Tree" + i))
            {
                int m = PlayerPrefs.GetInt("Tree" + i) / map.mapSize;
                int n = PlayerPrefs.GetInt("Tree" + i) % map.mapSize;

                
                GameObject instance = Instantiate(map.quad);
                instance.transform.position = new Vector3(n, m, -0.5f);
                map.graph[m, n] = instance.transform;
            }
            if(PlayerPrefs.HasKey("Rock" + i))
            {
                int m = PlayerPrefs.GetInt("Rock" + i) / map.mapSize;
                int n = PlayerPrefs.GetInt("Rock" + i) % map.mapSize;

                GameObject rockInst = Instantiate(map.rock);
                rockInst.transform.position = new Vector3(m + 0.5f, n + 0.5f, 0.5f);

                map.rockGraph[m, n] = rockInst.transform;
            }
            if(PlayerPrefs.HasKey("Water1" + i))
            {
                int m = PlayerPrefs.GetInt("Water1" + i) / map.mapSize;
                int n = PlayerPrefs.GetInt("Water1" + i) % map.mapSize;

                GameObject waterInst = Instantiate(map.water);
                waterInst.transform.position = new Vector3(n + 0.5f, m, -0.01f);

                map.waterGraph1[m, n] = waterInst.transform;
            }
            if(PlayerPrefs.HasKey("Water2" + i))
            {
                int m = PlayerPrefs.GetInt("Water2" + i) / map.mapSize;
                int n = PlayerPrefs.GetInt("Water2" + i) % map.mapSize;

                GameObject waterInst = Instantiate(map.water);
                waterInst.transform.position = new Vector3(n, m + 0.5f, -0.01f);
                waterInst.transform.Rotate(0, 90, 0);

                map.waterGraph2[m, n] = waterInst.transform;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        //Player Position
        PlayerPrefs.SetFloat("PlayerX", map.gameObject.transform.position.x);
        PlayerPrefs.SetFloat("PlayerY", map.gameObject.transform.position.y);
        PlayerPrefs.SetFloat("PlayerZ", map.gameObject.transform.position.z);

        //Obstacles
        

        PlayerPrefs.Save();
    }
}
