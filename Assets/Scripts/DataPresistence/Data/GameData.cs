using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class GameData
{
    public List<KeyValuePair<string, int>> scoreBoard;
    public GameData()
    {
        this.scoreBoard = new List<KeyValuePair<string, int>>();
    }
}
