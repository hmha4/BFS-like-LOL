using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            return g_Cost + h_Cost;
        }
    }
    public Node parent;
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
    Node before4AStar;
    Node neighbour;
    Node[] neighbourArray;

    //Move
    Stack<int> hashStack = new Stack<int>();
    Hashtable hash = new Hashtable();
    int before;
    int poppedStack;
    bool firstPop;
    bool isMoving;
    float moveX = 0;
    float moveY = 0;

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
        firstPop = false;
        isMoving = false;
        neighbourArray = new Node[cell];
        graph[(int)transform.position.y, (int)transform.position.x].gameObject.SetActive(false);
    }

    void Update()
    {
        //if (isMoving)
        //{
        //    if (!firstPop && hashStack.Count != 0)
        //    {
        //        poppedStack = hashStack.Pop();
        //        firstPop = true;
        //    }
        //    else
        //    {
        //        if (Vector3.Distance(transform.position, new Vector3(poppedStack % mapSize, poppedStack / mapSize, 0)) <= 0.4)
        //        {
        //            if (hashStack.Count != 0)
        //            {
        //                poppedStack = hashStack.Pop();
        //            }
        //        }
        //    }

        //    if (Vector3.Distance(transform.position, new Vector3(goalPos % mapSize, goalPos / mapSize)) <= 0.35)
        //    {
        //        print("arrive");
        //        transform.position = new Vector3(poppedStack % mapSize, poppedStack / mapSize, 0);
        //        transform.Translate(new Vector2(0, 0));
        //        firstPop = false;
        //        isMoving = false;
        //    }
        //    else
        //    {
        //        if (hashStack.Count >= 0)
        //        {
        //            if (poppedStack % mapSize > transform.position.x && poppedStack / mapSize > transform.position.y)
        //                transform.Translate(5 * Time.deltaTime, 5 * Time.deltaTime, 0);
        //            else if (poppedStack % mapSize > transform.position.x && poppedStack / mapSize == transform.position.y)
        //                transform.Translate(5 * Time.deltaTime, 0, 0);
        //            else if (poppedStack % mapSize > transform.position.x && poppedStack / mapSize < transform.position.y)
        //                transform.Translate(5 * Time.deltaTime, -5 * Time.deltaTime, 0);
        //            else if (poppedStack % mapSize == transform.position.x && poppedStack / mapSize < transform.position.y)
        //                transform.Translate(0, -5 * Time.deltaTime, 0);
        //            else if (poppedStack % mapSize < transform.position.x && poppedStack / mapSize < transform.position.y)
        //                transform.Translate(-5 * Time.deltaTime, -5 * Time.deltaTime, 0);
        //            else if (poppedStack % mapSize < transform.position.x && poppedStack / mapSize == transform.position.y)
        //                transform.Translate(-5 * Time.deltaTime, 0, 0);
        //            else if (poppedStack % mapSize < transform.position.x && poppedStack / mapSize > transform.position.y)
        //                transform.Translate(-5 * Time.deltaTime, 5 * Time.deltaTime, 0);
        //            else if (poppedStack % mapSize == transform.position.x && poppedStack / mapSize > transform.position.y)
        //                transform.Translate(0, 5 * Time.deltaTime, 0);
        //        }
        //    }
        //}


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

                                isMoving = true;
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
                                    for (int k = 0; k < neighbourArray.Length; k++)
                                        neighbourArray[k] = null;
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
                    {
                        hashStack.Pop();
                        break;
                    }
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
                    {
                        hashStack.Pop();
                        break;
                    }
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
        Node startNode = new Node { NodeCell = startPos};
        Node goalNode = new Node { NodeCell = goalPos };
        startNode.h_Cost = GetDistance(startNode, goalNode);
        open.Add(startNode);
        openList.Add(startNode.NodeCell);

        while(open.Count > 0)
        {
            Node current = open[0];
            for ( int i = 1; i < open.Count; i++)
            {
                if ((open[i].F_Cost < current.F_Cost || open[i].F_Cost == current.F_Cost)  && open[i].h_Cost < current.h_Cost)
                {
                    current = open[i];
                }
            }
            open.Remove(current);
            openList.Remove(current.NodeCell);
            closed.Add(current.NodeCell);

            if (current.NodeCell == goalNode.NodeCell || current.h_Cost == 0)
            {
                hashStack.Push(current.NodeCell);
                before4AStar = current.parent;
                hashStack.Push(before4AStar.NodeCell);

                Debug.DrawLine(graph[current.NodeCell / mapSize, current.NodeCell % mapSize].position, graph[before4AStar.NodeCell / mapSize, before4AStar.NodeCell % mapSize].position, Color.red, 3);

                if (before4AStar.NodeCell == startPos)
                    break;
                while (true)
                {
                    Debug.DrawLine(graph[before4AStar.NodeCell / mapSize, before4AStar.NodeCell % mapSize].position, graph[before4AStar.parent.NodeCell / mapSize, before4AStar.parent.NodeCell % mapSize].position, Color.red, 3);
                    before4AStar = before4AStar.parent;
                    hashStack.Push(before4AStar.NodeCell);

                    if (before4AStar.NodeCell == startPos)
                    {
                        hashStack.Pop();
                        break;
                    }
                }
                break;
            }

            for (int k = 0; k < 8; k++)
            {
                neighbour = new Node { NodeCell = current.NodeCell + direction[k] };
                

                if (Area(current.NodeCell, k)) continue;

                if (map[neighbour.NodeCell / mapSize, neighbour.NodeCell % mapSize] == 1 && !closed.Contains(neighbour.NodeCell))
                {
                    if (k == 1 || k == 3 || k == 5 || k == 7)
                    {
                        int numCheckPoint1 = current.NodeCell + direction[(k - 1) % 8];
                        int numCheckPoint2 = current.NodeCell + direction[(k + 1) % 8];

                        if (Area(current.NodeCell, k)) continue;
                        if (map[numCheckPoint1 / mapSize, numCheckPoint1 % mapSize] == 0 && map[numCheckPoint2 / mapSize, numCheckPoint2 % mapSize] == 0) continue;
                        
                    }
                    
                    if (neighbourArray[neighbour.NodeCell] == null)
                    {
                        neighbourArray[neighbour.NodeCell] = neighbour;
                    }
                    int newMoveCost = current.g_Cost + GetDistance(current, neighbour);
                    if(newMoveCost < neighbourArray[neighbour.NodeCell].g_Cost || !openList.Contains(neighbour.NodeCell))
                    {
                        neighbourArray[neighbour.NodeCell].g_Cost = newMoveCost;
                        neighbourArray[neighbour.NodeCell].h_Cost = GetDistance(neighbour, goalNode);
                        
                        neighbourArray[neighbour.NodeCell].parent = current;
                        if (!openList.Contains(neighbour.NodeCell))
                        {
                            open.Add(neighbourArray[neighbour.NodeCell]);
                            openList.Add(neighbour.NodeCell);
                        }
                    }
                }
            }
        }
    }

    //거리계산
    int GetDistance(Node nodeA, Node nodeB)
    {
        //Mathf.Abs 절대값
        int dstX = Mathf.Abs(nodeA.NodeCell % mapSize - nodeB.NodeCell % mapSize); 
        int dstY = Mathf.Abs(nodeA.NodeCell / mapSize - nodeB.NodeCell / mapSize);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
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
        graph[(int)transform.position.y, (int)transform.position.x].gameObject.SetActive(false);
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
