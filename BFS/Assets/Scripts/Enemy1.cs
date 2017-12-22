using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : MonoBehaviour {
    enum EnemyState { Idle, Chase, Attack, Die };
    EnemyState state = EnemyState.Idle;

    NewBehaviourScript map;

    List<Node> open = new List<Node>();
    List<int> openList = new List<int>();
    List<int> closed = new List<int>();
    Node before4AStar;
    Node neighbour;
    Node[] neighbourArray;

    //Move
    Stack<int> hashStack = new Stack<int>();
    int before;
    int poppedStack;
    bool firstPop;
    public bool isMoving;
    public float speed;

    int startPos;
    int goalPos;

    // Use this for initialization
    void Start () {
        map = GameObject.Find("Sphere").GetComponent<NewBehaviourScript>();
        isMoving = false;
        firstPop = false;
        neighbourArray = new Node[map.cell];
    }
	
	// Update is called once per frame
	void Update () {
        

        switch (state)
        {
            case EnemyState.Idle:
                if (Vector3.Distance(map.transform.position, transform.position) <= 10)
                {
                    startPos = map.ReturnCellNumber((int)transform.position.y, (int)transform.position.x);
                    goalPos = map.ReturnCellNumber((int)map.transform.position.y, (int)map.transform.position.x);
                    AStar(startPos, goalPos);

                    for (int i = 0; i < hashStack.Count; i++)
                        print(hashStack.Pop());
                    isMoving = true;
                    state = EnemyState.Chase;
                }
                break;
            case EnemyState.Chase:
                CharacterMove();
                if (Vector3.Distance(map.transform.position, transform.position) <= 1)
                {
                    isMoving = false;
                    hashStack.Clear();
                    state = EnemyState.Idle;
                }
                break;
            case EnemyState.Attack:

                break;
        }
        
	}

    void AStar(int startPos, int goalPos)
    {
        Node startNode = new Node { NodeCell = startPos };
        Node goalNode = new Node { NodeCell = goalPos };
        startNode.h_Cost = map.GetDistance(startNode, goalNode);
        open.Add(startNode);
        openList.Add(startNode.NodeCell);

        while (open.Count > 0)
        {
            Node current = open[0];
            for (int i = 1; i < open.Count; i++)
            {
                if ((open[i].F_Cost < current.F_Cost || open[i].F_Cost == current.F_Cost) && open[i].h_Cost < current.h_Cost)
                {
                    current = open[i];
                }
            }
            print(current.NodeCell);
            open.Remove(current);
            openList.Remove(current.NodeCell);
            closed.Add(current.NodeCell);

            if (current.NodeCell == goalNode.NodeCell || current.h_Cost == 0)
            {
                hashStack.Push(current.NodeCell);
                before4AStar = current.parent;
                hashStack.Push(before4AStar.NodeCell);

                //Debug.DrawLine(groundGraph[current.NodeCell / mapSize, current.NodeCell % mapSize].position, groundGraph[before4AStar.NodeCell / mapSize, before4AStar.NodeCell % mapSize].position, Color.red, 3);

                if (before4AStar.NodeCell == startPos)
                    break;
                while (true)
                {
                    //Debug.DrawLine(groundGraph[before4AStar.NodeCell / mapSize, before4AStar.NodeCell % mapSize].position, groundGraph[before4AStar.parent.NodeCell / mapSize, before4AStar.parent.NodeCell % mapSize].position, Color.red, 3);
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
                neighbour = new Node { NodeCell = current.NodeCell + map.direction[k] };


                if (map.Area(current.NodeCell, k)) continue;
                
                if (map.map[neighbour.NodeCell / map.mapSize, neighbour.NodeCell % map.mapSize] == 1 && !closed.Contains(neighbour.NodeCell))
                {
                    if (k == 1 || k == 3 || k == 5 || k == 7)
                    {
                        int numCheckPoint1 = current.NodeCell + map.direction[(k - 1) % 8];
                        int numCheckPoint2 = current.NodeCell + map.direction[(k + 1) % 8];

                        if (map.Area(current.NodeCell, k)) continue;
                        if (map.map[numCheckPoint1 / map.mapSize, numCheckPoint1 % map.mapSize] == 0 && map.map[numCheckPoint2 / map.mapSize, numCheckPoint2 % map.mapSize] == 0) continue;

                    }

                    if (neighbourArray[neighbour.NodeCell] == null)
                    {
                        neighbourArray[neighbour.NodeCell] = neighbour;
                    }
                    int newMoveCost = current.g_Cost + map.GetDistance(current, neighbour);
                    if (newMoveCost < neighbourArray[neighbour.NodeCell].g_Cost || !openList.Contains(neighbour.NodeCell))
                    {
                        neighbourArray[neighbour.NodeCell].g_Cost = newMoveCost;
                        neighbourArray[neighbour.NodeCell].h_Cost = map.GetDistance(neighbour, goalNode);

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

    void CharacterMove()
    {
        if (isMoving)
        {
            //Player.GetComponent<Animator>().SetBool("isRun", true);
            if (!firstPop && hashStack.Count != 0)
            {
                poppedStack = hashStack.Pop();
                firstPop = true;
            }
            else
            {
                if (Vector3.Distance(transform.position, new Vector3(poppedStack % map.mapSize, poppedStack / map.mapSize, 0)) <= 0.05f)
                {
                    if (hashStack.Count != 0)
                    {
                        
                        transform.position = new Vector3(poppedStack % map.mapSize, poppedStack / map.mapSize, 0);
                        poppedStack = hashStack.Pop();
                        print(poppedStack);
                    }
                }
            }

            if (Vector3.Distance(transform.position, new Vector3(goalPos % map.mapSize, goalPos / map.mapSize)) <= 0.05f)
            {
                print("arrive");
                transform.position = new Vector3(poppedStack % map.mapSize, poppedStack / map.mapSize, 0);
                transform.Translate(new Vector2(0, 0));
                firstPop = false;
                isMoving = false;
                hashStack.Clear();
                //Player.GetComponent<Animator>().SetBool("isRun", false);
            }
            else
            {
                if (hashStack.Count >= 0)
                {
                    if (poppedStack % map.mapSize > transform.position.x && poppedStack / map.mapSize > transform.position.y)
                        transform.Translate(speed * Time.deltaTime, speed * Time.deltaTime, 0);
                    else if (poppedStack % map.mapSize > transform.position.x && poppedStack / map.mapSize == transform.position.y)
                        transform.Translate(speed * Time.deltaTime, 0, 0);
                    else if (poppedStack % map.mapSize > transform.position.x && poppedStack / map.mapSize < transform.position.y)
                        transform.Translate(speed * Time.deltaTime, -speed * Time.deltaTime, 0);
                    else if (poppedStack % map.mapSize == transform.position.x && poppedStack / map.mapSize < transform.position.y)
                        transform.Translate(0, -speed * Time.deltaTime, 0);
                    else if (poppedStack % map.mapSize < transform.position.x && poppedStack / map.mapSize < transform.position.y)
                        transform.Translate(-speed * Time.deltaTime, -speed * Time.deltaTime, 0);
                    else if (poppedStack % map.mapSize < transform.position.x && poppedStack / map.mapSize == transform.position.y)
                        transform.Translate(-speed * Time.deltaTime, 0, 0);
                    else if (poppedStack % map.mapSize < transform.position.x && poppedStack / map.mapSize > transform.position.y)
                        transform.Translate(-speed * Time.deltaTime, speed * Time.deltaTime, 0);
                    else if (poppedStack % map.mapSize == transform.position.x && poppedStack / map.mapSize > transform.position.y)
                        transform.Translate(0, speed * Time.deltaTime, 0);

                    //Player.transform.LookAt(new Vector3(poppedStack % mapSize, poppedStack / mapSize, 0), Vector3.back);
                }
            }
        }
    }
}
