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
    private Vector3 front;
    private Vector3 back;
    private Vector3 right;
    private Vector3 left;
    private bool trapIntruder = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.gameObject.activeSelf == true) 
        {
            dungeonGenerator = GameManager.manager.levels[GameManager.manager.currentLevel].GetComponent<DungeonGenerator>();
            if (!trapIntruder) { OpenDoors(currentStructureType); }
        }
    }

    private void OpenDoors(DungeonGenerator.StructureType structureType)
    {
        if (dungeonGenerator == null) { return; }
        front = new Vector3(transform.position.x + dungeonGenerator.structureSpacing, transform.position.y, transform.position.z);
        back = new Vector3(transform.position.x - dungeonGenerator.structureSpacing, transform.position.y, transform.position.z);
        right = new Vector3(transform.position.x, transform.position.y, transform.position.z + dungeonGenerator.structureSpacing);
        left = new Vector3(transform.position.x, transform.position.y, transform.position.z - dungeonGenerator.structureSpacing);
        
        if (CanStructureConnect(front) && transform.eulerAngles.y == 0) { OpenDoor(front); }
        if (CanStructureConnect(back) && transform.eulerAngles.y == 0) { OpenDoor(back); }
        //else ifs are for strange hallway door-close bug
        if (CanStructureConnect(right) && transform.eulerAngles.y == 0) { OpenDoor(right); }
        else if(CanStructureConnect(right) && transform.eulerAngles.y == 90) { OpenDoor(back); }
        if (CanStructureConnect(left) && transform.eulerAngles.y == 0) { OpenDoor(left); }
        else if (CanStructureConnect(left) && transform.eulerAngles.y == 90) { OpenDoor(front); }
    }

    private void OpenDoor(Vector3 direction)
    {
        if (direction == front) { doors.transform.GetChild(0).gameObject.SetActive(false); }
        else if(direction == back) { doors.transform.GetChild(1).gameObject.SetActive(false); }
        else if (direction == right) { doors.transform.GetChild(2).gameObject.SetActive(false); }
        else if (direction == left) { doors.transform.GetChild(3).gameObject.SetActive(false); }
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
            if (chosenSpot == front || chosenSpot == back)
            {
                if (dungeonGenerator.structures[structureToBeChecked].transform.eulerAngles.y == 90) { return false; }
                return true;
            }
            else if (chosenSpot == left || chosenSpot == right)
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
