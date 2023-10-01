using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public GameObject[] TetrisPrefabs;
    public ResourceManager resourceManager;
    public static Transform[,] grids = new Transform[10,20];
    public static GameObject currBlock;

    private void Start() {
        resourceManager = FindObjectOfType<ResourceManager>();
        spawnTetris();
    }
    public void spawnTetris()
    {
        currBlock = Instantiate(TetrisPrefabs[Random.Range(0, TetrisPrefabs.Length)], transform.position, Quaternion.identity);
        foreach (Transform child in currBlock.transform)
        {
            int blockType = Random.Range(0, 3);
            child.GetComponent<WorkingBlock>().blockType = (WorkingBlock.BlockType) blockType;
            child.GetComponent<SpriteRenderer>().sprite = resourceManager.blockTypeSprites[blockType];
        }
    }
}
