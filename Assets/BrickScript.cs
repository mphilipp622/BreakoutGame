using UnityEngine;
using System.Collections;

public class BrickScript : MonoBehaviour
{

    [SerializeField]
    private int hp;
    [SerializeField]
    private int outlineInt;
    [SerializeField]
    private int index;

    void Start()
    {
        //hp = Random.Range(1, 3);
    }

    void Update()
    {

        switch (hp)
        {
            case 0:
                Master.instance.RemoveBrick(index);
                Destroy(gameObject);
                break;
            case 1:
                outlineInt = 1;
                break;
            case 2:
                outlineInt = 2;
                break;
            case 3:
                outlineInt = 3;
                break;
        }
        
        if (transform.position.y <= Master.instance.paddle.position.y)
            Master.instance.gameOver = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        hp--;
    }

    public int GetHP()
    {
        return hp;
    }

    public void SetHP(int hitPoints)
    {
        hp = hitPoints;
    }

    public void SetOutline(int randomColor)
    {
        outlineInt = randomColor;
    }

    public void SetIndex(int newIndex)
    {
        index = newIndex;
    }
}
