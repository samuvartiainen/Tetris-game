using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour 
{

	public static int gridWidth = 10;
	public static int gridHeight = 20;
	public float fallSpeed = 1.0f;
	public static bool isPaused = false;
	public float startTime;
	private GameObject previewTetromino;
	private GameObject nextTetromino;

	public int currentLevel = 1;
	public int numLinesCleared = 0;
	private bool gameStarted = false;
	private Vector2 previewTetrominoPosition = new Vector2 (-6.5f, 16);

    public static Transform[,] grid = new Transform[gridWidth, gridHeight];

	// scoring
	public int scoreOneLine = 40;
	public int scoreTwoLine = 100;
	public int scoreThreeLine = 300;
	public int scoreFourLine = 1200;

	public Text hud_score;
	public Text hud_level;


	private int numberOfRowsThisTurn = 0;

	public static int currentScore = 0;

	float timerInSecond = 0;
	private float levelTimer = 0.0f;
	private bool updateTimer = false; 

	void Start () 
	{
		updateTimer = true;
		levelTimer = 0.0f;
		SpawnNextTetromino();
	}

	void Update()
	{
		UpdateScore();

		UpdateUI();

		if (updateTimer)  
		{
			levelTimer += Time.deltaTime*1;

		/// float to int
		timerInSecond = Mathf.Round (levelTimer); 

		Debug.Log ("Current timer : " + timerInSecond);
		}
		UpdateLevel();
		UpdateSpeed();
		CheckUserInput();

	}

	void CheckUserInput ()
	{
		if (Input.GetKeyUp (KeyCode.P)) 
		{
			if (Time.timeScale == 1)
			{
				Time.timeScale = 0;
				isPaused = true;
			}
			else
			{
				Time.timeScale = 1;
				isPaused = false;
			}
		}
	}
	void UpdateLevel () 
	{

		currentLevel = 1 +( (int)timerInSecond / 45); // level changes every 45 seconds

		Debug.Log ("Current level: " + currentLevel);
	}

	void UpdateSpeed () // speed change as level changes
	{ 
		if (currentLevel <= 9)
		{
		fallSpeed = 1.0f - ((float)currentLevel * 0.1f);
		Debug.Log ("Current Fall Speed : " + fallSpeed);
		}

		else if(currentLevel > 9)
		{
		fallSpeed = 0.19f - ((float)currentLevel * 0.01f);
		Debug.Log ("Current Fall Speed : " + fallSpeed);
		}
	}
	// Update score on screen
	public void UpdateUI() 
	{						
		hud_score.text = currentScore.ToString();
		hud_level.text = currentLevel.ToString();
	}

	// Update score according to how many lines were cleared
	public void UpdateScore () 
	{
		if (numberOfRowsThisTurn > 0) 
		{
			if (numberOfRowsThisTurn == 1) 
			{
				ClearedOneLine ();
			}
			else if (numberOfRowsThisTurn == 2) 
			{
				ClearedTwoLines ();
			}
			else if (numberOfRowsThisTurn == 3) 
			{
				ClearedThreeLines ();
			}
			else if (numberOfRowsThisTurn == 4) 
			{
				ClearedFourLines ();		
			}

			numberOfRowsThisTurn = 0;
				
		}
	}

	public void ClearedOneLine()
	{
		currentScore += scoreOneLine;
		numLinesCleared++;
	}
	public void ClearedTwoLines()
	{
		currentScore += scoreTwoLine;
		numLinesCleared += 2;
	}
	public void ClearedThreeLines()
	{
		currentScore += scoreThreeLine;
		numLinesCleared += 3;
	}
	public void ClearedFourLines()
	{
		currentScore += scoreFourLine;
		numLinesCleared += 4;
	}

	public bool CheckIsAboveGrid (Tetromino tetromino) 
	{
		for (int x = 0; x < gridWidth; x++) 
		{
			foreach (Transform mino in tetromino.transform) 
			{
				Vector2 pos = Round (mino.position);

				if (pos.y > gridHeight - 1) 
				{
					return true;
				}
			}
		}
		return false;
	}

    public bool IsFullRowAt (int y)
    {
        for (int x = 0 ; x < gridWidth; x++)
        {
            if (grid[x,y] == null)
            {
                return false;
            }
        }

		//Full row increments the full row variable
		numberOfRowsThisTurn++;
        return true;
    }

    public void DeleteMinoAt (int y)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            Destroy(grid[x, y].gameObject);

            grid[x, y] = null;
        }
    }

    public void MoveRowDown (int y)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            if (grid[x,y] != null)
            {
                grid[x, y - 1] = grid[x, y];

                grid[x, y] = null;

                grid[x, y - 1].position += new Vector3(0, -1, 0); 
            }
        }
    }

    public void MoveAllRowsDown (int y)
    {
        for (int i = y; i < gridHeight; i++)
        {
            MoveRowDown(i);
        }
    }
    public void DeleteRow ()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            if (IsFullRowAt(y))
            {
                DeleteMinoAt(y);

                MoveAllRowsDown(y + 1);

                y--;
            }
        }
    }
    public void UpdateGrid (Tetromino tetromino)
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x,y] != null)
                {
                    if (grid[x,y].parent == tetromino.transform)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }
        foreach (Transform mino in tetromino.transform)
        {
            Vector2 pos = Round(mino.position);

            if(pos.y < gridHeight)
            {
                grid[(int)pos.x, (int)pos.y] = mino;
            }
        }
    }

    public Transform GetTransformGridPosition (Vector2 pos)
    {
        if (pos.y > gridHeight -1)
        {
            return null;
        }
        else
        {
            return grid[(int)pos.x, (int)pos.y];
        }
    }

	public void SpawnNextTetromino ()
    {	
		if (!gameStarted)  
		{
			gameStarted = true;

			nextTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), new Vector2(5.0f, 20.0f), Quaternion.identity);
			previewTetromino = (GameObject)Instantiate (Resources.Load (GetRandomTetromino (), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
			previewTetromino.GetComponent<Tetromino>().enabled = false;
		}
		else
		{
			previewTetromino.transform.localPosition = new Vector2 (5.0f, 20.0f);
			nextTetromino = previewTetromino;
			nextTetromino.GetComponent<Tetromino> ().enabled = true;

			previewTetromino = (GameObject)Instantiate (Resources.Load (GetRandomTetromino (), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
			previewTetromino.GetComponent<Tetromino>().enabled = false;
		}
	
	}


	public bool CheckIsInsideGrid(Vector2 pos)
	{
		return ((int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.y >= 0);
	}
	public Vector2 Round(Vector2 pos)
	{
		return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
	}

	string GetRandomTetromino() {

		int randomTetromino = Random.Range(1, 8);
		string randomTetrominoName = "Prefabs/Tetromino_S";

		switch (randomTetromino) {
		case 1:
			randomTetrominoName = "Prefabs/Tpala";
			break;
		case 2:
			randomTetrominoName = "Prefabs/Pitkapala";
			break;
		case 3:
			randomTetrominoName = "Prefabs/Neliopala";
			break;
		case 4:
			randomTetrominoName = "Prefabs/Jpala";
			break;
		case 5:
			randomTetrominoName = "Prefabs/Lpala";
			break;
		case 6:
			randomTetrominoName = "Prefabs/Spala";
			break;
		case 7:
			randomTetrominoName = "Prefabs/Zpala";
			break;
		}

		return randomTetrominoName;
	}

	public void GameOver() 
	{
		SceneManager.LoadScene("GameOver");
	}
}