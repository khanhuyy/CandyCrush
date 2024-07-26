using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum GameState {
    Wait,
    Move,
    Win,
    Lose,
    Pause
}

public enum TileKind {
    Frosting,
    Breakable,
    Blank,
    Lock,
    Concrete,
    Slime,
    Normal
}

[Serializable]
public class MatchType
{
    public int type;
    public string color;
}

[Serializable]
public class TileType {
    public int x;
    public int y;
    public TileKind tileKind;
}

public class Board : MonoBehaviour
{
    [Header("Scriptable Object Stuff")]
    public World world;
    public int level;
    
    public GameState currentState = GameState.Move;
    
    [Header("Board Dimension")]
    public int width, height, offset;
    public int rowOffSet; // dot offset position vertical to fall down
    
    [Header("Prefabs")]
    [SerializeField] private GameObject backgroundTilePrefab;
    public GameObject frostingTilePrefab;
    public GameObject breakableTilePrefab;
    public GameObject lockTilePrefab;
    public GameObject concreteTilePrefab;
    public GameObject slimePrefab;
    public GameObject [] dots;
    public GameObject destroyEffect;
    
    [Header("Layout")]
    public TileType[] boardLayout;
    public GameObject [,] AllDots;
    private bool[,] blankSpaces;
    private BackgroundTile[,] breakableTiles; // ~ jelly in candy crush
    public BackgroundTile[,] LockTiles; // ~ locked tile in candy crush
    private BackgroundTile[,] concreteTiles; // ~ blocked tile in candy crush
    private BackgroundTile[,] slimeTiles; // ~ chocolate tile in candy crush
    private BackgroundTile[,] frostingTiles; // ~ frosting tile in candy crush
    
    [Header("Match Stuff")] 
    public MatchType matchType;
    public Dot currentDot; // calculate first selected dot
    private FindMatches findMatches;
    public int basePieceValue = 20;
    private int streakValue = 1;
    private ScoreManager scoreManager;
    private SoundManager soundManager;
    private GoalManager goalManager;
    public float refillDelay = 0.5f;
    public int[] scoreGoals;
    private bool makeSlime = true;
    
    private void Awake()
    {
        if (PlayerPrefs.HasKey("Current Level"))
        {
            level = PlayerPrefs.GetInt("Current Level");
        }
        if (world != null)
        {
            if (Mathf.Abs(level) < world.levels.Length && world.levels[level] != null)
            {
                width = world.levels[level].width;
                height = world.levels[level].height;
                dots = world.levels[level].dots;
                scoreGoals = world.levels[level].scoreGoals;
                boardLayout = world.levels[level].boardLayout;
            }
        }
    }
    
    void Start()
    {
        goalManager = FindObjectOfType<GoalManager>();
        soundManager = FindObjectOfType<SoundManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        frostingTiles = new BackgroundTile[width, height];
        breakableTiles = new BackgroundTile[width, height];
        LockTiles = new BackgroundTile[width, height];
        concreteTiles = new BackgroundTile[width, height];
        slimeTiles = new BackgroundTile[width, height];
        findMatches = FindObjectOfType<FindMatches>();
        blankSpaces = new bool[width, height];  
        AllDots = new GameObject[width, height]; 
        Setup(); 
        currentState = GameState.Pause;
    }

    #region "Helper"

    // check blocker tiles
    private bool IsMovable(int column, int row)
    {
        return !frostingTiles[column, row] && !blankSpaces[column, row] && !concreteTiles[column, row] &&
               !slimeTiles[column, row];
    }
    

    #endregion

    #region "Special tiles generator"
    public void GenerateBlankSpace() {
        for (int i = 0; i < boardLayout.Length; i++) {
            if(boardLayout[i].tileKind == TileKind.Blank) {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }

    private void GenerateFrostingTiles()
    {
        // look at all the tiles in the layout
        foreach (var frostingTile in boardLayout)
        {
            // if a tile is a "frosting" tile
            if(frostingTile.tileKind == TileKind.Frosting) {
                Vector2 tempPosition = new Vector2(frostingTile.x, frostingTile.y);
                GameObject tile = Instantiate(frostingTilePrefab, tempPosition, Quaternion.identity, transform);
                tile.name = "FrostingTile(" + frostingTile.x + ", " + frostingTile.y + ")";
                frostingTiles[frostingTile.x, frostingTile.y] = tile.GetComponent<BackgroundTile>();
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
    
    private void GenerateLockTiles() {
        // look at all the tiles in the layout
        for (int i = 0; i < boardLayout.Length; i++) {
            // if a tile is a "locked" tile
            if(boardLayout[i].tileKind == TileKind.Lock) {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(lockTilePrefab, tempPosition, Quaternion.identity);
                LockTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }
    
    private void GenerateConcreteTiles() {
        // look at all the tiles in the layout
        for (int i = 0; i < boardLayout.Length; i++) {
            // if a tile is a "concrete" tile
            if(boardLayout[i].tileKind == TileKind.Concrete) {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(concreteTilePrefab, tempPosition, Quaternion.identity);
                concreteTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }
    
    private void GenerateSlimeTiles() {
        // look at all the tiles in the layout
        for (int i = 0; i < boardLayout.Length; i++) {
            // if a tile is a "slime" tile
            if(boardLayout[i].tileKind == TileKind.Slime) {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(slimePrefab, tempPosition, Quaternion.identity);
                slimeTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }
    #endregion

    private void Setup() {
        GenerateFrostingTiles();
        GenerateBlankSpace();
        GenerateBreakableTiles();
        GenerateLockTiles();
        GenerateConcreteTiles();
        GenerateSlimeTiles();
        for (int column = 0; column < width; column++) {
            for (int row = 0; row < height; row++) {
                if (!blankSpaces[column, row])
                {
                    Vector2 tilePosition = new Vector2(column, row);
                    GameObject backgroundTile = Instantiate(backgroundTilePrefab, tilePosition, Quaternion.identity, this.transform);
                    backgroundTile.name = "BackgroundTile( " + column + ", " + row + " )";
                }
                if(IsMovable(column, row)) {
                    Vector2 dotPosition = new Vector2(column, row);
                    int dotToUse = Random.Range(0, dots.Length);
                    int maxIterations = 0;
                    while(MatchesAt(column, row, dots[dotToUse]) && maxIterations < 100) {
                        dotToUse = Random.Range(0, dots.Length);
                        maxIterations++;
                    }
                    GameObject dot = Instantiate(dots[dotToUse], dotPosition, Quaternion.identity, this.transform);
                    dot.GetComponent<Dot>().column = column;
                    dot.GetComponent<Dot>().row = row;
                    dot.name = "( " + column + ", " + row + " )";
                    AllDots[column, row] = dot;
                }
            }
        }
        if (IsDeadlocked())
        {
            StartCoroutine(ShuffleBoard());
        }
    }

    private bool MatchesAt(int column, int row, GameObject dot) {
        if(column > 1 && row > 1) {
            if(AllDots[column - 1, row] != null && AllDots[column - 2, row] != null) {
                if(AllDots[column - 1, row].CompareTag(dot.tag) || AllDots[column - 2, row].CompareTag(dot.tag)) {
                    return true;
                }
            }
            if(AllDots[column, row - 1] != null && AllDots[column, row - 2] != null) {
                if(AllDots[column, row - 1].CompareTag(dot.tag) || AllDots[column, row - 2].CompareTag(dot.tag)) {
                    return true;
                }
            }
        } else if (column <= 1 || row <= 1) {
            if(column > 1) {
                if(AllDots[column - 1, row] != null && AllDots[column - 2, row] != null) {
                    if(AllDots[column - 1, row].CompareTag(dot.tag) || AllDots[column - 2, row].CompareTag(dot.tag)) {
                        return true;
                    }
                }
            }
            if(row > 1) {
                if(AllDots[column, row - 1] != null && AllDots[column, row - 2] != null) {
                    if(AllDots[column, row - 1].CompareTag(dot.tag) || AllDots[column, row - 2].CompareTag(dot.tag)) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private MatchType GenerateBomb()
    {
        // Make a copy of the current matches
        List<GameObject> simulateMatch = findMatches.currentMatches;

        matchType.type = 0;
        matchType.color = "";
        
        // Cycle through all match Copy and decide if a bomb needs to
        for (int i = 0; i < simulateMatch.Count; i++)
        {
            // store this dot
            if (simulateMatch[i].TryGetComponent(out Dot currentSimulateDot))
            {
                string currentColor = currentSimulateDot.tag;
                int column = currentSimulateDot.column;
                int row = currentSimulateDot.row;
                int columnMatch = 0;
                int rowMatch = 0;
                // Cycle through the rest of the dots in match to compare
                for (int j = 0; j < simulateMatch.Count; j++)
                {
                    if (simulateMatch[j].TryGetComponent(out Dot nextDot))
                    {
                        if (currentSimulateDot == nextDot)
                        {
                            continue;
                        }

                        if (nextDot.column == currentSimulateDot.column && nextDot.CompareTag(currentColor))
                        {
                            columnMatch++;
                        }

                        if (nextDot.row == currentSimulateDot.row && nextDot.CompareTag(currentColor))
                        {
                            rowMatch++;
                        }
                    }
                }
                // todo enum
                // return 3 if direct bomb
                // return 2 if adjacent bomb
                // return 1 if color bomb
                if (columnMatch == 4 || rowMatch == 4)
                {
                    matchType.type = 1;
                    matchType.color = currentColor;
                    return matchType;
                }
                if (columnMatch == 2 && rowMatch == 2)
                {
                    matchType.type = 2;
                    matchType.color = currentColor;
                    return matchType;
                }
                if (columnMatch == 3 || rowMatch == 3)
                {
                    matchType.type = 3;
                    matchType.color = currentColor;
                    return matchType;
                }
            }
        }
        
        matchType.type = 0;
        matchType.color = "";
        return matchType;
    }

    private void CheckToMakeBomb() {
        if (findMatches.currentMatches.Count > 3)
        {
            MatchType bombType = GenerateBomb();
            if (bombType.type == 1)
            {
                if(currentDot != null && currentDot.isMatched && currentDot.CompareTag(bombType.color)) {
                    currentDot.isMatched = false;
                    currentDot.MakeColorBomb();
                } else {
                    if(currentDot.otherDotGo != null) {
                        Dot otherDot = currentDot.otherDotGo.GetComponent<Dot>();
                        if (otherDot.isMatched && otherDot.CompareTag(bombType.color))
                        {
                            otherDot.isMatched = false;
                            otherDot.MakeColorBomb();
                            
                        }
                    }
                }
            } 
            else if (bombType.type == 2)
            {
                if(currentDot != null && currentDot.isMatched && currentDot.CompareTag(bombType.color)) {
                    currentDot.isMatched = false;
                    currentDot.MakeAdjacentBomb();
                } else {
                    if(currentDot.otherDotGo != null) {
                        Dot otherDot = currentDot.otherDotGo.GetComponent<Dot>();
                        if(otherDot.isMatched && otherDot.CompareTag(bombType.color)) {
                            otherDot.isMatched = false;
                            otherDot.MakeAdjacentBomb();
                        }
                    }
                    
                }
            }
            else if (bombType.type == 3)
            {
                findMatches.CheckDirectionBombs(matchType);
            }
        }
    }
    
    // damage all special tile in row
    public void BombRow(int destroyRow)
    {
        for (int column = 0; column < width; column++)
        {
            if (frostingTiles[column, destroyRow])
            {
                frostingTiles[column, destroyRow].TakeDamage(1);
                if (frostingTiles[column, destroyRow].hitPoints <= 0)
                {
                    frostingTiles[column, destroyRow] = null;
                }
            }
            if (concreteTiles[column, destroyRow])
            {
                concreteTiles[column, destroyRow].TakeDamage(1);
                if (concreteTiles[column, destroyRow].hitPoints <= 0)
                {
                    concreteTiles[column, destroyRow] = null;
                }
            }     
        }
    }
    
    // damage all special tile in column
    public void BombColumn(int destroyColumn)
    {
        for (int row = 0; row < height; row++)
        {
            if (frostingTiles[destroyColumn, row])
            {
                frostingTiles[destroyColumn, row].TakeDamage(1);
                if (frostingTiles[destroyColumn, row].hitPoints <= 0)
                {
                    frostingTiles[destroyColumn, row] = null;
                }
            }
            if (concreteTiles[destroyColumn, row])
            {
                concreteTiles[destroyColumn, row].TakeDamage(1);
                if (concreteTiles[destroyColumn, row].hitPoints <= 0)
                {
                    concreteTiles[destroyColumn, row] = null;
                }
            }
        }   
    }
    
    private void DestroyMatchesAt(int column, int row) {
        if(AllDots[column, row].TryGetComponent(out Dot destroyDot) && destroyDot.isMatched)
        {
            // check if tile is bounded jelly
            if(breakableTiles[column, row] != null)
            {
                breakableTiles[column, row].TakeDamage(1);
                if(breakableTiles[column, row].hitPoints <= 0)
                {
                    breakableTiles[column, row] = null;
                }
            }
            if(LockTiles[column, row] != null)
            {
                LockTiles[column, row].TakeDamage(1);
                if(LockTiles[column, row].hitPoints <= 0)
                {
                    LockTiles[column, row] = null;
                }
            }
            DamageBlocker(column, row);
            
            if(goalManager != null)
            {
                goalManager.CompareGoal(AllDots[column, row].tag);
                goalManager.UpdateGoals();
            }
            if(soundManager != null)
            {
                soundManager.PlayRandomDestroyNoise();
            }
            GameObject particle = Instantiate(destroyEffect, AllDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, 0.5f);
            Destroy(AllDots[column, row]);
            scoreManager.IncreaseScore(basePieceValue * streakValue);
            AllDots[column, row] = null;
        }
    }

    public void DestroyMatches() {
        // define how many pieces are destroyed
        if(findMatches.currentMatches.Count >= 4) {
            CheckToMakeBomb();
        }

        for ( int column = 0; column < width; column++) {
            for (int row = 0; row < height; row++) {
                if (AllDots[column, row] != null) {
                    DestroyMatchesAt(column, row);
                }
            }
        }
        findMatches.currentMatches.Clear();
        StartCoroutine(DecreaseRowCo2());

        // miss some case when refill board, todo fix
    }

    #region "Damage Blocker Manager"
    private void DamageBlocker(int column, int row)
    {
        DamageFrosting(column, row);
        DamageConcrete(column, row);
        DamageSlime(column, row);
    }
    
    private void DamageFrosting(int column, int row)
    {
        if (column > 0)
        {
            if (frostingTiles[column - 1, row])
            {
                frostingTiles[column - 1, row].TakeDamage(1);
                if (frostingTiles[column - 1, row].hitPoints <= 0)
                {
                    frostingTiles[column - 1, row] = null;
                }
            }
        }
        if (column < width - 1)
        {
            if (frostingTiles[column + 1, row])
            {
                frostingTiles[column + 1, row].TakeDamage(1);
                if (frostingTiles[column + 1, row].hitPoints <= 0)
                {
                    frostingTiles[column + 1, row] = null;
                }
            }
        }
        if (row > 0)
        {
            if (frostingTiles[column, row - 1])
            {
                frostingTiles[column, row - 1].TakeDamage(1);
                if (frostingTiles[column, row - 1].hitPoints <= 0)
                {
                    frostingTiles[column, row - 1] = null;
                }
            }
        }
        if (row < height - 1)
        {
            if (frostingTiles[column, row + 1])
            {
                frostingTiles[column, row + 1].TakeDamage(1);
                if (frostingTiles[column, row + 1].hitPoints <= 0)
                {
                    frostingTiles[column, row + 1] = null;
                }
            }
        }
    }
    
    private void DamageConcrete(int column, int row)
    {
        if (column > 0)
        {
            if (concreteTiles[column - 1, row])
            {
                concreteTiles[column - 1, row].TakeDamage(1);
                if (concreteTiles[column - 1, row].hitPoints <= 0)
                {
                    concreteTiles[column - 1, row] = null;
                }
            }
        }
        if (column < width - 1)
        {
            if (concreteTiles[column + 1, row])
            {
                concreteTiles[column + 1, row].TakeDamage(1);
                if (concreteTiles[column + 1, row].hitPoints <= 0)
                {
                    concreteTiles[column + 1, row] = null;
                }
            }
        }
        if (row > 0)
        {
            if (concreteTiles[column, row - 1])
            {
                concreteTiles[column, row - 1].TakeDamage(1);
                if (concreteTiles[column, row - 1].hitPoints <= 0)
                {
                    concreteTiles[column, row - 1] = null;
                }
            }
        }
        if (row < height - 1)
        {
            if (concreteTiles[column, row + 1])
            {
                concreteTiles[column, row + 1].TakeDamage(1);
                if (concreteTiles[column, row + 1].hitPoints <= 0)
                {
                    concreteTiles[column, row + 1] = null;
                }
            }
        }
    }
    
    private void DamageSlime(int column, int row)
    {
        if (column > 0)
        {
            if (slimeTiles[column - 1, row])
            {
                slimeTiles[column - 1, row].TakeDamage(1);
                if (slimeTiles[column - 1, row].hitPoints <= 0)
                {
                    slimeTiles[column - 1, row] = null;
                }
                makeSlime = false;
            }
        }
        if (column < width - 1)
        {
            if (slimeTiles[column + 1, row])
            {
                slimeTiles[column + 1, row].TakeDamage(1);
                if (slimeTiles[column + 1, row].hitPoints <= 0)
                {
                    slimeTiles[column + 1, row] = null;
                }
                makeSlime = false;
            }
        }
        if (row > 0)
        {
            if (slimeTiles[column, row - 1])
            {
                slimeTiles[column, row - 1].TakeDamage(1);
                if (slimeTiles[column, row - 1].hitPoints <= 0)
                {
                    slimeTiles[column, row - 1] = null;
                }
                makeSlime = false;
            }
        }
        if (row < height - 1)
        {
            if (slimeTiles[column, row + 1])
            {
                slimeTiles[column, row + 1].TakeDamage(1);
                if (slimeTiles[column, row + 1].hitPoints <= 0)
                {
                    slimeTiles[column, row + 1] = null;
                }
                makeSlime = false;
            }
        }
    }
    #endregion
    
    // todo refactor O(n^3)
    // row down with blank tile
    private IEnumerator DecreaseRowCo2() {
        for (int column = 0; column < width; column++) {
            for (int row = 0; row < height; row++) {
                // if the current spot isn't blank and is empty. . .
                if(AllDots[column, row] == null && IsMovable(column, row)) {
                    for(int k = row + 1; k < height; k++) {
                        // if a dot is found
                        if (AllDots[column, k] != null) {
                            // move this dot to this empty space
                            if (AllDots[column, k].TryGetComponent(out Dot dot))
                            {
                                dot.row = row;
                            }
                            // set that spot to be null
                            AllDots[column, k] = null;
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

    // Delete and row down if board has matches
    private IEnumerator DecreaseRowCo() {
        int totalNullRows = 0;
        for (int column = 0; column < width; column++) {
            for (int row = 0; row < height; row++) {
                if(AllDots[column, row] == null) {
                    totalNullRows++;
                } else if(totalNullRows > 0) {
                    if (AllDots[column, row].TryGetComponent(out Dot dot))
                    {
                        dot.row -= totalNullRows;
                    }
                    AllDots[column, row] = null;
                }
            }
            totalNullRows = 0;
        }
        yield return new WaitForSeconds(refillDelay * 0.5f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard() {
        for (int column = 0; column < width; column ++) {
            for (int row = 0; row < height; row++) {
                if (AllDots[column, row] == null && IsMovable(column, row))
                {
                    Vector2 tempPosition = new Vector2(column, row + rowOffSet);
                    int dotToUse = Random.Range(0, dots.Length);
                    // todo refactor
                    // while(MatchesAt(column, row, dots[dotToUse]))
                    // {
                    //     dotToUse = Random.Range(0, dots.Length);
                    // }
                    if (Instantiate(dots[dotToUse], tempPosition, Quaternion.identity, transform)
                        .TryGetComponent(out Dot dot))
                    {
                        AllDots[column, row] = dot.gameObject;
                        dot.column = column;
                        dot.row = row;
                    }
                }
            }   
        }
    }

    private bool MatchesOnBoard() {
        findMatches.FindAllMatches();
        for (int column = 0; column < width; column++) {
            for (int row = 0; row < height; row++) {
                if(AllDots[column, row] != null) {
                    if(AllDots[column, row].TryGetComponent(out Dot dot) && dot.isMatched) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo() {
        yield return new WaitForSeconds(refillDelay);
        RefillBoard();
        yield return new WaitForSeconds(refillDelay);
        while(MatchesOnBoard())
        {  
            streakValue ++;
            DestroyMatches();
            yield break;
        }
        Debug.Log(findMatches.currentMatches.Count);
        currentDot = null;
        CheckToMakeSlime();
        if(IsDeadlocked()) {
            StartCoroutine(ShuffleBoard());
        }
        yield return new WaitForSeconds(refillDelay);
        GC.Collect();
        if (currentState != GameState.Pause)
        {
            currentState = GameState.Move;
        }
        makeSlime = true;
        streakValue = 1;
    }

    private void CheckToMakeSlime()
    {
        for (int column = 0; column < width; column++)
        {
            for (int row = 0; row < height; row++)
            {
                if (slimeTiles[column, row] != null && makeSlime)
                {
                    // call sub method 
                    MakeNewSlime();
                    makeSlime = false;
                }
            }
        }
    }

    private Vector2 CheckForAdjacent(int column, int row)
    {
        if (column < width - 1 && AllDots[column + 1, row] )
        {
            return Vector2.right;
        }
        if (column > 0 && AllDots[column - 1, row])
        {
            return Vector2.left;
        }
        if (row < height - 1 && AllDots[column, row + 1])
        {
            return Vector2.up;
        }
        if (row > 0 && AllDots[column, row - 1])
        {
            return Vector2.down;
        }
        return Vector2.zero;
    }
    
    private void MakeNewSlime()
    {
        // todo refactor
        bool slime = false;
        int loops = 0;
        while (!slime && loops < 200)
        {
            int newColumn = Random.Range(0, width);
            int newRow = Random.Range(0, height);
            if (slimeTiles[newColumn, newRow])
            {
                Vector2 adjacent = CheckForAdjacent(newColumn, newRow);
                if (adjacent != Vector2.zero)
                {
                    Destroy(AllDots[newColumn + (int)adjacent.x, newRow + (int)adjacent.y]);
                    Vector2 tempPosition = new Vector2(newColumn + (int)adjacent.x, newRow + (int)adjacent.y);
                    GameObject tile = Instantiate(slimePrefab, tempPosition, Quaternion.identity);
                    slimeTiles[newColumn + (int)adjacent.x, newRow + (int)adjacent.y] = tile.GetComponent<BackgroundTile>();
                    slime = true;
                }
            }
            loops++;
        }
    }

    private void SwitchPieces(int column, int row, Vector2 direction) 
    {
        if (AllDots[column + (int)direction.x, row + (int)direction.y] != null)
        {
            // take the second piece and save it in a holder
            GameObject holder = AllDots[column + (int)direction.x, row + (int)direction.y];
            // switch the first dot to be
            AllDots[column + (int)direction.x, row + (int)direction.y] = AllDots[column, row];
            AllDots[column, row] = holder;
        }
    }

    private bool CheckForMatches()
    {
        for(int column = 0; column < width; column++)
        {
            for(int row = 0; row < height; row++)
            {
                if(AllDots[column, row] != null)
                {
                    // make sure that ond and two to the right are in the board
                    if(column < width - 2)
                    {
                        // check if the dots to the right and two to the right
                        if(AllDots[column + 1, row] != null && AllDots[column + 2, row] != null)
                        {
                            if(AllDots[column + 1, row].CompareTag(AllDots[column, row].tag)
                            && AllDots[column + 2, row].CompareTag(AllDots[column, row].tag))
                            {
                                return true;
                            }
                        }
                    }
                    if(row < height - 2)
                    {
                        // check if the dot above exist
                        if(AllDots[column, row + 1] != null && AllDots[column, row + 2] != null)
                        {
                            if(AllDots[column, row + 1].CompareTag(AllDots[column, row].tag)
                            && AllDots[column, row + 2].CompareTag(AllDots[column, row].tag))
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
        for (int column = 0; column < width; column++)
        {
            for (int row = 0; row < height; row++)
            {
                if(AllDots[column, row] != null)
                {
                    if(column < width - 1)
                    {
                        if(SwitchAndCheck(column, row, Vector2.right))
                        {
                            return false;
                        }
                    }
                    if(row < height - 1)
                    {
                        if(SwitchAndCheck(column, row, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    private IEnumerator ShuffleBoard()
    {
        yield return new WaitForSeconds(0.5f);
        List<GameObject> newBoardTiles = new List<GameObject>();
        for (int column = 0; column < width; column++)
        {
            for (int row = 0; row < height; row++)
            {
                if(AllDots[column, row] != null)
                {
                    newBoardTiles.Add(AllDots[column, row]);
                }
            }
        }
        yield return new WaitForSeconds(0.5f);
        // for every spot on the board
        for (int column = 0; column < width; column++)
        {
            for (int row = 0; row < height; row++)
            {
                // if this spot shouldn't be blank
                if(IsMovable(column, row))
                {
                    // pick a random number
                    int pieceToUse = Random.Range(0, newBoardTiles.Count);
                    
                    int maxIterations = 0;
                    while(MatchesAt(column, row, newBoardTiles[pieceToUse]) && maxIterations < 100) {
                        pieceToUse = Random.Range(0, newBoardTiles.Count);
                        maxIterations++;
                    }
                    // make a container for the piece
                    if (newBoardTiles[pieceToUse].TryGetComponent(out Dot piece))
                    {
                        piece.column = column;
                        piece.row = row;
                    }
                    // fill in the dots array with this new piece
                    AllDots[column, row] = newBoardTiles[pieceToUse];
                    newBoardTiles.Remove(newBoardTiles[pieceToUse]);
                }
            }
        }
        // check if it's still deadlock
        if(IsDeadlocked())
        {
            StartCoroutine(ShuffleBoard());
        }
    }
}
