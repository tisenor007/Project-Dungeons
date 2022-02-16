using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureBehavior : MonoBehaviour
{
    [HideInInspector]public DungeonGenerator.StructureType thisStructureType;
    private DungeonGenerator dungeonGenerator;
    private Vector3 front;
    private Vector3 back;
    private Vector3 right;
    private Vector3 left;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (dungeonGenerator == null) { dungeonGenerator = GameObject.Find("DungeonGenerator").GetComponent<DungeonGenerator>(); }
        OpenDoors(thisStructureType);
    }

    private void OpenDoors(DungeonGenerator.StructureType structureType)
    {
        if (dungeonGenerator == null) { return; }
        front = new Vector3(transform.position.x + dungeonGenerator.structureSpacing, transform.position.y, transform.position.z);
        back = new Vector3(transform.position.x - dungeonGenerator.structureSpacing, transform.position.y, transform.position.z);
        right = new Vector3(transform.position.x, transform.position.y, transform.position.z + dungeonGenerator.structureSpacing);
        left = new Vector3(transform.position.x, transform.position.y, transform.position.z - dungeonGenerator.structureSpacing);
        if (structureType == DungeonGenerator.StructureType.Hallway) 
        {
            if (CanStructureConnect(front) && transform.eulerAngles.y == 0) {OpenDoor(front);}
            if (CanStructureConnect(back) && transform.eulerAngles.y == 0) { OpenDoor(back); }
            if (CanStructureConnect(right) && transform.eulerAngles.y == 90) { OpenDoor(back); }
            if (CanStructureConnect(left) && transform.eulerAngles.y == 90) { OpenDoor(front); }
        }
        else if (structureType == DungeonGenerator.StructureType.Room)
        {
            if (CanStructureConnect(front)) { OpenDoor(front); }
            if (CanStructureConnect(back)) { OpenDoor(back); }
            if (CanStructureConnect(right)) { OpenDoor(right); }
            if (CanStructureConnect(left)) { OpenDoor(left); }
        }
    }

    private void OpenDoor(Vector3 direction)
    {
        if (direction == front) { transform.GetChild(0).GetChild(0).gameObject.SetActive(false); }
        else if(direction == back) { transform.GetChild(0).GetChild(1).gameObject.SetActive(false); }
        else if (direction == right) { transform.GetChild(0).GetChild(2).gameObject.SetActive(false); }
        else if (direction == left) { transform.GetChild(0).GetChild(3).gameObject.SetActive(false); }
    }

    private bool CanStructureConnect(Vector3 chosenSpot)
    {
        for (int i = 0; i < dungeonGenerator.structures.Count; i++)
        {
            if (dungeonGenerator.structures[i].transform.position == chosenSpot)
            {
                //clean this in the future
                if (dungeonGenerator.structures[i].GetComponent<StructureBehavior>().thisStructureType == DungeonGenerator.StructureType.Hallway)
                {
                    if(chosenSpot == front || chosenSpot == back)
                    {
                        if (dungeonGenerator.structures[i].transform.eulerAngles.y == 90) { return false; }
                        return true;
                    }
                    else if (chosenSpot == left || chosenSpot == right)
                    {
                        if (dungeonGenerator.structures[i].transform.eulerAngles.y == 0) { return false; }
                        return true;
                    }
                    return true;
                }
                else if (dungeonGenerator.structures[i].GetComponent<StructureBehavior>().thisStructureType == DungeonGenerator.StructureType.Room)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
