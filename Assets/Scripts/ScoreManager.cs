using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public Dictionary<string, int> scoreBoard = new Dictionary<string, int>();
    public string playerName;
    public int playerScore;
    private void Awake() {
        if (instance == null)
        {
            DontDestroyOnLoad(transform.gameObject);
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    // public void LoadData(GameData data)
    // {
    //     this.scoreBoard = data.scoreBoard.ToDictionary(x => x.Key, x => x.Value);
    // }
    // public void SaveData(ref GameData data)
    // {
    //     data.scoreBoard = this.scoreBoard.ToList();
    // }
    public List<(string, int)> getSortedPlayers()
    {
        List<(string,int)> result = new List<(string, int)>();

        var temp = scoreBoard.ToList();
        temp.Sort((x,y) => y.Value);

        foreach(var i in temp)
        {
            result.Add((i.Key, i.Value));
        }

        return result;
    }
}
