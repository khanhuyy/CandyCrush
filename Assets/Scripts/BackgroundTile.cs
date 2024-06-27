using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public int hitPoints;
    private SpriteRenderer sprite;
    private GoalManager goalManager;
    public GameObject[] dots;
    
    void Start()
    {
        // goalManager = FindObjectOfType<GoalManager>();
        // sprite = GetComponent<SpriteRenderer>();
        Initalize();
    }
    
    void Update()
    {
        // if(hitPoints <= 0)
        // {
        //     if(goalManager != null)
        //     {
        //         goalManager.CompareGoal(this.gameObject.tag);
        //         goalManager.UpdateGoals();
        //     }
        //     Destroy(this.gameObject);
        // }    
    }

    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        MakeLighter();
    }

    void MakeLighter()
    {
        Vector4 color = sprite.color;
        float newAlpha = color.w * 0.5f;
        sprite.color = new Vector4(color.x, color.y, color.z, newAlpha);
    }

    void Initalize() {
        // int dotToUse = Random.Range(0, dots.Length);
        // GameObject dot = Instantiate(dots[dotToUse], transform.position, Quaternion.identity);
        // dot.transform.parent = this.transform;
        // dot.name = this.gameObject.name;
    }
}
