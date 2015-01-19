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
	public int[] RowsToLevelUp = new int[9];
	
	public int CompletedRows = 0;
	public int Score = 0;
	
	Transform CurrentFallingPiece = null;
	Transform NextPiecePreview = null;
	
	public Transform NextPiecePreviewLocation;
	public Transform StartLocation;
	public Transform TileContainer;
	public Transform Background;
	public Transform GridLocation;
	
	public int CurrentLevel = 0;

	public delegate void dInputAction();
	InputHandler InputHandlerScript;
	
	public bool DEBUG_DisableDrop = false;
	public bool DEBUG_DisplayBuffer = false;
	public bool DEBUG_CreatePiecesRandom = true;
	
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
	
	
	int FindFullRows(bool[] fullRows)
	{
		int retCount = fullRows.Length;
		
		for (int row = 0; row < BoardHeight; row++)
		{
			fullRows[row] = true;
			for (int column = 0; column < BoardWidth; column++)
			{
				if (mBoard[row,column] == false)
				{
					fullRows[row] = false;
					retCount--;
					break;
				}
			}
		}
		
		return retCount;
	}
	
	void CompactBoard(bool[] fullRows)
	{
		int row = 0;
		
		while (row < BoardHeight)
		{
			if (fullRows[row])
			{
				int numFull = 1;
				// Find how many consecutive rows are full
				// remove all tiles in those rows
				// pull everything down.
			}
			
			row++;
		}
	}
	
	void ScoreFullRows(int fullRowCount)
	{
		Score += fullRowCount * 10;	// TODO -- better calculation.
	}
	
	void HandleLevelUp(int fullRowCount)
	{
		CompletedRows += fullRowCount;
		
		if (CurrentLevel < RowsToLevelUp.Length)
		{
			if (RowsToLevelUp[CurrentLevel] < CompletedRows)
			{
				CurrentLevel++;
			}
		}
	}
	
	void ClearCompleteLines()
	{
		bool [] fullRows = new bool[BoardHeight];
		int fullRowCount = FindFullRows(fullRows);
		ScoreFullRows(fullRowCount);
		CompactBoard(fullRows);
		HandleLevelUp(fullRowCount);
	}
	
	#region piece management
	public void DestroyCurrentPiece()
	{
		Destroy(CurrentFallingPiece.gameObject);
		CurrentFallingPiece = null;
		InputHandlerScript.NoPieceFallingInput();
	}
	
	void CreatePiece(int dropOnFrame, Transform piecePrefab)
	{
		Transform newPiece = Instantiate(piecePrefab, NextPiecePreviewLocation.transform.position, Quaternion.identity) as Transform;
		
		Drop dropScript = newPiece.gameObject.GetComponent<Drop>(); 
		dropScript.TileContainer = TileContainer;
		dropScript.MainLoopScriptObject = this.gameObject;
		dropScript.DropOnFrame = dropOnFrame;
		dropScript.HoldInPlace = true;
		
		if (NextPiecePreview != null)
		{
			CurrentFallingPiece = NextPiecePreview;
			CurrentFallingPiece.transform.position = StartLocation.transform.position;
			
			Drop cfpDropScript = CurrentFallingPiece.gameObject.GetComponent<Drop>(); 
			cfpDropScript.HoldInPlace = DEBUG_DisableDrop;

			InputHandlerScript.PieceFallingInput();
		}
		NextPiecePreview = newPiece;
	}
	
	void CreateRandomPiece()
	{
		int pieceIndex = Random.Range(0, PiecePrefabs.Length);
		
		CreatePiece(LevelDropFrames[CurrentLevel], PiecePrefabs[pieceIndex]);
	}
	
	int PieceIndex = 0;
	void CreatePieceInSequence()
	{
		if (PieceIndex >= PiecePrefabs.Length)
		{
			PieceIndex = 0;
		}
		
		CreatePiece(LevelDropFrames[CurrentLevel], PiecePrefabs[PieceIndex]);
		PieceIndex++;
	}
	
	public void CreateNextPiece()
	{
		if (DEBUG_CreatePiecesRandom)
		{
			CreateRandomPiece();
		}
		else
		{
			CreatePieceInSequence();
		}
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
		//	Game Over - Piece decomposed with any part on buffer?  Or all on buffer?  Back to pre-game start mode.
		//	Clear complete lines
		//	Update current drop speed based on # lines completed
		//	DONE - Preview next piece to drop
		//	DONE - Debounce key input
		//	DONE - Rotate pieces
		//	DONE - Disallow pieces hanging off top of board (IE: extend board edges up 3 or 4 more rows.
		
		
		
		ClearCompleteLines();
	}
}
