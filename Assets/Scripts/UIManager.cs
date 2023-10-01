using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject followHammer;
    public GameObject spawnedHammer;
    public static bool onDestroyMode;
    public ResourceManager resourceManager;

    private void Start() {
        resourceManager = FindObjectOfType<ResourceManager>();
    }

    public void switchDestroyMode()
    {
        if (!onDestroyMode)
        {
            if (resourceManager.goldAmount >= 200)
            {
                spawnedHammer = Instantiate(followHammer, transform.position, Quaternion.identity);
                resourceManager.goldAmount -= 200;
                resourceManager.updateDisplay();
                onDestroyMode = !onDestroyMode;
            }
        }
        else
        {
            onDestroyMode = !onDestroyMode;
        }
        if (onDestroyMode)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}
