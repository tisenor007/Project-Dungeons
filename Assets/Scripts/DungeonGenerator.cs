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
    public List<GameObject> structures = new List<GameObject>();
    private StructureType nextStructureType;
    private StructureType currentStructureType;

    private int hallwayDirection;
    private int roomDirection;

    private float rayRange;
    private RaycastHit rayHit;

    private int roomAmount = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(hallwayDirection);
        if (roomAmount < 20)
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
                if (IsSpotFree(structures[structures.Count - 1].transform.GetChild(roomDirection).transform.position, roomDirection))
                {
                    structures.Add(Instantiate(roomVariations[roomVariation], structures[structures.Count - 1].transform.GetChild(roomDirection).transform.position, structures[structures.Count - 1].transform.GetChild(roomDirection).transform.localRotation));
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
                if (IsSpotFree(structures[structures.Count - 1].transform.GetChild(hallwayDirection).transform.position, hallwayDirection))
                {
                    structures.Add(Instantiate(hallVariations[hallwayVariation], structures[structures.Count - 1].transform.GetChild(hallwayDirection).transform.position, structures[structures.Count - 1].transform.GetChild(hallwayDirection).transform.localRotation));
                    currentStructureType = (StructureType)nextStructureType;
                }
            }
        }

    }

    private bool IsSpotFree(Vector3 chosenSpot, int direction)
    {
       foreach (GameObject structure in structures)
       {
            if (structure.transform.position != chosenSpot)
            {
                return true;
            }
       }
        Debug.Log("failed");
        return false;
    }
}
