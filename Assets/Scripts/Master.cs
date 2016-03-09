﻿using UnityEngine;
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
	Powers[] powersToUse = new Powers[] {new Powers("Chaos"), new Powers("Sniper"), new Powers("Stretch")};

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
	float snipeTime = 0f;
	Powers activeSkill = new Powers();
	public bool isSniping = false;
	List<GameObject> snipedBricks = new List<GameObject>();

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
		if(Input.GetKeyDown(KeyCode.Alpha1) && !isSniping && !isWrecking)
			currentSelection = 0;
		else if(Input.GetKeyDown(KeyCode.Alpha2) && !isSniping && !isWrecking)
			currentSelection = 1;
		else if(Input.GetKeyDown(KeyCode.Alpha3) && !isSniping && !isWrecking)
			currentSelection = 2;

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
		if(isWrecking && Time.realtimeSinceStartup > wreckingTime)
		{
			wreckingDamage = (int)wreckingStacks; //take the stacks generated from mousewheel minigame and assign them to wreckingDamage. Decimal gets truncated during cast.
			Time.timeScale = 1f;
			isWrecking = false;
		}
		else if(isWrecking && Time.realtimeSinceStartup < wreckingTime && wreckingStacks < powersToUse[currentSelection].GetSkillLevel())
		{
			if(Input.GetAxis("Mouse ScrollWheel") > 0f)
				wreckingStacks += Input.GetAxis("Mouse ScrollWheel") * (powersToUse[currentSelection].GetSkillLevel() * 0.15f);
				//wreckingStacks += Input.GetAxis("Mouse ScrollWheel") / (powersToUse[currentSelection].GetSkillLevel());
			else if(Input.GetAxis("Mouse ScrollWheel") < 0f)
				//Add to wreckingStacks by multiplying the negative axis movement by -1 to create a positive result.
				wreckingStacks += -1 * (Input.GetAxis("Mouse ScrollWheel")) * (powersToUse[currentSelection].GetSkillLevel() * .15f);
				//wreckingStacks += -1 * (Input.GetAxis("Mouse ScrollWheel") / (powersToUse[currentSelection].GetSkillLevel()));
		}

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
			energy -= powersToUse[currentSelection].GetSkillCost();
			for(int i = 0; i < powersToUse[currentSelection].GetSkillLevel(); i++)
			{
				spawnedBalls[i] = (Rigidbody2D) Instantiate (spawnBall, ball.position, Quaternion.identity);
				spawnedBalls[i].velocity = new Vector2(UnityEngine.Random.Range(-20, 20), ball.velocity.y);		
			}
			powersToUse[currentSelection].StartCooldown();
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
					RemoveBrick(snipedBricks[i].transform);
					Destroy(snipedBricks[i]);
				}
				snipedBricks.Clear();
			}
			Time.timeScale = 1;
			isSniping = false;
		}

		if(energy >= powersToUse[currentSelection].GetSkillCost() && !isSniping && Time.timeSinceLevelLoad > powersToUse[currentSelection].GetCooldownTime())
		{
			Time.timeScale = 0;
			energy -= powersToUse[currentSelection].GetSkillCost();
			snipeTime = Time.realtimeSinceStartup + 5.0f;
			powersToUse[currentSelection].StartCooldown();
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
			Time.timeScale = 0;
			energy -= powersToUse[currentSelection].GetSkillCost();
			wreckingTime = Time.realtimeSinceStartup + 3.0f; //used for the timing window
			powersToUse[currentSelection].StartCooldown();
			wreckingStacks = 0f;
			isWrecking = true;
		}
	}

	void StretchPaddle()
	{
		if(energy >= powersToUse[currentSelection].GetSkillCost() && !isStretched && Time.timeSinceLevelLoad > powersToUse[currentSelection].GetCooldownTime())
		{
			paddle.localScale = new Vector3((powersToUse[currentSelection].GetSkillLevel() * 0.15f), paddle.localScale.y, paddle.localScale.z);
			stretchTime = Time.timeSinceLevelLoad + (powersToUse[currentSelection].GetSkillLevel() / 1.5f);
			powersToUse[currentSelection].StartCooldown();
			isStretched = true;
		}
	}

	public void SetBricksToSnipe(GameObject brick)
	{
		//This is how many bricks we can snipe
		if(!snipedBricks.Contains(brick) && snipedBricks.Count < powersToUse[currentSelection].GetSkillLevel())
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
		}

		return 0f;
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
}

[Serializable]
class Powers {

	//Need to think this through.
	float skillCost;
	int skillLevel;
	string skillIconPath;
	String name;
	float cooldownTime, timeOnCooldown;

	public Powers()
	{
	}

	public Powers(String powerName)
	{
		switch(powerName)
		{
		case "Chaos":
			name = "Chaos";
			skillCost = 10;
			skillLevel = 1;
			cooldownTime = 4.0f;
			skillIconPath = "HotkeyIcons/ChaosBall";
			break;
		case "Sniper":
			name = "Sniper";
			skillCost = 15;
			skillLevel = 1;
			cooldownTime = 8.0f;
			skillIconPath = "HotkeyIcons/SniperBall";
			break;
		case "WreckingBall":
			name = "WreckingBall";
			skillCost = 20;
			skillLevel = 6; //CHANGE THIS WHEN FINALIZING
			cooldownTime = 6.0f;
			skillIconPath = "HotkeyIcons/WreckingBall";
			break;
		case "Stretch":
			name = "Stretch";
			skillCost = 15;
			skillLevel = 6;
			cooldownTime = 8.0f;
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
		skillLevel++;
		skillCost = skillCost * (skillLevel * 0.5f);
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
	}


}
