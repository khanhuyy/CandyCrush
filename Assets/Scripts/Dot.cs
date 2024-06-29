using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Board Variables")]
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    public bool isMatched = false;

    [Header("Manager Variables")]
    private EndGameManager endGameManager;
    private HintManager hintManager;
    private FindMatches findMatches;
    private Board board;
    public GameObject otherDot;
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosition;

    [Header("Swipe Stuff")]
    public float swipeAngle = 0;
    public float swipeResist = 1f; // swipe distance must greater than 1

    [Header("Power Up Stuff")]
    public bool isColorBomb; // 5 similar in row or column
    public bool isColumnBomb; // 4 similar in row
    public bool isRowBomb; // 4 similar in column
    public bool isAdjacentBomb;
    public GameObject rowArrow;
    public GameObject columnArrow;
    public GameObject colorBomb;
    public GameObject adjacentMarker;
    void Start()
    {
        isColumnBomb = false;
        isRowBomb = false;
        // isColorBomb = false;
        // isAdjacentBomb = false;
        //
        // endGameManager = FindObjectOfType<EndGameManager>();
        // hintManager = FindObjectOfType<HintManager>();
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
            isColumnBomb = true;
            GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity, this.transform);
        }
    }


    void Update()
    {
        // todo remove
        FindMatches();
        if(isMatched) {
            SpriteRenderer mySrite = GetComponent<SpriteRenderer>();
            mySrite.color = new Vector4(255f, 255f, 255f, 0.2f);
        }
        
        targetX = column;
        targetY = row;
        
        
        if(Mathf.Abs(targetX - transform.position.x) > 0.1) {
            // Move towars the target
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.2f);
            if(board.allDots[column, row] != this.gameObject) {
                board.allDots[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        } else {
            // Directly set position, todo check
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
            board.allDots[column, row] = this.gameObject;
        }
        if(Mathf.Abs(targetY - transform.position.y) > 0.1) {
            // Move towars the target
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.2f);
            if(board.allDots[column, row] != this.gameObject) {
                board.allDots[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches(); // todo consider call 1 time
        } else {
            // Directly set position, todo check
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
            board.allDots[column, row] = this.gameObject;
        }
    }

    public IEnumerator CheckMoveCo() {
        // if(isColorBomb) {
        //     // this piece is a color bomb and the other pieceis the color to destroy
        //     findMatches.MatchPIecesOfColor(otherDot.tag);
        //     isMatched = true;
        // } else if(otherDot.GetComponent<Dot>().isColorBomb) {
        //     // other piece is color bomb
        //     findMatches.MatchPIecesOfColor(this.gameObject.tag);
        //     otherDot.GetComponent<Dot>().isMatched = true;
        // }
        yield return new WaitForSeconds(.5f);
        if(otherDot != null) {
            if(!isMatched && !otherDot.GetComponent<Dot>().isMatched) 
            {
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;
                row = previousRow;
                column = previousColumn;
                // yield return new WaitForSeconds(0.5f);
                // board.currentDot = null;
                board.currentState = GameState.Move;
            } 
            else 
            {
                // if(endGameManager.requirements.gameType == GameType.Moves)
                // {
                //     endGameManager.DecreaseCounterValue();
                // }
                board.DestroyMatches();
            }
            otherDot = null;
        }
    }

    private void OnMouseDown() {
        // Destroy hint
        // if(hintManager != null)
        // {
        //     hintManager.DestroyHint();
        // }
        if(board.currentState == GameState.Move) {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
    
    private void OnMouseUp() {
        if(board.currentState == GameState.Move) 
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    void CalculateAngle() {
        // swipe angle range right, top, left, bottom are 315 - 45, 45 - 135, 135 - 225, 225 - 315
        
        if(Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            MovePieces();
            board.currentState = GameState.Wait;
            board.currentDot = this;
        }
        else {
            board.currentState = GameState.Move;
        }
    }

    void MovePiecesActual(Vector2 direction) {
        otherDot = board.allDots[column + (int)direction.x, row + (int)direction.y];
        previousColumn = column;
        previousRow = row;
        if(otherDot != null) {
            otherDot.GetComponent<Dot>().column += (-1) * (int)direction.x;
            otherDot.GetComponent<Dot>().row += (-1) * (int)direction.y;
            column += (int)direction.x;
            row += (int)direction.y;
            StartCoroutine(CheckMoveCo());
        } else {
            board.currentState = GameState.Move;
        }
        
    }

    void MovePieces() {
        if(swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1) {
            // Right Swipe
            otherDot = board.allDots[column + 1, row];
            previousColumn = column;
            previousRow = row;
            otherDot.GetComponent<Dot>().column -= 1;
            column += 1;
            // StartCoroutine(CheckMoveCo());
            // MovePiecesActual(Vector2.right);
        } else if(swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1) {
            // Up Swipe
            otherDot = board.allDots[column, row + 1];
            previousColumn = column;
            previousRow = row;
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;
            // StartCoroutine(CheckMoveCo());
            // MovePiecesActual(Vector2.up);
        } else if((swipeAngle > 135 || swipeAngle <= -135) && column > 0) {
            // Left Swipe
            otherDot = board.allDots[column - 1, row];
            previousColumn = column;
            previousRow = row;
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1;
            // StartCoroutine(CheckMoveCo());
            // MovePiecesActual(Vector2.left);
        } else if(swipeAngle > -135 && swipeAngle <= -45 && row > 0) {
            // Down Swipe
            otherDot = board.allDots[column, row - 1];
            previousColumn = column;
            previousRow = row;
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;
            // StartCoroutine(CheckMoveCo());
            // MovePiecesActual(Vector2.down);
        } else {
            board.currentState = GameState.Move;
        }
        StartCoroutine(CheckMoveCo());
    }

    void FindMatches() {
        if(column > 0 && column < board.width - 1) {
            GameObject leftDot = board.allDots[column - 1, row];
            GameObject rightDot = board.allDots[column + 1, row];
            if(leftDot != null && rightDot != null) {
                if(leftDot.CompareTag(this.gameObject.tag) && rightDot.CompareTag(this.gameObject.tag)) {
                    leftDot.GetComponent<Dot>().isMatched = true;
                    rightDot.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
        if(row > 0 && row < board.height - 1) {
            GameObject upDot= board.allDots[column, row + 1];
            GameObject downDot = board.allDots[column, row - 1];
            if(upDot != null && downDot != null) {
                if(upDot.CompareTag(this.gameObject.tag) && downDot.CompareTag(this.gameObject.tag)) {
                    upDot.GetComponent<Dot>().isMatched = true;
                    downDot.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    }

    public void MakeRowBomb() {
        isRowBomb = true;
        GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity, this.transform);
    }

    public void MakeColumnBomb() {
        isColumnBomb = true;
        GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity, this.transform);
    }

    public void MakeColorBomb() {
        isColorBomb = true;
        Instantiate(colorBomb, transform.position, Quaternion.identity, this.transform);
        this.gameObject.tag = "Color";
    }

    public void MakeAdjacentBomb() {
        isAdjacentBomb = true;
        Instantiate(adjacentMarker, transform.position, Quaternion.identity, this.transform);
    }
}
