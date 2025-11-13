using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    private Board board;
    public float hintDelay;
    private float hintDelaySeconds;
    public GameObject hintParticle;
    public GameObject currentHint;

    void Start()
    {
        board = FindObjectOfType<Board>();
        hintDelaySeconds = hintDelay;
    }

    // Update is called once per frame
    void Update()
    {
        hintDelaySeconds -= Time.deltaTime;
        if(hintDelaySeconds <= 0 && currentHint is null)
        {
            MarkHint();
            hintDelaySeconds = hintDelay;
        }
    }

    List<Dot> FindAllMatches()
    {
        var possibleMoves = new List<Dot>();
        for (int column = 0; column < board.width; column++)
        {
            for (int row = 0; row < board.height; row++)
            {
                if(board.AllDots[column, row])
                {
                    if(column < board.width - 1)
                    {
                        if(board.SwitchAndCheck(column, row, Vector2.right))
                        {
                            possibleMoves.Add(board.AllDots[column, row]);
                        }
                    }
                    if(row < board.height - 1)
                    {
                        if(board.SwitchAndCheck(column, row, Vector2.up))
                        {
                            possibleMoves.Add(board.AllDots[column, row]);
                        }
                    }
                }
            }
        }
        return possibleMoves;
    }

    Dot PickOneRandomly()
    {
        var possibleMoves = FindAllMatches();
        if(possibleMoves.Count > 0)
        {
            int pieceToUse = Random.Range(0, possibleMoves.Count);
            return possibleMoves[pieceToUse];
        }
        return null;
    }

    private void MarkHint()
    {
        Dot move = PickOneRandomly();
        if(move != null)
        {
            currentHint = Instantiate(hintParticle, move.transform.position, Quaternion.identity);
        }
    }

    public void DestroyHint()
    {
        if(currentHint != null)
        {
            Destroy(currentHint);
            currentHint = null;
            hintDelaySeconds = hintDelay;
        }
    }
}
