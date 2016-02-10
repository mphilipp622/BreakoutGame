using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Master : MonoBehaviour {

	[SerializeField]
	private Transform[] bricks;
	public List<Transform> spawnedBricks = new List<Transform>();
	private Rect[] spawnGrid;
//	private Rect spawnArea;
	public Texture2D texture;
	private GUIStyle style = new GUIStyle();
	private Vector2 randomSpawn, organizedSpawn;
	public int numberOfBricks;
	private Rect lastRect;
	public float rowSpace, columnSpace;
	private SpriteRenderer brickRender;

	[SerializeField]
	float spawnTime;
	float nextSpawn;

	public static Master instance = null; //Singleton
	public bool gameOver = false;
	public Transform paddle;

    
    public List<Renderer> brickRenderers = new List<Renderer>(); //used for color outlines
    public List<int> brickInt = new List<int>(); //also used for color outlines
    //public List<BrickData> brickData = new List<BrickData>();
    public bool newSpawn = false;
    int index = 0;


	public Material[] outlineMats;

	//Power Bar Variables
	public float energy, maxEnergy = 100.0f;
	float energyRegenSpeed, energyRegenTime, energyRegenAmount = 1.0f;
	private Slider powerBar;

	//Chaos Ball Variables
	float chaosCost;
	int numberOfBalls = 1;
	[SerializeField]
	int numToSpawn = 4;
	public Rigidbody2D[] spawnedBalls;
	public bool chaosBall = false;
	private Rigidbody2D ball;
	[SerializeField]
	Rigidbody2D spawnBall;
	float ballSpeed;
	public bool ballInPlay = false;

	//snipe skill variables
	Powers activeSkill = new Powers();
	public bool isSniping = false;
	List<GameObject> snipedBricks = new List<GameObject>();

	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
	}

	void Start () {
		powerBar = GameObject.Find("PowerBar").GetComponent<Slider>();
		ball = GameObject.Find("Ball").GetComponent<Rigidbody2D>();
		ballSpeed = ball.GetComponent<BallScript>().ballSpeedMultiplier;
		spawnedBalls = new Rigidbody2D[numToSpawn];
		paddle = GameObject.Find("Paddle").transform;
		spawnedBricks = new List<Transform>();
		brickRender = bricks [0].GetComponent<SpriteRenderer> ();
		spawnGrid = new Rect[numberOfBricks];
        energy = 100.0f;
		energyRegenSpeed = 1.0f;
		energyRegenTime = Time.timeSinceLevelLoad + energyRegenSpeed;
		chaosCost = 15.0f;
		powerBar.maxValue = maxEnergy;
		texture = new Texture2D((int) bricks[0].GetComponent<SpriteRenderer>().sprite.textureRect.width, (int) bricks[0].GetComponent<SpriteRenderer>().sprite.textureRect.height);
		//texture = new Texture2D((int)bricks[0].GetComponent<SpriteRenderer>().sprite.textureRect.width, (int)bricks[0].GetComponent<SpriteRenderer>().sprite.textureRect.height);
		//spawnArea = new Rect (Camera.main.pixelRect.position, new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight/2));

		SpawnNewBricks();

		//ParentBricks();
	}
	
	void Update()
	{
		if(Time.timeSinceLevelLoad >= nextSpawn)
		{
			MoveBricks();
			SpawnNewBricks();
		}

		if(gameOver)
			GameOver();

		if(Input.GetKeyDown(KeyCode.Mouse1) && ballInPlay && energy >= chaosCost)
			ChaosBall();

		/*
		 * switch (selectedSkill)
		 * case chaosBall: 
		 * 	if(chaosBall.cost >= energy)
		 * 		ChaosBall();
		 * 		break;
		 * case snipeBall:
		 * 	if(snipeBall.cost >= energy)
		 * 		SnipeBall();
		 * 		break
		 * etc.
		 * etc.
		 */

		if(energy < maxEnergy && Time.timeSinceLevelLoad >= energyRegenTime)
		{
			energy += energyRegenAmount;
			energyRegenTime = Time.timeSinceLevelLoad + energyRegenSpeed;
		}

		if(energy > maxEnergy)
			energy = maxEnergy;

		if(Input.GetKeyDown(KeyCode.Space) && ballInPlay && !isSniping)
			SniperPower();
		else if(Input.GetKeyDown(KeyCode.Space) && ballInPlay && isSniping)
		{
			isSniping = false;
			Time.timeScale = 1;
		}

	}

	void OnGUI()
	{
		powerBar.value = energy;
		//if(powerBar.value == 0)
		//	powerBar.fillRect.gameObject.SetActive(false);
		/*for(int i = 0; i < brickRenderers.Count; i++)
		{
			switch(spawnedBricks[i].GetComponent<BrickScript>().GetHP())
			{
			case 1:
				for ( int a = 0; a < bricks[0].GetComponent<SpriteRenderer>().bounds.size.x; a++)
				{
					for ( int b = 0; b < bricks[0].GetComponent<SpriteRenderer>().bounds.size.y; b++)
					{
						Debug.Log((int) spawnedBricks[i].GetComponent<SpriteRenderer>().bounds.min.x + a);
						Debug.Log((int) spawnedBricks[i].GetComponent<SpriteRenderer>().bounds.min.y + b);
						texture.SetPixel((int) spawnedBricks[i].GetComponent<SpriteRenderer>().bounds.min.x + a, 
										 (int) spawnedBricks[i].GetComponent<SpriteRenderer>().bounds.min.y + b, Color.green);
					}
				}
				texture.Apply();
				style.normal.background = texture;
				GUI.Box(spawnGrid[i], texture, style);
				break;
			case 2:
				for ( int a = 0; a < bricks[0].GetComponent<SpriteRenderer>().bounds.size.x; a++)
				{
					for ( int b = 0; b < bricks[0].GetComponent<SpriteRenderer>().bounds.size.y; b++)
					{
						texture.SetPixel(a, b, Color.yellow);
					}
				}
				texture.Apply();
				style.normal.background = texture;
				GUI.Box(spawnGrid[i], texture, style);
				break;
			case 3:
				for ( int a = 0; a < bricks[0].GetComponent<SpriteRenderer>().bounds.size.x; a++)
				{
					for ( int b = 0; b < bricks[0].GetComponent<SpriteRenderer>().bounds.size.y; b++)
					{
						texture.SetPixel(a, b, Color.red);
					}
				}
				texture.Apply();
				style.normal.background = texture;
				GUI.Box(spawnGrid[i], texture, style);
				break;
			}

		}*/
		//texture.SetPixel(512, 512, Color.green);

			
		//texture.Apply();
	//	style.normal.background = texture;
		//GUI.Box(new Rect(0, 0, 512, 512), texture, style);
		/*for(int i = 0; i < spawnedBricks.Count; i++)
		{
			//GUI.Box(spawnGrid[i], texture);
			DrawQuad(new Rect(spawnedBricks[i].position.x, spawnedBricks[i].position.y, brickRenderers[i].bounds.size.x, brickRenderers[i].bounds.size.y), Color.green);
		}
*/
		//for (int c = 0; c < spawnGrid.Length; c++) {
		//	GUI.Box (spawnGrid [c], texture, style);
			//DrawQuad(spawnGrid[c], Color.green);
		//}
	}

	//Move bricks down by a row
	void MoveBricks()
	{
		for(int i = 0; i < spawnedBricks.Count; i++)
		{
			spawnedBricks[i].position = new Vector2(spawnedBricks[i].position.x, spawnedBricks[i].position.y - brickRender.bounds.size.y);
		}

	}

	void SpawnNewBricks()
	{
		Rect spawnRect = new Rect(new Vector2((Camera.main.orthographicSize * Screen.width/Screen.height)*-1 + bricks[0].GetComponent<SpriteRenderer>().bounds.extents.x, 
			Camera.main.orthographicSize - bricks[0].GetComponent<SpriteRenderer>().bounds.extents.y),
			new Vector2(bricks[0].GetComponent<SpriteRenderer>().bounds.size.x,
				bricks[0].GetComponent<SpriteRenderer>().bounds.size.y));

		lastRect.position = new Vector2 (spawnRect.position.x - brickRender.bounds.size.x, spawnRect.position.y - rowSpace);

		for (int b = 0; b < spawnGrid.Length; b++) 
		{
			if(lastRect.position.x + brickRender.bounds.size.x > Camera.main.ScreenToWorldPoint(Camera.main.pixelRect.max).x)
			{
				spawnGrid[b] = new Rect(new Vector2(spawnRect.position.x + columnSpace, lastRect.position.y - brickRender.bounds.size.y - rowSpace), 
					new Vector2(brickRender.bounds.size.x, brickRender.bounds.size.y));
				//Debug.Log("Max Reached. Make New Row");
			}
			else if(lastRect.position.x + brickRender.bounds.size.x < Camera.main.ScreenToWorldPoint(Camera.main.pixelRect.max).x)
			{
				spawnGrid[b] = new Rect(new Vector2(lastRect.position.x + brickRender.bounds.size.x + columnSpace, lastRect.position.y), 
					new Vector2(brickRender.bounds.size.x, brickRender.bounds.size.y));
				//Debug.Log ("Not Hitting Max");
			}
			lastRect = spawnGrid[b];
		}
		//Debug.Log (spawnRect.position);
		//Debug.Log (spawnRect.max);

		//Instanttiate New Bricks
		for(int i = 0; i < numberOfBricks; i++)
		{
            //int outlineColor = UnityEngine.Random.Range(1, 3);
            //int hp = UnityEngine.Random.Range(1, 4);
			//Transform newBrick = Instantiate(bricks[UnityEngine.Random.Range (0, bricks.Length)], randomSpawn, Quaternion.identity) as Transform;
            
			spawnedBricks.Add(Instantiate(bricks[UnityEngine.Random.Range (0, bricks.Length)], spawnGrid[i].position, Quaternion.identity) as Transform);
			//spawnedBricks[i].gameObject.layer = LayerMask.NameToLayer("Default");
          //  spawnedBricks[index].GetComponent<BrickScript>().SetIndex(index);
           // brickData.Add(new BrickData(outlineColor, Instantiate(bricks[UnityEngine.Random.Range(0, bricks.Length)], spawnGrid[i].position, Quaternion.identity) as Transform));
          //  brickRenderers.Add(spawnedBricks[index].GetComponent<Renderer>());
           // spawnedBricks[index].GetComponent<BrickScript>().SetHP(hp);
           // brickInt.Add(spawnedBricks[index].GetComponent<BrickScript>().GetHP());


            index = spawnedBricks.Count;
			/*if(spawnedBricks[i].position.x < Camera.main.pixelRect.xMax){
				//Debug.Log("True");
				spawnRect = new Rect (new Vector2(newBrick.GetComponent<SpriteRenderer>().bounds.max.x + newBrick.GetComponent<SpriteRenderer>().bounds.extents.x,
			                                 	  newBrick.GetComponent<SpriteRenderer>().bounds.max.y - newBrick.GetComponent<SpriteRenderer>().bounds.extents.y),
			                     	 			  newBrick.GetComponent<SpriteRenderer>().bounds.size);
			}*/
			/*else if(spawnedBricks[i].position.x > Camera.main.pixelRect.xMax){
			Debug.Log("Not True");
			spawnRect = new Rect (new Vector2((Camera.main.orthographicSize * Screen.width/Screen.height)*-1 + bricks[0].GetComponent<SpriteRenderer>().bounds.extents.x, 
				Camera.main.orthographicSize - (bricks[0].GetComponent<SpriteRenderer>().bounds.extents.y * 2)),
				new Vector2(bricks[0].GetComponent<SpriteRenderer>().bounds.size.x,
					bricks[0].GetComponent<SpriteRenderer>().bounds.size.y));
		}*/
		}
		
		nextSpawn = Time.timeSinceLevelLoad + spawnTime;
        newSpawn = true;
		
		//Parent all the new bricks to the Master Object
		//for (int i = 0; i < spawnedBricks.Count; i++)
		//{
		//	if(spawnedBricks[i].parent == null)
		//		spawnedBricks[i].parent = transform;
		//}

		/*for(int i = 0; i < numberOfBricks; i++)
		{
			randomSpawn = new Vector2(UnityEngine.Random.Range(Camera.main.orthographicSize * Screen.width/ Screen.height, 
				(Camera.main.orthographicSize * Screen.width/Screen.height) * -1),
				UnityEngine.Random.Range (0, Camera.main.orthographicSize));
			organizedSpawn = new Vector2(spawnRect.x, spawnRect.y);
			//Transform newBrick = Instantiate(bricks[UnityEngine.Random.Range (0, bricks.Length)], randomSpawn, Quaternion.identity) as Transform;
			Transform newBrick = Instantiate(bricks[UnityEngine.Random.Range (0, bricks.Length)], spawnGrid[i].position, Quaternion.identity) as Transform;
			newBrick.SetParent(gameObject.transform);

			spawnedBricks.Add(newBrick);
			/*if(spawnedBricks[i].position.x < Camera.main.pixelRect.xMax){
					//Debug.Log("True");
					spawnRect = new Rect (new Vector2(newBrick.GetComponent<SpriteRenderer>().bounds.max.x + newBrick.GetComponent<SpriteRenderer>().bounds.extents.x,
				                                 	  newBrick.GetComponent<SpriteRenderer>().bounds.max.y - newBrick.GetComponent<SpriteRenderer>().bounds.extents.y),
				                     	 			  newBrick.GetComponent<SpriteRenderer>().bounds.size);
				}*/
			/*else if(spawnedBricks[i].position.x > Camera.main.pixelRect.xMax){
			Debug.Log("Not True");
			spawnRect = new Rect (new Vector2((Camera.main.orthographicSize * Screen.width/Screen.height)*-1 + bricks[0].GetComponent<SpriteRenderer>().bounds.extents.x, 
				Camera.main.orthographicSize - (bricks[0].GetComponent<SpriteRenderer>().bounds.extents.y * 2)),
				new Vector2(bricks[0].GetComponent<SpriteRenderer>().bounds.size.x,
					bricks[0].GetComponent<SpriteRenderer>().bounds.size.y));
		}
		}*/
		//texture.wrapMode = TextureWrapMode.Repeat;
		//texture.Apply ();
		//style = new GUIStyle ();
		//style.normal.background = texture;

		/*for (int i = 0; i < spawnedBricks.Count; i++){
					SpriteRenderer currentRenderer = spawnedBricks[i].GetComponent<SpriteRenderer>();
					for(int a = 0; a < spawnedBricks.Count; a++){
						if(spawnedBricks[i] == spawnedBricks[a]){
							continue;
						}
						SpriteRenderer subRenderer = spawnedBricks[a].GetComponent<SpriteRenderer>();
						if(currentRenderer.bounds.Intersects(subRenderer.bounds)){
							Debug.Log("spawnedBricks " + i + " Intersects " + "spawnedBricks " + a);
							spawnedBricks[i].position = new Vector2(UnityEngine.Random.Range(Camera.main.orthographicSize * Screen.width/ Screen.height, 
							                                                     (Camera.main.orthographicSize * Screen.width/Screen.height) * -1),
							                                        UnityEngine.Random.Range (Camera.main.orthographicSize/2, Camera.main.orthographicSize));
							a = 0;
						}
					}
				}*/
	}

	void ChaosBall()
	{
		energy -= chaosCost;
		for(int i = 0; i < numToSpawn; i++)
		{
			spawnedBalls[i] = (Rigidbody2D) Instantiate (spawnBall, ball.position, Quaternion.identity);
			spawnedBalls[i].velocity = new Vector2(UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10));
			//spawnedBalls[i].AddForce (new Vector2(UnityEngine.Random.Range(-10 * ballSpeed, 10 * ballSpeed), UnityEngine.Random.Range(-10 * ballSpeed, 10 * ballSpeed)), ForceMode2D.Impulse);
			spawnedBalls[i].AddForce(spawnedBalls[i].velocity, ForceMode2D.Impulse);
			
		}
	}

void SniperPower()
{
	isSniping = true;
	energy -= activeSkill.GetSkillCost();
	Time.timeScale = 0;


}

public void SetBricksToSnipe(GameObject brick)
{
	activeSkill.GetSkillLevel();
	if(!snipedBricks.Contains(brick) && snipedBricks.Count < activeSkill.GetSkillLevel())
		snipedBricks.Add(brick);

	Debug.Log(snipedBricks.Count);
}

    //Remove Bricks from List when they're destroyed
    public void RemoveBrick(Transform brickToDestroy)
	{
       // int index = spawnedBricks.FindIndex(brickToDestroy);
		spawnedBricks.Remove(brickToDestroy);
       // brickRenderers.Remove(brickToDestroy.GetComponent<Renderer>());
       // brickRenderers.Remove(brickToDestroyRenderer);
       // brickInt.RemoveAt(brickIndex);
	}

   /* public void RemoveBrick(int brickIndex)
    {
        // int index = spawnedBricks.FindIndex(brickToDestroy);
        spawnedBricks.RemoveAt(brickIndex);
        brickRenderers.RemoveAt(brickIndex);
        // brickRenderers.Remove(brickToDestroyRenderer);
        brickInt.RemoveAt(brickIndex);
    }*/

    void GameOver()
	{
		Time.timeScale = 0;
		Camera.main.GetComponent<Bloom>().bloomIntensity = 0.7f;
	}

    public void GetRenderer()
    {
        foreach (Transform brick in spawnedBricks)
        {
            brickRenderers.Add(brick.GetComponent<Renderer>());
        }
    }

	public void SaveData(String skillName)
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

		Powers data = new Powers();
		
		switch(skillName)
		{
			case "ChaosBall":
				data.SetSkillCost(10);
				data.SetSkillLevel(1);
				data.SetSprite(Resources.Load("ChaosBallIcon") as Sprite);
				break;
		}

		bf.Serialize(file, data);
		file.Close();
	}

	public void LoadData()
	{
		if(File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
		{
			BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
			Powers data = (Powers) bf.Deserialize(file);
			file.Close();

			data.GetSkillCost();
			data.GetSkillLevel();
			data.GetSprite();
		}
	}
}

[Serializable]
class Powers {

	//Need to think this through.
	float skillCost;
	int skillLevel;
	Sprite skillIcon;
	//skillIcon = Resources.Load ("ChaosBallIcon");
	//int numberOfBalls = skillLevel + 1?
	public float GetSkillCost()
	{
		return skillCost;
	}

	public void SetSkillCost(int cost)
	{
		skillCost = cost;
	}

	public int GetSkillLevel()
	{
		return skillLevel;
	}

	public void SetSkillLevel(int level)
	{
		skillLevel = level;
	}

	public Sprite GetSprite()
	{
		return skillIcon;
	}

	public void SetSprite(Sprite spriteToUse)
	{
		skillIcon = spriteToUse;
	}

	//public Powers (string name)
	//{
		//switch(name)
		//{
		//case "ChaosBall":
			
		//	break;
		//}
	//}
}
