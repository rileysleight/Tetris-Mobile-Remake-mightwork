using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
	public Board board { get; private set; }
	public TetrominoData data { get; private set; }
	public Vector3Int[] cells { get; private set; }
	public Vector3Int position { get; private set; }
	public int rotationIndex { get; private set; }
	//
	private Vector2 startPos;
	public int pixelDistToDetect = 200;
	//public int pixelDistForSwipe = 1000;
	private bool fingerDown;
	public int playerPoints;

	

	public float stepDelay = 1f;
	public float lockDelay = 0.5f;

	private float stepTime;
	private float lockTime; 

	public void Initialize(Board board, Vector3Int position, TetrominoData data)
	{
		this.board = board;
		this.position = position;
		this.data = data;
		this.rotationIndex = 0;
		this.stepTime = Time.time + this.stepDelay;
		this.lockTime = 0f;

		if (this.cells == null)
		{
			this.cells = new Vector3Int[data.cells.Length];
		}

		for (int i = 0; i < data.cells.Length; i++)
		{
			this.cells[i] = (Vector3Int)data.cells[i];
		}
	}

	public void Update()
	{
		this.board.Clear(this);

		this.lockTime += Time.deltaTime;

//---
		if(fingerDown == false && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
		{
			startPos = Input.touches[0].position;
			fingerDown = true;
		}
		if(fingerDown && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended)
		{
			fingerDown = false;
		}

		if (fingerDown)
		{
			if(Input.touches[0].position.y >= startPos.y + pixelDistToDetect)
			{
				fingerDown = false;
				Debug.Log("Swipe Up");
				Rotate(-1);	
			}
			else if(Input.touches[0].position.x <= startPos.x - pixelDistToDetect)
			{
				fingerDown = false;
				Debug.Log("Swipe Left");
				Move(Vector2Int.left);
			}
			else if(Input.touches[0].position.x >= startPos.x + pixelDistToDetect)
			{
				fingerDown = false;
				Debug.Log("Swipe Right");
				Move(Vector2Int.right);
			}
			else if(Input.touches[0].position.y <= startPos.y - pixelDistToDetect)
			{
				fingerDown = false;
				Debug.Log("Swipe Down");
				Move(Vector2Int.down);
			}
			
		}
//---

		if(Input.GetKeyDown(KeyCode.W))
		{
			Rotate(-1);		
		}

		if(Input.GetKeyDown(KeyCode.A))
		{
			Move(Vector2Int.left);
		}
		//-----
        
		//-----
		else if(Input.GetKeyDown(KeyCode.D))
		{
			Move(Vector2Int.right);
		}
		
		if(Input.GetKeyDown(KeyCode.S))
		{
			Move(Vector2Int.down);
		}

		//FOR TESTING - HARD DROP
		if (Input.GetKeyDown(KeyCode.Space))
		{
			HardDrop();
		}
		//FOR TESTING - HARD DROP

		if (Time.time >= this.stepTime)
		{
			Step();
		}

		this.board.Set(this);	
	}

	private void Step()
	{
		this.stepTime = Time.time + this.stepDelay;
		Move(Vector2Int.down);

		if(this.lockTime >= this.lockDelay)
		{
			Lock();
		}
	}

	private void Lock()
	{
		this.board.Set(this);
		this.board.ClearLines();
		this.board.SpawnPiece();
	}


	//FOR TESTING - HARD DROP
	private void HardDrop()
	{
		while (Move(Vector2Int.down))
		{
			continue;
		}
		Lock();
	}
	//FOR TESTING - HARD DROP

	private bool Move(Vector2Int translation)
	{
		Vector3Int newPosition = this.position;
		newPosition.x += translation.x;
		newPosition.y += translation.y;

		bool valid = this.board.IsValidPosition(this, newPosition);

		if (valid)
		{
			this.position = newPosition;
			this.lockTime = 0f;
		}

		return valid;
	}

	private void Rotate(int direction)
	{
		int originalRotation = this.rotationIndex;
		this.rotationIndex = Wrap(this.rotationIndex + direction, 0, 4);

		ApplyRotationMatrix(direction);
		
		if(!TestWallKicks(this.rotationIndex, direction))
		{
			this.rotationIndex = originalRotation;
			ApplyRotationMatrix(-direction);
		}
	}

	private void ApplyRotationMatrix(int direction)
	{
		for(int i = 0; i < this.cells.Length; i++)
		{
			Vector3 cell = this.cells[i];
			
			int x, y;

			switch (this.data.tetromino)
			{
				case Tetromino.I:
				case Tetromino.O:
					cell.x -= 0.5f;
					cell.y -= 0.5f;
					x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
					y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));						
					break;

				default:
					x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
					y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));					
					break;
			}

			this.cells[i] = new Vector3Int(x, y, 0);
		}
	}

	private bool TestWallKicks(int rotationIndex, int rotationDirection)
	{
		int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

		for (int i = 0; i < this.data.wallKicks.GetLength(1); i++)
		{
			Vector2Int translation = this.data.wallKicks[wallKickIndex, i];

			if(Move(translation))
			{
				return true;
			}
		}
		return false;
	}

	private int GetWallKickIndex(int rotationIndex, int rotationDirection)
	{
		int wallKickIndex = rotationIndex * 2;

		if(rotationDirection < 0)
		{
			wallKickIndex--;
		}

		return Wrap(wallKickIndex, 0, this.data.wallKicks.GetLength(0));
	}

	private int Wrap(int input, int min, int max)
	{
		if (input < min)
		{
			return max - (min - input) % (max - min);
		}
		else
		{
			return min + (input - min) % (max - min);
		}
	}




}
