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

	InputHandler InputHandlerScript;
	
	public delegate void InputAction(int val);
	
	Dictionary<InputHandler.InputType, InputAction> InputMap;
	Dictionary<InputHandler.InputType, int> InputActionData;
	
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
	}
	
	void InitInputHandling()
	{
		InputHandlerScript = gameObject.GetComponent<InputHandler>();
		
		InputMap = new Dictionary<InputHandler.InputType, InputAction>();
		InputActionData = new Dictionary<InputHandler.InputType, int>();
		
		InputMap[InputHandler.InputType.Space_Up] = CreateRandomPiece;
		InputMap[InputHandler.InputType.None] = DoNothing;
		
		UpdateInputData();
	}
	
	void PieceFallingInput()
	{
		InputMap[InputHandler.InputType.Space_Up] = DoNothing;
	}
	
	void NoPieceFallingInput()
	{
		InputMap[InputHandler.InputType.Space_Up] = CreateRandomPiece;
	}
	
	void UpdateInputData()
	{
		InputActionData[InputHandler.InputType.Space_Up] = LevelDropFrames[CurrentLevel];
		InputActionData[InputHandler.InputType.None] = 0;
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
		
		UpdateInputData();
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
	
	void CreateRandomPiece(int dropOnFrame)
	{
		int pieceIndex = Random.Range(0, PiecePrefabs.Length);
		
		CreatePiece(dropOnFrame, PiecePrefabs[pieceIndex]);
	}
	#endregion // piece management
	
	void DoNothing(int ignore)
	{
	}
	
	void HandleInput()
	{
		InputHandler.InputType inputType = InputHandlerScript.CheckForInput();
		InputMap[inputType](InputActionData[inputType]);
	}
	
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
