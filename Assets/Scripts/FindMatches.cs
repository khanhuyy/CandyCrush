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
        }
        if(dot2.isRowBomb) {
            currentMatches.Union(GetRowDots(dot2.row));
        }
        if(dot3.isRowBomb) {
            currentMatches.Union(GetRowDots(dot3.row));
        }
        return currentDots;
    }

    private List<GameObject> IsColumnBomb(Dot dot1, Dot dot2, Dot dot3) {
        List<GameObject> currentDots = new List<GameObject>();
        if(dot1.isColorBomb) {
            currentMatches.Union(GetColumnDots(dot1.column));
        }
        if(dot2.isColorBomb) {
            currentMatches.Union(GetColumnDots(dot2.column));
        }
        if(dot3.isColorBomb) {
            currentMatches.Union(GetColumnDots(dot3.column));
        }
        return currentDots;
    }

    private void AddToListAndMatch(GameObject piece) {
        if(!currentMatches.Contains(piece)) {
            currentMatches.Add(piece);
        }
        piece.GetComponent<Dot>().isMatched = true;
    }

    private void GetNearbyPieces(GameObject dot1, GameObject dot2, GameObject dot3) {
        AddToListAndMatch(dot1);
        AddToListAndMatch(dot2);
        AddToListAndMatch(dot3);
    }

    private IEnumerator FindAllMatchesCo() {
        yield return new WaitForSeconds(0.2f);
        for (int column = 0; column < board.width; column++) {
            for (int row = 0; row < board.height; row++) {
                GameObject currentDotGo = board.allDots[column, row];
                if(currentDotGo != null) {
                    Dot currentDot = currentDotGo.GetComponent<Dot>();
                    if(column > 0 && column < board.width - 1) {
                        GameObject leftDotGo = board.allDots[column - 1, row];
                        GameObject rightDotGo = board.allDots[column + 1, row];
                        if(leftDotGo != null && rightDotGo != null) {
                            Dot leftDot = leftDotGo.GetComponent<Dot>();
                            Dot rightDot = rightDotGo.GetComponent<Dot>();
                            if(leftDotGo.CompareTag(currentDotGo.tag) && rightDotGo.CompareTag(currentDotGo.tag))
                            {
                                if (currentDot.isRowBomb || leftDot.isRowBomb || rightDot.isRowBomb)
                                {
                                    currentMatches.Union(GetRowDots(row));
                                }
                                if (leftDot.isColumnBomb)
                                {
                                    currentMatches.Union((GetColumnDots(column - 1)));
                                }
                                if (currentDot.isColumnBomb) // redundant?
                                {
                                    currentMatches.Union((GetColumnDots(column)));
                                }
                                if (rightDot.isColumnBomb)
                                {
                                    currentMatches.Union((GetColumnDots(column + 1)));
                                }
                                // trace all dot in valid matches
                                if (!currentMatches.Contains(leftDotGo))
                                { 
                                    currentMatches.Add(leftDotGo);
                                }
                                leftDot.isMatched = true;
                                if (!currentMatches.Contains(rightDotGo))
                                {
                                    currentMatches.Add(rightDotGo);
                                }
                                rightDot.isMatched = true;
                                if (!currentMatches.Contains(currentDotGo))
                                {
                                    currentMatches.Add(currentDotGo);
                                }
                                currentDot.isMatched = true;
                                
                                // currentMatches.Union(IsRowBomb(leftDot, currentDotDot, rightDot));
                                // currentMatches.Union(IsColumnBomb(leftDot, currentDotDot, rightDot));
                                // currentMatches.Union(IsAdjacentBomb(leftDot, currentDotDot, rightDot));
                                // GetNearbyPieces(leftDotGo, currentDot, rightDotGo);
                            }
                        }
                    }

                    if(row > 0 && row < board.height - 1) {
                        GameObject upDotGo = board.allDots[column, row + 1];
                        GameObject downDotGo = board.allDots[column, row - 1];
                        if(upDotGo != null && downDotGo != null) {
                            Dot upDot = upDotGo.GetComponent<Dot>();
                            Dot downDot = downDotGo.GetComponent<Dot>();
                            if(upDotGo.CompareTag(currentDotGo.tag) && downDotGo.CompareTag(currentDotGo.tag))
                            {
                                if (currentDot.isColumnBomb || upDot.isColumnBomb || downDot.isColumnBomb)
                                {
                                    currentMatches.Union(GetColumnDots(column));
                                }
                                
                                if (downDot.isRowBomb)
                                {
                                    currentMatches.Union(GetRowDots(row - 1));
                                }
                                if (currentDot.isRowBomb)
                                {
                                    currentMatches.Union(GetRowDots(row));
                                }
                                if (upDot.isRowBomb)
                                {
                                    currentMatches.Union(GetRowDots(row + 1));
                                }
                                
                                if (!currentMatches.Contains(upDotGo))
                                {
                                    currentMatches.Add(upDotGo);
                                }
                                upDot.isMatched = true;
                                if (!currentMatches.Contains(downDotGo))
                                {
                                    currentMatches.Add(downDotGo);
                                }
                                downDot.isMatched = true;
                                if (!currentMatches.Contains(currentDotGo))
                                {
                                    currentMatches.Add(currentDotGo);
                                }
                                currentDot.isMatched = true;
                                // currentMatches.Union(IsColumnBomb(upDot, currentDot, downDot));
                                // currentMatches.Union(IsRowBomb(upDot, currentDot, downDot));
                                // currentMatches.Union(IsAdjacentBomb(upDot, currentDot, downDot));
                                // GetNearbyPieces(upDotGo, currentDotGo, downDotGo);
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
                if(i >= 0 && i <board.width && j >=0 && j < board.height) {
                    if(board.allDots[i, j] != null)
                    {
                        dots.Add(board.allDots[i, j]);
                        board.allDots[i, j].GetComponent<Dot>().isMatched = true;
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
                // Dot dot = board.allDots[column, row].GetComponent<Dot>();
                // if(dot.isRowBomb)
                // {
                //     dots.Union(GetRowPieces(row)).ToList();
                // }
                dots.Add(board.allDots[column, row]);
                board.allDots[column, row].GetComponent<Dot>().isMatched = true;
            }
        }
        return dots;
    }

    // todo refactor name
    public void MatchPIecesOfColor(string color) {
        for(int i = 0; i < board.width; i++) {
            for (int j = 0; j< board.height; j++) {
                // check if that piece exists
                if(board.allDots[i, j] != null) {
                    // check the tag on that piece
                    if(board.allDots[i, j].tag == color) {
                        // set that piece to be matched
                        board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                    }
                }
            }
        }
    }

    List<GameObject> GetRowDots(int row) {
        List<GameObject> dots = new List<GameObject>();
        for (int column = 0; column < board.width; column++) {
            if(board.allDots[column, row] != null) {
                // Dot dot = board.allDots[i, row].GetComponent<Dot>();
                // if(dot.isColumnBomb)
                // {
                //     dots.Union(GetColumnDots(i)).ToList();
                // }
                dots.Add(board.allDots[column, row]);
                board.allDots[column, row].GetComponent<Dot>().isMatched = true;
            }
        }
        return dots;
    }

    // public void CheckBombs() {
    //     if(board.currentDot != null) {
    //         Dot otherDot = board.currentDot.otherDot.GetComponent<Dot>();
    //         if(board.currentDot.isMatched) {
    //             board.currentDot.isMatched = false;
    //             int typeOfBombs = Random.Range(0, 100);
    //             if(typeOfBombs < 50) {
    //                 // make a row bomb
    //                 otherDot.MakeRowBomb();
    //             } else if(typeOfBombs >= 50) {
    //                 // column bomb
    //                 otherDot.MakeColumnBomb();
    //             }
    //         } else if(board.currentDot.otherDot != null) {
    //             if(otherDot.isMatched) {
    //                 otherDot.isMatched = false;
    //             }
    //         }
    //     }
    // }

    public void CheckBombs() {
        if(board.currentDot != null) {
            if(board.currentDot.isMatched) {
                board.currentDot.isMatched = false;
                if((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45)
                  ||(board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135)) {
                    board.currentDot.MakeRowBomb();
                }else {
                    board.currentDot.MakeColumnBomb();
                }
            } 
        } else if(board.currentDot.otherDot != null) { // todo fix bugs
            Dot otherDot = board.currentDot.otherDot.GetComponent<Dot>();
            if(otherDot.isMatched) {
                otherDot.isMatched = false;
            }
            if((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45)
                  ||(board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135)) {
                    otherDot.MakeRowBomb();
                }else {
                    otherDot.MakeColumnBomb();
                }
        }
    }

}
