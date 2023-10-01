using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    private float updateTimer;
    private float eventTimer;
    private bool enableAutoFill;
    private int mineToFill;
    private int farmToFill;
    public int score;
    public int populationAmount;
    public int minerAmount;
    public float goldAmount;
    public int farmerAmount;
    public float foodAmount;
    public int nextEvent;
    [SerializeField] public Sprite[] blockTypeSprites;
    [SerializeField] public Sprite[] eventSprites;
    [SerializeField] public GameObject breakEffect;
    [SerializeField] public GameObject fireEffect;
    [SerializeField] public GameObject diseaseEffect;
    [SerializeField] public GameObject endScreen;
    [SerializeField] public Animator autoAnimator;
    public List<WorkingBlock> populationBlocks = new List<WorkingBlock>();
    public List<WorkingBlock> mineBlocks = new List<WorkingBlock>();
    public List<WorkingBlock> farmBlocks = new List<WorkingBlock>();
    [SerializeField] TextMeshProUGUI populationDisplay;
    [SerializeField] TextMeshProUGUI goldDisplay;
    [SerializeField] TextMeshProUGUI foodDisplay;
    [SerializeField] TextMeshProUGUI scoreDisplay;
    [SerializeField] TextMeshProUGUI mineSetting;
    [SerializeField] TextMeshProUGUI farmSetting;
    [SerializeField] Image eventSetting;
    [SerializeField] TextMeshProUGUI eventTimerDisplay;

    private void Start() {
        updateTimer = Time.time;
        eventTimer = Time.time;
        nextEvent = Random.Range(0,3);
        updateEventDisplay();
    }

    private void Update() {
        eventTimerDisplay.text = string.Format("{0:00}:{1:00}", 0, 15 - Time.time + eventTimer);
        if (foodAmount <= -500)
        {
            processEnd();
        }
        if (Time.time - updateTimer >= 1.0f)
        {
            // production
            int sickMiner = getSickCount(WorkingBlock.BlockType.GOLD);
            int sickFarmer = getSickCount(WorkingBlock.BlockType.FOOD);
            goldAmount += (minerAmount - sickMiner) * 10.0f + sickMiner * 5.0f;
            foodAmount += (farmerAmount - sickFarmer) * 10.0f + sickFarmer * 5.0f;
            foodAmount -= populationAmount * 5.0f;

            // display
            populationDisplay.text = "POP:" + populationAmount.ToString();
            goldDisplay.text = "GOLD:" + goldAmount.ToString();
            foodDisplay.text = "FOOD:" + foodAmount.ToString();
            if (enableAutoFill)
            {
                autoFill();
            }
            updateTimer = Time.time;
        }

        if (Time.time - eventTimer >= 15.0f)
        {
            populationAmount += 1;

            int randomBlockType = Random.Range(0,3);
            switch(randomBlockType)
            {
                case 0:
                var freeBlocks = getFreeBlocks(WorkingBlock.BlockType.POP);
                if (freeBlocks != null && freeBlocks.Count != 0)
                {
                    Events e = freeBlocks[Random.Range(0, freeBlocks.Count)].GetComponent<Events>();
                    e.eventType = (Events.Event) nextEvent;
                    e.enabled = true;
                }
                break;

                case 1:
                freeBlocks = getFreeBlocks(WorkingBlock.BlockType.GOLD);
                if (freeBlocks != null && freeBlocks.Count != 0)
                {
                    Events e = freeBlocks[Random.Range(0, freeBlocks.Count)].GetComponent<Events>();
                    e.eventType = (Events.Event) nextEvent;
                    e.enabled = true;
                }
                break;

                case 2:
                freeBlocks = getFreeBlocks(WorkingBlock.BlockType.FOOD);
                if (freeBlocks != null && freeBlocks.Count != 0)
                {
                    Events e = freeBlocks[Random.Range(0, freeBlocks.Count)].GetComponent<Events>();
                    e.eventType = (Events.Event) nextEvent;
                    e.enabled = true;
                }
                break;
            }
            nextEvent = Random.Range(0,3);
            updateEventDisplay();
            eventTimer = Time.time;
        }
    }

    public int getSickCount(WorkingBlock.BlockType blockType)
    {
        switch(blockType)
        {
            case WorkingBlock.BlockType.POP:
            return 0;

            case WorkingBlock.BlockType.GOLD:
            return mineBlocks.FindAll(x => x.isSick).Count;

            case WorkingBlock.BlockType.FOOD:
            return farmBlocks.FindAll(x => x.isSick).Count;
        }
        return 0;
    }

    // get blocks without events
    public List<WorkingBlock> getFreeBlocks(WorkingBlock.BlockType blockType)
    {
        switch(blockType)
        {
            case WorkingBlock.BlockType.POP:
            return populationBlocks.FindAll((x) => !x.GetComponent<Events>().enabled);

            case WorkingBlock.BlockType.GOLD:
            return mineBlocks.FindAll((x) => !x.GetComponent<Events>().enabled);

            case WorkingBlock.BlockType.FOOD:
            return farmBlocks.FindAll((x) => !x.GetComponent<Events>().enabled);

            default:
            return null;
        }
    }

    // get available positions in mines
    public int getMineCapacity()
    {
        int total = 0;
        foreach (WorkingBlock block in mineBlocks)
        {
            total += block.capacity;
        }
        return total;
    }

    // get available positions in farms
    public int getFarmCapacity()
    {
        int total = 0;
        foreach (WorkingBlock block in farmBlocks)
        {
            total += block.capacity;
        }
        return total;
    }

    // update working list display
    public void updateDisplay()
    {
        mineSetting.text = "Mine: " + minerAmount.ToString() + "/" + getMineCapacity();
        farmSetting.text = "Farm: " + farmerAmount.ToString() + "/" + getFarmCapacity();
    }

    // update score display
    public void updateScore()
    {
        scoreDisplay.text = "SCORE:\n" + score.ToString();
    }

    // update event display
    public void updateEventDisplay()
    {
        eventSetting.sprite = eventSprites[nextEvent];
    }

    // add one miner to working list
    public void addMiner()
    {
        if (minerAmount + farmerAmount >= populationAmount || mineToFill >= mineBlocks.Count)
        {
            return;
        }

        mineBlocks[mineToFill].usedSpace += 1;
        minerAmount += 1;

        if (mineBlocks[mineToFill].usedSpace == mineBlocks[mineToFill].capacity)
        {
            mineToFill += 1;
        }
        updateDisplay();
    }

    // remove one miner to working list
    public void removeMiner()
    {
        if (minerAmount <= 0)
        {
            return;
        }
        if (mineToFill >= mineBlocks.Count)
        {
            mineToFill -= 1;
        }
        mineBlocks[mineToFill].usedSpace -= 1;
        minerAmount -= 1;
        updateDisplay();
    }

    // add one farmer to working list
    public void addFarmer()
    {
        if (minerAmount + farmerAmount >= populationAmount || farmToFill >= farmBlocks.Count)
        {
            return;
        }

        farmBlocks[farmToFill].usedSpace += 1;
        farmerAmount += 1;

        if (farmBlocks[farmToFill].usedSpace == farmBlocks[farmToFill].capacity)
        {
            farmToFill += 1;
        }
        updateDisplay();
    }

    // remove one farmer from working list
    public void removeFarmer()
    {
        if (farmerAmount <= 0)
        {
            return;
        }
        if (farmToFill >= farmBlocks.Count)
        {
            farmToFill -= 1;
        }
        farmBlocks[farmToFill].usedSpace -= 1;
        farmerAmount -= 1;
        updateDisplay();
    }

    // remove block from lists, remove current positions
    public void removeBlock(WorkingBlock block)
    {
        score += block.usedSpace * 200;
        updateScore();
        switch (block.blockType)
        {
            case WorkingBlock.BlockType.POP:
            for (int i = block.index + 1; i < populationBlocks.Count; i++)
            {
                populationBlocks[i].index -= 1;
            }
            populationBlocks.RemoveAt(block.index);
            for (int i = 0; i < block.usedSpace; i++)
            {
                if (minerAmount >= farmerAmount)
                {
                    removeMiner();
                }
                else
                {
                    removeFarmer();
                }
            }
            populationAmount -= block.usedSpace;
            break;

            case WorkingBlock.BlockType.GOLD:
            if (block.index < mineToFill)
            {
                mineToFill -= 1;
            }
            for (int i = block.index + 1; i < mineBlocks.Count; i++)
            {
                mineBlocks[i].index -= 1;
            }
            mineBlocks.RemoveAt(block.index);
            break;
            
            case WorkingBlock.BlockType.FOOD:
            if (block.index < farmToFill)
            {
                farmToFill -= 1;
            }
            for (int i = block.index + 1; i < farmBlocks.Count; i++)
            {
                farmBlocks[i].index -= 1;
            }
            farmBlocks.RemoveAt(block.index);
            break;
        }
        updateDisplay();
        if (block.transform.parent.childCount == 1)
        {
            Destroy(block.transform.parent.gameObject);
        }
    }

    // auto fill vacant positions
    public void autoFill()
    {
        int amountToFill = populationAmount - minerAmount - farmerAmount;
        for (int i = 0; i < amountToFill; i++)
        {
            if (minerAmount <= farmerAmount)
            {
                addMiner();
            }
            else
            {
                addFarmer();
            }
        }
    }

    public void switchAutoFill()
    {
        enableAutoFill = !enableAutoFill;
        if (enableAutoFill)
        {
            autoAnimator.Play("AutoIcon");
        }
        else
        {
            autoAnimator.Play("Idle");
        }
    }

    public void processEnd()
    {
        FindObjectOfType<ScoreManager>().playerScore = score;
        SceneManager.LoadScene("EndScreen");
    }
}
