using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainLoop : MonoBehaviour
{
	#region vars
	public int BoardHeight = 20;
	public int BoardWidth = 10;
	public int BoardTopBufferHeight = 4;
	
	bool[,] mBoard;
	
	public Transform[] PiecePrefabs = new Transform[7];
	public int[] LevelDropFrames = new int[10];
	
	Transform CurrentFallingPiece = null;
	
	public Transform StartLocation;
	public Transform TileContainer;
	public Transform Background;
	public Transform GridLocation;
	
	public int CurrentLevel = 0;

	public delegate void dInputAction();
	InputHandler InputHandlerScript;
	
	public bool DEBUG_DisableDrop = false;
	public bool DEBUG_DisplayBuffer = false;
	
	#endregion // vars

	#region init
	void Start ()
	{
		// Init board with a 1-tile boarder on sides and bottom, top is open, all other slots open
		mBoard = new bool[BoardHeight, BoardWidth];
		
		for (int row = 0; row < BoardHeight; row++)
		{
			for (int column = 0; column < BoardWidth; column++)
			{
				mBoard[row, column] = false;
			}
		}
		
		Random.seed = (int)System.DateTime.Now.Ticks;
		
		InputHandlerScript = gameObject.GetComponent<InputHandler>();
	}
	
	#endregion // init
	
	#region grid utility
	public void TranslateCoordtoGridCell(float x, float y, out int gridColumn, out int gridRow)
	{
		gridColumn = Mathf.FloorToInt(x - GridLocation.position.x);
		gridRow = Mathf.FloorToInt(y - GridLocation.position.y);
	}
	
	public bool InGridRange(int gridColumn, int gridRow)
	{
		return (gridRow >= 0) && (gridRow < BoardHeight) && (gridColumn >= 0) && (gridColumn < BoardWidth);
	}
	
	public bool InGirdBuffer(int gridColumn, int gridRow)
	{
		return (((gridRow >= BoardHeight) && (gridRow < (BoardHeight + BoardTopBufferHeight))) &&
				((gridColumn >= 0) && (gridColumn < BoardWidth)));
	}
	
	public bool GridCellOccupied(int gridColumn, int gridRow)
	{
		return mBoard[gridRow, gridColumn];
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
		
		InputHandlerScript.PieceFallingInput();
	}
	
	public void DestroyCurrentPiece()
	{
		Destroy(CurrentFallingPiece.gameObject);
		CurrentFallingPiece = null;
		InputHandlerScript.NoPieceFallingInput();
	}
	
	// TODO -- temp public - remove after dev
	public void CreateRandomPiece()
	{
		int pieceIndex = Random.Range(0, PiecePrefabs.Length);
		
		CreatePiece(LevelDropFrames[CurrentLevel], PiecePrefabs[pieceIndex]);
	}
	#endregion // piece management
	
	#region input
		
	public void DoNothing()
	{
	}
	
	public void MovePieceLeft()
	{		
		Drop dropScript = CurrentFallingPiece.gameObject.GetComponent<Drop>();
		dropScript.MovePieceLeft();
	}
	
	public void MovePieceRight()
	{
		Drop dropScript = CurrentFallingPiece.gameObject.GetComponent<Drop>();
		dropScript.MovePieceRight();
	}
	
	public void RotatePieceCW()
	{
		Drop dropScript = CurrentFallingPiece.gameObject.GetComponent<Drop>();
		dropScript.RotatePieceCW();
	}
	
	public void RotatePieceCCW()
	{
		Drop dropScript = CurrentFallingPiece.gameObject.GetComponent<Drop>();
		dropScript.RotatePieceCCW();
	}
	
	public void MovePieceDown()
	{
		Drop dropScript = CurrentFallingPiece.gameObject.GetComponent<Drop>();
		dropScript.MovePieceDown();
	}
	
	#endregion // input
	
	// Update is called once per frame
	void Update()
	{
		// TODO list:
		//
		//	Automatic piece drops
		//	Game modes - paused, main menu, playing
		//	Clear complete lines
		//	Update current drop speed based on # lines completed
		//	DONE - Debounce key input
		//	Preview next piece to drop
		//	Rotate pieces
		//	DONE - Disallow pieces hanging off top of board (IE: extend board edges up 3 or 4 more rows.
		
		
		
		CheckForCompleteLines();
	}
}
