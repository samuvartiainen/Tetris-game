using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour {

	float fall = 0;

	public float fallSpeed;

	public bool allowRotation = true;
	public bool limitRotation = false;

	public int individualScore = 20;

	private float individualScoreTime;

	private float continuousVerticalSpeed = 0.05f; // - The speed at which the tetromino will move when the down button is held down
	private float continuousHorizontalSpeed = 0.1f; // - The speed at which the tetromino will move when the left or right buttons are held down
	private float buttonDownWaitMax = 0.2f; 		// - How long to wait before the tetromino recognizes that a button is being held down

	private float verticalTimer = 0;
	private float horizontalTimer = 0;

	private float buttonTimerHorizontal = 0;
	private float buttonTimerVertical = 0;

	private bool movedImmediateHorizontal = false;
	private bool movedImmediateVertical = false;


	// Use this for initialization
	void Start ()
    {
		fallSpeed = GameObject.Find ("Grid").GetComponent<Game> ().fallSpeed;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (!Game.isPaused)
		{
		CheckUserinput ();

		UpdateIndividualScore ();
		
		}
	}


	void UpdateIndividualScore ()
	{
		if (individualScoreTime < 1)
		{
			individualScoreTime += Time.deltaTime;
		}
		else
		{
			individualScoreTime = 0;
			individualScore = Mathf.Max (individualScore - 2, 0);
		}

	}

	void CheckUserinput ()
    {
		if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
		{
			movedImmediateHorizontal = false;

			horizontalTimer = 0;

			buttonTimerHorizontal = 0;
		}

		if (Input.GetKeyUp(KeyCode.DownArrow))
		{
			movedImmediateVertical = false;
			verticalTimer = 0;
			buttonTimerVertical = 0;
		}
		if (Input.GetKey (KeyCode.RightArrow))
		{
			MoveRight();
		}
		if (Input.GetKey (KeyCode.LeftArrow))
		{	
			MoveLeft();
		}
	
		if (Input.GetKeyDown (KeyCode.UpArrow))
		{
			Rotate();
		}
		if (Input.GetKey (KeyCode.DownArrow) || Time.time - fall >= fallSpeed)
		{
			MoveDown();
		}
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
			Drop();
        }

	}
		
	void MoveLeft()
	{    
			if (movedImmediateHorizontal)
			{

				if (horizontalTimer < continuousHorizontalSpeed)
				{
					horizontalTimer += Time.deltaTime;
					return;
				}
			if (buttonTimerHorizontal < buttonDownWaitMax)
				{
					buttonTimerHorizontal += Time.deltaTime;
					return;
				}
			}
			if(!movedImmediateHorizontal)
			{
				movedImmediateHorizontal = true;
			}
			horizontalTimer = 0;

			transform.position += new Vector3 (-1, 0, 0);

			if (CheckisValidPosition ()) {

				FindObjectOfType<Game>().UpdateGrid(this);
			}

			else
			{
				transform.position += new Vector3 (1, 0, 0);
			}
		}
		
	void MoveRight()
	{
		
			if (movedImmediateHorizontal)
			{

				if (buttonTimerHorizontal < buttonDownWaitMax)
				{
				buttonTimerHorizontal += Time.deltaTime;
					return;
				}

				if (horizontalTimer < continuousHorizontalSpeed)
				{
					horizontalTimer += Time.deltaTime;
					return;
				}
			}
			if (!movedImmediateHorizontal)
			{
				movedImmediateHorizontal = true;
			}
			horizontalTimer = 0;

			transform.position += new Vector3 (1, 0, 0);

			if (CheckisValidPosition ())
			{
				FindObjectOfType<Game>().UpdateGrid(this);
			}

			else
			{
				transform.position += new Vector3 (-1, 0, 0);
			}
		}
	
	void MoveDown()
	{
				if (movedImmediateVertical)
				{

					if (verticalTimer < continuousVerticalSpeed)
					{
						verticalTimer += Time.deltaTime;
						return;
					}

			if (buttonTimerVertical < buttonDownWaitMax)
					{
						buttonTimerVertical += Time.deltaTime;
						return;
					}
				}

				if (!movedImmediateVertical)
				{
					movedImmediateVertical = true;
				}

				verticalTimer = 0;

				transform.position += new Vector3 (0, -1, 0);

				if (CheckisValidPosition ())
				{
					FindObjectOfType<Game>().UpdateGrid(this);
				}
				else
				{
					transform.position += new Vector3 (0, 1, 0);

					FindObjectOfType<Game>().DeleteRow();

					if (FindObjectOfType<Game>().CheckIsAboveGrid (this)) 
					{
						FindObjectOfType<Game>().GameOver();
					}

					enabled = false;

					FindObjectOfType<Game>().SpawnNextTetromino();

					Game.currentScore += individualScore;
				}

				fall = Time.time;
			}

	void Drop()
	{
	while (CheckisValidPosition())
	{
		transform.position += new Vector3(0, -1, 0);
	}
	if (!CheckisValidPosition())
	{
		transform.position += new Vector3(0, 1, 0);
		FindObjectOfType<Game>().UpdateGrid(this);

		FindObjectOfType<Game>().DeleteRow();

	}


	FindObjectOfType<Game>().SpawnNextTetromino();
	Game.currentScore += individualScore;
	enabled = false;
	}

	void Rotate()
	{
			if (allowRotation)
			{
				if (limitRotation)
				{
					if (transform.rotation.eulerAngles.z >= 90)
					{
						transform.Rotate (0, 0, -90);
					}
					else
					{
						transform.Rotate (0, 0, 90);
					}
				}

				else
				{
					transform.Rotate (0, 0, 90);
				}

				if (CheckisValidPosition ()) {

					FindObjectOfType<Game>().UpdateGrid(this);
				}

				else {
					if (limitRotation)
					{
						if (transform.rotation.eulerAngles.z >= 90)
						{
							transform.Rotate (0, 0, -90);
						}
						else
						{
							transform.Rotate (0, 0, 90);
						}
					}
					else
					{
						transform.Rotate (0, 0, -90);
					}
				}
			}
		}

	bool CheckisValidPosition()
	{
		foreach (Transform mino in transform)
        {

			Vector2 pos = FindObjectOfType<Game>().Round (mino.position);

			if (FindObjectOfType<Game>().CheckIsInsideGrid (pos) == false)
            {
				return false;	
			}

            if (FindObjectOfType<Game>().GetTransformGridPosition(pos) != null && FindObjectOfType<Game>().GetTransformGridPosition(pos).parent != transform)
            {
                return false;
            }
		}
		return true;
	}

}