using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainLoop : MonoBehaviour
{
	#region vars
	public int BoardHeight = 21;
	public int BoardWidth = 12;
	
	bool[,] mBoard;
	
	public Transform[] PiecePrefabs = new Transform[7];
	public int[] LevelDropFrames = new int[10];
	
	Transform CurrentFallingPiece = null;
	
	public Transform BorderRow;
	public Transform BorderedEdgesRow;
	
	public Transform StartLocation;
	public Transform TileContainer;
	public Transform Background;
	
	public int CurrentLevel = 0;

	
	public delegate void InputAction();
	
	Dictionary<KeyCode, InputAction> InputMap;
	
	#endregion // vars

	#region init
	void Start ()
	{
		// Init board with a 1-tile boarder on sides and bottom, top is open, all other slots open
		mBoard = new bool[BoardHeight, BoardWidth];
		
		Vector3 loc = Background.position;
		
		CreateBorderRow(loc, 0);
		
		for (int row = 1; row < BoardHeight; row++)
		{
			loc.y += 1;
			CreateBorderedEdgesRow(loc, row);
		}
		
		InitInputHandling();
		
		Random.seed = (int)System.DateTime.Now.Ticks;
	}
	
	void CreateBorderRow(Vector3 loc, int rowNumber)
	{
		Transform borderRow = Instantiate(BorderRow, loc, Quaternion.identity) as Transform;		
		borderRow.gameObject.transform.parent = Background.gameObject.transform;
		
		for (int column = 0; column < BoardWidth; column++) // Bottom row is all border
		{
			mBoard[rowNumber, column] = true;
		}
	}
	
	void CreateBorderedEdgesRow(Vector3 loc, int rowNum)
	{
		Transform borderedEdgesRow = Instantiate(BorderedEdgesRow, loc, Quaternion.identity) as Transform;		
		borderedEdgesRow.gameObject.transform.parent = Background.gameObject.transform;
		
		mBoard[rowNum, 0] = true;
		for (int column = 1; column < (BoardWidth - 1); column++)
		{
			mBoard[rowNum, column] = false;
		}
		mBoard[rowNum, BoardWidth - 1] = true;
	}
	#endregion // init
	
	#region grid utility
	public void TranslateCoordtoGridCell(float x, float y, out int gridColumn, out int gridRow)
	{
		gridColumn = Mathf.FloorToInt(x - Background.position.x);
		gridRow = Mathf.FloorToInt(y - Background.position.y);
	}
	
	bool InGridRange(int gridColumn, int gridRow)
	{
		return (gridRow >= 0) && (gridRow < BoardHeight) && (gridColumn >= 0) && (gridColumn < BoardWidth);
	}
	
	public bool GridCellOccupied(int gridColumn, int gridRow)
	{
		if (InGridRange(gridColumn, gridRow))
		{
			return mBoard[gridRow, gridColumn];
		}
		else
		{
			return false;	// Grid isn't there, so... no, not occupied.
		}
	}
	
	public void OccupyGridCell(int gridColumn, int gridRow)
	{
		if (InGridRange(gridColumn, gridRow))
		{
			mBoard[gridRow, gridColumn] = true;
		}
	}
	#endregion // grid utility

	#region piece management
	void CheckForCompleteLines()
	{
		// TODO -- remove complete lines, increment completed line count, check for level up.
	}
	
	void CreatePiece(int dropOnFrame, Transform piecePrefab)
	{
		CurrentFallingPiece = Instantiate(piecePrefab, StartLocation.transform.position, Quaternion.identity) as Transform;
		Drop dropScript = CurrentFallingPiece.gameObject.GetComponent<Drop>(); 
		dropScript.TileContainer = TileContainer;
		dropScript.MainLoopScriptObject = this.gameObject;
		dropScript.DropOnFrame = dropOnFrame;
		
		PieceFallingInput();
	}
	
	public void DestroyCurrentPiece()
	{
		Destroy(CurrentFallingPiece.gameObject);
		CurrentFallingPiece = null;
		NoPieceFallingInput();
	}
	
	void CreateRandomPiece()
	{
		int pieceIndex = Random.Range(0, PiecePrefabs.Length);
		
		CreatePiece(LevelDropFrames[CurrentLevel], PiecePrefabs[pieceIndex]);
	}
	#endregion // piece management
	
	#region input
		
	void InitInputHandling()
	{
		InputMap = new Dictionary<KeyCode, InputAction>();
		
		NoPieceFallingInput();
		InputMap[KeyCode.None] = DoNothing;
	}
	
	void PieceFallingInput()
	{
		InputMap[KeyCode.Space] = MovePieceDown;
		InputMap[KeyCode.LeftArrow] = MovePieceLeft;
		InputMap[KeyCode.RightArrow] = MovePieceRight;
		InputMap[KeyCode.UpArrow] = RotatePieceCW;
		InputMap[KeyCode.DownArrow] = RotatePieceCCW;
	}
	
	void NoPieceFallingInput()
	{
		InputMap[KeyCode.Space] = CreateRandomPiece;	// TODO -- DoNothing
		InputMap[KeyCode.LeftArrow] = DoNothing;
		InputMap[KeyCode.RightArrow] = DoNothing;
		InputMap[KeyCode.UpArrow] = DoNothing;
		InputMap[KeyCode.DownArrow] = DoNothing;
	}
	
	public KeyCode CheckForInput()
	{
		// TODO -- lots more in here.  Simple dev test.
		// TODO -- is there really no better way to do this?  Can I not just get keyboard events directly?
		if (Input.GetKey(KeyCode.Space))
		{
			return KeyCode.Space;
		}
		else if (Input.GetKey(KeyCode.LeftArrow))
		{
			return KeyCode.LeftArrow;
		}
		else if (Input.GetKey(KeyCode.RightArrow))
		{
			return KeyCode.RightArrow;
		}
		else if (Input.GetKey(KeyCode.UpArrow))
		{
			return KeyCode.UpArrow;
		}
		else if (Input.GetKey(KeyCode.DownArrow))
		{
			return KeyCode.DownArrow;
		}
		else
		{
			return KeyCode.None;
		}
	}
	
	void HandleInput()
	{
		KeyCode inputType = CheckForInput();
		if (InputMap.ContainsKey(inputType))
		{
			InputMap[inputType]();
		}
	}
	
	void DoNothing()
	{
	}
	
	void MovePieceLeft()
	{		
		Drop dropScript = CurrentFallingPiece.gameObject.GetComponent<Drop>();
		dropScript.MovePieceLeft();
	}
	
	void MovePieceRight()
	{
		Drop dropScript = CurrentFallingPiece.gameObject.GetComponent<Drop>();
		dropScript.MovePieceRight();
	}
	
	void RotatePieceCW()
	{
		Drop dropScript = CurrentFallingPiece.gameObject.GetComponent<Drop>();
		dropScript.RotatePieceCW();
	}
	
	void RotatePieceCCW()
	{
		Drop dropScript = CurrentFallingPiece.gameObject.GetComponent<Drop>();
		dropScript.RotatePieceCCW();
	}
	
	void MovePieceDown()
	{
		Drop dropScript = CurrentFallingPiece.gameObject.GetComponent<Drop>();
		dropScript.MovePieceDown();
	}
	
	#endregion // input
	
	// Update is called once per frame
	void Update ()
	{
		// TODO -- decide if it's time to drop a piece
		//         handle game pause -- or not?  Maybe something else just pauses this and the Drop script?
		//  TODO -- preview next piece to drop while waiting.
		
		HandleInput();
		
		CheckForCompleteLines();
	}
}
