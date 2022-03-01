using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public enum StructureType
    {
        Room,
        Hallway,
        Length,
        StartStructure,
        EndStructure
    }
    public float structureSpacing = 10;
    public int roomChance = 80;
    public int hallwayChance = 10;
    public bool dungeonIsGenerating = false;
    [HideInInspector] public bool dungeonGenerated = false;
    [HideInInspector] public List<GameObject> structures = new List<GameObject>();
    [SerializeField] private GameObject startStructure;
    [SerializeField] private GameObject endStructure;
    [SerializeField] private GameObject[] roomVariations;
    [SerializeField] private GameObject[] hallwayVariations;
    [SerializeField] private int maxStructures = 10;
    private StructureType nextStructureType;
    private StructureType currentStructureType = StructureType.Room;
    private Vector3 nextStructureLoc;
    private Vector3 nextStructureRot;
    private int nextStructureDirection;
    private int nextStructureVariation;
    private int currentStructureVariation;
   
    // Update is called once per frame
    void Update()
    {
        if (dungeonIsGenerating) { GenerateNewDungeon(); }
    }

    //master method
    public void GenerateNewDungeon()
    {
        InstantiateStructure(startStructure, Vector3.zero, Vector3.zero, StructureType.StartStructure, 0);
        //adds rooms to dungeon until max number is met
        if (structures.Count < maxStructures)
        {
            GenerateNewStructure();
        }
        //makes them all children after generation
        else if (structures.Count >= maxStructures) 
        {
            if (DirectionIsRandomized() && !StructureIsHere(nextStructureLoc)) 
            {
                InstantiateStructure(endStructure, nextStructureLoc, nextStructureRot, StructureType.EndStructure, 0);
                CompleteGeneration();
            }
        }
    }

    public void ClearDungeon()
    {
        foreach (Transform child in this.transform) { GameObject.Destroy(child.gameObject); }
        structures.Clear();
        dungeonGenerated = false;
        dungeonIsGenerating = false;
    }

    public void CompleteGeneration()
    {
        foreach (GameObject structure in structures) { structure.transform.parent = this.transform; }
        dungeonIsGenerating = false;
        dungeonGenerated = true;
    }

    public void GenerateSavedRoom(StructureType savedStructureType, int savedStructureVariation, Vector3 savedPos, Vector3 savedRot)
    {
        if (savedStructureType == StructureType.Room)
        {
            InstantiateStructure(roomVariations[savedStructureVariation], savedPos, savedRot, savedStructureType, savedStructureVariation);
        }
        else if (savedStructureType == StructureType.Hallway)
        {
            InstantiateStructure(hallwayVariations[savedStructureVariation], savedPos, savedRot, savedStructureType, savedStructureVariation);
        }
        else if (savedStructureType == StructureType.StartStructure)
        {
            InstantiateStructure(startStructure, savedPos, savedRot, savedStructureType, savedStructureVariation);
        }
        else if (savedStructureType == StructureType.EndStructure)
        {
            InstantiateStructure(endStructure, savedPos, savedRot, savedStructureType, savedStructureVariation);
        }
    }

    //tries to add room to dungeon
    private void GenerateNewStructure()
    {
        nextStructureType = (StructureType)ChooseNumbByChance((int)StructureType.Room, (int)StructureType.Hallway, roomChance, hallwayChance);
        if (nextStructureType == StructureType.Room)
        {
            if (!VariantIsRandomized(roomVariations)) { return; }
            if (!DirectionIsRandomized()) { return; }
            InstantiateStructure(roomVariations[nextStructureVariation], nextStructureLoc, nextStructureRot, nextStructureType, nextStructureVariation);
        }
        else if (nextStructureType == StructureType.Hallway)
        {
            if (!VariantIsRandomized(roomVariations)) { return; }
            if (!DirectionIsRandomized()) { return; }
            InstantiateStructure(hallwayVariations[nextStructureVariation], nextStructureLoc, nextStructureRot, nextStructureType, nextStructureVariation);
        }
    }

    private bool VariantIsRandomized(GameObject[] structureVariations)
    {
        if (structures.Count <= 0) { return false; }
        if (structureVariations.Length <= 0) { return false; }

        nextStructureVariation = Random.Range(0, structureVariations.Length);
        if (nextStructureVariation == currentStructureVariation && nextStructureType == currentStructureType) { return false; }

        return true;
    }

    //instantiates & stores room
    private void InstantiateStructure(GameObject structure, Vector3 structureDestination, Vector3 structureRotation, StructureType nextStructType, int nextStructVariation)
    {
        if (StructureIsHere(structureDestination)) { return; }

        structures.Add(Instantiate(structure, structureDestination, Quaternion.Euler(structureRotation)));

        currentStructureType = nextStructType;
        structure.GetComponent<StructureBehavior>().currentStructureType = nextStructType;

        currentStructureVariation = nextStructVariation;
        structure.GetComponent<StructureBehavior>().currentVariation = nextStructVariation;
    }


    //makes random direction that next room / structure can go
    private bool DirectionIsRandomized()
    {
        if (structures.Count <= 0) { nextStructureLoc = Vector3.zero; nextStructureRot = Vector3.zero; return false; }
        //restricts directions based off where it is coming from and where it is going.....
        if (currentStructureType == StructureType.Hallway && structures[structures.Count - 1].transform.eulerAngles.y == 0) { nextStructureDirection = Random.Range(0, 2); }
        else if (currentStructureType == StructureType.Hallway && structures[structures.Count - 1].transform.eulerAngles.y == 90) { nextStructureDirection = Random.Range(2, 3); }
        else if (currentStructureType == StructureType.Room) { nextStructureDirection = Random.Range(0, 3); }

        if (nextStructureDirection <= 0)
        {
            nextStructureRot = new Vector3(0, 0, 0);
            nextStructureLoc = new Vector3(structures[structures.Count - 1].transform.position.x + structureSpacing, structures[structures.Count - 1].transform.position.y, structures[structures.Count - 1].transform.position.z);
            return true;
        }
        else if (nextStructureDirection == 1)
        {
            nextStructureRot = new Vector3(0, 0, 0);
            nextStructureLoc = new Vector3(structures[structures.Count - 1].transform.position.x - structureSpacing, structures[structures.Count - 1].transform.position.y, structures[structures.Count - 1].transform.position.z);
            return true;
        }
        else if (nextStructureDirection == 2)
        {
            if (nextStructureType == StructureType.Hallway) { nextStructureRot = new Vector3(0, 90, 0); }
            else if (nextStructureType != StructureType.Hallway) { nextStructureRot = new Vector3(0, 0, 0); }
            nextStructureLoc = new Vector3(structures[structures.Count - 1].transform.position.x, structures[structures.Count - 1].transform.position.y, structures[structures.Count - 1].transform.position.z + structureSpacing);
            return true;
        }
        else if (nextStructureDirection >= 3)
        {
            if (nextStructureType == StructureType.Hallway) { nextStructureRot = new Vector3(0, 90, 0); }
            else if (nextStructureType != StructureType.Hallway) { nextStructureRot = new Vector3(0, 0, 0); }
            nextStructureLoc = new Vector3(structures[structures.Count - 1].transform.position.x, structures[structures.Count - 1].transform.position.y, structures[structures.Count - 1].transform.position.z - structureSpacing);
            return true;
        }
        return false;
    }

    private int ChooseNumbByChance(int output1, int output2, int chance1, int chance2)
    {
        int chance = Random.Range(0, 100);
        if (chance < chance1) { return output1; }
        else if (chance < chance1+chance2) { return output2; }
        return 0;
    }

    //prevents ovelapping
    private bool StructureIsHere(Vector3 chosenSpot)
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
