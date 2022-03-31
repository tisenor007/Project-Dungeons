using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public enum Directions
    {
        front,
        back,
        left,
        right
    }

    public enum StructureType
    {
        Room,
        Hallway,
        Length,
        StartStructure,
        EndStructure, 
        TrapStructure
    }
    public float structureSpacing = 10;
    public int roomChance = 80;
    [HideInInspector] public bool dungeonIsGenerating = false;
    [HideInInspector] public bool dungeonPreGenerating = false;
    [HideInInspector] public bool dungeonGenerated = false;
    [HideInInspector] public List<GameObject> structures = new List<GameObject>();
    [SerializeField] private int branchChance;
    [SerializeField] private int minBranchLengthRange;
    [SerializeField] private int maxBranchLengthRange;
    [SerializeField] private GameObject startStructure;
    [SerializeField] private GameObject endStructure;
    [SerializeField] private GameObject[] roomVariations;
    [SerializeField] private GameObject[] hallwayVariations;
    [SerializeField] private bool includeTraps;
    [SerializeField] private GameObject[] trapVariations;
    [SerializeField] private int maxStructures = 10;
    private List<GameObject> mainStructures = new List<GameObject>();
    private StructureType nextStructureType;
    private StructureType currentStructureType = StructureType.Room;
    private Vector3 nextStructureLoc;
    private Vector3 nextStructureRot;
    private int nextStructureDirection;
    private int nextStructureVariation;
    private int currentStructureVariation;
    private int branchDecision;
    private GameObject mainStructureBase;
    private GameObject currBranchStructureBase;
    private bool branchIsGenerating = false;
    private int branchStartStructNum;
    private int currentBranchStructCount;
    private int currentBranchLength;
    private bool trapInstantiated = false;

    // Update is called once per frame
    void Update()
    {
        if (!dungeonIsGenerating) { return; }
        //pregenerates small dungeon quickly so saving & loading next dungeon will work......
        if (dungeonPreGenerating && !dungeonGenerated) { GenerateNewDungeon(5);}
        else if (!dungeonPreGenerating && !dungeonGenerated) { GenerateNewDungeon(maxStructures); }
    }

    //master method
    public void GenerateNewDungeon(int maxStructureAmount)
    {
        if (structures.Count <=0) 
        {
            GenerateNewStructure(StructureType.StartStructure, mainStructureBase);
        }
        //adds rooms to dungeon until max number is met
        else if (structures.Count < maxStructureAmount && structures.Count > 0)
        {
            nextStructureType = (StructureType)ChooseNumbByChance((int)StructureType.Room, (int)StructureType.Hallway, roomChance);
            switch (branchIsGenerating)
            {
                case false:
                    GenerateNewStructure(nextStructureType, mainStructureBase);
                    if (IsBranchStuck(mainStructureBase))
                    {
                        mainStructureBase = mainStructures[Random.Range(0, mainStructures.Count)];
                        UpdateCurrentStructure(mainStructureBase);
                    }
                    //sets up branch
                    ResetBranchStats();
                    currentBranchLength = Random.Range(minBranchLengthRange, maxBranchLengthRange + 1);
                    branchStartStructNum = Random.Range(1, mainStructures.Count);
                    currBranchStructureBase = mainStructures[branchStartStructNum];
                    branchDecision = ChooseNumbByChance(1, 2, branchChance);
                    if (branchDecision == 1 && mainStructures.Count > 1) { branchIsGenerating = true; }
                    break;
                case true:
                    if (currentBranchStructCount >= currentBranchLength){CompleteBranch(includeTraps, mainStructureBase);}
                    if (IsBranchStuck(currBranchStructureBase)) { CompleteBranch(false, mainStructureBase); }
                    GenerateNewStructure(nextStructureType, currBranchStructureBase);
                    break;
            }
        }
        //makes them all children after generation
        else if (structures.Count >= maxStructureAmount) 
        {
            GenerateNewStructure(StructureType.EndStructure, mainStructureBase);
        }
    }

    public void ClearDungeon()
    {
        dungeonGenerated = false;
        dungeonIsGenerating = false;
        dungeonPreGenerating = false;
        branchIsGenerating = false;
        foreach (GameObject structure in structures) 
        { 
            Destroy(structure);
        }
        structures.Clear();
        mainStructures.Clear();
        mainStructureBase = null;
        currBranchStructureBase = null;
        ResetBranchStats();
    }

    public void CompleteGeneration()
    {
        foreach (GameObject structure in structures) { structure.transform.parent = this.transform; }
        dungeonIsGenerating = false;
        branchIsGenerating = false;
        dungeonGenerated = true;
        if (!dungeonPreGenerating) { return; }
        ClearDungeon(); 
        dungeonIsGenerating = true; 
    }

    public void GenerateSavedRoom(StructureType savedStructureType, int savedStructureVariation, Vector3 savedPos, Vector3 savedRot)
    {
        switch (savedStructureType)
        {
            case StructureType.Room:
                InstantiateStructure(roomVariations[savedStructureVariation], savedPos, savedRot, savedStructureType, savedStructureVariation);
                break;
            case StructureType.Hallway:
                InstantiateStructure(hallwayVariations[savedStructureVariation], savedPos, savedRot, savedStructureType, savedStructureVariation);
                break;
            case StructureType.StartStructure:
                InstantiateStructure(startStructure, savedPos, savedRot, savedStructureType, savedStructureVariation);
                break;
            case StructureType.EndStructure:
                InstantiateStructure(endStructure, savedPos, savedRot, savedStructureType, savedStructureVariation);
                break;
            case StructureType.TrapStructure:
                InstantiateStructure(trapVariations[savedStructureVariation], savedPos, savedRot, savedStructureType, savedStructureVariation);
                break;
        }
    }

    public Vector3 Direction(Directions wantedDirection, Vector3 basePos)
    {
        switch (wantedDirection)
        {
            case Directions.front:
                return new Vector3(basePos.x + structureSpacing, basePos.y, basePos.z);
                //break;
            case Directions.back:
                return new Vector3(basePos.x - structureSpacing, basePos.y, basePos.z);
                //break;
            case Directions.left:
                return new Vector3(basePos.x, basePos.y, basePos.z - structureSpacing);
                //break;
            case Directions.right:
                return new Vector3(basePos.x, basePos.y, basePos.z + structureSpacing);
                //break;
        }
        return Vector3.zero;
    }

    //tries to add room to dungeon
    private void GenerateNewStructure(StructureType structureType, GameObject baseStruct)
    {
        switch (structureType)
        {
            case StructureType.StartStructure:
                InstantiateStructure(startStructure, Vector3.zero, Vector3.zero, StructureType.StartStructure, 0);
                break;
            case StructureType.Room:
                if (!VariantIsRandomized(roomVariations)) { return; }
                if (!DirectionIsRandomized(baseStruct)) { return; }
                InstantiateStructure(roomVariations[nextStructureVariation], nextStructureLoc, nextStructureRot, nextStructureType, nextStructureVariation);
                break;
            case StructureType.Hallway:
                if (!VariantIsRandomized(hallwayVariations)) { return; }
                if (!DirectionIsRandomized(baseStruct)) { return; }
                InstantiateStructure(hallwayVariations[nextStructureVariation], nextStructureLoc, nextStructureRot, nextStructureType, nextStructureVariation);
                break;
            case StructureType.EndStructure:
                if (!DirectionIsRandomized(baseStruct)) { return; }
                InstantiateStructure(endStructure, nextStructureLoc, nextStructureRot, StructureType.EndStructure, 0);
                break;
            case StructureType.TrapStructure:
                if (!VariantIsRandomized(trapVariations)) { return; }
                if (!DirectionIsRandomized(baseStruct)) { return; }
                InstantiateStructure(trapVariations[nextStructureVariation], nextStructureLoc, nextStructureRot, StructureType.TrapStructure, nextStructureVariation);
                break;
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

        if (branchIsGenerating) 
        { 
            currentBranchStructCount++;
            currBranchStructureBase = structures[structures.Count-1];
            UpdateCurrentStructure(currBranchStructureBase);
        }
        else if (!branchIsGenerating) 
        {
            mainStructures.Add(structures[structures.Count - 1]);
            mainStructureBase = structures[structures.Count - 1];
            UpdateCurrentStructure(mainStructureBase);
        }

        if (currentStructureType == StructureType.EndStructure) { CompleteGeneration(); }
        if (currentStructureType == StructureType.TrapStructure) { trapInstantiated = true; }
    }

    //makes random direction that next room / structure can go
    private bool DirectionIsRandomized(GameObject baseStruct)
    {
        if (structures.Count <= 0) { nextStructureLoc = Vector3.zero; nextStructureRot = Vector3.zero; return false; }
        //restricts directions based off where it is coming from and where it is going.....
        if (currentStructureType == StructureType.Hallway && baseStruct.transform.eulerAngles.y == 0) { nextStructureDirection = ChooseNumbByChance(0, 1, 50); }// Random.Range(0, 1); }
        else if (currentStructureType == StructureType.Hallway && baseStruct.transform.eulerAngles.y == 90) { nextStructureDirection = ChooseNumbByChance(2, 3, 50); }// Random.Range(2, 3); }
        else if (currentStructureType == StructureType.Room || currentStructureType == StructureType.TrapStructure) { nextStructureDirection = Random.Range(0, 4); }

        if (nextStructureDirection <= 0)
        {
            nextStructureRot = new Vector3(0, 0, 0);
            nextStructureLoc = Direction(Directions.front, baseStruct.transform.position);
            if (StructureIsHere(nextStructureLoc)) { return false; }
            return true;
        }
        else if (nextStructureDirection == 1)
        {
            nextStructureRot = new Vector3(0, 0, 0);
            nextStructureLoc = Direction(Directions.back, baseStruct.transform.position);
            if (StructureIsHere(nextStructureLoc)) { return false; }
            return true;
        }
        else if (nextStructureDirection == 2)
        {
            if (nextStructureType == StructureType.Hallway) { nextStructureRot = new Vector3(0, 90, 0); }
            else if (nextStructureType != StructureType.Hallway) { nextStructureRot = new Vector3(0, 0, 0); }
            nextStructureLoc = Direction(Directions.right, baseStruct.transform.position);
            if (StructureIsHere(nextStructureLoc)) { return false; }
            return true;
        }
        else if (nextStructureDirection >= 3)
        {
            if (nextStructureType == StructureType.Hallway) { nextStructureRot = new Vector3(0, 90, 0); }
            else if (nextStructureType != StructureType.Hallway) { nextStructureRot = new Vector3(0, 0, 0); }
            nextStructureLoc = Direction(Directions.left, baseStruct.transform.position);
            if (StructureIsHere(nextStructureLoc)) { return false; }
            return true;
        }
        return false;
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

    private void UpdateCurrentStructure(GameObject structureBase)
    {
        currentStructureType = structureBase.GetComponent<StructureBehavior>().currentStructureType;
        currentStructureVariation = structureBase.GetComponent<StructureBehavior>().currentVariation;
    }
    
    private void CompleteBranch(bool trapsIncluded, GameObject structureBase)
    {
        if (trapsIncluded)
        {
            trapInstantiated = false;
            GenerateNewStructure(StructureType.TrapStructure, currBranchStructureBase);
        }
        if (!trapsIncluded || trapsIncluded && trapInstantiated) { UpdateCurrentStructure(structureBase); branchIsGenerating = false; }
    }

    private void ResetBranchStats()
    {
        currentBranchStructCount = 0;
        currentBranchLength = -1;
        branchStartStructNum = -1;
        branchDecision = -1;
    }

    private bool IsBranchStuck(GameObject baseStruct)
    {
        if (currentStructureType == StructureType.Room ||
            currentStructureType == StructureType.StartStructure ||
            currentStructureType == StructureType.TrapStructure ||
            currentStructureType == StructureType.EndStructure)
        {
            if (!StructureIsHere(Direction(Directions.front, baseStruct.transform.position))) { return false; }
            if (!StructureIsHere(Direction(Directions.back, baseStruct.transform.position))) { return false; }
            if (!StructureIsHere(Direction(Directions.left, baseStruct.transform.position))) { return false; }
            if (!StructureIsHere(Direction(Directions.right, baseStruct.transform.position))) { return false; }
            return true;
        }
        else if (currentStructureType == StructureType.Hallway && baseStruct.transform.eulerAngles.y == 0)
        {
            if (!StructureIsHere(Direction(Directions.front, baseStruct.transform.position))) { return false; }
            if (!StructureIsHere(Direction(Directions.back, baseStruct.transform.position))) { return false; }
            return true;
        }
        else if (currentStructureType == StructureType.Hallway && baseStruct.transform.eulerAngles.y == 90)
        {
            if (!StructureIsHere(Direction(Directions.left, baseStruct.transform.position))) { return false; }
            if (!StructureIsHere(Direction(Directions.right, baseStruct.transform.position))) { return false; }
            return true;
        }
        return false;
    }

    private int ChooseNumbByChance(int output1, int output2, int chanceNum)
    {
        int chance = Random.Range(0, 101);
        if (chance < chanceNum) { return output1; }
        else if (chance > chanceNum) { return output2; }
        return 0;
    }
}
