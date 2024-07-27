using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlankGoal
{
    public int numberNeeded;
    public int numberCollected;
    public Sprite goalSprite;
    public string matchValue;
}

public class GoalManager : MonoBehaviour
{
    public BlankGoal[] levelGoals;
    public List<GoalPanel> currentGoals = new List<GoalPanel>();
    public GameObject goalPrefab;
    public GameObject goalIntroParent;
    public GameObject goalGameParent;
    private Board board;
    private EndGameManager endGame;

    void Start()
    {
        board = FindObjectOfType<Board>();
        endGame = FindObjectOfType<EndGameManager>();
        GetGoals();
        SetupGoals();
    }

    void GetGoals()
    {
        if (board != null && board.world != null)
        {
            if (board.world.levels[board.level] != null)
            {
                levelGoals = board.world.levels[board.level].levelGoals;
                foreach (var goal in levelGoals)
                {
                    goal.numberCollected = 0;
                }
            }
        }
    }
    
    public void SetupGoals()
    {
        foreach (var levelGoal in levelGoals)
        {
            GameObject goal = Instantiate(goalPrefab, goalIntroParent.transform.position, Quaternion.identity, goalIntroParent.transform);
            GoalPanel panel = goal.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoal.goalSprite;
            panel.thisString = "0/" + levelGoal.numberNeeded;
            GameObject gameGoal = Instantiate(goalPrefab, goalGameParent.transform.position, Quaternion.identity, goalGameParent.transform);
            panel = gameGoal.GetComponent<GoalPanel>();
            currentGoals.Add(panel);
            panel.thisSprite = levelGoal.goalSprite;
            panel.thisString = "0/" + levelGoal.numberNeeded;
        }
    }


    // ReSharper disable Unity.PerformanceAnalysis
    public void UpdateGoals()
    {
        int goalsCompleted = 0;
        for (int i = 0; i < levelGoals.Length; i++)
        {
            currentGoals[i].thisText.SetText(levelGoals[i].numberCollected + "/" + levelGoals[i].numberNeeded);
            if (levelGoals[i].numberCollected >= levelGoals[i].numberNeeded)
            {
                goalsCompleted++;
                currentGoals[i].thisText.SetText(levelGoals[i].numberNeeded + "/" + levelGoals[i].numberNeeded);
            }
        }
        if(goalsCompleted >= levelGoals.Length)
        {
            if(endGame)
            {
                endGame.WinGame();
            }
        }
    }

    public void CompareGoal(string goalToCompare, bool isAreaBomb, bool isDirectBomb)
    {
        foreach (var levelGoal in levelGoals)
        {
            // todo refactor, this will check dot special bomb
            if ((levelGoal.matchValue == "Direct Bomb" && isDirectBomb) || (levelGoal.matchValue == "Area Bomb" && isAreaBomb))
            {
                levelGoal.numberCollected++;
            }
            if(goalToCompare == levelGoal.matchValue)
            {
                levelGoal.numberCollected++;
            }
        }
    }
}
