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
    public bool newSpawn = false;
    int index = 0;

	public Material[] outlineMats;

	//Hotkey Power Variables
	int currentSelection = 0;
	Powers[] powersToUse = new Powers[] {new Powers("Chaos"), new Powers("Sniper"), new Powers("WreckingBall"), new Powers("Stretch")};

	//Power Bar Variables
	public float energy, maxEnergy = 100.0f;
	float energyRegenSpeed, energyRegenTime, energyRegenAmount = 2.0f;
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
	float snipeTime = 0f;
	Powers activeSkill = new Powers();
	public bool isSniping = false;
	List<GameObject> snipedBricks = new List<GameObject>();
	[SerializeField]
	GameObject energyObject;

	//Wrecking Ball variables
	float wreckingTime = 0f;
	public bool isWrecking = false;
	public int wreckingDamage = 0;
	public float wreckingStacks = 0f;

	//Stretch Paddle variables
	public bool isStretched = false;
	float stretchTime;
	Vector3 originalScale;
	Animator paddleAnimator;

	//Score and Leveling Variables
	int score, milestone = 5000, milestonesReached = 0; //Used for Leveling. When Score reaches milestone, milestonesReached increments and skills level up accordingly.

	int MAX_LEVEL = 10;
	int stacks = 1;

	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Confined;
	}

	void Start () 
	{
		score = 0;
		powerBar = GameObject.Find("PowerBar").GetComponent<Slider>();
		ball = GameObject.Find("Ball").GetComponent<Rigidbody2D>();
		ballSpeed = ball.GetComponent<BallScript>().ballSpeedMultiplier;
		spawnedBalls = new Rigidbody2D[numToSpawn];
		paddle = GameObject.Find("Paddle").transform;
		paddleAnimator = paddle.GetComponent<Animator>();
		originalScale = paddle.localScale;
		spawnedBricks = new List<Transform>();
		brickRender = bricks [0].GetComponent<SpriteRenderer> ();
		spawnGrid = new Rect[numberOfBricks];
        energy = 100.0f;
		energyRegenSpeed = 1.0f;
		energyRegenTime = Time.timeSinceLevelLoad + energyRegenSpeed;
		chaosCost = 15.0f;
		powerBar.maxValue = maxEnergy;
		texture = new Texture2D((int) bricks[0].GetComponent<SpriteRenderer>().sprite.textureRect.width, (int) bricks[0].GetComponent<SpriteRenderer>().sprite.textureRect.height);

		SpawnNewBricks();

		//activeSkill.SetSkillLevel(2);
		//activeSkill.SetSkillCost(10);
	}
	
	void Update()
	{
		//Hotkey Functionality
		if(Input.GetKeyDown(KeyCode.Q) && !isSniping && !isWrecking)
			currentSelection = 0;
		else if(Input.GetKeyDown(KeyCode.W) && !isSniping && !isWrecking)
			currentSelection = 1;
		else if(Input.GetKeyDown(KeyCode.E) && !isSniping && !isWrecking)
			currentSelection = 2;
		else if(Input.GetKeyDown(KeyCode.R) && !isSniping && !isWrecking)
			currentSelection = 3;

		//score milestone. Used for speeding up spawn and leveling up
		if(score >= milestone && milestonesReached < MAX_LEVEL)
		{
			//change spawn time for bricks
			if(spawnTime - 1.0f > 0f)
			{
				spawnTime -= 1.0f;
			}

			for(int i = 0; i < powersToUse.Length; i++)
			{
				powersToUse[i].LevelUp();
			}
				
			milestone *= 2; //Next milestone equals double current milestone
			milestonesReached++;

			switch(milestonesReached+1)
			{
			case 4:
				stacks++;
				break;
			case 6:
				stacks++;
				break;
			case 8:
				stacks++;
				break;
			case 10:
				stacks++;
				break;
			default:
				break;
			}
		}

		if(Time.timeSinceLevelLoad > powersToUse[0].GetStackTimer() && powersToUse[0].GetStacks() < stacks)
		{
			powersToUse[0].AddStack();
			powersToUse[0].StackTimer();
		}
		else if(Time.timeSinceLevelLoad > powersToUse[1].GetStackTimer() && powersToUse[1].GetStacks() < stacks)
		{
			powersToUse[1].AddStack();
			powersToUse[1].StackTimer();
		}
		else if(Time.timeSinceLevelLoad > powersToUse[2].GetStackTimer() && powersToUse[2].GetStacks() < stacks)
		{
			powersToUse[2].AddStack();
			powersToUse[2].StackTimer();
		}
		else if(Time.timeSinceLevelLoad > powersToUse[3].GetStackTimer() && powersToUse[3].GetStacks() < stacks)
		{
			powersToUse[3].AddStack();
			powersToUse[3].StackTimer();
		}

		if(Time.timeSinceLevelLoad >= nextSpawn && ballInPlay)
		{
			MoveBricks();
			SpawnNewBricks();
		}

		if(gameOver)
			GameOver();

		//if(Input.GetKeyDown(KeyCode.Mouse1) && ballInPlay && energy >= chaosCost)
		if(Input.GetKeyDown(KeyCode.Mouse1) && ballInPlay)
			UsePower(currentSelection);

		if(isSniping && Time.realtimeSinceStartup > snipeTime)
		{
			Time.timeScale = 1f;
			snipedBricks.Clear(); // clear the selected bricks list
			isSniping = false;
		}

		//Used for Mousewheel mini game. If we have exceeded our time limit for the minigame, resume play. Otherwise, allow mousewheel input to dictate wreckingDamage.
		//if(isWrecking /*&& Time.realtimeSinceStartup > wreckingTime*/)
		//{
		//	wreckingDamage = powersToUse[currentSelection].GetStacks();
			//wreckingDamage = (int)wreckingStacks; //take the stacks generated from mousewheel minigame and assign them to wreckingDamage. Decimal gets truncated during cast.
			//Time.timeScale = 1f;
		//	isWrecking = false;
		//}
		/*else if(isWrecking && Time.realtimeSinceStartup < wreckingTime && wreckingStacks < powersToUse[currentSelection].GetSkillLevel())
		{
			if(Input.GetAxis("Mouse Y") > 0f)
				wreckingStacks += Input.GetAxis("Mouse Y") * (powersToUse[currentSelection].GetSkillLevel() * 0.01f);
				//wreckingStacks += Input.GetAxis("Mouse ScrollWheel") / (powersToUse[currentSelection].GetSkillLevel());
			else if(Input.GetAxis("Mouse Y") < 0f)
				//Add to wreckingStacks by multiplying the negative axis movement by -1 to create a positive result.
				wreckingStacks += -1 * (Input.GetAxis("Mouse Y")) * (powersToUse[currentSelection].GetSkillLevel() * 0.01f);
				//wreckingStacks += -1 * (Input.GetAxis("Mouse ScrollWheel") / (powersToUse[currentSelection].GetSkillLevel()));
		}*/

		//Monitor Stretch Time and reset paddle when it's done
		if (isStretched && Time.timeSinceLevelLoad > stretchTime)
		{
			paddle.localScale = originalScale;
			paddleAnimator.SetBool("threeSeconds", false);
			isStretched = false;
		}
		else if(isStretched && Time.timeSinceLevelLoad >= stretchTime - 3.0f) //start flashing the paddle when 3 seconds remain to warn players
		{
			paddleAnimator.SetBool("threeSeconds", true);
		}
			
		/*if(isWrecking && wreckingDamage == 0)
			isWrecking = false;
		else if(isWrecking && wreckingDamage > 0)
			isWrecking = true;
*/
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

		//if(Input.GetKeyDown(KeyCode.Space) && ballInPlay && !isSniping)
		//	SniperPower();
		

	}

	void OnGUI()
	{
		powerBar.value = energy;
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
			}
			else if(lastRect.position.x + brickRender.bounds.size.x < Camera.main.ScreenToWorldPoint(Camera.main.pixelRect.max).x)
			{
				spawnGrid[b] = new Rect(new Vector2(lastRect.position.x + brickRender.bounds.size.x + columnSpace, lastRect.position.y), 
					new Vector2(brickRender.bounds.size.x, brickRender.bounds.size.y));
			}
			lastRect = spawnGrid[b];
		}

		//Instanttiate New Bricks
		for(int i = 0; i < numberOfBricks; i++)
		{
			spawnedBricks.Add(Instantiate(bricks[UnityEngine.Random.Range (0, bricks.Length)], spawnGrid[i].position, Quaternion.identity) as Transform);
            index = spawnedBricks.Count;
		}
		
		nextSpawn = Time.timeSinceLevelLoad + spawnTime;
        newSpawn = true;
	}

	void UsePower(int index)
	{
		switch(powersToUse[index].GetName())
		{
		case "Chaos":
			ChaosBall();
			break;
		case "Sniper":
			SniperPower();
			break;
		case "WreckingBall":
			WreckingBall();
			break;
		case "Stretch":
			StretchPaddle();
			break;
		default:
			break;
		}
	}

	void ChaosBall()
	{
		if(energy >= powersToUse[currentSelection].GetSkillCost() && Time.timeSinceLevelLoad > powersToUse[currentSelection].GetCooldownTime())
		{
			if(powersToUse[currentSelection].GetStacks() > 1)
				energy -= powersToUse[currentSelection].GetSkillCost() * (powersToUse[currentSelection].GetStacks() * 0.5f);
			else
				energy -= powersToUse[currentSelection].GetSkillCost();
			for(int i = 0; i < powersToUse[currentSelection].GetStacks() /*powersToUse[currentSelection].GetSkillLevel()*/; i++)
			{
				spawnedBalls[i] = (Rigidbody2D) Instantiate (spawnBall, ball.position, Quaternion.identity);
				spawnedBalls[i].velocity = new Vector2(UnityEngine.Random.Range(-20, 20), ball.velocity.y);		
			}
			powersToUse[currentSelection].StartCooldown();
			powersToUse[currentSelection].StackTimer();
		}
	}

	void SniperPower()
	{
		if(isSniping && Time.timeScale == 0)
		{
			//Debug.Log("True");
			if(snipedBricks.Count > 0) //Destroy bricks if there are any added
			{
				for(int i = 0; i < snipedBricks.Count; i++)
				{
					if(snipedBricks[i].GetComponent<BrickScript>().isEnergy())
						Instantiate(energyObject,snipedBricks[i].transform.position,Quaternion.identity);
					
					RemoveBrick(snipedBricks[i].transform);
					SetScore(snipedBricks[i].GetComponent<BrickScript>().GetScoreValue());
					Destroy(snipedBricks[i]);
				}
				snipedBricks.Clear();
			}
			Time.timeScale = 1;
			powersToUse[currentSelection].StartCooldown();
			powersToUse[currentSelection].StackTimer();
			isSniping = false;
		}

		if(energy >= powersToUse[currentSelection].GetSkillCost() && !isSniping && Time.timeSinceLevelLoad > powersToUse[currentSelection].GetCooldownTime())
		{
			Time.timeScale = 0;
			if(powersToUse[currentSelection].GetStacks() > 1)
				energy -= powersToUse[currentSelection].GetSkillCost() * (powersToUse[currentSelection].GetStacks() * 0.5f);
			else
				energy -= powersToUse[currentSelection].GetSkillCost();
			snipeTime = Time.realtimeSinceStartup + 5.0f;
			isSniping = true;
		}
	}

	void WreckingBall()
	{
		/*
		//Use this is player is in Wrecking Ball pause state and presses right-click again.
		if(isWrecking && Time.timeScale == 0)
		{
			wreckingDamage = 0;
			Time.timeScale = 1;
			isWrecking = false;
		}
		*/

		if(energy >= powersToUse[currentSelection].GetSkillCost() && !isWrecking && Time.timeSinceLevelLoad > powersToUse[currentSelection].GetCooldownTime())
		{
			//Time.timeScale = 0;
			if(powersToUse[currentSelection].GetStacks() > 1)
				energy -= powersToUse[currentSelection].GetSkillCost() * (powersToUse[currentSelection].GetStacks() * 0.5f);
			else
				energy -= powersToUse[currentSelection].GetSkillCost();
			//wreckingTime = Time.realtimeSinceStartup + 3.0f; //used for the timing window
			wreckingDamage = powersToUse[currentSelection].GetStacks();
			powersToUse[currentSelection].StartCooldown();
			powersToUse[currentSelection].StackTimer();
			//wreckingStacks = 0f;
		//	isWrecking = true;
		}
	}

	void StretchPaddle()
	{
		if(energy >= powersToUse[currentSelection].GetSkillCost() && !isStretched && Time.timeSinceLevelLoad > powersToUse[currentSelection].GetCooldownTime())
		{
			if(powersToUse[currentSelection].GetStacks() == 1)
				paddle.localScale = new Vector3((powersToUse[currentSelection].GetStacks() * .75f), paddle.localScale.y, paddle.localScale.z);
			else
				paddle.localScale = new Vector3((powersToUse[currentSelection].GetStacks() * .40f), paddle.localScale.y, paddle.localScale.z);
			//stretchTime = Time.timeSinceLevelLoad + (powersToUse[currentSelection].GetSkillLevel() / 1.5f);
			stretchTime = Time.timeSinceLevelLoad + (powersToUse[currentSelection].GetBaseCooldown() / 2);
			powersToUse[currentSelection].StartCooldown();
			powersToUse[currentSelection].StackTimer();
			isStretched = true;
		}
	}

	public void SetBricksToSnipe(GameObject brick)
	{
		//This is how many bricks we can snipe
		if(!snipedBricks.Contains(brick) && snipedBricks.Count < powersToUse[currentSelection].GetStacks() /*powersToUse[currentSelection].GetSkillLevel()*/)
		{
			snipedBricks.Add(brick);
		}
		//Debug.Log(snipedBricks.Count);
	}

	public bool GetBrickToSnipe(GameObject brick)
	{
		return snipedBricks.Contains(brick);
	}

	public void RemoveBricksToSnipe(GameObject brick)
	{
		if(snipedBricks.Contains(brick))
			snipedBricks.Remove(brick);
	}

    //Remove Bricks from List when they're destroyed
    public void RemoveBrick(Transform brickToDestroy)
	{
		spawnedBricks.Remove(brickToDestroy);
	}

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

	public float GetCooldownTime(int index)
	{
		switch(index)
		{
		case 0:
			return powersToUse[0].GetCooldownTime();
			break;
		case 1:
			return powersToUse[1].GetCooldownTime();
			break;
		case 2:
			return powersToUse[2].GetCooldownTime();
			break;
		case 3:
			return powersToUse[3].GetCooldownTime();
			break;
		}

		return 0f;
	}

	public int GetMilestone()
	{
		return milestonesReached + 1;
	}

	public Transform GetPaddle()
	{
		return paddle;
	}

	public void SaveData(String skillName)
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

		Powers data = new Powers();
		
		/*switch(skillName)
		{
			case "ChaosBall":
				data.SetSkillCost(10);
				data.SetSkillLevel(1);
				data.SetImage(Resources.Load("ChaosBallIcon") as Sprite);
				break;
		}*/

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
			data.GetIconPath();
		}
	}

	public int GetCurrentHotkey()
	{
		return currentSelection;
	}

	public Sprite GetSkillIcons(int num)
	{
		return Resources.Load<Sprite>( powersToUse[num].GetIconPath());
	}

	public void SetScore(int value)
	{
		score += value;
	}

	public int GetScore()
	{
		return score;
	}

	public int GetSkillStacks(int index)
	{
		return powersToUse[index].GetStacks();
	}
}

[Serializable]
class Powers {

	//Need to think this through.
	float skillCost;
	int skillLevel;
	string skillIconPath;
	String name;
	float cooldownTime, timeOnCooldown, stackTimer;
	int stacks = 1;

	public Powers()
	{
	}

	public Powers(String powerName)
	{
		switch(powerName)
		{
		case "Chaos":
			name = "Chaos";
			skillLevel = 1;
			skillCost = 10;
			cooldownTime = 5.0f;
		//	stackTimer = cooldownTime;
			skillIconPath = "HotkeyIcons/ChaosBall";
			break;
		case "Sniper":
			name = "Sniper";
			skillLevel = 1;
			skillCost = 15;
			cooldownTime = 8.0f;
		//	stackTimer = cooldownTime;
			skillIconPath = "HotkeyIcons/SniperBall";
			break;
		case "WreckingBall":
			name = "WreckingBall";
			skillLevel = 1; //CHANGE THIS WHEN FINALIZING
			skillCost = 20;
			cooldownTime = 10.0f;
		//	stackTimer = cooldownTime;
			skillIconPath = "HotkeyIcons/WreckingBall";
			break;
		case "Stretch":
			name = "Stretch";
			skillLevel = 1;
			skillCost = 15;
			cooldownTime = 10.0f;
		//	stackTimer = cooldownTime;
			skillIconPath = "HotkeyIcons/PaddleStretch";
			break;
		}
	}
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

	public String GetIconPath()
	{
		return skillIconPath;
	}

	public void SetIconPath(string pathToUse)
	{
		skillIconPath = pathToUse;
	}

	public void LevelUp()
	{
		switch(name)
		{
		case "Chaos":
			cooldownTime -= (cooldownTime / 10);
			break;
		case "Sniper":
			cooldownTime -= (cooldownTime / 10) / 2;
			break;
		case "Wrecking":
			cooldownTime -= (cooldownTime / 10) / 2;
			break;
		case "Stretch":
			cooldownTime -= (cooldownTime / 10);
			break;
		}
		//skillLevel++;
		//skillCost = skillCost * (skillLevel * 0.5f);
	}

	public String GetName()
	{
		return name;
	}

	public float GetBaseCooldown()
	{
		return cooldownTime;
	}

	public float GetCooldownTime()
	{
		return timeOnCooldown;
	}

	public void StartCooldown()
	{
		timeOnCooldown = Time.timeSinceLevelLoad + cooldownTime;
		stacks = 0;
	}

	public void StackTimer()
	{
		stackTimer = Time.timeSinceLevelLoad + cooldownTime;
	}

	public float GetStackTimer()
	{
		return stackTimer;
	}

	public void AddStack()
	{
		stacks++;
	}

	public int GetStacks()
	{
		return stacks;
	}

}
