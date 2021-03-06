﻿// Copyright Greg Underwood, 2015.
// All files in this project, including this one, are covered under the GNU Public License, V3.0.
// See the file gpl-3.0.txt included in this repository for full details of the license.

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
	
	public delegate void dGameVarWatcherFunc(int oldValue, int newValue);
	
	class GameVar
	{
		public int Value;
		public List<dGameVarWatcherFunc> Watchers;
		
		public GameVar()
		{
			Value = 0;
			Watchers = new List<dGameVarWatcherFunc>();
		}
	};
	
	Dictionary<string, GameVar> GameVars;
	
	public const string kCurrentLevel = "CurrentLevel";
	public const string kCompletedRows = "CompletedRows";
	public const string kScore = "Score";
	
	public Transform[] PiecePrefabs = new Transform[7];
	public int[] LevelDropFrames = new int[10];
	public int[] RowsToLevelUp = new int[9];
	
	Transform CurrentFallingPiece = null;
	Transform NextPiecePreview = null;
	
	public Transform NextPiecePreviewLocation;
	public Transform StartLocation;
	public Transform TileContainer;
	public Transform Background;
	
	public GameObject PauseMenu;
	public GameObject InfoPanel;
	public GameObject StartScreen;

	public delegate void dInputAction(float axisValue);
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
	
	bool PieceSwapped;
		
	public bool DEBUG_DisableDrop = false;
	public bool DEBUG_DisplayBuffer = false;
	public bool DEBUG_CreatePiecesRandom = true;
	#endregion // vars

	#region init
	void Start()
	{
		Random.seed = (int)System.DateTime.Now.Ticks;
		
		InputHandlerScript = gameObject.GetComponent<InputHandler>();

		InitGameModes();
		InitGameVars();
		
		PauseMenu.SetActive(false);
		StartScreen.SetActive(true);
		
		PieceSwapped = false;
	}
	
	void InitGameModes()
	{
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
	
	void InitGameVars()
	{
		GameVars = new Dictionary<string, GameVar>();
		
		GameVars[kCurrentLevel] = new GameVar();
		GameVars[kCompletedRows] = new GameVar();
		GameVars[kScore] = new GameVar();
	}
	#endregion init

	#region GameVar stuff
	void SetGameVar(string varName, int value)
	{
		if (GameVars.ContainsKey(varName))
		{
			int oldValue = GameVars[varName].Value;
			GameVars[varName].Value = value;
			
			foreach (dGameVarWatcherFunc watcher in GameVars[varName].Watchers)
			{
				watcher(oldValue, value);
			}
		}
	}
	
	bool GetGameVar(string varName, out int value)
	{
		value = 0;
		bool exists = GameVars.ContainsKey(varName);
		
		if (exists)
		{
			value = GameVars[varName].Value;
		}

		return exists;
	}
	
	public void RegisterWatcher(string varName, dGameVarWatcherFunc watcherFunc)
	{
		if (GameVars.ContainsKey(varName))
		{
			GameVars[varName].Watchers.Add(watcherFunc);
		}
	}
	#endregion GameVar stuff
	
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
		int score = 0;
		GetGameVar(kScore, out score);
		SetGameVar(kScore, score + fullRowCount * 10);	// TODO -- better calculation.
	}
	
	void HandleLevelUp(int fullRowCount)
	{
		int rows = 0;
		GetGameVar(kCompletedRows, out rows);
		rows += fullRowCount;
		SetGameVar(kCompletedRows, rows);
		
		int curLevel = 0;
		GetGameVar(kCurrentLevel, out curLevel);
		
		if (curLevel < RowsToLevelUp.Length)
		{
			if (RowsToLevelUp[curLevel] < rows)
			{
				curLevel++;
				SetGameVar(kCurrentLevel, curLevel);
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
	#endregion clear rows
	
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
	
	void SwapCurrentAndNext()
	{
		Transform tmp = NextPiecePreview;
		NextPiecePreview = CurrentFallingPiece;
		CurrentFallingPiece = tmp;
		
		Vector3 tmpLoc = NextPiecePreview.transform.position;
		NextPiecePreview.transform.position = CurrentFallingPiece.transform.position;
		CurrentFallingPiece.transform.position = tmpLoc;
		
		Drop nextPieceDropScript = NextPiecePreview.GetComponent<Drop>();
		nextPieceDropScript.HoldInPlace = true;
		
		Drop currentPieceDropScript = CurrentFallingPiece.GetComponent<Drop>();
		currentPieceDropScript.HoldInPlace = DEBUG_DisableDrop;
	}
	
	void CreateRandomPiece()
	{
		int pieceIndex = Random.Range(0, PiecePrefabs.Length);
		int curLevel = 0;
		GetGameVar(kCurrentLevel, out curLevel);
		
		CreatePiece(LevelDropFrames[curLevel], PiecePrefabs[pieceIndex]);
	}
	
	int PieceIndex = 0;
	void CreatePieceInSequence()
	{
		if (PieceIndex >= PiecePrefabs.Length)
		{
			PieceIndex = 0;
		}
		
		int curLevel = 0;
		GetGameVar(kCurrentLevel, out curLevel);
		CreatePiece(LevelDropFrames[curLevel], PiecePrefabs[PieceIndex]);
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
		
	public void DoNothing(float axisValue)
	{
	}
	
	public void MovePiece(float axisValue)
	{
		
		if (CurrentFallingPiece != null)
		{
			if (axisValue > 0f)
			{
				Drop dropScript = CurrentFallingPiece.gameObject.GetComponent<Drop>();
				dropScript.MovePieceRight();
			}
			else	// < 0f
			{
				Drop dropScript = CurrentFallingPiece.gameObject.GetComponent<Drop>();
				dropScript.MovePieceLeft();
			}
		}
	}
	
	public void RotatePiece(float axisValue)
	{
		if (CurrentFallingPiece != null)
		{
			if (axisValue > 0f)
			{
				Drop dropScript = CurrentFallingPiece.gameObject.GetComponent<Drop>();
				dropScript.RotatePieceCW();
			}
			else	// < 0f
			{
				Drop dropScript = CurrentFallingPiece.gameObject.GetComponent<Drop>();
				dropScript.RotatePieceCCW();
			}
		}
	}
		
	public void MovePieceDown(float axisValue)
	{
		if (CurrentFallingPiece != null)
		{
			Drop dropScript = CurrentFallingPiece.gameObject.GetComponent<Drop>();
			dropScript.MovePieceDown();
		}
	}
	#endregion movement input
	
	#region menu input
	public void StartPressed(float axisValue)
	{
		// From StartScreen, or GameOver, Start was pressed
		ChangeMode(Mode.StartPlay);
	}
	
	public void ExitPressed(float axisValue)
	{
		Application.Quit();
	}
	
	public void PausePressed(float axisValue)
	{
		ChangeMode(Mode.Paused);
		if (CurrentFallingPiece != null)
		{
			CurrentFallingPiece.gameObject.SetActive(false);
		}
		if (NextPiecePreview != null)
		{
			NextPiecePreview.gameObject.SetActive(false);
		}
		
		PauseMenu.SetActive(true);
	}
	
	public void ResumePressed(float axisValue)
	{
		ChangeMode(Mode.Playing);
		if (CurrentFallingPiece != null)
		{
			CurrentFallingPiece.gameObject.SetActive(true);
		}
		if (NextPiecePreview != null)
		{
			NextPiecePreview.gameObject.SetActive(true);
		}
		PauseMenu.SetActive(false);
	}
	
	public void SwapPressed(float axisValue)
	{
		if (PieceSwapped == false)
		{
			Drop nextPieceDropScript = NextPiecePreview.GetComponent<Drop>();
			
			bool canSwap = nextPieceDropScript.WouldFit(CurrentFallingPiece.transform.position);
			
			if (canSwap)
			{
				SwapCurrentAndNext();	
				PieceSwapped = true;
			}
		}
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
		audio.Play();
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
		
		SetGameVar(kScore, 0);
		SetGameVar(kCurrentLevel, 0);
		SetGameVar(kCompletedRows, 0);
		
		StartScreen.SetActive(false);
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
			
			PieceSwapped = false;		// Allow further piece swaps.
		}
	}
	
	void GameMode_Paused()
	{
	}
	
	void GameMode_GameOver()
	{
		// TODO - Trigger GameOver sound
		StartScreen.SetActive(true);
	}
	
	#endregion game mode funcs
	
	// Update is called once per frame
	void Update()
	{
		// TODO list:
		//
		//	Sound.
		//	Swap preview piece.
		
		GameModeFuncs[GameMode]();
	}
}
