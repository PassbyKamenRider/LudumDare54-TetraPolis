using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeHover : MonoBehaviour
{
    private TetrisBlock tetrisBlock;

    private void Start() {
        tetrisBlock = GetComponent<TetrisBlock>();
    }
    private void OnMouseEnter() {
        if (UIManager.onDestroyMode)
        {
            //Debug.Log(233333);
        }
    }

    private void OnMouseDown() {
        if (UIManager.onDestroyMode)
        {
            foreach (Transform child in transform)
            {
                int childx = Mathf.RoundToInt(child.position.x);
                int childy = Mathf.RoundToInt(child.position.y);
                // update resources in the block
                FindObjectOfType<ResourceManager>().removeBlock(SpawnPoint.grids[childx, childy].GetComponent<WorkingBlock>());
                // destroy the block
                Destroy(SpawnPoint.grids[childx, childy].gameObject);
                SpawnPoint.grids[childx, childy] = null;
                // move down
            }
            Destroy(this.gameObject);
            UIManager uIManager = FindObjectOfType<UIManager>();
            Destroy(uIManager.spawnedHammer);
            uIManager.switchDestroyMode();
        }
    }
}
