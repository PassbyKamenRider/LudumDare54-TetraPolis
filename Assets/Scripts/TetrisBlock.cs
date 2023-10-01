using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBlock : MonoBehaviour
{
    private float timer;
    [SerializeField] private float fallTime = 0.8f;
    public static int boardHeight = 20;
    public static int boardWidth = 10;
    public Vector3 rotationPoint;

    void Update()
    {
        // L&R control
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (isValidMove(Vector3.left))
                transform.position += Vector3.left;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (isValidMove(Vector3.right))
                transform.position += Vector3.right;
        }
        
        // fall control
        if (Time.time - timer > ((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) ? fallTime / 10 : fallTime))
        {
            if (isValidMove(Vector3.down))
            {
                transform.position += Vector3.down;
            } else {
                // reaches the ground
                this.enabled = false;
                if (transform.TransformPoint(rotationPoint).y >= 16)
                {
                    Destroy(FindObjectOfType<SpawnPoint>());
                    FindObjectOfType<ResourceManager>().processEnd();
                    return;
                }
                gameObject.AddComponent<ShapeHover>();
                FindObjectOfType<SpawnPoint>().spawnTetris();
                occupyGrid();
                checkLine();
            }
            timer = Time.time;
        }

        // rotation control
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.R))
        {
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0,0,1), 90);
            foreach (Transform child in transform)
            {
                child.Rotate(0, 0, -90);
            }
            if (!isValidMove(Vector3.zero))
            {
                transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0,0,1), -90);
                foreach (Transform child in transform)
                {
                    child.Rotate(0, 0, 90);
                }
            }
        }
    }

    // moved to here because removing current moving block crashes the game
    private void initializeBlock(WorkingBlock block)
    {
        ResourceManager resourceManager = FindObjectOfType<ResourceManager>();
        switch(block.blockType)
        {
            case WorkingBlock.BlockType.POP:
            block.index = resourceManager.populationBlocks.Count;
            resourceManager.populationBlocks.Add(block);
            break;

            case WorkingBlock.BlockType.GOLD:
            block.index = resourceManager.mineBlocks.Count;
            resourceManager.mineBlocks.Add(block);
            break;

            case WorkingBlock.BlockType.FOOD:
            block.index = resourceManager.farmBlocks.Count;
            resourceManager.farmBlocks.Add(block);
            break;
        }
    }

    private void occupyGrid()
    {
        foreach (Transform child in transform)
        {
            int childx = Mathf.RoundToInt(child.transform.position.x);
            int childy = Mathf.RoundToInt(child.transform.position.y);
            SpawnPoint.grids[childx, childy] = child;
            child.GetComponent<WorkingBlock>().enabled = true;
            initializeBlock(child.GetComponent<WorkingBlock>());
        }
    }

    private void checkLine()
    {
        for (int i = boardHeight - 1; i >= 0; i--)
        {
            if (isLine(i))
            {
                removeLine(i, false);
                moveDown(i);
            }
        }
    }

    private bool isLine(int line)
    {
        for (int row = 0; row < boardWidth; row++)
        {
            if (!SpawnPoint.grids[row, line])
            {
                return false;
            }
        }
        return true;
    }

    public void removeLine(int line, bool isEarthQuake)
    {
        for (int row = 0; row < boardWidth; row++)
        {
            if (SpawnPoint.grids[row, line])
            {
                if (isEarthQuake)
                {
                    Instantiate(FindObjectOfType<ResourceManager>().breakEffect, SpawnPoint.grids[row,line].position, Quaternion.identity);
                }
                // update resources in the block
                FindObjectOfType<ResourceManager>().removeBlock(SpawnPoint.grids[row, line].GetComponent<WorkingBlock>());
                // destroy the block
                Destroy(SpawnPoint.grids[row, line].gameObject);
                SpawnPoint.grids[row, line] = null;
            }
        }
    }

    public void moveDown(int line)
    {
        for (int i = line; i < boardHeight; i++)
        {
            for (int j = 0; j < boardWidth; j++)
            {
                if (SpawnPoint.grids[j, i])
                {
                    SpawnPoint.grids[j,i].position -= Vector3.up;
                    SpawnPoint.grids[j, i-1] = SpawnPoint.grids[j,i];
                    SpawnPoint.grids[j, i] = null;
                }
            }
        }
    }

    private bool isValidMove(Vector3 movement)
    {
        foreach (Transform child in transform)
        {
            int childx = Mathf.RoundToInt(child.transform.position.x + movement.x);
            int childy = Mathf.RoundToInt(child.transform.position.y + movement.y);
            if (childx < 0 || childx >= boardWidth || childy < 0 || childy >= boardHeight)
            {
                return false;
            }
            if (SpawnPoint.grids[childx, childy])
            {
                return false;
            }
        }
        return true;
    }

    public void moveToGround()
    {
        foreach (Transform child in transform)
        {
            int childx = Mathf.RoundToInt(child.transform.position.x);
            int childy = Mathf.RoundToInt(child.transform.position.y);
            SpawnPoint.grids[childx, childy] = null;
        }
        while (isValidMove(Vector3.down))
        {
            transform.position += Vector3.down;
        }
        foreach (Transform child in transform)
        {
            int childx = Mathf.RoundToInt(child.transform.position.x);
            int childy = Mathf.RoundToInt(child.transform.position.y);
            SpawnPoint.grids[childx, childy] = child;
        }
    }
}
