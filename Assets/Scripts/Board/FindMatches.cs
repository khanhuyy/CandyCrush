using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void IsAdjacentBomb(Dot dot1, Dot dot2, Dot dot3) {
        if(dot1.isAdjacentBomb) {
            MatchAdjacent(dot1.column, dot1.row);
        }
        if(dot2.isAdjacentBomb) {
            MatchAdjacent(dot2.column, dot2.row);
        }
        if(dot3.isAdjacentBomb) {
            MatchAdjacent(dot3.column, dot3.row);
        }
    }

    private void IsRowBomb(Dot dot1, Dot dot2, Dot dot3) {
        if(dot1.isRowBomb && !dot1.isSolving)
        {
            dot1.isSolving = true;
            MatchRow(dot1.row);
            board.BombRow(dot1.row);
        }
        if(dot2.isRowBomb && !dot2.isSolving)
        {
            dot2.isSolving = true;
            MatchRow(dot2.row);
            board.BombRow(dot2.row);
        }
        if(dot3.isRowBomb && !dot3.isSolving)
        {
            dot3.isSolving = true;
            MatchRow(dot3.row);
            board.BombRow(dot3.row);
        }
    }

    private void IsColumnBomb(Dot dot1, Dot dot2, Dot dot3) {
        if(dot1.isColumnBomb && !dot1.isSolving)
        {
            dot1.isSolving = true;
            MatchColumn(dot1.column);
            board.BombColumn(dot1.column);
        }
        if(dot2.isColumnBomb && !dot2.isSolving) 
        {
            dot2.isSolving = true;
            MatchColumn(dot2.column);
            board.BombColumn(dot2.column);
        }
        if(dot3.isColumnBomb && !dot3.isSolving) 
        {
            dot3.isSolving = true;
            MatchColumn(dot3.column);
            board.BombColumn(dot3.column);
        }
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
        for (int column = 0; column < board.width; column++) {
            for (int row = 0; row < board.height; row++) {
                if(board.AllDots[column, row]) {
                    if (board.AllDots[column, row].TryGetComponent(out Dot currentDot))
                    {
                        if (column > 0 && column < board.width - 1) {
                            if (board.AllDots[column - 1, row] && board.AllDots[column + 1, row])
                            {
                                if (board.AllDots[column - 1, row].TryGetComponent(out Dot leftDot) &&
                                    board.AllDots[column + 1, row].TryGetComponent(out Dot rightDot))
                                {
                                    if (leftDot.color == currentDot.color && rightDot.color == currentDot.color)
                                    {
                                        IsRowBomb(leftDot, currentDot, rightDot);
                                        IsColumnBomb(leftDot, currentDot, rightDot);
                                        IsAdjacentBomb(leftDot, currentDot, rightDot);
                                        MatchNearbyPieces(leftDot.gameObject, currentDot.gameObject,
                                            rightDot.gameObject);
                                    }
                                }
                            }
                        }
                        if(row > 0 && row < board.height - 1) {
                            if (board.AllDots[column, row - 1] && board.AllDots[column, row + 1])
                            {
                                if (board.AllDots[column, row - 1].TryGetComponent(out Dot downDot) &&
                                    board.AllDots[column, row + 1].TryGetComponent(out Dot upDot))
                                {
                                    if (upDot.color == currentDot.color && downDot.color == currentDot.color)
                                    {
                                        IsRowBomb(downDot, currentDot, upDot);
                                        IsColumnBomb(downDot, currentDot, upDot);
                                        IsAdjacentBomb(downDot, currentDot, upDot);
                                        MatchNearbyPieces(downDot.gameObject, currentDot.gameObject, upDot.gameObject);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    
    public void FindAllMatches2() {
        for (int column = 0; column < board.width; column++) {
            for (int row = 0; row < board.height; row++) {
                if(board.AllDots[column, row]) {
                    if (board.AllDots[column, row].TryGetComponent(out Dot currentDot))
                    {
                        if (column > 0 && column < board.width - 1) {
                            if (board.AllDots[column - 1, row] && board.AllDots[column + 1, row])
                            {
                                if (board.AllDots[column - 1, row].TryGetComponent(out Dot leftDot) &&
                                    board.AllDots[column + 1, row].TryGetComponent(out Dot rightDot))
                                {
                                    if (leftDot.color == currentDot.color && rightDot.color == currentDot.color)
                                    {
                                        IsRowBomb(leftDot, currentDot, rightDot);
                                        IsColumnBomb(leftDot, currentDot, rightDot);
                                        IsAdjacentBomb(leftDot, currentDot, rightDot);
                                        MatchNearbyPieces(leftDot.gameObject, currentDot.gameObject,
                                            rightDot.gameObject);
                                    }
                                }
                            }
                        }
                        if(row > 0 && row < board.height - 1) {
                            if (board.AllDots[column, row - 1] && board.AllDots[column, row + 1])
                            {
                                if (board.AllDots[column, row - 1].TryGetComponent(out Dot downDot) &&
                                    board.AllDots[column, row + 1].TryGetComponent(out Dot upDot))
                                {
                                    if (upDot.color == currentDot.color && downDot.color == currentDot.color)
                                    {
                                        IsRowBomb(downDot, currentDot, upDot);
                                        IsColumnBomb(downDot, currentDot, upDot);
                                        IsAdjacentBomb(downDot, currentDot, upDot);
                                        MatchNearbyPieces(downDot.gameObject, currentDot.gameObject, upDot.gameObject);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void MatchAdjacent(int column, int row) {
        for(int i = column - 1; i <= column + 1; i++) {
            for(int j = row - 1; j <= row + 1; j++) {
                if(i >= 0 && i <board.width && j >= 0 && j < board.height) {
                    if(board.AllDots[i, j])
                    {
                        if (board.AllDots[i, j].TryGetComponent(out Dot dot))
                        {
                            dot.isMatched = true;
                        }
                        currentMatches.Add(board.AllDots[i, j]);
                    }
                }
            }
        }
    }
    
    private void MatchColumn(int column) {
        for (int row = 0; row < board.height; row++) {
            if(board.AllDots[column, row]) {
                if (board.AllDots[column, row].TryGetComponent(out Dot dot) && dot.column == column)
                {
                    if(dot.isRowBomb)
                    {
                        MatchRow(row);
                    }
                    dot.isMatched = true;
                    currentMatches.Add(board.AllDots[column, row]);
                }
            }
        }
    }
    
    private void MatchRow(int row) {
        for (int column = 0; column < board.width; column++) {
            if(board.AllDots[column, row]) {
                if (board.AllDots[column, row].TryGetComponent(out Dot dot) && dot.row == row)
                {
                    if(dot.isColumnBomb)
                    {
                        MatchColumn(column);
                    }
                    dot.isMatched = true;
                    currentMatches.Add(board.AllDots[column, row]);
                }
                
            }
        }
    }
    
    public void MatchDotsSameColor(PieceColor color) {
        for(int column = 0; column < board.width; column++) {
            for (int row = 0; row< board.height; row++) {
                // check if that dot exists
                if(board.AllDots[column, row])
                {
                    if (board.AllDots[column, row].TryGetComponent(out Dot otherDot) 
                        && otherDot.color == color && otherDot.color != PieceColor.None)
                    {
                        otherDot.isMatched = true;
                    }
                }
            }
        }
    }
    
    public void MakeAdjacentSameColor(PieceColor color) {
        for(int column = 0; column < board.width; column++) {
            for (int row = 0; row< board.height; row++) {
                if(board.AllDots[column, row]) {
                    if (board.AllDots[column, row].TryGetComponent(out Dot otherDot) && otherDot.color == color)
                    {
                        otherDot.isMatched = true;
                        otherDot.MakeAdjacentBomb();
                    }
                }
            }
        }
    }
    
    public void MakeDirectionBombSameColor(PieceColor color) {
        for(int column = 0; column < board.width; column++) {
            for (int row = 0; row< board.height; row++) {
                if(board.AllDots[column, row]) {
                    if (board.AllDots[column, row].TryGetComponent(out Dot otherDot) && otherDot.color == color)
                    {
                        var columnOrRow = Random.Range(0, 2);
                        otherDot.isMatched = true;
                        if (columnOrRow == 0)
                        {
                            otherDot.MakeColumnBomb();
                        }
                        else
                        {
                            otherDot.MakeRowBomb();
                        }
                        currentMatches.Add(board.AllDots[column, row]);
                    }
                }
            }
        }
    }
    
    public void MatchAllBoard() {
        for(int column = 0; column < board.width; column++) {
            for (int row = 0; row< board.height; row++) {
                if(board.AllDots[column, row]) {
                    if (board.AllDots[column, row].TryGetComponent(out Dot dot))
                    {
                        dot.isMatched = true;
                    }
                }
            }
        }
    }

    // gen row or column bomb
    public void CheckDirectionBombs(MatchType matchType) {
        if(!board.currentDot) {
            if (board.currentDot.isMatched && board.currentDot.color == matchType.color)
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
            else if(board.currentDot.otherDotObject) {
                if (board.currentDot.otherDotObject.TryGetComponent(out Dot otherDot))
                {
                    if(otherDot.isMatched && otherDot.color == matchType.color) {
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

    public void MatchDoubleAdjacent(Dot firstDot, Dot secondDot)
    {
        var minCalculateColumn = Mathf.Min(firstDot.column, secondDot.column);
        var minCalculateRow = Mathf.Min(firstDot.row, secondDot.row);
        var maxCalculateColumn = Mathf.Max(firstDot.column, secondDot.column);
        var maxCalculateRow = Mathf.Max(firstDot.row, secondDot.row);
        for (int column = minCalculateColumn - 2; column <= maxCalculateColumn + 2; column++)
        {
            for (int row = minCalculateRow - 2; row <= maxCalculateRow + 2; row++)
            {
                if (column >= 0 && column < board.width && row >= 0 && row < board.height)
                {
                    if (board.AllDots[column, row] && board.AllDots[column, row].TryGetComponent(out Dot dotComponent))
                    {
                        dotComponent.isMatched = true;
                    }
                }
            }
        }
    }

    public void MatchAdjacentColumn(Dot secondDot)
    {
        for (int column = secondDot.column - 1; column < secondDot.column + 1; column++)
        {
            for (int row = 0; row < board.width; row++)
            {
                if (column > 0 && column < board.width)
                {
                    if (board.AllDots[column, row] && board.AllDots[column, row].TryGetComponent(out Dot dot))
                    {
                        if(dot.IsBomb())
                        {
                            MatchBombProperly(dot);
                        }
                        if (dot.row == secondDot.row)
                        {
                            dot.MakeColumnBomb();
                        }
                        dot.isMatched = true;
                        currentMatches.Add(board.AllDots[column, row]);
                    }
                }
            }
        }
    }
    
    public void MatchAdjacentRow(Dot secondDot)
    {
        for (int row = secondDot.row - 1; row < secondDot.row + 1; row++)
        {
            for (int column = 0; column < board.width; column++)
            {
                if (row > 0 && row < board.height)
                {
                    if (board.AllDots[column, row] && board.AllDots[column, row].TryGetComponent(out Dot dot))
                    {
                        if(dot.IsBomb())
                        {
                            MatchBombProperly(dot);
                        }
                        if (dot.column == secondDot.column)
                        {
                            dot.MakeRowBomb();
                        }
                        dot.isMatched = true;
                        currentMatches.Add(board.AllDots[column, row]);
                    }
                }
            }
        }
    }

    public void MatchBombProperly(Dot bombDot)
    {
        if (bombDot.isAdjacentBomb)
        {
            MatchAdjacent(bombDot.column, bombDot.row);
        }
        else if (bombDot.IsDirectionBomb())
        {
            MatchDirection(bombDot);
            bombDot.isMatched = true; // todo refactor
        }
    }
    
    public void MatchDirection(Dot directionDot)
    {
        if (directionDot.IsDirectionBomb())
        {
            if (directionDot.isColumnBomb)
            {
                MatchColumn(directionDot.column);
            }
            if (directionDot.isRowBomb)
            {
                MatchRow(directionDot.row);
            }
        }
    }
    
    public void MatchCross(Dot dot)
    {
        for (int column = 0; column < board.width; column++)
        {
            if (board.AllDots[column, dot.row].TryGetComponent(out Dot sameRowDot))
            {
                if(dot.isRowBomb)
                {
                    MatchColumn(column);
                }
                sameRowDot.isMatched = true;
                currentMatches.Add(board.AllDots[column, dot.row]);
            }
        }

        for (int row = 0; row < board.height; row++)
        {
            if (board.AllDots[dot.column, row].TryGetComponent(out Dot sameColumnDot))
            {
                if(dot.isRowBomb)
                {
                    MatchRow(row);
                }
                sameColumnDot.isMatched = true;
                currentMatches.Add(board.AllDots[dot.column, row]);
            }
        }
    }
}
