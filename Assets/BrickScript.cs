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
	Material brickMat;
	LineRenderer outline;

    void Start()
    {
		outline = GetComponent<LineRenderer>();
		outline.SetWidth(GetComponent<SpriteRenderer>().bounds.extents.y * 2, GetComponent<SpriteRenderer>().bounds.extents.y * 2);
		outline.SetPosition(0, new Vector3(GetComponent<SpriteRenderer>().bounds.min.x, transform.position.y, -1.0f));
		outline.SetPosition(1, new Vector3(GetComponent<SpriteRenderer>().bounds.max.x, transform.position.y, -1.0f));
		brickMat = outline.material;
        hp = Random.Range(1, 4);
    }

    void Update()
    {
		outline.SetPosition(0, new Vector3(GetComponent<SpriteRenderer>().bounds.min.x, transform.position.y, -1.0f));
		outline.SetPosition(1, new Vector3(GetComponent<SpriteRenderer>().bounds.max.x, transform.position.y, -1.0f));

        switch (hp)
        {
            case 0:
				Master.instance.RemoveBrick(transform);
                Destroy(gameObject);
                break;
            case 1:
                outlineInt = 1;
				outline.material = Master.instance.outlineMats[0];
                break;
            case 2:
                outlineInt = 2;
				outline.material = Master.instance.outlineMats[1];
                break;
            case 3:
                outlineInt = 3;
				outline.material = Master.instance.outlineMats[2];
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
