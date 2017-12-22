using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A* 계살할 때 사용되는 Node Class
public class Node
{
    public int NodeCell { get; set; }       //각 Node CellNumber
    public int g_Cost;                      //캐릭터와 Node와의 거리
    public int h_Cost;                      //목표점과 Node와이 거리
    public int f_Cost;                      //가중치(g_Cost + h_Cost)
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
    public GameObject quad;          //나무
    public Transform[,] graph;       //나무 Transform
    public GameObject rock;          //바위
    public Transform[,] rockGraph;   //바위 Transform
    public GameObject ground;        //땅
    public Transform[,] groundGraph; //땅 Transform
    public GameObject water;         //물
    public Transform[,] waterGraph1;  //물 Transform 가로
    public Transform[,] waterGraph2;  //물 Transform 세로

    public int[,] map;                      //Map : 0, 1로 구분
    int[,] cellNumber;               //각 Map의 CellNumber
    public int mapSize;              //Map 크기
    public int cell = 0;
    int startPos;                    //시작 CellNumber
    int goalPos;                     //목표 CellNumber
    Vector2 mousePos;                //Mouse Position
    public int[] direction = new int[8];    //이동 방향

    //Map Reorder할 때 랜덤 장애물 크기
    public int randomNumSize;        
    int randomNum1;
    int randomNum2;
    int tempNum1 = 0;
    int tempNum2 = 0;
    
    //BFS
    Queue<int> q = new Queue<int>();

    //DFS
    Stack<int> DFSStack = new Stack<int>();

    int currentCellNumber; //BFS, DFS 계산할 때 사용

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
    public bool isMoving;
    public int speed;

    //Algorithm selection
    bool isBFS;
    bool isDFS;
    bool isAStar;

    GrooundManager[,] GM;
    GameObject Player;
    GameObject[] Bullet = new GameObject[10];
    int bulletNum = 9;
    float time = 0;
    float gunTime = 0;
    bool reload = false;
    bool gunShoot = false;
    bool isGun = true;
    public bool isGrenade;

    private void Awake()
    {
        GM = new GrooundManager[mapSize, mapSize];
        graph = new Transform[mapSize, mapSize];
        rockGraph = new Transform[mapSize, mapSize];
        groundGraph = new Transform[mapSize, mapSize];
        waterGraph1 = new Transform[mapSize, mapSize];
        waterGraph2 = new Transform[mapSize, mapSize];
        cellNumber = new int[mapSize, mapSize];
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                //GameObject instance = Instantiate(quad);
                //instance.transform.position = new Vector3(j, i, -0.5f);
                GameObject groundInst = Instantiate(ground);
                groundInst.transform.position = new Vector3(j, i, 0);

                groundGraph[i, j] = groundInst.transform;

                graph[i, j] = null;
                //graph[i, j] = instance.transform;
                //graph[i, j].gameObject.SetActive(false);
                cellNumber[i, j] = cell++;
                GM[i,j] = groundGraph[i, j].gameObject.GetComponent<GrooundManager>();
            }
        }
        Player = transform.Find("Hero").gameObject;
        for (int i = 0; i < Bullet.Length; i++)
            Bullet[i] = GameObject.Find("Bullet").transform.Find("Bullet (" + i + ")").gameObject;
        //for (int i = 0; i < randomNumSize; i++)
        //{
        //    randomNum1 = Random.Range(0, mapSize);
        //    randomNum2 = Random.Range(0, mapSize);
        //    while (randomNum1 == tempNum1) randomNum1 = Random.Range(0, mapSize);
        //    while (randomNum2 == tempNum2) randomNum2 = Random.Range(0, mapSize);
        //    tempNum1 = randomNum1;
        //    tempNum2 = randomNum2;

        //    graph[randomNum1, randomNum2].gameObject.SetActive(true);
        //}
        
    }
    // Use this for initialization
    void Start()
    {
        isGrenade = false;
        transform.position = new Vector3(PlayerPrefs.GetFloat("PlayerX"), PlayerPrefs.GetFloat("PlayerY"), PlayerPrefs.GetFloat("PlayerZ"));
        map = new int[mapSize, mapSize];
        direction[0] = 1;
        direction[1] = -mapSize + 1;
        direction[2] = -mapSize;
        direction[3] = -mapSize - 1;
        direction[4] = -1;
        direction[5] = mapSize - 1;
        direction[6] = mapSize;
        direction[7] = mapSize + 1;
        
        isBFS = false;
        isDFS = false;
        isAStar = false;
        //StartCoroutine(Move());
        firstPop = false;
        isMoving = false;
        neighbourArray = new Node[cell];
    }

    void Update()
    {
        CharacterMove();

        //Map 0, 1 구분
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                if (graph[i, j] != null)
                {
                    map[i, j] = 0;
                }
                else
                    map[i, j] = 1;

                if (GM[i, j].unmovable == true)
                {
                    map[i, j] = 0;
                }
            }
        }
        if(Input.GetKeyDown(KeyCode.Q))
        {
            if (isGun)
            {
                isGun = false;
                isGrenade = true;
            }
            else
            {
                isGun = true;
                isGrenade = false;
            }
        }
        if (isGun)
        {
            if (Input.GetKey(KeyCode.Z))
            {
                gunShoot = true;
                Player.GetComponent<Animator>().SetBool("isShoot", true);
                if (Input.GetKey(KeyCode.X))
                {
                    if (bulletNum >= 0)
                    {
                        if (gunTime >= 0.1f)
                        {
                            Bullet[bulletNum--].GetComponent<BulletMove>().isShoot = true;
                            gunTime = 0;
                        }
                    }

                }
            }
            if (gunShoot)
                gunTime += Time.deltaTime;
            else
                gunTime = 0;
            if (Input.GetKeyUp(KeyCode.Z))
            {
                Player.GetComponent<Animator>().SetBool("isShoot", false);
                gunShoot = false;
            }
            if (bulletNum < 0)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    reload = true;
                }
                if (reload)
                {
                    time += Time.deltaTime;
                    Player.GetComponent<Animator>().SetBool("needReload", true);
                    if (time > 1.20f)
                    {
                        bulletNum = 9;
                        Player.GetComponent<Animator>().SetBool("needReload", false);
                        time = 0;
                        reload = false;
                    }
                }
            }
        }
        //Mouse Click
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

                                if (map[goalPos / mapSize, goalPos % mapSize] == 1 && !Input.GetKey(KeyCode.LeftControl))
                                {
                                    hash.Clear();
                                    hashStack.Clear();
                                    if (isBFS)
                                    {
                                        isMoving = true;
                                        q.Clear();
                                        BFS(startPos, goalPos);
                                    }
                                    if (isDFS)
                                    {
                                        isMoving = true;
                                        DFSStack.Clear();
                                        DFS(startPos, goalPos);
                                    }
                                    if (isAStar)
                                    {
                                        isMoving = true;
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
    }

    //BFS 계산
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

                Debug.DrawLine(groundGraph[currentCellNumber / mapSize, currentCellNumber % mapSize].position, groundGraph[before / mapSize, before % mapSize].position, Color.red, 3);

                if (before == startPos)
                    break;
                while (true)
                {
                    Debug.DrawLine(groundGraph[before / mapSize, before % mapSize].position, groundGraph[(int)hash[before] / mapSize, (int)hash[before] % mapSize].position, Color.red, 3);
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
    //DFS 계산
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

                Debug.DrawLine(groundGraph[currentCellNumber / mapSize, currentCellNumber % mapSize].position, groundGraph[before / mapSize, before % mapSize].position, Color.red, 3);

                if (before == startPos)
                    break;
                while (true)
                {
                    Debug.DrawLine(groundGraph[before / mapSize, before % mapSize].position, groundGraph[(int)hash[before] / mapSize, (int)hash[before] % mapSize].position, Color.red, 3);
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
    //A* 계산
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

                Debug.DrawLine(groundGraph[current.NodeCell / mapSize, current.NodeCell % mapSize].position, groundGraph[before4AStar.NodeCell / mapSize, before4AStar.NodeCell % mapSize].position, Color.red, 3);

                if (before4AStar.NodeCell == startPos)
                    break;
                while (true)
                {
                    Debug.DrawLine(groundGraph[before4AStar.NodeCell / mapSize, before4AStar.NodeCell % mapSize].position, groundGraph[before4AStar.parent.NodeCell / mapSize, before4AStar.parent.NodeCell % mapSize].position, Color.red, 3);
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
    //거리 계산
    public int GetDistance(Node nodeA, Node nodeB)
    {
        //Mathf.Abs 절대값
        int dstX = Mathf.Abs(nodeA.NodeCell % mapSize - nodeB.NodeCell % mapSize); 
        int dstY = Mathf.Abs(nodeA.NodeCell / mapSize - nodeB.NodeCell / mapSize);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    //범위 판정
    public bool Area(int cellNum, int k)
    {
        if (cellNum % mapSize == 0) if (k == 3 || k == 4 || k == 5) return true;            //왼쪽
        if (cellNum % mapSize == mapSize - 1) if (k == 0 || k == 1 || k == 7) return true;  //오른쪽
        if (cellNum / mapSize == 0) if (k == 1 || k == 2 || k == 3) return true;            //아래쪽
        if (cellNum / mapSize == mapSize - 1) if (k == 5 || k == 6 || k == 7) return true;  //위쪽

        return false;
    }

    //셀 넘버 계산
    public int ReturnCellNumber(int x, int y)
    {
        return cellNumber[x, y];
    }

    //캐릭터 이동
    IEnumerator Move()
    {
        while (true)
        {
            if (hashStack.Count != 0)
            { 
                if(Vector3.Distance(transform.position, new Vector3(poppedStack%mapSize, poppedStack/mapSize, 0)) < 0.05)
                    poppedStack = hashStack.Pop();
                  
                transform.position = new Vector3(poppedStack % mapSize, poppedStack / mapSize, 0);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    //Update()에서의 캐릭터 이동(미완성)
    void CharacterMove()
    {
        if (isMoving)
        {
            Player.GetComponent<Animator>().SetBool("isRun", true);
            if (!firstPop && hashStack.Count != 0)
            {
                poppedStack = hashStack.Pop();
                firstPop = true;
            }
            else
            {
                if (Vector3.Distance(transform.position, new Vector3(poppedStack % mapSize, poppedStack / mapSize, 0)) <= 0.05f)
                {
                    if (hashStack.Count != 0)
                    {
                        transform.position = new Vector3(poppedStack % mapSize, poppedStack / mapSize, 0);
                        poppedStack = hashStack.Pop();
                    }
                }
            }

            if (Vector3.Distance(transform.position, new Vector3(goalPos % mapSize, goalPos / mapSize)) <= 0.05f)
            {
                print("arrive");
                transform.position = new Vector3(poppedStack % mapSize, poppedStack / mapSize, 0);
                transform.Translate(new Vector2(0, 0));
                firstPop = false;
                isMoving = false;
                Player.GetComponent<Animator>().SetBool("isRun", false);
            }
            else
            {
                if (hashStack.Count >= 0)
                {
                    if (poppedStack % mapSize > transform.position.x && poppedStack / mapSize > transform.position.y)
                        transform.Translate(speed * Time.deltaTime, speed * Time.deltaTime, 0);
                    else if (poppedStack % mapSize > transform.position.x && poppedStack / mapSize == transform.position.y)
                        transform.Translate(speed * Time.deltaTime, 0, 0);
                    else if (poppedStack % mapSize > transform.position.x && poppedStack / mapSize < transform.position.y)
                        transform.Translate(speed * Time.deltaTime, -speed * Time.deltaTime, 0);
                    else if (poppedStack % mapSize == transform.position.x && poppedStack / mapSize < transform.position.y)
                        transform.Translate(0, -speed * Time.deltaTime, 0);
                    else if (poppedStack % mapSize < transform.position.x && poppedStack / mapSize < transform.position.y)
                        transform.Translate(-speed * Time.deltaTime, -speed * Time.deltaTime, 0);
                    else if (poppedStack % mapSize < transform.position.x && poppedStack / mapSize == transform.position.y)
                        transform.Translate(-speed * Time.deltaTime, 0, 0);
                    else if (poppedStack % mapSize < transform.position.x && poppedStack / mapSize > transform.position.y)
                        transform.Translate(-speed * Time.deltaTime, speed * Time.deltaTime, 0);
                    else if (poppedStack % mapSize == transform.position.x && poppedStack / mapSize > transform.position.y)
                        transform.Translate(0, speed * Time.deltaTime, 0);

                    Player.transform.LookAt(new Vector3(poppedStack % mapSize, poppedStack / mapSize, 0), Vector3.back);
                }
            }
        }
    }

    //Reorder Map
    public void Click()
    {
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                if (graph[i, j] != null)
                {
                    Destroy(graph[i, j].gameObject);
                    PlayerPrefs.DeleteKey("Tree" + ReturnCellNumber(i, j));
                }
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

            
            GameObject instance = Instantiate(quad);
            instance.transform.position = new Vector3(randomNum2, randomNum1, -0.5f);
            graph[randomNum1, randomNum2] = instance.transform;
            

            PlayerPrefs.SetInt("Tree" + ReturnCellNumber(randomNum1, randomNum2), ReturnCellNumber(randomNum1, randomNum2));
        }
        if (graph[(int)transform.position.y, (int)transform.position.x] != null)
            Destroy(graph[(int)transform.position.y, (int)transform.position.x].gameObject);
    }
    //BFS 작동
    public void DoBFS()
    {
        isBFS = true;
        isDFS = false;
        isAStar = false;
    }
    //DFS 작동
    public void DoDFS()
    {
        isBFS = false;
        isDFS = true;
        isAStar = false;
    }
    //A* 작동
    public void DoAStar()
    {
        isBFS = false;
        isDFS = false;
        isAStar = true;
    }
    
}
