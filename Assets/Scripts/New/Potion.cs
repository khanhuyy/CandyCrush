using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public enum PotionType
{
    Blue,
    Green,
    Orange,
    Purple,
    Red,
    Yellow,
}

public class Potion : MonoBehaviour
{
    public PotionType PotionType;
    public int xIndex; // column
    public int yIndex; // row

    public bool isMatched;
    private Vector2 currentPos;
    private Vector2 targetPos;

    public bool isMoving;

    [FormerlySerializedAs("isBombAtive")] [FormerlySerializedAs("makeBombYet")] [Header("Bomb asset")] 
    public bool isBombActive;
    public bool isColumnBomb;
    public bool isRowBomb;
    public bool isAreaBomb;
    public bool isColorBomb;
    [SerializeField] private GameObject rowBomArrow;
    [SerializeField] private GameObject columnBomArrow;
    [SerializeField] private Sprite areaBombSprite;
    [SerializeField] private Sprite colorBombSprite;
    
    public Potion(int _x, int _y)
    {
        xIndex = _x;
        yIndex = _y;
    }

    public void SetIndicies(int _x, int _y)
    {
        xIndex = _x;
        yIndex = _y;
    }
    
    // Move Target
    public void MoveToTarget(Vector2 _targetPos)
    {
        StartCoroutine(MoveCoroutine(_targetPos));
    }

    private IEnumerator MoveCoroutine(Vector2 _targetPos)
    {
        isMoving = true;
        float duration = 0.2f;
        Vector2 startPosition = transform.position;
        float elaspedTime = 0f;
        while (elaspedTime < duration)
        {
            float t = elaspedTime / duration;
            transform.position = Vector2.Lerp(startPosition, _targetPos, t);
            elaspedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = _targetPos;
        isMoving = false;
    }

    #region Bomb Manager
    public bool IsBomb()
    {
        return isColumnBomb || isRowBomb || isAreaBomb || isColorBomb;
    }
    
    public bool CanDestroyBomb()
    {
        return (isColumnBomb || isRowBomb || isAreaBomb || isColorBomb) && isBombActive;
    }

    public void ActivateBomb()
    {
        isBombActive = true;
    }
    
    public void MakeColumnBomb()
    {
        Debug.Log("Make column Bomb");
        isColumnBomb = true;
        columnBomArrow.SetActive(true);
    }
    
    public void MakeRowBomb()
    {
        Debug.Log("Make row Bomb");
        isRowBomb = true;
        rowBomArrow.SetActive(true);
    }
    
    public void MakeAreaBomb()
    {
        Debug.Log("Make area Bomb");
        isColumnBomb = true;
        gameObject.GetComponent<SpriteRenderer>().sprite = areaBombSprite;
    }
    
    public void MakeColorBomb()
    {
        Debug.Log("Make color Bomb");
        isColorBomb = true;
        gameObject.GetComponent<SpriteRenderer>().sprite = colorBombSprite;
    }
    #endregion
}
