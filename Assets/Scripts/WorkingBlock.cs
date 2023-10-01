using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkingBlock : MonoBehaviour
{
    public enum BlockType
    {
        POP,
        GOLD,
        FOOD
    }
    public BlockType blockType;
    public int index;
    //private float timer;
    //production variables
    //private float efficiency = 1.0f;
    //private float productivity;
    //private float produceLoop;
    public int capacity;
    public int usedSpace;
    public bool isSick;
    private void Start() {
        ResourceManager resourceManager = FindObjectOfType<ResourceManager>();
        switch(blockType)
        {
            case BlockType.POP:
            usedSpace = 1;
            resourceManager.populationAmount += 1;
            capacity = 10;
            break;

            case BlockType.GOLD:
            capacity = 5;
            break;

            case BlockType.FOOD:
            capacity = 5;
            break;
        }
        resourceManager.updateDisplay();
    }
}
