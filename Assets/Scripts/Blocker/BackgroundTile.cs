using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public int hitPoints;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private GoalManager goalManager;
    public GameObject[] dots;
    public Sprite[] backgroundSprites;
    
    void Start()
    {
        goalManager = FindObjectOfType<GoalManager>();
        Initalize();
    }
    
    void Update()
    {
        if(hitPoints <= 0)
        {
            if(goalManager)
            {
                goalManager.CompareGoal(this.gameObject.tag, false, false);
                goalManager.UpdateGoals();
            }
            Destroy(this.gameObject);
        }    
    }

    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        // MakeLighter();
    }

    void MakeLighter()
    {
        Vector4 color = spriteRenderer.color;
        float newAlpha = color.w * 0.5f;
        spriteRenderer.color = new Vector4(color.x, color.y, color.z, newAlpha);
    }

    void Initalize() {
        // int dotToUse = Random.Range(0, dots.Length);
        // GameObject dot = Instantiate(dots[dotToUse], transform.position, Quaternion.identity);
        // dot.transform.parent = this.transform;
        // dot.name = this.gameObject.name;
    }

    public void UseFirstSprite()
    {
        spriteRenderer.sprite = backgroundSprites[0];
    }
    
    public void UseSecondSprite()
    {
        spriteRenderer.sprite = backgroundSprites[1];
    }
}
