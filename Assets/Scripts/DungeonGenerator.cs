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
    public int structureSpacing = 10;
    public int roomChance = 80;
    public int hallwayChance = 10;
    [HideInInspector] public List<GameObject> structures = new List<GameObject>();
    [SerializeField] private GameObject[] roomVariations;
    [SerializeField] private GameObject[] hallwayVariations;
    [SerializeField] private int maxStructures = 10;
    private StructureType nextStructureType;
    private StructureType currentStructureType = StructureType.Room;
    private Vector3 nextStructureLoc;
    private Vector3 nextStructureRot;
    private int nextStructureDirection;
    private int roomVariation;
    private int hallwayVariation;
   
    // Update is called once per frame
    void Update()
    {
        GenerateNewDungeon();
    }

    //master method
    public void GenerateNewDungeon()
    {
        //adds rooms to dungeon until max number is met
        if (structures.Count < maxStructures)
        {
            GenerateNewRoom();
        }
        //makes them all children after generation
        else if (structures.Count >= maxStructures) 
        {
            foreach (GameObject structure in structures)
            {
                structure.transform.parent = this.transform;
            }
        }
    }

    //tries to add room to dungeon
    private void GenerateNewRoom()
    {
        nextStructureType = (StructureType)ChooseNumbByChance((int)StructureType.Room, (int)StructureType.Hallway, roomChance, hallwayChance);
        roomVariation = Random.Range(0, roomVariations.Length);
        hallwayVariation = Random.Range(0, hallwayVariations.Length);
        ChooseNextRoomDirection();

        if (nextStructureType == StructureType.Room)
        {
            InstantiateStructure(roomVariations, roomVariation, nextStructureLoc, nextStructureRot);
        }
        else if (nextStructureType == StructureType.Hallway)
        {
            InstantiateStructure(hallwayVariations, hallwayVariation, nextStructureLoc, nextStructureRot);
        }
    }

    //instantiates & stores room
    private void InstantiateStructure(GameObject[] structureVariations, int structureVariation, Vector3 instantiateLoc, Vector3 instantiateRot)
    {
        if (structureVariations.Length <= 0) { return; }
        if (IsStructureHere(instantiateLoc)) { return; }
        structures.Add(Instantiate(structureVariations[structureVariation], instantiateLoc, Quaternion.Euler(instantiateRot)));
        currentStructureType = (StructureType)nextStructureType;
        structureVariations[structureVariation].GetComponentInChildren<StructureBehavior>().thisStructureType = currentStructureType;
        
    }

    //makes random direction that next room / structure can go
    private void ChooseNextRoomDirection()
    {
        if (structures.Count <= 0) { nextStructureLoc = Vector3.zero; nextStructureRot = Vector3.zero; return; }
        //restricts directions based off where it is coming from and where it is going.....
        if (currentStructureType == StructureType.Hallway && structures[structures.Count - 1].transform.eulerAngles.y == 0) { nextStructureDirection = Random.Range(0, 2); }
        else if (currentStructureType == StructureType.Hallway && structures[structures.Count - 1].transform.eulerAngles.y == 90) { nextStructureDirection = Random.Range(2, 3); }
        else if (currentStructureType == StructureType.Room) { nextStructureDirection = Random.Range(0, 3); }

        if (nextStructureDirection <= 0)
        {
            nextStructureRot = new Vector3(0, 0, 0);
            nextStructureLoc = new Vector3(structures[structures.Count - 1].transform.position.x + structureSpacing, structures[structures.Count - 1].transform.position.y, structures[structures.Count - 1].transform.position.z);
        }
        else if (nextStructureDirection == 1)
        {
            nextStructureRot = new Vector3(0, 0, 0);
            nextStructureLoc = new Vector3(structures[structures.Count - 1].transform.position.x - structureSpacing, structures[structures.Count - 1].transform.position.y, structures[structures.Count - 1].transform.position.z);
        }
        else if (nextStructureDirection == 2)
        {
            if (nextStructureType == StructureType.Hallway) { nextStructureRot = new Vector3(0, 90, 0); }
            else if (nextStructureType != StructureType.Hallway) { nextStructureRot = new Vector3(0, 0, 0); }
            nextStructureLoc = new Vector3(structures[structures.Count - 1].transform.position.x, structures[structures.Count - 1].transform.position.y, structures[structures.Count - 1].transform.position.z + structureSpacing);
        }
        else if (nextStructureDirection >= 3)
        {
            if (nextStructureType == StructureType.Hallway) { nextStructureRot = new Vector3(0, 90, 0); }
            else if (nextStructureType != StructureType.Hallway) { nextStructureRot = new Vector3(0, 0, 0); }
            nextStructureLoc = new Vector3(structures[structures.Count - 1].transform.position.x, structures[structures.Count - 1].transform.position.y, structures[structures.Count - 1].transform.position.z - structureSpacing);
        }
    }

    private int ChooseNumbByChance(int output1, int output2, int chance1, int chance2)
    {
        int chance = Random.Range(0, 100);
        if (chance < chance1) { return output1; }
        else if (chance < chance1+chance2) { return output2; }
        return 0;
    }

    //prevents ovelapping
    private bool IsStructureHere(Vector3 chosenSpot)
    {
        for (int i = 0; i < structures.Count; i++)
        {
            if (structures[i].transform.position == chosenSpot)
            {
                return true;
            }
        }
        return false;
    }
}
