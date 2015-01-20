using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class LooseTileManager
{
	#region vars
	public Transform TileContainer;
	public MainLoop mlScript;
	#endregion // vars
	
	Transform FindRowContainer(int row)
	{
		Component[] rowContainerXForms = TileContainer.gameObject.GetComponentsInChildren<Transform>();
		
		foreach (Transform t in rowContainerXForms)
		{
			if (t.gameObject != TileContainer.gameObject)
			{
				if ((int)t.localPosition.y == row)
				{
					return t;
				}
			}
		}
		
		return TileContainer;
	}
	
	public void AddTile(Transform tileInstance, Vector3 loc)
	{
		int gridColumn = 0;
		int gridRow = 0;
		mlScript.TranslateCoordtoGridCell(loc.x, loc.y, out gridColumn, out gridRow);
		
		Transform rowContainer = FindRowContainer(gridRow);

		tileInstance.gameObject.transform.parent = rowContainer.gameObject.transform;
		
		mlScript.OccupyGridCell(gridColumn, gridRow);
	}
	
	public void ClearRow(int row)
	{
		Transform rowContainer = FindRowContainer(row);
		
		Component[] tiles = rowContainer.gameObject.GetComponentsInChildren<Transform>();
		
		foreach (Transform t in tiles)
		{
			if (t.gameObject != rowContainer.gameObject)
			{
				mlScript.DestroyTile(t);
			}
		}
	}
	
	public void CompactTiles(int startGridRow)
	{
		// TODO -- compact all tiles down 1 row, starting at row above 'startGridRow'
		
		for (int row = startGridRow; row < (mlScript.BoardHeight - 2); row++)
		{
			Transform targetRow = FindRowContainer(row);
			Transform sourceRow = FindRowContainer(row + 1);
			
			Component[] sourceTiles = sourceRow.gameObject.GetComponentsInChildren<UnityEngine.Transform>();
			
			foreach (UnityEngine.Transform t in sourceTiles)
			{
				if (t.gameObject != sourceRow.gameObject)
				{
					t.gameObject.transform.parent = targetRow.gameObject.transform;
					
					// TODO -- do I need to translate the tiles down by 1?
				}
			}
		}
		
	}
	
};

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
	
	public int CurrentLevel = 0;
	public int CompletedRows = 0;
	public int Score = 0;
	
	Transform CurrentFallingPiece = null;
	Transform NextPiecePreview = null;
	
	public Transform NextPiecePreviewLocation;
	public Transform StartLocation;
	public Transform TileContainer;
	public Transform Background;
	public Transform GridLocation;
	
	LooseTileManager TileManager;

	public delegate void dInputAction();
	InputHandler InputHandlerScript;
	
	public bool DEBUG_DisableDrop = false;
	public bool DEBUG_DisplayBuffer = false;
	public bool DEBUG_CreatePiecesRandom = true;
	
	#endregion // vars

	#region init
	void Start()
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
		
		TileManager = new LooseTileManager();
		
		TileManager.TileContainer = TileContainer;
		TileManager.mlScript = this;
	}
	
	#endregion // init
	
	#region grid utility
	public void TranslateCoordtoGridCell(float x, float y, out int gridColumn, out int gridRow)
	{
		gridColumn = Mathf.FloorToInt(x - GridLocation.position.x);
		gridRow = Mathf.FloorToInt(y - GridLocation.position.y);
	}
	
	void TranslateGridCelltoCoord(int gridColumn, int gridRow, out float x, out float y)
	{
		x = GridLocation.position.x + gridColumn;
		y = GridLocation.position.y + gridRow;
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
	
	void ClearGridCell(int gridColumn, int gridRow)
	{
		if (InGridRange(gridColumn, gridRow))
		{
			mBoard[gridRow, gridColumn] = false;
		}
	}
	
	void ClearGridRow(int gridRow)
	{
		if ((gridRow >= 0) && (gridRow < BoardHeight))
		{
			for (int column = 0; column < BoardWidth; column++)
			{
				mBoard[gridRow, column] = false;
			}
		}
	}
	
	void CompactGrid(int startGridRow)
	{
		// TODO - compact all rows down by one, starting at startGridRow and working up
	}
	
	#endregion // grid utility
	
	#region clear rows
	public void CreateTile(Transform tile, Vector3 loc)
	{
		Transform tileInstance = Instantiate(tile, loc, Quaternion.identity) as Transform;
		TileManager.AddTile(tileInstance, loc);
	}
	
	public void DestroyTile(Transform tileInstance)
	{
		Destroy(tileInstance);
	}
	
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
	
	void RemoveFullRows(bool[] fullRows)
	{
		int row = BoardHeight - 1;
		
		while (row >= 0)
		{
			if (fullRows[row])
			{
				ClearGridRow(row);
				CompactGrid(row);
				TileManager.ClearRow(row);
				TileManager.CompactTiles(row);
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
		bool [] fullRows = new bool[BoardHeight];
		int fullRowCount = FindFullRows(fullRows);
		if (fullRowCount > 0)
		{
			RemoveFullRows(fullRows);
			//ScoreFullRows(fullRowCount);
			//HandleLevelUp(fullRowCount);
		}
	}
	#endregion // clear rows
	
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
