using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches= new List<GameObject>();

    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches() {
        StartCoroutine(FindAllMatchesCo());
    }

    private List<GameObject> IsAdjacentBomb(Dot dot1, Dot dot2, Dot dot3) {
        List<GameObject> currentDots = new List<GameObject>();
        if(dot1.isAdjacentBomb) {
            currentMatches.Union(GetAdjacentPieces(dot1.column, dot1.row));
        }
        if(dot2.isAdjacentBomb) {
            currentMatches.Union(GetAdjacentPieces(dot2.column, dot2.row));
        }
        if(dot3.isAdjacentBomb) {
            currentMatches.Union(GetAdjacentPieces(dot3.column, dot3.row));
        }
        return currentDots;
    }

    private List<GameObject> IsRowBomb(Dot dot1, Dot dot2, Dot dot3) {
        List<GameObject> currentDots = new List<GameObject>();
        if(dot1.isRowBomb) {
            currentMatches.Union(GetRowDots(dot1.row));
            board.BombRow(dot1.row);
        }
        if(dot2.isRowBomb) {
            currentMatches.Union(GetRowDots(dot2.row));
            board.BombRow(dot2.row);
        }
        if(dot3.isRowBomb) {
            currentMatches.Union(GetRowDots(dot3.row));
            board.BombRow(dot3.row);
        }
        return currentDots;
    }

    private List<GameObject> IsColumnBomb(Dot dot1, Dot dot2, Dot dot3) {
        List<GameObject> currentDots = new List<GameObject>();
        if(dot1.isColumnBomb) {
            Debug.Log("Dot 1");
            currentMatches.Union(GetColumnDots(dot1.column));
            board.BombColumn(dot1.column);
        }
        if(dot2.isColumnBomb) {
            Debug.Log("Dot 2");
            currentMatches.Union(GetColumnDots(dot2.column));
            board.BombColumn(dot2.column);
        }
        if(dot3.isColumnBomb) {
            Debug.Log("Dot 3");
            currentMatches.Union(GetColumnDots(dot3.column));
            board.BombColumn(dot3.column);
        }
        return currentDots;
    }

    private void AddToListAndMatch(GameObject piece) {
        if(!currentMatches.Contains(piece)) {
            currentMatches.Add(piece);
        }
        if (piece.TryGetComponent(out Dot dot))
        {
            dot.isMatched = true;
        }
    }

    private void MatchNearbyPieces(GameObject dot1, GameObject dot2, GameObject dot3) {
        AddToListAndMatch(dot1);
        AddToListAndMatch(dot2);
        AddToListAndMatch(dot3);
    }

    private IEnumerator FindAllMatchesCo() {
        yield return null;
        // todo more test
        Debug.Log("Runing......");
        for (int column = 0; column < board.width; column++) {
            for (int row = 0; row < board.height; row++) {
                GameObject currentDotObject = board.allDots[column, row];
                if(currentDotObject != null) {
                    Dot currentDot = currentDotObject.GetComponent<Dot>();
                    if(column > 0 && column < board.width - 1) {
                        GameObject leftDotObject = board.allDots[column - 1, row];
                        GameObject rightDotObject = board.allDots[column + 1, row];
                        if(leftDotObject != null && rightDotObject != null) {
                            Dot leftDot = leftDotObject.GetComponent<Dot>();
                            Dot rightDot = rightDotObject.GetComponent<Dot>();
                            if(leftDotObject.CompareTag(currentDotObject.tag) && rightDotObject.CompareTag(currentDotObject.tag))
                            {
                                currentMatches.Union(IsRowBomb(leftDot, currentDot, rightDot));
                                Debug.Log("From Row Check");
                                currentMatches.Union(IsColumnBomb(leftDot, currentDot, rightDot));
                                currentMatches.Union(IsAdjacentBomb(leftDot, currentDot, rightDot));
                                MatchNearbyPieces(leftDotObject, currentDotObject, rightDotObject);
                            }
                        }
                    }

                    if(row > 0 && row < board.height - 1) {
                        GameObject upDotGo = board.allDots[column, row + 1];
                        GameObject downDotGo = board.allDots[column, row - 1];
                        if(upDotGo != null && downDotGo != null) {
                            Dot upDot = upDotGo.GetComponent<Dot>();
                            Dot downDot = downDotGo.GetComponent<Dot>();
                            if(upDotGo.CompareTag(currentDotObject.tag) && downDotGo.CompareTag(currentDotObject.tag))
                            {
                                currentMatches.Union(IsRowBomb(downDot, currentDot, upDot));
                                Debug.Log("From Column Check");
                                currentMatches.Union(IsColumnBomb(downDot, currentDot, upDot));
                                currentMatches.Union(IsAdjacentBomb(downDot, currentDot, upDot));
                                MatchNearbyPieces(downDotGo, currentDotObject, upDotGo);
                            }
                        }
                    }
                }
            }
        }
    }

    List<GameObject> GetAdjacentPieces(int column, int row) {
        List<GameObject> dots = new List<GameObject>();
        for(int i = column - 1; i <= column + 1; i++) {
            for(int j = row - 1; j <= row + 1; j++) {
                if(i >= 0 && i <board.width && j >= 0 && j < board.height) {
                    if(board.allDots[i, j] != null)
                    {
                        dots.Add(board.allDots[i, j]);
                        if (board.allDots[i, j].TryGetComponent(out Dot dot))
                        {
                            dot.isMatched = true;
                        }
                    }
                }
            }
        }
        return dots;
    }

    // all column dots
    List<GameObject> GetColumnDots(int column) {
        List<GameObject> dots = new List<GameObject>();
        for (int row = 0; row < board.height; row++) {
            if(board.allDots[column, row] != null) {
                if (board.allDots[column, row].TryGetComponent(out Dot dot))
                {
                    if(dot.isRowBomb)
                    {
                        dots.Union(GetRowDots(row)).ToList();
                    }
                    dots.Add(board.allDots[column, row]);
                    dot.isMatched = true;
                }
            }
        }
        return dots;
    }

    // match the same color dot
    // todo refactor name
    public void MatchDotsOfColor(string color) {
        for(int column = 0; column < board.width; column++) {
            for (int row = 0; row< board.height; row++) {
                // check if that dot exists
                if(board.allDots[column, row] != null) {
                    // check the tag on that piece
                    if(board.allDots[column, row].CompareTag(color)) {
                        // set that piece to be matched
                        if (board.allDots[column, row].TryGetComponent(out Dot otherDot))
                        {
                            otherDot.isMatched = true;
                        }
                    }
                }
            }
        }
    }

    List<GameObject> GetRowDots(int row) {
        List<GameObject> dots = new List<GameObject>();
        for (int column = 0; column < board.width; column++) {
            if(board.allDots[column, row] != null) {
                if (board.allDots[column, row].TryGetComponent(out Dot dot))
                {
                    if(dot.isColumnBomb)
                    {
                        dots.Union(GetColumnDots(column)).ToList();
                    }
                    dots.Add(board.allDots[column, row]);
                    dot.isMatched = true;
                }
                
            }
        }
        return dots;
    }

    // gen row or column bomb
    public void CheckDirectionBombs(MatchType matchType) {
        if(board.currentDot != null) {
            if (board.currentDot.isMatched && board.currentDot.CompareTag(matchType.color))
            {
                board.currentDot.isMatched = false;
                if (board.currentDot.swipeAngle is > -45 and <= 45 or < -135 or >= 135)
                {
                    board.currentDot.MakeRowBomb();
                }
                else
                {
                    board.currentDot.MakeColumnBomb();
                }
            }
            else if(board.currentDot.otherDotGo != null) {
                if (board.currentDot.otherDotGo.TryGetComponent(out Dot otherDot))
                {
                    if(otherDot.isMatched && otherDot.CompareTag(matchType.color)) {
                        otherDot.isMatched = false;
                    }
                    if (board.currentDot.swipeAngle is > -45 and <= 45 or < -135 or >= 135)
                    {
                        otherDot.MakeRowBomb();
                    }
                    else
                    {
                        otherDot.MakeColumnBomb();
                    }
                }
                
            }
        }
    }
}
