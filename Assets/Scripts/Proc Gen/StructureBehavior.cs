using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureBehavior : MonoBehaviour
{
    public DungeonGenerator.StructureType currentStructureType;
    [HideInInspector] public int currentVariation;
    [SerializeField] private Light[] lights;
    [SerializeField] private GameObject doors;
    [Header("Only pass this in if room is an end room!")]
    [SerializeField] private GameObject bossObj;
    private DungeonGenerator dungeonGenerator;
    private bool trapIntruder = false;
    // Start is called before the first frame update
    void Awake()
    {
        dungeonGenerator = GameManager.manager.levels[GameManager.manager.currentLevel].GetComponent<DungeonGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (currentStructureType == DungeonGenerator.StructureType.EndStructure)
        //{
        //    Debug.Log("End Is Here");
        //}
        //if (currentStructureType == DungeonGenerator.StructureType.StartStructure)
        //{
        //    Debug.Log("Start Is Here");
        //}
        if (this.transform.gameObject.activeSelf == true) 
        {
            if (!trapIntruder) { OpenDoors(currentStructureType); }
        }
    }

    private void OpenDoors(DungeonGenerator.StructureType structureType)
    {
        if (dungeonGenerator == null) { return; }
        
        if (CanStructureConnect(dungeonGenerator.Direction(DungeonGenerator.Directions.front, transform.position)) && transform.eulerAngles.y == 0) 
        { OpenDoor(DungeonGenerator.Directions.front); }
        if (CanStructureConnect(dungeonGenerator.Direction(DungeonGenerator.Directions.back, transform.position)) && transform.eulerAngles.y == 0) 
        { OpenDoor(DungeonGenerator.Directions.back); }
        //else ifs are for strange hallway door-close bug
        if (CanStructureConnect(dungeonGenerator.Direction(DungeonGenerator.Directions.right, transform.position)) && transform.eulerAngles.y == 0) 
        { OpenDoor(DungeonGenerator.Directions.right); }
        else if(CanStructureConnect(dungeonGenerator.Direction(DungeonGenerator.Directions.right, transform.position)) && transform.eulerAngles.y == 90) 
        { OpenDoor(DungeonGenerator.Directions.back); }
        if (CanStructureConnect(dungeonGenerator.Direction(DungeonGenerator.Directions.left, transform.position)) && transform.eulerAngles.y == 0) 
        { OpenDoor(DungeonGenerator.Directions.left); }
        else if (CanStructureConnect(dungeonGenerator.Direction(DungeonGenerator.Directions.left, transform.position)) && transform.eulerAngles.y == 90) 
        { OpenDoor(DungeonGenerator.Directions.front); }
    }

    private void OpenDoor(DungeonGenerator.Directions direction)
    {
        if (direction == DungeonGenerator.Directions.front) { doors.transform.GetChild(0).gameObject.SetActive(false); }
        else if(direction == DungeonGenerator.Directions.back) { doors.transform.GetChild(1).gameObject.SetActive(false); }
        else if (direction == DungeonGenerator.Directions.right) { doors.transform.GetChild(2).gameObject.SetActive(false); }
        else if (direction == DungeonGenerator.Directions.left) { doors.transform.GetChild(3).gameObject.SetActive(false); }
    }

    private bool CanStructureConnect(Vector3 chosenSpot)
    {
        for (int i = 0; i < dungeonGenerator.structures.Count; i++)
        {
            if (dungeonGenerator.structures[i].transform.position == chosenSpot)
            {
                return IsStructureConnectable(chosenSpot, i);
            }
        }
        return false;
    }

    private bool IsStructureConnectable(Vector3 chosenSpot, int structureToBeChecked)
    {
        if (dungeonGenerator.structures[structureToBeChecked].GetComponent<StructureBehavior>().currentStructureType == DungeonGenerator.StructureType.Hallway)
        {
            if (chosenSpot == dungeonGenerator.Direction(DungeonGenerator.Directions.front, transform.position) || 
                chosenSpot == dungeonGenerator.Direction(DungeonGenerator.Directions.back, transform.position))
            {
                if (dungeonGenerator.structures[structureToBeChecked].transform.eulerAngles.y == 90) { return false; }
                return true;
            }
            else if (chosenSpot == dungeonGenerator.Direction(DungeonGenerator.Directions.left, transform.position) || 
                chosenSpot == dungeonGenerator.Direction(DungeonGenerator.Directions.right, transform.position))
            {
                if (dungeonGenerator.structures[structureToBeChecked].transform.eulerAngles.y == 0) { return false; }
                return true;
            }
        }
        return true;
    }

    private void ShutAllDoors()
    {
        for (int i = 0; i < doors.transform.childCount; i++)
        {
            doors.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") { foreach (Light light in lights) { light.enabled = true; } }
        if (currentStructureType == DungeonGenerator.StructureType.EndStructure) 
        {
            trapIntruder = true;
            ShutAllDoors();
            if (bossObj.GetComponent<Boss>().IsAlive == true) { bossObj.SetActive(true); }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player") { foreach (Light light in lights) { light.enabled = false; } }
    }
}
