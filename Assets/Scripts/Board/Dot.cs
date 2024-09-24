using System;
using System.Collections;
using Controller;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public enum PieceColor
{
    None,
    Blue,
    Green,
    Orange,
    Purple,
    Red,
    Yellow,
}

public class Dot : MonoBehaviour
{
    [Header("System")] 
    private new Camera camera;

    [Header("Board Variables")]
    public PieceColor color;
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    public bool isMatched;

    [Header("Manager Variables")]
    private EndGameManager endGameManager;
    private HintManager hintManager;
    private FindMatches findMatches;
    private Board board;
    public GameObject otherDotObject;
    private Vector2 firstTouchPosition = Vector2.zero;
    private Vector2 finalTouchPosition = Vector2.zero;
    private Vector2 tempPosition;

    [Header("Swipe Stuff")]
    public bool isSolving; // for check if this is bomb solving problem
    public float swipeAngle;
    public float swipeResist = 1f; // swipe distance must greater than 1

    [Header("Power Up Stuff And Animation")] 
    public SpriteRenderer spriteRenderer;
    public bool isColorBomb; // 5 similar in row or column
    public bool isColumnBomb; // 4 similar in row
    public bool isRowBomb; // 4 similar in column
    public bool isAdjacentBomb; // L shape, > 3 length in both coordinate
    [SerializeField] private GameObject directBombIdleEffect;
    [SerializeField] private GameObject directBombDestroyEffect;
    public Sprite horizonBombSprite;
    public Sprite verticalBombSprite;
    [SerializeField] private Sprite areaBombSprite;
    [SerializeField] private Sprite colorBombSprite;
    [SerializeField] private GameObject colorBombPrefab;

    [Header("Destroy Asset")] public GameObject destroyDestination;

    private void OnEnable()
    {
        GameEvent.DestroyPiece += GameEvent_DestroyPiece;
    }
    
    private void OnDisable()
    {
        GameEvent.DestroyPiece -= GameEvent_DestroyPiece;
    }

    private void GameEvent_DestroyPiece(int arg1, int arg2)
    {
        if ((isColumnBomb || isRowBomb) && isMatched)
        {
            if (isColumnBomb && isRowBomb)
            {
                var destroyEffColumn = Instantiate(directBombDestroyEffect, transform.position, Quaternion.identity);
                var destroyEffRow = Instantiate(directBombDestroyEffect, transform.position, Quaternion.identity);
                if (destroyEffColumn.TryGetComponent(out Animator columnAnimator))
                {
                    columnAnimator.SetTrigger("Column");
                }
                if (destroyEffRow.TryGetComponent(out Animator rowAnimator))
                {
                    rowAnimator.SetTrigger("Row");
                }
                Destroy(destroyEffColumn, 1f);
                Destroy(destroyEffRow, 1f);
            }
            else
            {
                var destroyEff = Instantiate(directBombDestroyEffect, transform.position, Quaternion.identity);
                if (destroyEff.TryGetComponent(out Animator animator))
                {
                    if (isColumnBomb)
                    {
                        animator.SetTrigger("Column");
                    }
                    else if (isRowBomb)
                        animator.SetTrigger("Row");
                }
                Destroy(destroyEff, 1f);
            }
        }
        else if (isColorBomb)
        {
            
        }
        else if (isAdjacentBomb)
        {
            
        }
        else
        {
            // transform.localScale = Vector3.zero;
        }
    }

    void Start()
    {
        camera = Camera.main;
        endGameManager = FindObjectOfType<EndGameManager>();
        hintManager = FindObjectOfType<HintManager>();
        board = FindObjectOfType<Board>();    
        findMatches = FindObjectOfType<FindMatches>();
        destroyDestination = GameObject.Find("PieceDestroyDestination");
    }
    


    void Update()
    {
        // if (color == PieceColor.None)
        // {
        //     isColorBomb = true;
        // }
        targetX = column;
        targetY = row;
        if(Mathf.Abs(targetX - transform.position.x) > 0.1) {
            tempPosition = new Vector2(targetX, transform.position.y);
            // transform.position = Vector2.Lerp(transform.position, tempPosition, 0.06f);
            // Debug
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.2f);
            if(board.AllDots[column, row] != gameObject) {
                board.AllDots[column, row] = gameObject;
                findMatches.FindAllMatches();
            }
            else
            {
                isSolving = false;
            }
        } else {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
        }
        if(Mathf.Abs(targetY - transform.position.y) > 0.1) {
            tempPosition = new Vector2(transform.position.x, targetY);
            // transform.position = Vector2.Lerp(transform.position, tempPosition, 0.06f);
            // Debug
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.2f);
            if(board.AllDots[column, row] != gameObject && !isSolving) {
                board.AllDots[column, row] = gameObject;
                findMatches.FindAllMatches();
            }
            else
            {
                isSolving = false;
            }
        } else {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
        }
        if (transform.localScale == Vector3.zero)
            Destroy(gameObject);
    }

    public IEnumerator CheckMoveCo() {
        // solve bomb
        var otherDotComponent = otherDotObject.GetComponent<Dot>();
        if (IsBomb() || (otherDotComponent && otherDotComponent.IsBomb()))
        {
            ValidateSpecialBombMove(this, otherDotComponent);
        }
        yield return new WaitForSeconds(1f);
        if (otherDotObject)
        {
            if (!isMatched && !otherDotComponent.isMatched)
            {
                otherDotComponent.row = row;
                otherDotComponent.column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(0.5f);
                board.currentDot = null;
                board.currentState = GameState.Move;
            }
            else
            {
                if (endGameManager.requirements.gameType == GameType.Moves)
                {
                    endGameManager.DecreaseCounterValue();
                }
                board.DestroyMatches();
            }
        }
    }

    private void ValidateSpecialBombMove(Dot firstDot, Dot secondDot)
    {
        if(firstDot.isColorBomb) {
            if (secondDot.isColorBomb)
            {
                findMatches.MatchAllBoard();
            } 
            else if (secondDot.isAdjacentBomb)
            {
                findMatches.MakeAdjacentSameColor(secondDot.color);
            }
            else if (secondDot.isColumnBomb || secondDot.isRowBomb)
            {
                findMatches.MakeDirectionBombSameColor(secondDot.color);
                isMatched = true;
            }
            else
            {
                findMatches.MatchDotsSameColor(secondDot.color);
                isMatched = true;
            }
        }
        else if (firstDot.isAdjacentBomb)
        {
            if (secondDot.isColorBomb)
            {
                findMatches.MakeAdjacentSameColor(firstDot.color);
            }
            else if (secondDot.isAdjacentBomb)
            {
                findMatches.MatchDoubleAdjacent(firstDot, secondDot);
            }
            else if (secondDot.isColumnBomb)
            {
                findMatches.MatchAdjacentColumn(firstDot, secondDot);
            }
            else if (secondDot.isRowBomb)
            {
                findMatches.MatchAdjacentRow(firstDot, secondDot);
            }
            else
            {
                findMatches.MatchAdjacent(firstDot.column, firstDot.row);
            }
        }
        else if (firstDot.IsDirectionBomb())
        {
            if (secondDot.isColorBomb)
            {
                findMatches.MatchDotsSameColor(firstDot.color);
            }
            else if (secondDot.isAdjacentBomb)
            {
                if (firstDot.isColumnBomb)
                {
                    findMatches.MatchAdjacentColumn(firstDot, secondDot);
                }
                else
                {
                    findMatches.MatchAdjacentRow(firstDot, secondDot);
                }
            }
            else if (secondDot.IsDirectionBomb())
            {
                findMatches.MatchCross(firstDot, secondDot);
            }
            else
            {
                findMatches.MatchDirection(firstDot);
                firstDot.isMatched = true; // todo refactor
            }
        }
        else
        {
            findMatches.MatchBombProperly(secondDot);
        }
    }

    private void OnMouseDown() {
        if(hintManager != null)
        {
            hintManager.DestroyHint();
        }
        if(board.currentState == GameState.Move) {
            firstTouchPosition = camera.ScreenToWorldPoint(Input.mousePosition);
        }
    }
    
    private void OnMouseUp() {
        if(hintManager != null)
        {
            hintManager.DestroyHint();
        }
        if(board.currentState == GameState.Move) 
        {
            finalTouchPosition = camera.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    void CalculateAngle() {
        // swipe angle range right, top, left, bottom are 315 - 45, 45 - 135, 135 - 225, 225 - 315
        if(Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            board.currentState = GameState.Wait;
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            MovePieces();
            board.currentDot = this;
        }
        else {
            board.currentState = GameState.Move;
        }
    }

    void MovePiecesActual(Vector2 direction) {
        otherDotObject = board.AllDots[column + (int)direction.x, row + (int)direction.y];
        previousColumn = column;
        previousRow = row;
        if (board.LockTiles[column, row] == null &&
            board.LockTiles[column + (int)direction.x, row + (int)direction.y] == null)
        {
            if (otherDotObject != null)
            {
                otherDotObject.GetComponent<Dot>().column += (-1) * (int)direction.x;
                otherDotObject.GetComponent<Dot>().row += (-1) * (int)direction.y;
                column += (int)direction.x;
                row += (int)direction.y;
                StartCoroutine(CheckMoveCo());
            }
            else
            {
                board.currentState = GameState.Move;
            }
        } 
        else
        {
            board.currentState = GameState.Move;
        }
    }

    void MovePieces() {
        if(swipeAngle is > -45 and <= 45 && column < board.width - 1) {
            // Right Swipe
            MovePiecesActual(Vector2.right);
        } else if(swipeAngle is > 45 and <= 135 && row < board.height - 1) {
            // Up Swipe
            MovePiecesActual(Vector2.up);
        } else if(swipeAngle is > 135 or <= -135 && column > 0) {
            // Left Swipe
            MovePiecesActual(Vector2.left);
        } else if(swipeAngle is > -135 and <= -45 && row > 0) {
            // Down Swipe
            MovePiecesActual(Vector2.down);
        }
        else
        {
            board.currentState = GameState.Move;
        }
    }

    public void MakeBombProperly(BombType bombType)
    {
        if (bombType == BombType.Color)
        {
            MakeColorBomb();
        }
        else if (bombType == BombType.Adjacent)
        {
            MakeAdjacentBomb();
        }
        else if (bombType == BombType.Column)
        {
            MakeColumnBomb();
        }
        else if (bombType == BombType.Row)
        {
            MakeRowBomb();
        }
    }
    
    public void MakeRowBomb()
    {
        // Debug.Log("Making Row Bomb at (" + column + ", " + row + ")");
        if (!isColumnBomb && !isColorBomb && !isAdjacentBomb)
        {
            tag = "Row Bomb";
            color = PieceColor.None;
            isMatched = false;
            isRowBomb = true;
            spriteRenderer.sprite = horizonBombSprite;
        }
    }

    public void MakeColumnBomb() {
        if (!isRowBomb && !isColorBomb && !isAdjacentBomb)
        {
            // Debug.Log("Making Column Bomb at (" + column + ", " + row + ")");
            tag = "Column Bomb";
            color = PieceColor.None;
            isMatched = false;
            isColumnBomb = true;
            spriteRenderer.sprite = verticalBombSprite;
        }
    }

    public void MakeCrossBomb()
    {
        if (!isColorBomb && !isAdjacentBomb)
        {
            tag = "Cross Bomb";
            color = PieceColor.None;
            isColumnBomb = true;
            isRowBomb = true;
        }
    }

    public void MakeColorBomb() {
        // Debug.Log("Making Color Bomb at (" + column + ", " + row + ")");
        if (!isColumnBomb && !isRowBomb && !isAdjacentBomb)
        {
            tag = "Color Bomb";
            color = PieceColor.None;
            isMatched = false;
            isColorBomb = true;
            spriteRenderer.sprite = colorBombSprite;
        }
    }

    public void MakeAdjacentBomb() {
        // Debug.Log("Making Adjacent Bomb at (" + column + ", " + row + ")");
        if (!isColumnBomb && !isRowBomb && !isColorBomb)
        {
            tag = "Adjacent Bomb";
            color = PieceColor.None;
            isMatched = false;
            isAdjacentBomb = true;
            spriteRenderer.sprite = areaBombSprite;
        }
    }

    public bool IsBomb()
    {
        return color == PieceColor.None && (isColorBomb || isAdjacentBomb || isColumnBomb || isRowBomb);
    }

    public bool IsDirectionBomb()
    {
        return color == PieceColor.None && (isRowBomb || isColumnBomb);
    }

    public void DefuseBomb()
    {
        isColorBomb = false;
        isAdjacentBomb = false;
        isColumnBomb = false;
        isRowBomb = false;
    }
}
