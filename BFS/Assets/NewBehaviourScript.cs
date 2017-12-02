using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject quad;
    public Transform[,] graph;
    int[,] map;
    public int mapSize;
    int[,] cellNumber;
    int cell = 0;

    Queue<int> q = new Queue<int>();
    int startPos;
    Vector2 mousePos;
    int goalPos;
    int currentCellNumber;

    int[] direction = new int[8];

    Stack<int> hashStack = new Stack<int>();
    Hashtable hash = new Hashtable();
    int before;
    int poppedStack;

    public int randomNumSize;
    int randomNum1;
    int randomNum2;
    int tempNum1 = 0;
    int tempNum2 = 0;

    private void Awake()
    {
        graph = new Transform[mapSize, mapSize];
        cellNumber = new int[mapSize, mapSize];
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                GameObject instance = Instantiate(quad);
                instance.transform.position = new Vector2(j, i);
                graph[i, j] = instance.transform;
                graph[i, j].gameObject.SetActive(false);
                cellNumber[i, j] = cell++;
            }
        }
        for (int i = 0; i < randomNumSize; i++)
        {
            randomNum1 = Random.Range(0, mapSize);
            randomNum2 = Random.Range(0, mapSize);
            while (randomNum1 == tempNum1) randomNum1 = Random.Range(0, mapSize);
            while (randomNum2 == tempNum2) randomNum2 = Random.Range(0, mapSize);
            tempNum1 = randomNum1;
            tempNum2 = randomNum2;

            graph[randomNum1, randomNum2].gameObject.SetActive(true);
        }
    }
    // Use this for initialization
    void Start()
    {
        map = new int[mapSize, mapSize];
        direction[0] = 1;
        direction[1] = -mapSize + 1;
        direction[2] = -mapSize;
        direction[3] = -mapSize - 1;
        direction[4] = -1;
        direction[5] = mapSize - 1;
        direction[6] = mapSize;
        direction[7] = mapSize + 1;
        graph[(int)transform.position.y, (int)transform.position.x].gameObject.SetActive(false);
        StartCoroutine(Move());
    }

    void Update()
    {
        graph[(int)transform.position.y, (int)transform.position.x].gameObject.SetActive(false);
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                if (graph[i, j].gameObject.activeSelf)
                {
                    map[i, j] = 0;
                }
                else
                    map[i, j] = 1;
            }
        }
        
        if (Input.GetButtonDown("Fire1"))
        {
            
            mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (mousePos.x > i - .5f && mousePos.x < i + .5f &&
                        mousePos.y < j + .5f && mousePos.y > j - .5f)
                    {
                        if (graph[j, i].gameObject.activeSelf == false)
                        {
                            hash.Clear();
                            q.Clear();
                            hashStack.Clear();

                            startPos = ReturnCellNumber((int)transform.position.y, (int)transform.position.x);
                            goalPos = ReturnCellNumber(j, i);
                            
                            BFS(startPos, goalPos);
                        }
                    }
                }
            }
            
        }
    }

    void BFS(int startPos, int goalPos)
    {
        currentCellNumber = startPos;
        q.Enqueue(currentCellNumber);
        map[currentCellNumber / mapSize, currentCellNumber % mapSize] = 1;

        while (q.Count != 0)
        {
            currentCellNumber = q.Dequeue();
            if (currentCellNumber == goalPos)
            {
                hashStack.Push(currentCellNumber);
                before = (int)hash[currentCellNumber];
                hashStack.Push(before);

                Debug.DrawLine(graph[currentCellNumber / mapSize, currentCellNumber % mapSize].position, graph[before / mapSize, before % mapSize].position, Color.red, 3);

                if (before == startPos)
                    break;
                while (true)
                {
                    Debug.DrawLine(graph[before / mapSize, before % mapSize].position, graph[(int)hash[before] / mapSize, (int)hash[before] % mapSize].position, Color.red, 3);
                    before = (int)hash[before];
                    hashStack.Push(before);


                    if (before == startPos)
                        break;
                }
            }

            for (int k = 0; k < 8; k++)
            {
                int nextCellNumber = currentCellNumber + direction[k];

                if (currentCellNumber % mapSize == 0) if (k == 3 || k == 4 || k == 5) continue;            //왼쪽
                if (currentCellNumber % mapSize == mapSize - 1) if (k == 0 || k == 1 || k == 7) continue;  //오른쪽
                if (currentCellNumber / mapSize == 0) if (k == 1 || k == 2 || k == 3) continue;            //아래쪽
                if (currentCellNumber / mapSize == mapSize - 1) if (k == 5 || k == 6 || k == 7) continue;  //위쪽

                if (map[nextCellNumber / mapSize, nextCellNumber % mapSize] == 1)
                {
                    if (k == 1 || k == 3 || k == 5 || k == 7)
                    {
                        if (nextCellNumber % mapSize == 0) if (k == 3 || k == 4 || k == 5) continue;            //왼쪽
                        if (nextCellNumber % mapSize == mapSize - 1) if (k == 0 || k == 1 || k == 7) continue;  //오른쪽
                        if (nextCellNumber / mapSize == 0) if (k == 1 || k == 2 || k == 3) continue;            //아래쪽
                        if (nextCellNumber / mapSize == mapSize - 1) if (k == 5 || k == 6 || k == 7) continue;  //위쪽

                        if ((map[(nextCellNumber - mapSize) / mapSize, (nextCellNumber - mapSize) % mapSize] == 0 && map[(nextCellNumber - 1) / mapSize, (nextCellNumber - 1) % mapSize] == 0 ||
                        map[(nextCellNumber + mapSize) / mapSize, (nextCellNumber + mapSize) % mapSize] == 0 && map[(nextCellNumber - 1) / mapSize, (nextCellNumber - 1) % mapSize] == 0 ||
                        map[(nextCellNumber + 1) / mapSize, (nextCellNumber + 1) % mapSize] == 0 && map[(nextCellNumber + mapSize) / mapSize, (nextCellNumber + mapSize) % mapSize] == 0 ||
                        map[(nextCellNumber - mapSize) / mapSize, (nextCellNumber - mapSize) % mapSize] == 0 && map[(nextCellNumber + 1) / mapSize, (nextCellNumber + 1) % mapSize] == 0))
                        {
                            continue;
                        }
                    }
                    q.Enqueue(nextCellNumber);
                    map[nextCellNumber / mapSize, nextCellNumber % mapSize] = map[currentCellNumber / mapSize, currentCellNumber % mapSize] + 1;
                    hash.Add(nextCellNumber, currentCellNumber);
                }
            }
        }
    }
    void Click()
    {
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                graph[i, j].gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < randomNumSize; i++)
        {
            randomNum1 = Random.Range(0, 10);
            randomNum2 = Random.Range(0, 10);
            while (randomNum1 == tempNum1) randomNum1 = Random.Range(0, 10);
            while (randomNum2 == tempNum2) randomNum2 = Random.Range(0, 10);
            tempNum1 = randomNum1;
            tempNum2 = randomNum2;

            graph[randomNum1, randomNum2].gameObject.SetActive(true);
        }
    }

    int ReturnCellNumber(int x, int y)
    {
        return cellNumber[x, y];
    }

    IEnumerator Move()
    {
        while (true)
        {
            if (hashStack.Count != 0)
            {
                poppedStack = hashStack.Pop();
                transform.position = new Vector3(poppedStack % mapSize, poppedStack / mapSize, 0);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
