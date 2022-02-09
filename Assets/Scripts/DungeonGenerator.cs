using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public enum StructureType
    {
        Room,
        Hallway,
        Length
    }
    public LayerMask playerBody;
    public GameObject[] roomVariations;
    public GameObject[] hallVariations;
    public int roomLength = 10;
    public List<GameObject> structures = new List<GameObject>();
    private StructureType nextStructureType;
    private StructureType currentStructureType;

    private int hallwayDirection;
    private int roomDirection;

    private float rayRange;
    private RaycastHit rayHit;
    private Vector3 nextStructureLoc;
    private Vector3 nextStructureRot;

    private int roomAmount = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(hallwayDirection);
        if (roomAmount < 40)
        {
            GenerateNewRoom();
            roomAmount++;
        }
    }

    public void GenerateNewRoom()
    {
        int structureChoice = Random.Range(0, (int)StructureType.Length);
        nextStructureType = (StructureType)structureChoice;
        if (nextStructureType == StructureType.Room)
        {
            if (currentStructureType == StructureType.Hallway) { roomDirection = Random.Range(0, 2); }
            else if (currentStructureType == StructureType.Room) { roomDirection = Random.Range(0, 4); }

            int roomVariation = Random.Range(0, roomVariations.Length);
            if (structures.Count <= 0) 
            {
                structures.Add(Instantiate(roomVariations[roomVariation], Vector3.zero, Quaternion.identity));
                currentStructureType = (StructureType)nextStructureType;
            }
            else if (structures.Count > 0) 
            {
                if (roomDirection <= 0)
                {
                    nextStructureRot = new Vector3(0, 0, 0);
                    nextStructureLoc = new Vector3(structures[structures.Count - 1].transform.position.x + roomLength, structures[structures.Count - 1].transform.position.y, structures[structures.Count - 1].transform.position.z);
                }
                else if (roomDirection == 1)
                {
                    nextStructureRot = new Vector3(0, 0, 0);
                    nextStructureLoc = new Vector3(structures[structures.Count - 1].transform.position.x - roomLength, structures[structures.Count - 1].transform.position.y, structures[structures.Count - 1].transform.position.z);
                }
                else if (roomDirection == 2)
                {
                    nextStructureRot = new Vector3(0, -90, 0);
                    nextStructureLoc = new Vector3(structures[structures.Count - 1].transform.position.x, structures[structures.Count - 1].transform.position.y, structures[structures.Count - 1].transform.position.z + roomLength);
                }
                else if (roomDirection >= 3)
                {
                    nextStructureRot = new Vector3(0, 90, 0);
                    nextStructureLoc = new Vector3(structures[structures.Count - 1].transform.position.x, structures[structures.Count - 1].transform.position.y, structures[structures.Count - 1].transform.position.z - roomLength);
                }
                if (!IsStructureHere(nextStructureLoc))
                {
                    structures.Add(Instantiate(roomVariations[roomVariation],  nextStructureLoc, Quaternion.Euler(nextStructureRot)));
                    currentStructureType = (StructureType)nextStructureType;
                }
            }
        }
        if (nextStructureType == StructureType.Hallway)
        {
            if (currentStructureType == StructureType.Hallway) { hallwayDirection = Random.Range(0, 2); }
            else if (currentStructureType == StructureType.Room) { hallwayDirection = Random.Range(0, 4); }

            int hallwayVariation = Random.Range(0, hallVariations.Length);
            if (structures.Count <= 0)
            {
                structures.Add(Instantiate(hallVariations[hallwayVariation], Vector3.zero, Quaternion.identity));
                currentStructureType = (StructureType)nextStructureType;
            }
            else if (structures.Count > 0)
            {
                if (roomDirection <= 0)
                {
                    nextStructureRot = new Vector3(0, 0, 0);
                    nextStructureLoc = new Vector3(structures[structures.Count - 1].transform.position.x + roomLength, structures[structures.Count - 1].transform.position.y, structures[structures.Count - 1].transform.position.z);
                }
                else if (roomDirection == 1)
                {
                    nextStructureRot = new Vector3(0, 0, 0);
                    nextStructureLoc = new Vector3(structures[structures.Count - 1].transform.position.x - roomLength, structures[structures.Count - 1].transform.position.y, structures[structures.Count - 1].transform.position.z);
                }
                else if (roomDirection == 2)
                {
                    nextStructureRot = new Vector3(0, -90, 0);
                    nextStructureLoc = new Vector3(structures[structures.Count - 1].transform.position.x, structures[structures.Count - 1].transform.position.y, structures[structures.Count - 1].transform.position.z + roomLength);
                }
                else if (roomDirection >= 3)
                {
                    nextStructureRot = new Vector3(0, 90, 0);
                    nextStructureLoc = new Vector3(structures[structures.Count - 1].transform.position.x, structures[structures.Count - 1].transform.position.y, structures[structures.Count - 1].transform.position.z - roomLength);
                }
                if (!IsStructureHere(nextStructureLoc))
                {
                    structures.Add(Instantiate(hallVariations[hallwayVariation], nextStructureLoc, Quaternion.Euler(nextStructureRot)));
                }
                currentStructureType = (StructureType)nextStructureType;
            }
        }
    }

    private bool IsStructureHere(Vector3 chosenSpot)
    {
       for (int i = 0; i < structures.Count; i++)
       {
            if (structures[i].transform.position == chosenSpot)
            {
                return true;
            }
       }
       Debug.Log("failed");
        roomAmount = roomAmount - 1;
       return false;
    }
}
