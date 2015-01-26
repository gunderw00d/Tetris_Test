using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IModeChanger
{
	void ChangeMode(MainLoop.Mode newMode);
};

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

	public enum Mode
	{
		StartScreen = 0,
		StartPlay = 1,
		Playing = 2,
		Paused = 3,
		GameOver = 4
	};
	
	Mode GameMode;
	
	delegate void dGameModeFunc();
	Dictionary<Mode, dGameModeFunc> GameModeFuncs;
	
	List<IModeChanger> ModeChangers;
		
	public bool DEBUG_DisableDrop = false;
	public bool DEBUG_DisplayBuffer = false;
	public bool DEBUG_CreatePiecesRandom = true;
	#endregion // vars

	#region init
	void Start()
	{
		Random.seed = (int)System.DateTime.Now.Ticks;
		
		InputHandlerScript = gameObject.GetComponent<InputHandler>();
		
		GameMode = Mode.StartScreen;
		
		GameModeFuncs = new Dictionary<Mode, dGameModeFunc>();
		GameModeFuncs[Mode.StartScreen] = GameMode_StartScreen;
		GameModeFuncs[Mode.StartPlay] = GameMode_StartPlay;
		GameModeFuncs[Mode.Playing] = GameMode_Playing;
		GameModeFuncs[Mode.Paused] = GameMode_Paused;
		GameModeFuncs[Mode.GameOver] = GameMode_GameOver;
		
		TileManager tmScript = TileContainer.gameObject.GetComponent<TileManager>();
		Grid gScript = TileContainer.gameObject.GetComponent<Grid>();
		
		ModeChangers = new List<IModeChanger>();
		ModeChangers.Add(InputHandlerScript);
		ModeChangers.Add(tmScript);
		ModeChangers.Add(gScript);
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
			ScoreFullRows(fullRowCount);
			HandleLevelUp(fullRowCount);
		}
	}
	#endregion // clear rows
	
	#region piece management
	void DestroyPiece(Transform piece)
	{
		Drop dropScript = piece.gameObject.GetComponent<Drop>();
		ModeChangers.Remove(dropScript);
		
		Destroy(piece.gameObject);
	}
	
	void CreatePiece(int dropOnFrame, Transform piecePrefab)
	{
		Transform newPiece = Instantiate(piecePrefab, NextPiecePreviewLocation.transform.position, Quaternion.identity) as Transform;
		
		Drop dropScript = newPiece.gameObject.GetComponent<Drop>(); 
		dropScript.GridScript = TileContainer.gameObject.GetComponent<Grid>();
		dropScript.TileManagerScript = TileContainer.GetComponent<TileManager>();
		dropScript.DropOnFrame = dropOnFrame;
		dropScript.HoldInPlace = true;
		ModeChangers.Add(dropScript);
		
		if (NextPiecePreview != null)
		{
			CurrentFallingPiece = NextPiecePreview;
			CurrentFallingPiece.transform.position = StartLocation.transform.position;
			
			Drop cfpDropScript = CurrentFallingPiece.gameObject.GetComponent<Drop>(); 
			cfpDropScript.HoldInPlace = DEBUG_DisableDrop;
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
	
	void CreateNextPiece()
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
	
	#region movement input
		
	public void DoNothing()
	{
	}
	
	public void MovePieceLeft()
	{		
		if (CurrentFallingPiece != null)
		{
			Drop dropScript = CurrentFallingPiece.gameObject.GetComponent<Drop>();
			dropScript.MovePieceLeft();
		}
	}
	
	public void MovePieceRight()
	{
		if (CurrentFallingPiece != null)
		{
			Drop dropScript = CurrentFallingPiece.gameObject.GetComponent<Drop>();
			dropScript.MovePieceRight();
		}
	}
	
	public void RotatePieceCW()
	{
		if (CurrentFallingPiece != null)
		{
			Drop dropScript = CurrentFallingPiece.gameObject.GetComponent<Drop>();
			dropScript.RotatePieceCW();
		}
	}
	
	public void RotatePieceCCW()
	{
		if (CurrentFallingPiece != null)
		{
			Drop dropScript = CurrentFallingPiece.gameObject.GetComponent<Drop>();
			dropScript.RotatePieceCCW();
		}
	}
	
	public void MovePieceDown()
	{
		if (CurrentFallingPiece != null)
		{
			Drop dropScript = CurrentFallingPiece.gameObject.GetComponent<Drop>();
			dropScript.MovePieceDown();
		}
	}
	#endregion movement input
	
	#region menu input
	public void StartPressed()
	{
		// From StartScreen, or GameOver, Start was pressed
		ChangeMode(Mode.StartPlay);
	}
	
	public void ExitPressed()
	{
		// TODO -- quit.
	}
	
	public void PausePressed()
	{
		ChangeMode(Mode.Paused);
	}
	
	public void ResumePressed()
	{
		ChangeMode(Mode.Playing);
	}
	
	#endregion menu input
	
	void ChangeMode(Mode newMode)
	{
		GameMode = newMode;
		
		// TODO -- need to prevent mode change if !AllChangersReady()?
		
		foreach (IModeChanger mc in ModeChangers)
		{
			mc.ChangeMode(newMode);
		}
	}
		
	#region game mode funcs
	void GameMode_StartScreen()
	{
	}
	
	void GameMode_StartPlay()
	{
		if (CurrentFallingPiece != null)
		{
			DestroyPiece(CurrentFallingPiece);
			CurrentFallingPiece = null;
		}
		
		if (NextPiecePreview != null)
		{
			DestroyPiece(NextPiecePreview);
			NextPiecePreview = null;
		}
		
		ChangeMode(Mode.Playing);
	}
	
	void GameMode_Playing()
	{
		// TODO:
		//	Consider - swap current tile for previewed.
		//				Can always do it?
		//				Have to earn by getting Tetris(es)?
		//				Where does piece swap to - current location, or top?  (current, seems most fair)
		
		while (CurrentFallingPiece == null)
		{
			CreateNextPiece();
		}
		
		Drop dropScript = CurrentFallingPiece.GetComponent<Drop>();
		if (dropScript.AtBottom)
		{
			bool tilesInBuffer = dropScript.DecomposePiece();
			
			DestroyPiece(CurrentFallingPiece);
			CurrentFallingPiece = null;
			
			if (tilesInBuffer)
			{
				ChangeMode(Mode.GameOver);
			}
			else
			{
				ClearCompleteLines();
			}
		}
	}
	
	void GameMode_Paused()
	{
		// TODO -- tell current falling piece to stop moving
	}
	
	void GameMode_GameOver()
	{
		// TODO -- should be decomposed... and no new falling piece, so... do nothing?
	}
	
	#endregion game mode funcs
	
	// Update is called once per frame
	void Update()
	{
		// TODO list:
		//
		//	IN PROGRESS - Game modes - paused, main menu, playing
		//					TODO - Pause, Exit.
		//	Menus!
		//	Display Score, lines, level.
		//	Sound.
		//	Swap preview piece.
		
		GameModeFuncs[GameMode]();
	}
}
