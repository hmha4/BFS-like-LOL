using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class gCostExtensionMethod
{
    public static int G_Cost(this int input)
    {
        return input;
    }
}

public static class hCostExtensionMethod
{
    public static int H_Cost(this int input)
    {
        return input;
    }
}

public static class fCostExtensionMethod
{
    public static int F_Cost(this int input)
    {
        return input.G_Cost() + input.H_Cost();
    }
}

public class Node
{
    public int NodeCell { get; set; }
    public int g_Cost;
    public int h_Cost;
    public int f_Cost;
    public int F_Cost
    {
        get
        {
            return f_Cost;
        }
        set
        {
            f_Cost = g_Cost + h_Cost;
        }
    }
}


public class NewBehaviourScript : MonoBehaviour
{
    //map
    public GameObject quad;
    public GameObject ground;
    public Transform[,] graph;
    int[,] map;
    public int mapSize;
    int[,] cellNumber;
    int cell = 0;
    int startPos;
    Vector2 mousePos;
    int goalPos;
    int[] direction = new int[8];
    public int randomNumSize;
    int randomNum1;
    int randomNum2;
    int tempNum1 = 0;
    int tempNum2 = 0;
    int currentCellNumber;

    //BFS
    Queue<int> q = new Queue<int>();

    //DFS
    Stack<int> DFSStack = new Stack<int>();

    //A*
    List<Node> open = new List<Node>();
    List<int> openList = new List<int>();
    List<int> closed = new List<int>();

    //Move
    Stack<int> hashStack = new Stack<int>();
    Hashtable hash = new Hashtable();
    int before;
    int poppedStack;

    //Algorithm selection
    bool isBFS;
    bool isDFS;
    bool isAStar;

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
                GameObject groundInst = Instantiate(ground);
                groundInst.transform.position = new Vector2(j, i);
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
        isBFS = false;
        isDFS = false;
        isAStar = false;
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
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.name == "ground(Clone)")
                {
                    for (int i = 0; i < mapSize; i++)
                    {
                        for (int j = 0; j < mapSize; j++)
                        {
                            if (hit.point.x > i - .5f && hit.point.x < i + .5f &&
                                hit.point.y < j + .5f && hit.point.y > j - .5f)
                            {
                                startPos = ReturnCellNumber((int)transform.position.y, (int)transform.position.x);
                                goalPos = ReturnCellNumber(j, i);

                                hash.Clear();
                                hashStack.Clear();
                                if (isBFS)
                                {
                                    q.Clear();
                                    BFS(startPos, goalPos);
                                }
                                if (isDFS)
                                {
                                    DFSStack.Clear();
                                    DFS(startPos, goalPos);
                                }
                                if (isAStar)
                                {
                                    open.Clear();
                                    openList.Clear();
                                    closed.Clear();
                                    AStar(startPos, goalPos);
                                }
                            }

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

                if (Area(currentCellNumber, k)) continue;

                if (map[nextCellNumber / mapSize, nextCellNumber % mapSize] == 1)
                {
                    if (k == 1 || k == 3 || k == 5 || k == 7)
                    {
                        int numCheckPoint1 = currentCellNumber + direction[(k - 1)%8];
                        int numCheckPoint2 = currentCellNumber + direction[(k + 1)%8];
                        
                        if (Area(nextCellNumber, k)) continue;

                        if (map[numCheckPoint1 / mapSize, numCheckPoint1 % mapSize] == 0 && map[numCheckPoint2 / mapSize, numCheckPoint2 % mapSize] == 0) continue;
                    }
                    
                    q.Enqueue(nextCellNumber);
                    map[nextCellNumber / mapSize, nextCellNumber % mapSize] = map[currentCellNumber / mapSize, currentCellNumber % mapSize] + 1;
                    hash.Add(nextCellNumber, currentCellNumber);
                }
            }
        }
    }

    void DFS(int startPos, int goalPos)
    {
        currentCellNumber = startPos;
        DFSStack.Push(currentCellNumber);
        map[currentCellNumber / mapSize, currentCellNumber % mapSize] = 1;

        while (DFSStack.Count != 0)
        {
            currentCellNumber = DFSStack.Pop();
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

                if (Area(currentCellNumber, k)) continue;

                if (map[nextCellNumber / mapSize, nextCellNumber % mapSize] == 1)
                {
                    if (k == 1 || k == 3 || k == 5 || k == 7)
                    {
                        int numCheckPoint1 = currentCellNumber + direction[(k - 1) % 8];
                        int numCheckPoint2 = currentCellNumber + direction[(k + 1) % 8];

                        if (Area(nextCellNumber, k)) continue;

                        if (map[numCheckPoint1 / mapSize, numCheckPoint1 % mapSize] == 0 && map[numCheckPoint2 / mapSize, numCheckPoint2 % mapSize] == 0) continue;
                    }

                    DFSStack.Push(nextCellNumber);
                    map[nextCellNumber / mapSize, nextCellNumber % mapSize] = map[currentCellNumber / mapSize, currentCellNumber % mapSize] + 1;
                    hash.Add(nextCellNumber, currentCellNumber);
                }
            }
        }
    }
    
    void AStar(int startPos, int goalPos)
    {
        Node startNode = new Node { NodeCell = startPos };
        Node goalNode = new Node { NodeCell = goalPos };

        open = new List<Node>();
        openList = new List<int>();
        closed = new List<int>();
        open.Add(startNode);
        openList.Add(startNode.NodeCell);

        while(open.Count > 0)
        {
            Node current = open[0];
            for ( int i = 1; i < open.Count; i++)
            {
                if (open[i].F_Cost <= current.F_Cost && open[i].h_Cost < current.h_Cost)
                    current = open[i];
            }
            
            open.Remove(current);
            openList.Remove(current.NodeCell);
            closed.Add(current.NodeCell);
            for(int m = 0; m < closed.Count; m++)
                print("closed" + closed[m]);
            for (int n = 0; n < open.Count; n++)
                print("open" + open[n].NodeCell);

            if (current.NodeCell == goalNode.NodeCell)
            {
                hashStack.Push(current.NodeCell);
                before = (int)hash[current.NodeCell];
                hashStack.Push(before);

                Debug.DrawLine(graph[current.NodeCell / mapSize, current.NodeCell % mapSize].position, graph[before / mapSize, before % mapSize].position, Color.red, 3);

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
                //break;
            }

            for (int k = 0; k < 8; k++)
            {
                Node neighbour = new Node { NodeCell = current.NodeCell + direction[k] };

                if (Area(current.NodeCell, k)) continue;

                if (map[neighbour.NodeCell / mapSize, neighbour.NodeCell % mapSize] == 1 && !closed.Contains(neighbour.NodeCell))
                {
                    if (k == 1 || k == 3 || k == 5 || k == 7)
                    {
                        int numCheckPoint1 = current.NodeCell + direction[(k - 1) % 8];
                        int numCheckPoint2 = current.NodeCell + direction[(k + 1) % 8];

                        if (Area(neighbour.NodeCell, k)) continue;

                        if (map[numCheckPoint1 / mapSize, numCheckPoint1 % mapSize] == 0 && map[numCheckPoint2 / mapSize, numCheckPoint2 % mapSize] == 0) continue;
                    }
                    int newMoveCost;
                    if (k == 0 || k == 2 || k == 4 || k == 6)
                        newMoveCost = current.g_Cost + 10;
                    else
                        newMoveCost = current.g_Cost + 14;

                    if(newMoveCost < neighbour.g_Cost || !openList.Contains(neighbour.NodeCell))
                    {
                        neighbour.g_Cost = newMoveCost;
                        neighbour.h_Cost = (int)Vector3.Distance(new Vector3(neighbour.NodeCell % mapSize, neighbour.NodeCell / mapSize, 0), new Vector3(goalNode.NodeCell % mapSize, goalNode.NodeCell / mapSize, 0));
                        print(current.NodeCell + " " + neighbour.NodeCell + " " + neighbour.g_Cost);
                        
                        if (!openList.Contains(neighbour.NodeCell))
                        {
                            print(open.Contains(neighbour));
                            open.Add(neighbour);
                            openList.Add(neighbour.NodeCell);
                            
                            print("added");
                            hash.Add(neighbour.NodeCell, current.NodeCell);
                        }
                    }
                }
            }
        }
    }

    bool Area(int cellNum, int k)
    {
        if (cellNum % mapSize == 0) if (k == 3 || k == 4 || k == 5) return true;            //왼쪽
        if (cellNum % mapSize == mapSize - 1) if (k == 0 || k == 1 || k == 7) return true;  //오른쪽
        if (cellNum / mapSize == 0) if (k == 1 || k == 2 || k == 3) return true;            //아래쪽
        if (cellNum / mapSize == mapSize - 1) if (k == 5 || k == 6 || k == 7) return true;  //위쪽

        return false;
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

    public void Click()
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
    public void DoBFS()
    {
        isBFS = true;
        isDFS = false;
        isAStar = false;
    }
    public void DoDFS()
    {
        isBFS = false;
        isDFS = true;
        isAStar = false;
    }
    public void DoAStar()
    {
        isBFS = false;
        isDFS = false;
        isAStar = true;
    }
}


//if (Input.GetButtonDown("Fire1"))
//{
//    mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
//    print(mousePos);
//    for (int i = 0; i < mapSize; i++)
//    {
//        for (int j = 0; j < mapSize; j++)
//        {
//            if (mousePos.x > i - .5f && mousePos.x < i + .5f &&
//                mousePos.y < j + .5f && mousePos.y > j - .5f)
//            {
//                if (graph[j, i].gameObject.activeSelf == false)
//                {
//                    hash.Clear();
//                    q.Clear();
//                    hashStack.Clear();

//                    startPos = ReturnCellNumber((int)transform.position.y, (int)transform.position.x);
//                    goalPos = ReturnCellNumber(j, i);

//                    BFS(startPos, goalPos);
//                }
//            }
//        }
//    } 
//}
