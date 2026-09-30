using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * 
 */
public class Path : MonoBehaviour
{    
    public Vector2 PathVector;

    public PathManager _pm;

    //Starting position for rays
    private Vector3[] startingPoints = new Vector3[8];

    // if the player touched the ray disable the ray
    private bool[] borderCheck = new bool[8];

    private float checkerLength = 1000f;
    private Vector3[] _directions = new Vector3[4];
    public bool isActivePath;

    private void Start()
    {
        
        isActivePath = false;

        for (int i = 0; i < borderCheck.Length; i++)
        {
            borderCheck[i] = false;
        }
    }

    public void Init()
    {
        _pm = GeneralFunctions.PathManager;
        PathVector = new Vector2(transform.position.x, transform.position.z);

        //rays starting points
        startingPoints[0] = new Vector3(PathVector.x - 500f, 0f, PathVector.y - 400f); // South
        startingPoints[1] = new Vector3(PathVector.x + 400f, 0f, PathVector.y - 500f); // East
        startingPoints[2] = new Vector3(PathVector.x + 500f, 0f, PathVector.y + 400f); // North
        startingPoints[3] = new Vector3(PathVector.x - 400f, 0f, PathVector.y + 500f); // West
        startingPoints[4] = new Vector3(PathVector.x - 500f, 0f, PathVector.y + -400f);// SouthWest
        startingPoints[5] = new Vector3(PathVector.x + 400f, 0f, PathVector.y - 500f); // SouthEast
        startingPoints[6] = new Vector3(PathVector.x + 500f, 0f, PathVector.y + 400f); // NorthEast
        startingPoints[7] = new Vector3(PathVector.x - 400f, 0f, PathVector.y + 500f); // NorthWest

        //directions for diagonal ray
        _directions[0] = startingPoints[4] - new Vector3(PathVector.x - 400f, 0f, PathVector.y - 500f);
        _directions[1] = startingPoints[5] - new Vector3(PathVector.x + 500f, 0f, PathVector.y - 400f);
        _directions[2] = startingPoints[6] - new Vector3(PathVector.x + 400f, 0f, PathVector.y + 500f);
        _directions[3] = startingPoints[7] - new Vector3(PathVector.x - 500f, 0f, PathVector.y + 400f);

        //ActiveRays();
    }
    // Update is called once per frame
    void Update()
    {
        if (isActivePath)
        {
            ActivePath();
        }        
    }
    public void ActivePath()
    {
        if (!borderCheck[0])
        {
            CreateRay("South");
        }

        if (!borderCheck[1])
        {
            CreateRay("East");
        }

        if (!borderCheck[2])
        {
            CreateRay("North");
        }

        if (!borderCheck[3])
        {
            CreateRay("West");
        }

        if (!borderCheck[4])
        {
            CreateRay("SouthWest");
        }

        if (!borderCheck[5])
        {
            CreateRay("SouthEast");
        }

        if (!borderCheck[6])
        {
            CreateRay("NorthEast");
        }

        if (!borderCheck[7])
        {
            CreateRay("NorthWest");
        }
    }
    private void CreateRay(string location)
    {        
        switch (location)
        {
            case "South":
                RaycastHit hit_south;
                if (Physics.Raycast(startingPoints[0], Vector3.right, out hit_south, checkerLength, 1 << 10))
                {
                    borderCheck[0] = true;
                    _pm.CheckPlane(new Vector2(PathVector.x, PathVector.y - 1000f), true);
                }
                Debug.DrawRay(startingPoints[0], Vector3.right * checkerLength, Color.red);
                break;

            case "East":
                RaycastHit hit_east;
                if (Physics.Raycast(startingPoints[1], Vector3.forward, out hit_east, checkerLength, 1 << 10))
                {
                    borderCheck[1] = true;
                    _pm.CheckPlane(new Vector2(PathVector.x + 1000f, PathVector.y), true);
                }
                Debug.DrawRay(startingPoints[1], Vector3.forward * checkerLength, Color.red);
                break;

            case "North":
                RaycastHit hit_north;
                if (Physics.Raycast(startingPoints[2], Vector3.left, out hit_north, checkerLength, 1 << 10))
                {
                    borderCheck[2] = true;
                    _pm.CheckPlane(new Vector2(PathVector.x, PathVector.y + 1000f), true);
                }
                Debug.DrawRay(startingPoints[2], Vector3.left * checkerLength, Color.red);
                break;                        



            case "West":
                RaycastHit hit_west;
                if (Physics.Raycast(startingPoints[3], Vector3.back, out hit_west, checkerLength, 1 << 10))
                {
                    borderCheck[3] = true;
                    _pm.CheckPlane(new Vector2(PathVector.x - 1000f, PathVector.y), true);
                }
                Debug.DrawRay(startingPoints[3], Vector3.back * checkerLength, Color.red);
                break;

            case "SouthWest":
                RaycastHit hit_SouthWest;
                if (Physics.Raycast(startingPoints[4], -_directions[0], out hit_SouthWest, 424.27f, 1 << 10))
                {
                    borderCheck[4] = true;
                    _pm.CheckPlane(new Vector2(PathVector.x - 1000f, PathVector.y - 1000f), true);
                }
                Debug.DrawRay(startingPoints[4], -_directions[0], Color.red);
                break;

            case "SouthEast":
                RaycastHit hit_SouthEast;
                if (Physics.Raycast(startingPoints[5], -_directions[1], out hit_SouthEast, 424.27f, 1 << 10))
                {
                    borderCheck[5] = true;
                    _pm.CheckPlane(new Vector2(PathVector.x + 1000f, PathVector.y - 1000f), true);
                }
                Debug.DrawRay(startingPoints[5], -_directions[1], Color.red);
                break;

            case "NorthEast":
                RaycastHit hit_NorthEast;
                if (Physics.Raycast(startingPoints[6], -_directions[2], out hit_NorthEast, 424.27f, 1 << 10))
                {
                    borderCheck[6] = true;
                    _pm.CheckPlane(new Vector2(PathVector.x + 1000f, PathVector.y + 1000f), true);
                }
                Debug.DrawRay(startingPoints[6], -_directions[2], Color.red);
                break;

            case "NorthWest":
                RaycastHit hit_NorthWest;
                if (Physics.Raycast(startingPoints[7], -_directions[3], out hit_NorthWest, 424.27f, 1 << 10))
                {
                    borderCheck[7] = true;
                    _pm.CheckPlane(new Vector2(PathVector.x - 1000f, PathVector.y + 1000), true);
                }
                Debug.DrawRay(startingPoints[7], -_directions[3], Color.red);
                break;

            default:
                break;
        }
    }


    //checks which ray to activate
    public void ActiveRays()
    {
        if (_pm.CheckPlane(new Vector2(PathVector.x, PathVector.y - 1000f), false)) // south
        {
            borderCheck[0] = true;
        }
        if (_pm.CheckPlane(new Vector2(PathVector.x + 1000f, PathVector.y), false)) //east
        {
            borderCheck[1] = true;
        }
        if (_pm.CheckPlane(new Vector2(PathVector.x, PathVector.y + 1000f), false)) //north
        {
            borderCheck[2] = true;
        }
        if (_pm.CheckPlane(new Vector2(PathVector.x - 1000f, PathVector.y), false)) //west
        {
            borderCheck[3] = true;
        }
        if (_pm.CheckPlane(new Vector2(PathVector.x - 1000f, PathVector.y - 1000f), false)) //sw
        {
            borderCheck[4] = true;
        }
        if (_pm.CheckPlane(new Vector2(PathVector.x + 1000f, PathVector.y - 1000f), false)) //se
        {
            borderCheck[5] = true;
        }
        if (_pm.CheckPlane(new Vector2(PathVector.x + 1000f, PathVector.y + 1000f), false)) //ne
        {
            borderCheck[6] = true;
        }
        if (_pm.CheckPlane(new Vector2(PathVector.x - 1000f, PathVector.y + 1000), false)) //nw
        {
            borderCheck[7] = true;
        }
    }
}
