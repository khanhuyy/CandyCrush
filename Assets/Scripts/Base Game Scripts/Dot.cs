using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Dot : MonoBehaviour
{
    [Header("System")] 
    private new Camera camera;
    
    [Header("Board Variables")]
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
    public GameObject otherDotGo;
    private Vector2 firstTouchPosition = Vector2.zero;
    private Vector2 finalTouchPosition = Vector2.zero;
    private Vector2 tempPosition;

    [FormerlySerializedAs("isMoving")] [Header("Swipe Stuff")]
    public bool isSolving; // for check if this is bomb solving problem
    public float swipeAngle;
    public float swipeResist = 1f; // swipe distance must greater than 1

    [Header("Power Up Stuff")]
    public bool isColorBomb; // 5 similar in row or column
    public bool isColumnBomb; // 4 similar in row
    public bool isRowBomb; // 4 similar in column
    public bool isAdjacentBomb; // L shape, 3 length in both coordinate
    public GameObject rowArrow;
    public GameObject columnArrow;
    public GameObject colorBomb;
    public GameObject adjacentMarker;
    void Start()
    {
        camera = Camera.main;
        
        isColumnBomb = false;
        isRowBomb = false;
        isColorBomb = false;
        isAdjacentBomb = false;
        
        endGameManager = FindObjectOfType<EndGameManager>();
        hintManager = FindObjectOfType<HintManager>();
        board = FindObjectOfType<Board>();    
        findMatches = FindObjectOfType<FindMatches>();    
        // targetX = (int) transform.position.x;
        // targetY = (int) transform.position.y;
        // column = targetX;
        // row = targetY;
        // previousColumn = column;
        // previousRow = row;
    }

    // for testing and debug
    private void OnMouseOver() {
        if(Input.GetMouseButtonDown(1)) {
            isColorBomb = true;
            Instantiate(colorBomb, transform.position, Quaternion.identity, this.transform);
        }
    }


    void Update()
    {
        targetX = column;
        targetY = row;
        if(Mathf.Abs(targetX - transform.position.x) > 0.1) {
            tempPosition = new Vector2(targetX, transform.position.y);
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

    }

    public IEnumerator CheckMoveCo() {
        if(isColorBomb) {
            // this piece is a color bomb and the other pieces the color to destroy
            findMatches.MatchDotsOfColor(otherDotGo.tag);
            isMatched = true;
        } else if(otherDotGo.TryGetComponent(out Dot otherDot) && otherDot.isColorBomb) {
            // other piece is color bomb
            findMatches.MatchDotsOfColor(this.gameObject.tag);
            otherDot.isMatched = true;
        }
        yield return new WaitForSeconds(0.5f);
        if(otherDotGo) {
            if(!isMatched && otherDotGo.TryGetComponent(out Dot otherDot) && !otherDot.isMatched) 
            {
                otherDot.row = row;
                otherDot.column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(0.5f);
                board.currentDot = null;
                board.currentState = GameState.Move;
            } 
            else 
            {
                if(endGameManager.requirements.gameType == GameType.Moves)
                {
                    endGameManager.DecreaseCounterValue();
                }
                board.DestroyMatches();
            }
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
        otherDotGo = board.AllDots[column + (int)direction.x, row + (int)direction.y];
        previousColumn = column;
        previousRow = row;
        if (board.LockTiles[column, row] == null &&
            board.LockTiles[column + (int)direction.x, row + (int)direction.y] == null)
        {
            if (otherDotGo != null)
            {
                otherDotGo.GetComponent<Dot>().column += (-1) * (int)direction.x;
                otherDotGo.GetComponent<Dot>().row += (-1) * (int)direction.y;
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

    public void MakeRowBomb() {
        if (!isColumnBomb && !isColorBomb && !isAdjacentBomb)
        {
            isRowBomb = true;
            Instantiate(rowArrow, transform.position, Quaternion.identity, transform);

        }
    }

    public void MakeColumnBomb() {
        if (!isRowBomb && !isColorBomb && !isAdjacentBomb)
        {
            isColumnBomb = true;
            Instantiate(columnArrow, transform.position, Quaternion.identity, transform);
        }
    }

    public void MakeColorBomb() {
        if (!isColumnBomb && !isRowBomb && !isAdjacentBomb)
        {
            isColorBomb = true;
            Instantiate(colorBomb, transform.position, Quaternion.identity, this.transform);
            gameObject.tag = "Color";
        }
    }

    public void MakeAdjacentBomb() {
        if (!isColumnBomb && !isRowBomb && !isColorBomb)
        {
            isAdjacentBomb = true;
            Instantiate(adjacentMarker, transform.position, Quaternion.identity, this.transform);
        }
        
    }
}
