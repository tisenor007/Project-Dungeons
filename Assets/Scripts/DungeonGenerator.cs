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
    public GameObject[] roomVariations;
    public GameObject[] hallVariations;
    public List<GameObject> structures;
    private StructureType nextStructureType;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void generateNewRoom()
    {
       
        int structureChoice = Random.Range(0, (int)StructureType.Length);
        nextStructureType = (StructureType)structureChoice;
        if (nextStructureType == StructureType.Room)
        {
            int roomDirection = Random.Range(1, 5);
            int roomVariation = Random.Range(0, roomVariations.Length);
            if (structures.Count <= 0) 
            {
                structures.Add(Instantiate(roomVariations[roomVariation], Vector3.zero, Quaternion.identity));
            }
            else if (structures.Count > 0) 
            {
                int room = Random.Range(0, roomVariations.Length);
                structures.Add(Instantiate(roomVariations[roomVariation], roomVariations[roomVariation].transform.GetChild(roomDirection).transform.position, transform.GetChild(roomDirection).transform.localRotation)); 
            }
        }
        if (nextStructureType == StructureType.Hallway)
        {
            int hallwayDirection = Random.Range(5, 8);
            int hallwayVariation = Random.Range(0, roomVariations.Length);
            if (structures.Count <= 0)
            {
                structures.Add(Instantiate(roomVariations[hallwayVariation], Vector3.zero, Quaternion.identity));
            }
            else if (structures.Count > 0)
            {
                int room = Random.Range(0, roomVariations.Length);
                structures.Add(Instantiate(roomVariations[hallwayVariation], roomVariations[hallwayVariation].transform.GetChild(hallwayDirection).transform.position, transform.GetChild(hallwayDirection).transform.localRotation));
            }
        }

    }
}
