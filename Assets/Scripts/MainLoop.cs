using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MainLoop : MonoBehaviour
{
	#region vars
	public Transform[] PiecePrefabs = new Transform[7];
	public int[] LevelDropFrames = new int[10];
	public int[] RowsToLevelUp = new int[9];
	
	public int CurrentLevel = 0;
	public int CompletedRows = 0;
	public int Score = 0;
	
	Transform CurrentFallingPiece = null;
	Transform NextPiecePreview = null;
	
	public Transform NextPiecePreviewLocation;
	public Transform StartLocation;
	public Transform TileContainer;
	public Transform Background;

	public delegate void dInputAction();
	InputHandler InputHandlerScript;
	
	public bool DEBUG_DisableDrop = false;
	public bool DEBUG_DisplayBuffer = false;
	public bool DEBUG_CreatePiecesRandom = true;
	
	#endregion // vars

	#region init
	void Start()
	{
		Random.seed = (int)System.DateTime.Now.Ticks;
		
		InputHandlerScript = gameObject.GetComponent<InputHandler>();
	}
	#endregion // init
	
	
	#region clear rows	
	
	void RemoveFullRows(bool[] fullRows)
	{
		Grid grid = TileContainer.GetComponent<Grid>();
		int row = grid.BoardHeight - 1;
		TileManager tm = TileContainer.GetComponent<TileManager>();
		
		while (row >= 0)
		{
			if (fullRows[row])
			{
				grid.ClearGridRow(row);
				tm.ClearRow(row);
			}
			
			row--;
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
		Grid grid = TileContainer.GetComponent<Grid>();
		bool [] fullRows = new bool[grid.BoardHeight];
		int fullRowCount = grid.FindFullRows(fullRows);
		if (fullRowCount > 0)
		{
			RemoveFullRows(fullRows);
			//ScoreFullRows(fullRowCount);
			//HandleLevelUp(fullRowCount);
		}
	}
	#endregion // clear rows
	
	#region piece management
	void DestroyCurrentPiece()
	{
		Destroy(CurrentFallingPiece.gameObject);
		CurrentFallingPiece = null;
		InputHandlerScript.NoPieceFallingInput();
	}
	
	void CreatePiece(int dropOnFrame, Transform piecePrefab)
	{
		Transform newPiece = Instantiate(piecePrefab, NextPiecePreviewLocation.transform.position, Quaternion.identity) as Transform;
		
		Drop dropScript = newPiece.gameObject.GetComponent<Drop>(); 
		dropScript.GridScript = TileContainer.gameObject.GetComponent<Grid>();
		dropScript.TileManagerScript = TileContainer.GetComponent<TileManager>();
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
		//	DONE - Clear complete lines
		//	Update current drop speed based on # lines completed
		//	DONE - Preview next piece to drop
		//	DONE - Debounce key input
		//	DONE - Rotate pieces
		//	DONE - Disallow pieces hanging off top of board (IE: extend board edges up 3 or 4 more rows.
		
		
		if (CurrentFallingPiece != null)
		{
			Drop dropScript = CurrentFallingPiece.GetComponent<Drop>();
			if (dropScript.AtBottom)
			{
				dropScript.DecomposePiece();
				DestroyCurrentPiece();
				ClearCompleteLines();
			}
		}
		
	}
}
