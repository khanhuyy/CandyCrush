using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {
    wait,
    move,
    win,
    lose,
    pause
}

public enum TileKind {
    Breakable,
    Blank,
    Normal
}

[System.Serializable]
public class TileType {
    public int x;
    public int y;
    public TileKind tileKind;
}

public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move;
    public int width, height;
    public int offSet;
    public GameObject tilePrefab;
    public GameObject breakableTilePrefab;
    public GameObject [] dots;
    public GameObject destroyEffect;
    public TileType[] boardLayout;
    public GameObject [,] allDots;
    public Dot currentDot;

    private bool[,] blankSpaces;
    private BackgroundTile[,] breakableTiles;
    private FindMatches findMatches;
    public int basePieceValue = 20;
    private int streakValue = 1;
    private ScoreManager scoreManager;
    private SoundManager soundManager;
    private GoalManager goalManager;
    public float refillDelay = 0.5f;
    public int[] scoreGoals;

    void Start()
    {
        goalManager = FindObjectOfType<GoalManager>();
        soundManager = FindObjectOfType<SoundManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        breakableTiles = new BackgroundTile[width, height];
        findMatches = FindObjectOfType<FindMatches>();
        blankSpaces = new bool[width, height];  
        allDots = new GameObject[width, height]; 
        Setup(); 
        currentState = GameState.pause;
    }

    public void GenerateBlankSpace() {
        for (int i = 0; i < boardLayout.Length; i++) {
            if(boardLayout[i].tileKind == TileKind.Blank) {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }

    public void GenerateBreakableTiles() {
        // look at all the tiles in the layout
        for (int i = 0; i < boardLayout.Length; i++) {
            // if a tile is a "jelly" tile
            if(boardLayout[i].tileKind == TileKind.Breakable) {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(breakableTilePrefab, tempPosition, Quaternion.identity);
                breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    private void Setup() {
        GenerateBlankSpace();
        GenerateBreakableTiles();
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if(!blankSpaces[i, j]) {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    Vector2 tilePosition = new Vector2(i, j);
                    // GameObject backgroundTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity, this.transform) as GameObject;
                    // backgroundTile.transform.parent = this.transform;
                    // backgroundTile.name = "( " + i + ", " + j + " )";
                    int dotToUse = Random.Range(0, dots.Length);
                    int maxIterations = 0;
                    while(MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100) {
                        dotToUse = Random.Range(0, dots.Length);
                        maxIterations++;
                    }
                    maxIterations = 0; // todo consider
                    GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity, this.transform);
                    dot.GetComponent<Dot>().column = i;
                    dot.GetComponent<Dot>().row = j;
                    dot.name = "( " + i + ", " + j + " )";
                    allDots[i, j] = dot;
                }
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject piece) {
        if(column > 1 && row > 1) {
            if(allDots[column - 1, row] != null && allDots[column - 2, row] != null) {
                if(allDots[column - 1, row].tag == piece.tag || allDots[column - 2, row].tag == piece.tag) {
                    return true;
                }
            }
            if(allDots[column, row - 1] != null && allDots[column, row - 2] != null) {
                if(allDots[column, row - 1].tag == piece.tag || allDots[column, row - 2].tag == piece.tag) {
                    return true;
                }
            }
        } else if (column <= 1 || row <= 1) {
            if(column > 1) {
                if(allDots[column - 1, row] != null && allDots[column - 2, row] != null) {
                    if(allDots[column - 1, row].tag == piece.tag || allDots[column - 2, row].tag == piece.tag) {
                        return true;
                    }
                }
            }
            if(row > 1) {
                if(allDots[column, row - 1] != null && allDots[column, row - 2] != null) {
                    if(allDots[column, row - 1].tag == piece.tag || allDots[column, row - 2].tag == piece.tag) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool ColumnOrRow() {
        int numberHorizontal = 0;
        int numberVertical = 0;
        Dot firstPiece = findMatches.currentMatches[0].GetComponent<Dot>();
        if (firstPiece != null) {
            foreach(GameObject currentPiece in findMatches.currentMatches) {
                Dot dot = currentPiece.GetComponent<Dot>();
                if(dot.row == firstPiece.row) {
                    numberHorizontal++;
                }
                if(dot.column == firstPiece.column) {
                    numberVertical ++;
                }
            }
        }
        return (numberVertical == 5 || numberHorizontal == 5);
    }

    private void CheckToMakeBomb() {
        if(findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7) {
            findMatches.CheckBombs();
        }
        if(findMatches.currentMatches.Count == 5 || findMatches.currentMatches.Count == 8) {
            if(ColumnOrRow()) {
                if(currentDot != null) {
                    if(currentDot.isMatched) {
                        if(currentDot.isColorBomb) {
                            currentDot.isMatched = false;
                            currentDot.MakeColorBomb();
                        }
                    } else {
                        if(currentDot.otherDot != null) {
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if(otherDot.isMatched) {
                                if(!otherDot.isColorBomb) {
                                    otherDot.isMatched = false;
                                    otherDot.MakeColorBomb();
                                }
                            }
                        }
                    }
                }
            } else {
                if(currentDot != null) {
                    if(currentDot.isMatched) {
                        if(currentDot.isAdjacentBomb) {
                            currentDot.isMatched = false;
                            currentDot.MakeAdjacentBomb();
                        }
                    } else {
                        if(currentDot.otherDot != null) {
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if(otherDot.isMatched) {
                                if(!otherDot.isAdjacentBomb) {
                                    otherDot.isMatched = false;
                                    otherDot.MakeAdjacentBomb();
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void DestroyMatchesAt(int column, int row) {
        if(allDots[column, row].GetComponent<Dot>().isMatched) {
            // define how many pieces are destroyed
            if(findMatches.currentMatches.Count >= 4) {
                CheckToMakeBomb();
            }

            // check if tile is bounded jelly
            if(breakableTiles[column, row] != null)
            {
                breakableTiles[column, row].TakeDamage(1);
                if(breakableTiles[column, row].hitPoints <= 0)
                {
                    breakableTiles[column, row] = null;
                }
            }
            if(goalManager != null)
            {
                goalManager.CompareGoal(allDots[column, row].tag.ToString());
                goalManager.UpdateGoals();
            }
            if(soundManager != null)
            {
                soundManager.PlayRandomDestroyNoise();
            }
            GameObject particle = Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, 0.5f);
            Destroy(allDots[column, row]);
            scoreManager.IncreaseScore(basePieceValue * streakValue);
            allDots[column, row] = null;
        }
    }

    public void DestroyMatches() {
        for ( int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (allDots[i, j] != null) {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        findMatches.currentMatches.Clear(); // todo consider ???

        StartCoroutine(DecreaseRowCo2());
    }

    // todo check again
    private IEnumerator DecreaseRowCo2() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                // if the current spot isn't black and is empty. . .
                if(!blankSpaces[i, j] && allDots[i, j] == null) {
                    for(int k = j + 1; k < height; k++) {
                        // if a dot is found
                        if (allDots[i, k] != null) {
                            // move this dot to this empty space
                            allDots[i, k].GetComponent<Dot>().row = j;
                            // set that spot to be null
                            allDots[i, k] = null;
                            // break out of the loop;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(refillDelay * 0.5f);
        StartCoroutine(FillBoardCo());
    }

    private IEnumerator DecreaseRowCo() {
        int nullCount = 0;
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if(allDots[i, j] == null) {
                    nullCount++;
                } else if(nullCount > 0) {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard() {
        for (int i = 0; i < width; i ++) {
            for (int j = 0; j < height; j++) {
                if(allDots[i, j] == null && !blankSpaces[i, j]) {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int dotToUse = Random.Range(0, dots.Length);
                    int maxIterations = 0;
                    while(MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                    {
                        maxIterations++;
                        dotToUse = Random.Range(0, dots.Length);
                    }
                    maxIterations = 0;
                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity, this.transform);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().column = i;
                    piece.GetComponent<Dot>().row = j;
                }
            }   
        }
    }

    private bool MatchedOnBoard() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if(allDots[i, j] != null) {
                    if(allDots[i, j].GetComponent<Dot>().isMatched) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo() {
        RefillBoard();
        yield return new WaitForSeconds(refillDelay);
        while(MatchedOnBoard()) {
            streakValue += 1;
            DestroyMatches();
            yield return new WaitForSeconds(2 * refillDelay);
        }
        findMatches.currentMatches.Clear();
        currentDot = null;
        yield return new WaitForSeconds(refillDelay);
        if(IsDeadlocked()) {
            ShuffleBoard();
            Debug.Log("Deadlocked");
        }
        currentState = GameState.move;
        streakValue = 1;
    }

    private void SwitchPieces(int column, int row, Vector2 direction) 
    {
        // take the second piece and save it in a holder
        GameObject holder = allDots[column + (int)direction.x, row + (int)direction.y] as GameObject;
        // switch the first dot to be
        allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row];
        allDots[column, row] = holder;
    }

    private bool CheckForMatches()
    {
        for(int i = 0; i <width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allDots[i, j] != null)
                {
                    // make sure that ond and two to the right are in the board
                    if(i < width - 2)
                    {
                        // check if the dots to the right and two to the right
                        if(allDots[i + 1, j] != null && allDots[i + 2, j] != null)
                        {
                            if(allDots[i + 1, j].tag == allDots[i, j].tag
                            && allDots[i + 2, j].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                    if(j < height - 2)
                    {
                        // check if the dot above exist
                        if(allDots[i, j + 1] != null && allDots[i, j + 2] != null)
                        {
                            if(allDots[i, j + 1].tag == allDots[i, j].tag
                            && allDots[i, j + 2].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                    
                }
            }
        }
        return false;
    }

    public bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchPieces(column, row, direction);
        if(CheckForMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }
        SwitchPieces(column, row, direction);
        return false;
    }

    private bool IsDeadlocked()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allDots[i, j] != null)
                {
                    if(i < width - 1)
                    {
                        if(SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }
                    if(j < height - 1)
                    {
                        if(SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    private void ShuffleBoard()
    {
        List<GameObject> newBoard = new List<GameObject>();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allDots[i, j] != null)
                {
                    newBoard.Add(allDots[i, j]);
                }
            }
        }
        // yield return new WaitForSeconds(0.5f); // ???
        // for every spot on the board
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                // if this spot shouldn't be blank
                if(!blankSpaces[i, j])
                {
                    // pick a random number
                    int pieceToUse = Random.Range(0, newBoard.Count);
                    // make a container for the piece
                    Dot piece = newBoard[pieceToUse].GetComponent<Dot>();
                    int maxIterations = 0;
                    while(MatchesAt(i, j, newBoard[pieceToUse]) && maxIterations < 100) {
                        pieceToUse = Random.Range(0, newBoard.Count);
                        maxIterations++;
                    }
                    maxIterations = 0; // todo consider
                    piece.column = i;
                    piece.row = j;
                    // fill in the dots array with this new piece
                    allDots[i, j] = newBoard[pieceToUse];
                    newBoard.Remove(newBoard[pieceToUse]);
                }
            }
        }
        // check if it's still deadlock
        if(IsDeadlocked())
        {
            ShuffleBoard();
        }
    }
}
