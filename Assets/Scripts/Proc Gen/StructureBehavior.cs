using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureBehavior : MonoBehaviour
{
    public GameObject doors;
    public DungeonGenerator.StructureType currentStructureType;
    [HideInInspector] public bool trapPlayer = false;
    [HideInInspector] public int currentVariation;
    [SerializeField] private Light[] lights;
    private DungeonGenerator dungeonGenerator;
    private bool playerInStructure = false;
    // Start is called before the first frame update
    void Awake()
    {
        dungeonGenerator = GameManager.manager.levels[GameManager.manager.currentLevel].GetComponent<DungeonGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (trapPlayer)
        {
            case true:
                ShutAllDoors();
                break;
            case false:
                OpenAvailableDoors();
                break;
        }

        switch (playerInStructure)
        {
            case true:
                ShowStructure(true);
                ShowLights(true);
                break;
            case false:
                ShowStructure(false);
                ShowLights(false);
                break;
        }
    }

    public void ShutAllDoors()
    {
        for (int i = 0; i < doors.transform.childCount; i++)
        {
            doors.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void OpenAvailableDoors()
    {
        if (dungeonGenerator == null) { return; }
        switch (currentStructureType)
        {
            case DungeonGenerator.StructureType.Hallway:
                if (CanStructureConnect(dungeonGenerator.Direction(DungeonGenerator.Directions.front, transform.position)))
                { OpenDoor(DungeonGenerator.Directions.front); }
                if (CanStructureConnect(dungeonGenerator.Direction(DungeonGenerator.Directions.back, transform.position)))
                { OpenDoor(DungeonGenerator.Directions.back); }
                if (CanStructureConnect(dungeonGenerator.Direction(DungeonGenerator.Directions.right, transform.position)) && transform.eulerAngles.y == 90)
                { OpenDoor(DungeonGenerator.Directions.back); }
                if (CanStructureConnect(dungeonGenerator.Direction(DungeonGenerator.Directions.left, transform.position)) && transform.eulerAngles.y == 90)
                { OpenDoor(DungeonGenerator.Directions.front); }
                break;
            default:
                if(CanStructureConnect(dungeonGenerator.Direction(DungeonGenerator.Directions.front, transform.position)))
                { OpenDoor(DungeonGenerator.Directions.front); }
                if (CanStructureConnect(dungeonGenerator.Direction(DungeonGenerator.Directions.back, transform.position)))
                { OpenDoor(DungeonGenerator.Directions.back); }
                if (CanStructureConnect(dungeonGenerator.Direction(DungeonGenerator.Directions.right, transform.position)))
                { OpenDoor(DungeonGenerator.Directions.right); }
                if (CanStructureConnect(dungeonGenerator.Direction(DungeonGenerator.Directions.left, transform.position)))
                { OpenDoor(DungeonGenerator.Directions.left); }
                break;
        }
    }

    private void ShowStructure(bool visualStatus)
    {
        foreach (Transform child in transform)
        {
            if (child.tag != "DeathBox") { child.gameObject.SetActive(visualStatus); }
        }
    }

    private void ShowLights(bool lightStatus)
    {
        foreach (Light light in lights) { light.enabled = lightStatus; }
    }

    private void OpenDoor(DungeonGenerator.Directions direction)
    {
        switch (direction)
        {
            case DungeonGenerator.Directions.front:
                doors.transform.GetChild(0).gameObject.SetActive(false);
                break;
            case DungeonGenerator.Directions.back:
                doors.transform.GetChild(1).gameObject.SetActive(false);
                break;
            case DungeonGenerator.Directions.right:
                doors.transform.GetChild(2).gameObject.SetActive(false);
                break;
            case DungeonGenerator.Directions.left:
                doors.transform.GetChild(3).gameObject.SetActive(false);
                break;
        }
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") { playerInStructure = true; }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player") { playerInStructure = false; }
    }
}
