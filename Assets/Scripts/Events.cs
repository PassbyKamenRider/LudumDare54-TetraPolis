using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Events : MonoBehaviour
{
    public enum Event
    {
        FIRE,
        EARTHQUAKE,
        DISEASE
    }

    public Event eventType;
    private WorkingBlock wBlock;
    private TetrisBlock tBlock;
    private float updateTime = 15.0f;
    private float updateTimer;
    private float destroyTime = 20.0f;
    private float destroyTimer;

    private void OnEnable() {
        wBlock = GetComponent<WorkingBlock>();
        tBlock = GetComponentInParent<TetrisBlock>();
        updateTimer = Time.time;
        destroyTimer = Time.time;
        
        switch(eventType)
        {
            case Event.FIRE:
            GameObject obj = Instantiate(FindObjectOfType<ResourceManager>().fireEffect, transform.position, Quaternion.identity);
            obj.transform.SetParent(transform);
            break;

            case Event.EARTHQUAKE:
            int yPosition = Mathf.RoundToInt(transform.position.y);
            tBlock.removeLine(yPosition, true);
            tBlock.moveDown(yPosition);
            enabled = false;
            break;

            case Event.DISEASE:
            wBlock.isSick = true;
            obj = Instantiate(FindObjectOfType<ResourceManager>().diseaseEffect, transform.position, Quaternion.identity);
            obj.transform.SetParent(transform);
            break;
        }
    }

    private void Update() {
        if (eventType != Event.EARTHQUAKE && Time.time - updateTimer >= updateTime)
        {
            eventSpread(eventType);
            updateTimer = Time.time;
        }
        if (eventType == Event.FIRE && Time.time - destroyTimer >= destroyTime)
        {
            int xPos = Mathf.RoundToInt(transform.position.x);
            int yPos = Mathf.RoundToInt(transform.position.y);
            // update resources in the block
            FindObjectOfType<ResourceManager>().removeBlock(SpawnPoint.grids[xPos, yPos].GetComponent<WorkingBlock>());
            // destroy the block
            SpawnPoint.grids[xPos, yPos] = null;
            Destroy(this.gameObject);
        }
    }

    public void eventSpread(Event spreadEvent)
    {
        int xPos = Mathf.RoundToInt(transform.position.x);
        int yPos = Mathf.RoundToInt(transform.position.y);
        if (xPos > 0 && SpawnPoint.grids[xPos-1, yPos])
        {
            Events e = SpawnPoint.grids[xPos-1, yPos].GetComponent<Events>();
            if (!e.enabled)
            {
                e.enabled = true;
                e.eventType = spreadEvent;
            }
        }
        if (xPos < TetrisBlock.boardWidth - 1 && SpawnPoint.grids[xPos+1, yPos])
        {
            Events e = SpawnPoint.grids[xPos+1, yPos].GetComponent<Events>();
            if (!e.enabled)
            {
                e.enabled = true;
                e.eventType = spreadEvent;
            }
        }
        if (yPos > 0 && SpawnPoint.grids[xPos, yPos-1])
        {
            Events e = SpawnPoint.grids[xPos, yPos-1].GetComponent<Events>();
            if (!e.enabled)
            {
                e.enabled = true;
                e.eventType = spreadEvent;
            }
        }
        if (yPos < TetrisBlock.boardHeight - 1 && SpawnPoint.grids[xPos, yPos+1])
        {
            Events e = SpawnPoint.grids[xPos, yPos+1].GetComponent<Events>();
            if (!e.enabled)
            {
                e.enabled = true;
                e.eventType = spreadEvent;
            }
        }
    }
}
