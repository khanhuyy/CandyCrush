using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using New;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


public class PotionBoard : MonoBehaviour
{
    public int width = 6;
    public int height = 8;
    
    [Header("Center tile offset")]
    public float spacingX;
    public float spacingY;

    public GameObject[] potionPrefabs;
    public GameObject backgroundTile;
    
    private Node[,] potionBoard;
    public GameObject potionBoardGO;

    public List<GameObject> potionsToDestroy = new();
    public GameObject potionParent;
    public GameObject areaDestroyEffect;
    public GameObject columnDestroyEffect;
    public GameObject rowDestroyEffect;
    [SerializeField] private Potion selectedPotion;

    [SerializeField] private bool isProcessingMove;
    [SerializeField] private List<Potion> potionsToRemove = new();
    
    // layout Array
    public ArrayLayout arrayLayout;
    
    public static PotionBoard Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitializeBoard();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider != null && hit.collider.gameObject.GetComponent<Potion>())
            {
                if (isProcessingMove)
                {
                    return;
                }

                Potion potion = hit.collider.gameObject.GetComponent<Potion>();
                SelectPotion(potion);
            }
        }
    }

    private void InitializeBoard()
    {
        DestroyPotions();
        potionBoard = new Node[width, height];
        spacingX = (float)(width - 1) / 2;
        spacingY = (float)(height - 1) / 2;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 position = new Vector2(x - spacingX, y - spacingY);
                if (arrayLayout.rows[y].row[x])
                {
                    potionBoard[x, y] = new Node(false, null);
                }
                else
                {
                    // background tile
                    GameObject tile =
                        Instantiate(backgroundTile, position, Quaternion.identity, transform);
                    if ((x + y) % 2 == 0)
                    {
                        tile.GetComponent<BackgroundTile>().UseFirstSprite();
                    }
                    else
                    {
                        tile.GetComponent<BackgroundTile>().UseSecondSprite();
                    }
                    
                    int randomIndex = Random.Range(0, potionPrefabs.Length);
                    GameObject potion = Instantiate(potionPrefabs[randomIndex], position, Quaternion.identity, transform);
                    potion.GetComponent<Potion>().SetIndicies(x, y);
                    potionBoard[x, y] = new Node(true, potion);
                    potionsToDestroy.Add(potion);
                }
            }
        }

        if (CheckBoard())
        {
            InitializeBoard();
        }
        else
        {
            // todo check deadlock
        }
    }

    private void DestroyPotions()
    {
        if (potionsToDestroy != null)
        {
            foreach (GameObject potion in potionsToDestroy)
            {
                Destroy(potion);
            }
            potionsToDestroy.Clear();
        }
    }

    public bool CheckBoard()
    {
        // Debug.Log("Checking Board");
        bool hasMatched = false;
        potionsToRemove.Clear();

        foreach (Node nodePotion in potionBoard)
        {
            if (nodePotion.potion)
            {
                nodePotion.potion.GetComponent<Potion>().isMatched = false;
            }
        }
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (potionBoard[x, y].isUsable)
                {
                    Potion potion = potionBoard[x, y].potion.GetComponent<Potion>();

                    if (potion && !potion.isMatched)
                    {
                        MatchResult matchedPotions = IsConnected(potion);
                        if (matchedPotions.connectedPotions.Count >= 3)
                        {
                            // complex matching    
                            MatchResult superMatchedPotions = SuperMatch(matchedPotions);
                            
                            potionsToRemove.AddRange(superMatchedPotions.connectedPotions);
                            foreach (Potion pot in superMatchedPotions.connectedPotions)
                            {
                                pot.isMatched = true;
                            }

                            hasMatched = true;
                        }
                    }
                }
            }
        }
        return hasMatched;
    }

    public IEnumerator ProcessTurnOnMatchedBoard(bool _substractMoves)
    {
        // make bomb
        MakeBomb();
        
        foreach (Potion potionToRemove in potionsToRemove)
        {
            potionToRemove.isMatched = false;
        }
        RemoveAndRefill(potionsToRemove);
        // GameManager.Instance.ProcessTurn(potionsToRemove.Count, false);
        yield return new WaitForSeconds(0.4f);

        if (CheckBoard())
        {
            StartCoroutine(ProcessTurnOnMatchedBoard(false));
        }
    }

    private void MakeBomb()
    {
        Dictionary<Vector2, int> dictBomb;
        foreach (Potion potion in potionsToRemove)
        {
            List<Potion> rowMatchPotions = new(){potion};
            List<Potion> columnMatchPotions = new(){potion};
            CheckBomb(potion, Vector2Int.up, columnMatchPotions);
            CheckBomb(potion, Vector2Int.down, columnMatchPotions);
            CheckBomb(potion, Vector2Int.right, rowMatchPotions);
            CheckBomb(potion, Vector2Int.left, rowMatchPotions);
            if (rowMatchPotions.Count >= 3)
            {
                if (columnMatchPotions.Count >= 3)
                {
                    potion.MakeAreaBomb();
                }
                else if (rowMatchPotions.Count >= 4)
                {
                    if (columnMatchPotions.Count >= 5)
                    {
                        potion.MakeColorBomb();
                    }
                    potion.MakeColumnBomb();
                }
            } else if (columnMatchPotions.Count >= 3)
            {
                if (rowMatchPotions.Count >= 3)
                {
                    potion.MakeAreaBomb();
                }
                else if (columnMatchPotions.Count >= 4)
                {
                    if (columnMatchPotions.Count >= 5)
                    {
                        potion.MakeColorBomb();
                    }
                    potion.MakeRowBomb();
                }
            }
        }
    }

    #region Cascadeing Potions

    // RemoveAndRefill
    private void RemoveAndRefill(List<Potion> _potionsToRemove)
    {
        // add bomb
        // foreach (Potion potion in _potionsToRemove)
        // {
        //     if (potion.IsBomb())
        //     {
        //         if (potion.CanDestroyBomb())
        //         {
        //             AddPotionDestroyByBomb(potion);
        //         }
        //         else
        //         {
        //             potion.ActivateBomb();
        //         }
        //     }
        // }
        
        foreach (Potion potion in _potionsToRemove)
        {
            if (!potion.IsBomb() || potion.IsBomb() && potion.CanDestroyBomb())
            {
                int _xIndex = potion.xIndex;
                int _yIndex = potion.yIndex;
                Destroy(potion.gameObject);
                potionBoard[_xIndex, _yIndex] = new Node(true, null);
            }
            else
            {
                // if (potion.CanDestroyBomb())
                // {
                //     AddPotionDestroyByBomb(potion);
                // }
                // else
                // {
                    potion.ActivateBomb();
                // }
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!potionBoard[x, y].potion)
                {
                    RefillPotion(x, y);    
                }
            }
        }
    }

    private void MatchRelatePotionBomb(Potion potion)
    {
        if (!potion.CanDestroyBomb())
            return;
        if (potion.isAreaBomb)
        {
            DestroyArea(potion);
        } 
        else if (potion.isColumnBomb)
        {
            DestroyColumn(potion);
        }
        else if (potion.isRowBomb)
        {
            DestroyRow(potion);
        }
    }

    private void DestroyRow(Potion potion)
    {
        Destroy(Instantiate(rowDestroyEffect, potion.transform.position, quaternion.identity), 2f);
        for (int x = 0; x < width; x++)
        {
            int _xIndex = x;
            int _yIndex = potion.yIndex;
            if (potionBoard[_xIndex, _yIndex])
            {
                var tempPotion = potionBoard[_xIndex, _yIndex].potion.GetComponent<Potion>();
                if (!potionsToRemove.Contains(tempPotion))
                {
                    potionsToRemove.Add(tempPotion);
                    if (tempPotion.IsBomb())
                    {
                        Debug.Log(x);
                        MatchRelatePotionBomb(tempPotion);
                    }
                }
            }
        }
    }

    private void DestroyColumn(Potion potion)
    {
        Destroy(Instantiate(rowDestroyEffect, potion.transform.position, quaternion.identity), 2f);
        for (int y = 0; y < height; y++)
        {
            int _xIndex = potion.xIndex;
            int _yIndex = y;
            if (potionBoard[_xIndex, _yIndex])
            {
                var tempPotion = potionBoard[_xIndex, _yIndex].potion.GetComponent<Potion>();
                if (!potionsToRemove.Contains(tempPotion))
                {
                    potionsToRemove.Add(tempPotion);
                    if (tempPotion.IsBomb())
                    {
                        MatchRelatePotionBomb(tempPotion);
                    }
                }
            }
        }
    }

    private void DestroyArea(Potion potion)
    {
        Destroy(Instantiate(rowDestroyEffect, potion.transform.position, quaternion.identity), 2f);
        for (int y = potion.yIndex - 1; y <= potion.yIndex + 1; y++)
        {
            for (int x = potion.xIndex - 1; x <= potion.xIndex + 1; x++)
            {
                if (x < 0 || x >= width || y < 0 || y >= height)
                {
                    continue;
                }

                if (potionBoard[x, y])
                {
                    int _xIndex = x;
                    int _yIndex = y;
                    var tempPotion = potionBoard[_xIndex, _yIndex].potion.GetComponent<Potion>();
                    if (!potionsToRemove.Contains(tempPotion))
                    {
                        potionsToRemove.Add(tempPotion);
                        if (tempPotion.IsBomb())
                        {
                            MatchRelatePotionBomb(tempPotion);
                        }
                    }
                }
            }
        }
    }

    // RefilPotions
    private void RefillPotion(int x, int y)
    {
        int yOffset = 1;
        while (y + yOffset < height && !potionBoard[x, y + yOffset].potion)
        {
            yOffset++;
        }

        if (y + yOffset < height && potionBoard[x, y + yOffset].potion != null)
        {
            Potion potionAbove = potionBoard[x, y + yOffset].potion.GetComponent<Potion>();
            Vector3 targetPos = new Vector3(x - spacingX, y - spacingY, potionAbove.transform.position.z);
            potionAbove.MoveToTarget(targetPos);
            
            potionAbove.SetIndicies(x, y);
            potionBoard[x, y] = potionBoard[x, y + yOffset];
            potionBoard[x, y + yOffset] = new Node(true, null);
        }

        if (y + yOffset == height)
        {
            SpawnPotionAtTop(x);
        }
    }

    private void SpawnPotionAtTop(int x)
    {
        int index = FindIndexOfLowestNull(x);
        int locationToMoveTo = 8 - index;
        int randomIndex = Random.Range(0, potionPrefabs.Length);
        GameObject newPotion = Instantiate(potionPrefabs[randomIndex],
            new Vector2(x - spacingX, height - spacingY), Quaternion.identity);
        newPotion.transform.SetParent(potionParent.transform);
        newPotion.GetComponent<Potion>().SetIndicies(x, index);
        potionBoard[x, index] = new Node(true, newPotion);
        Vector3 targetPosition = new Vector3(newPotion.transform.position.x,
            newPotion.transform.position.y - locationToMoveTo, newPotion.transform.position.z);
        newPotion.GetComponent<Potion>().MoveToTarget(targetPosition);
    }

    private int FindIndexOfLowestNull(int x)
    {
        int lowestNull = 99;
        for (int y = 7; y >= 0; y--)
        {
            if (potionBoard[x, y].potion == null)
            {
                lowestNull = y;
            }
        }
        return lowestNull;
    }

    #endregion
    
    private MatchResult SuperMatch(MatchResult _matchedPotions)
    {
        if (_matchedPotions.direction == MatchDirection.Horizontal ||
            _matchedPotions.direction == MatchDirection.LongHorizontal)
        {
            foreach (var pot in _matchedPotions.connectedPotions)
            {
                List<Potion> extraConnectedPotions = new();
                CheckDirection(pot, Vector2Int.up, extraConnectedPotions);
                CheckDirection(pot, Vector2Int.down, extraConnectedPotions);
                if (extraConnectedPotions.Count >= 2)
                {
                    Debug.Log("super horizontal match" + pot.PotionType);
                    pot.isAreaBomb = true;
                    // pot.
                    extraConnectedPotions.AddRange(_matchedPotions.connectedPotions);
                    return new MatchResult
                    {
                        connectedPotions = extraConnectedPotions,
                        direction = MatchDirection.Super
                    };
                }
            }

            if (_matchedPotions.direction == MatchDirection.LongHorizontal)
            {
                
            }

            return new MatchResult
            {
                connectedPotions = _matchedPotions.connectedPotions,
                direction = _matchedPotions.direction
            };
        }
        else if (_matchedPotions.direction == MatchDirection.Vertical ||
                 _matchedPotions.direction == MatchDirection.LongVertical)
        {
            foreach (var pot in _matchedPotions.connectedPotions)
            {
                List<Potion> extraConnectedPotions = new();
                CheckDirection(pot, Vector2Int.right, extraConnectedPotions);
                CheckDirection(pot, Vector2Int.left, extraConnectedPotions);
                if (extraConnectedPotions.Count >= 2)
                {
                    Debug.Log("super verical match");
                    extraConnectedPotions.AddRange(_matchedPotions.connectedPotions);
                    return new MatchResult
                    {
                        connectedPotions = extraConnectedPotions,
                        direction = MatchDirection.Super
                    };
                }
            }

            return new MatchResult
            {
                connectedPotions = _matchedPotions.connectedPotions,
                direction = _matchedPotions.direction
            };
        }

        return null;
    }


    // IsConnected todo refactor
    private MatchResult IsConnected(Potion potion)
    {
        List<Potion> connectedPotions = new();
        PotionType potionType = potion.PotionType;
        connectedPotions.Add(potion);
        //check right
        CheckDirection(potion, Vector2Int.right, connectedPotions);
        // check left
        CheckDirection(potion, Vector2Int.left, connectedPotions);

        // have we made a 3 match? ( horizontal match)
        if (connectedPotions.Count == 3)
        {
            foreach (Potion connectedPotion in connectedPotions)
            {
                if (connectedPotion.IsBomb())
                {
                    MatchRelatePotionBomb(connectedPotion);
                }
            }
            Debug.Log("Horizon Match");
            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.Horizontal,
            };
        }
        else if (connectedPotions.Count > 3)
        {
            foreach (Potion connectedPotion in connectedPotions)
            {
                if (connectedPotion.IsBomb())
                {
                    MatchRelatePotionBomb(connectedPotion);
                }
            }
            Debug.Log("Long horizon Match");
            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.LongHorizontal,
            };
        }
        // clear out the connected potions
        connectedPotions.Clear();

        connectedPotions.Add(potion);
        CheckDirection(potion, Vector2Int.up, connectedPotions);
        CheckDirection(potion, Vector2Int.down, connectedPotions);
        if (connectedPotions.Count == 3)
        {
            foreach (Potion connectedPotion in connectedPotions)
            {
                if (connectedPotion.IsBomb())
                {
                    MatchRelatePotionBomb(connectedPotion);
                }
            }
            Debug.Log("Vertical Match");
            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.Vertical,
            };
        }
        else if (connectedPotions.Count > 3)
        {
            foreach (Potion connectedPotion in connectedPotions)
            {
                if (connectedPotion.IsBomb())
                {
                    MatchRelatePotionBomb(connectedPotion);
                }
            }
            Debug.Log("Long vertical Match");
            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.LongVertical,
            };
        }
        
        return new MatchResult
        {
            connectedPotions = connectedPotions,
            direction = MatchDirection.None,
        };
        // checking for more than 3 ( long vertail match
    }

    // Check Diredtion
    void CheckDirection(Potion pot, Vector2Int direction, List<Potion> connectedPotions)
    {
        PotionType potionType = pot.PotionType;
        int x = pot.xIndex + direction.x;
        int y = pot.yIndex + direction.y;

        // check that we're within the boudaries of the board
        while (x >= 0 && x < width && y >= 0 && y < height)
        {
            if (potionBoard[x, y].isUsable)
            {
                Potion neighbourPotion = potionBoard[x, y].potion.GetComponent<Potion>();
                
                // does our potion tpye match is must also not be matched
                if (!neighbourPotion.isMatched && neighbourPotion.PotionType == potionType)
                {
                    connectedPotions.Add(neighbourPotion);
                    x += direction.x;
                    y += direction.y;
                    // match bomb
                }
                else
                {
                    // todo damage blocker here
                    break;
                }
            }
            else
            {
                break;
            }
        }
    }

    void CheckBomb(Potion pot, Vector2Int direction, List<Potion> connectedPotions)
    {
        PotionType potionType = pot.PotionType;
        int x = pot.xIndex + direction.x;
        int y = pot.yIndex + direction.y;

        // check that we're within the boudaries of the board
        while (x >= 0 && x < width && y >= 0 && y < height)
        {
            if (potionBoard[x, y].isUsable)
            {
                Potion neighbourPotion = potionBoard[x, y].potion.GetComponent<Potion>();
                
                // does our potion tpye match is must also not be matched
                if (!neighbourPotion.IsBomb() && neighbourPotion.PotionType == potionType)
                {
                    connectedPotions.Add(neighbourPotion);
                    x += direction.x;
                    y += direction.y;
                }
                else
                {
                    // todo damage blocker here
                    break;
                }
            }
            else
            {
                break;
            }
        }
    }
    
    #region Swapping Potions

    // Select potion
    public void SelectPotion(Potion _potion)
    {
        if (!selectedPotion)
        {
            selectedPotion = _potion;
        }
        else if (selectedPotion == _potion)
        {
            selectedPotion = null;
        }
        else if (selectedPotion != _potion)
        {
            SwapPotion(selectedPotion, _potion);
            selectedPotion = null;
        }
    }


    // swap potion - logic
    private void SwapPotion(Potion _currentPotion, Potion _targetPotion)
    {
        if (!IsAdjacent(_currentPotion, _targetPotion))
        {
            return;
        }
        DoSwap(_currentPotion, _targetPotion);
        isProcessingMove = true;
        StartCoroutine((ProcessMatches(_currentPotion, _targetPotion)));
    }

    // do swap
    private void DoSwap(Potion _currentPotion, Potion _targetPotion)
    {
        // todo refactor
        GameObject temp = potionBoard[_currentPotion.xIndex, _currentPotion.yIndex].potion;
        
        potionBoard[_currentPotion.xIndex, _currentPotion.yIndex].potion =
            potionBoard[_targetPotion.xIndex, _targetPotion.yIndex].potion;
        potionBoard[_targetPotion.xIndex, _targetPotion.yIndex].potion = temp;
        
        // update indicies
        int tempXIndex = _currentPotion.xIndex;
        int tempYIndex = _currentPotion.yIndex;
        _currentPotion.xIndex = _targetPotion.xIndex;
        _currentPotion.yIndex = _targetPotion.yIndex;
        _targetPotion.xIndex = tempXIndex;
        _targetPotion.yIndex = tempYIndex;
        _currentPotion.MoveToTarget(potionBoard[_targetPotion.xIndex, _targetPotion.yIndex].potion.transform.position);
        _targetPotion.MoveToTarget(potionBoard[_currentPotion.xIndex, _currentPotion.yIndex].potion.transform.position);
        
    }

    private IEnumerator ProcessMatches(Potion _currentPotion, Potion _targetPotion)
    {
        yield return new WaitForSeconds(0.4f);
        if (CheckBoard())
        {
            StartCoroutine(ProcessTurnOnMatchedBoard(true));
        }
        else
        {
            DoSwap(_currentPotion, _targetPotion);
        }

        isProcessingMove = false;
    }
    
    // is adjacent
    private bool IsAdjacent(Potion _currentPotion, Potion _targetPotion)
    {
        return Mathf.Abs(_currentPotion.xIndex - _targetPotion.xIndex) +
            Mathf.Abs(_currentPotion.yIndex - _targetPotion.yIndex) == 1;
    }
    
    // processMatches
    
    #endregion
    
}

public class MatchResult
{
    public List<Potion> connectedPotions;
    public MatchDirection direction;
}

public enum MatchDirection
{
    Vertical,
    Horizontal,
    LongVertical,
    LongHorizontal,
    None,
    Super
}