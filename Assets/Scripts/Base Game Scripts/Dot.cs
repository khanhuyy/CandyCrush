using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public enum DotColorType {
    Red,
    Orange,
    Yellow,
    Green,
    Indigo,
    Violet
}

public class Dot : MonoBehaviour
{
    [Header("System")] 
    private new Camera camera;

    [Header("Board Variables")] 
    public DotColorType colorType;
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    public bool isMatched;

    [Header("Manager Variables")]
    private EndGameManager _endGameManager;
    private HintManager _hintManager;
    private FindMatches _findMatches;
    private Board _board;
    public Dot otherDotGo;
    private Vector2 _srcTouchPosition = Vector2.zero;
    private Vector2 _dstTouchPosition = Vector2.zero;
    private Vector2 _tempPosition;

    [FormerlySerializedAs("isMoving")] [Header("Swipe Stuff")]
    public bool isSolving; // for check if this is bomb solving problem
    public float swipeAngle;
    public float swipeResist = 1f; // swipe distance must greater than 1

    [Header("Power Up Stuff And Animation")] 
    [SerializeField] private SpriteRenderer spriteRenderer;
    public bool isColorBomb; // 5 similar in row or column
    public bool isColumnBomb; // 4 similar in row
    public bool isRowBomb; // 4 similar in column
    public bool isAdjacentBomb; // L shape, 3 length in both coordinate
    [SerializeField] private Sprite rowBombSprite;
    [SerializeField] private Sprite columnBombSprite;
    [SerializeField] private Sprite areaBombSprite;
    [SerializeField] private GameObject colorBombPrefab;

    private void Start()
    {
        camera = Camera.main;
        
        isColumnBomb = false;
        isRowBomb = false;
        isColorBomb = false;
        isAdjacentBomb = false;
        
        _endGameManager = FindFirstObjectByType<EndGameManager>();
        _hintManager = FindFirstObjectByType<HintManager>();
        _board = FindFirstObjectByType<Board>();    
        _findMatches = FindFirstObjectByType<FindMatches>();   
    }

    // for testing and debug
    private void OnMouseOver() {
        if(Input.GetMouseButtonDown(1)) {
            MakeColorBomb();
        }
    }


    private void Update()
    {
        if (CompareTag("Color"))
        {
            isColorBomb = true;
        }
        targetX = column;
        targetY = row;
        if(Mathf.Abs(targetX - transform.position.x) > 0.1) {
            _tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, _tempPosition, 0.02f);
            if(_board.AllDots[column, row] != this  && !isSolving) {
                _board.AllDots[column, row] = this;
                _findMatches.FindAllMatches();
            }
            else
            {
                isSolving = false;
            }
        } else {
            _tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = _tempPosition;
        }
        if(Mathf.Abs(targetY - transform.position.y) > 0.1) {
            _tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, _tempPosition, 0.02f);
            if(_board.AllDots[column, row] != this && !isSolving) {
                _board.AllDots[column, row] = this;
                _findMatches.FindAllMatches();
            }
            else
            {
                isSolving = false;
            }
        } else {
            _tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = _tempPosition;
        }

    }

    private IEnumerator CheckMoveCo() {
        if(isColorBomb) {
            // this piece is a color bomb and the other pieces the color to destroy
            _findMatches.MatchDotsOfColor(otherDotGo.tag);
            isMatched = true;
        } else if(otherDotGo.TryGetComponent(out Dot otherDot) && otherDot.isColorBomb) {
            // other piece is color bomb
            _findMatches.MatchDotsOfColor(gameObject.tag);
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
                _board.currentDot = null;
                _board.currentState = GameState.Move;
            } 
            else 
            {
                if(_endGameManager.requirements.gameType == GameType.Moves)
                {
                    _endGameManager.DecreaseCounterValue();
                }
                _board.DestroyMatches();
            }
        }
    }

    private void OnMouseDown() {
        if(_hintManager)
        {
            _hintManager.DestroyHint();
        }
        if(_board.currentState == GameState.Move) {
            _srcTouchPosition = camera.ScreenToWorldPoint(Input.mousePosition);
        }
    }
    
    private void OnMouseUp() {
        if(_hintManager)
        {
            _hintManager.DestroyHint();
        }

        if (_board.currentState != GameState.Move) return;
        _dstTouchPosition = camera.ScreenToWorldPoint(Input.mousePosition);
        TrySwapDots();
    }

    private void TrySwapDots() {
        // swipe angle range right, top, left, bottom are 315 - 45, 45 - 135, 135 - 225, 225 - 315
        if(Mathf.Abs(_dstTouchPosition.y - _srcTouchPosition.y) > swipeResist || Mathf.Abs(_dstTouchPosition.x - _srcTouchPosition.x) > swipeResist)
        {
            _board.currentState = GameState.Wait;
            swipeAngle = Mathf.Atan2(_dstTouchPosition.y - _srcTouchPosition.y, _dstTouchPosition.x - _srcTouchPosition.x) * 180 / Mathf.PI;
            HandleSwapDotsDirection();
            _board.currentDot = this;
        }
        else {
            _board.currentState = GameState.Move;
        }
    }

    private void HandleSwapDotsDirection()
    {
        switch (swipeAngle)
        {
            case > -45 and <= 45 when column < _board.width - 1:
                // Right Swipe
                ExecuteSwapDots(Vector2.right);
                break;
            case > 45 and <= 135 when row < _board.height - 1:
                // Up Swipe
                ExecuteSwapDots(Vector2.up);
                break;
            case > 135 or <= -135 when column > 0:
                // Left Swipe
                ExecuteSwapDots(Vector2.left);
                break;
            case > -135 and <= -45 when row > 0:
                // Down Swipe
                ExecuteSwapDots(Vector2.down);
                break;
            default:
                _board.currentState = GameState.Move;
                break;
        }
    }
    
    private void ExecuteSwapDots(Vector2 direction) {
        otherDotGo = _board.AllDots[column + (int)direction.x, row + (int)direction.y];
        previousColumn = column;
        previousRow = row;
        if (_board.LockTiles[column, row] == null &&
            _board.LockTiles[column + (int)direction.x, row + (int)direction.y] == null)
        {
            if (otherDotGo != null)
            {
                otherDotGo.GetComponent<Dot>().column += -1 * (int)direction.x;
                otherDotGo.GetComponent<Dot>().row += -1 * (int)direction.y;
                column += (int)direction.x;
                row += (int)direction.y;
                StartCoroutine(CheckMoveCo());
            }
            else
            {
                _board.currentState = GameState.Move;
            }
        } 
        else
        {
            _board.currentState = GameState.Move;
        }
    }

    public void MakeRowBomb()
    {
        if (isColumnBomb || isColorBomb || isAdjacentBomb) return;
        isRowBomb = true;
        spriteRenderer.sprite = rowBombSprite;
    }

    public void MakeColumnBomb()
    {
        if (isRowBomb || isColorBomb || isAdjacentBomb) return;
        isColumnBomb = true;
        spriteRenderer.sprite = columnBombSprite;
    }

    public void MakeAdjacentBomb()
    {
        if (isColumnBomb || isRowBomb || isColorBomb) return;
        isAdjacentBomb = true;
        spriteRenderer.sprite = areaBombSprite;
    }
    
    public void MakeColorBomb()
    {
        if (isColumnBomb || isRowBomb || isAdjacentBomb) return;
        if (!Instantiate(colorBombPrefab, transform.position, Quaternion.identity, _board.dotsContainer.transform)
                .TryGetComponent(out Dot colorBomb)) return;
        colorBomb.column = column;
        colorBomb.row = row;
        colorBomb.targetX = targetX;
        colorBomb.targetY = targetY;
        colorBomb.previousColumn = previousColumn;
        colorBomb.previousRow = previousRow;
        colorBomb.isColorBomb = true;
        Destroy(_board.AllDots[column, row]);
        _board.AllDots[column, row] = colorBomb;
    }
}
