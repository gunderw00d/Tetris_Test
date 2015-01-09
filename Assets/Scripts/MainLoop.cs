﻿using UnityEngine;
using System.Collections;


public class MainLoop : MonoBehaviour
{
	public int BoardHeight = 21;
	public int BoardWidth = 12;
	
	bool[,] mBoard;
	
	
	public Transform Piece_BackL;
	public Transform Piece_L;
	public Transform Piece_Long;
	public Transform Piece_S;
	public Transform Piece_Sqaure;
	public Transform Piece_T;
	public Transform Piece_Z;
	
	public Transform BorderRow;
	public Transform BorderedEdgesRow;
	
	public Transform StartLocation;
	public Transform TileContainer;
	public Transform Background;
	
		
	
	void Start ()
	{
		// Init board with a 1-tile boarder on sides and bottom, top is open, all other slots open
		mBoard = new bool[BoardHeight, BoardWidth];
		
		Vector3 loc = Background.position;
		
		CreateBorderRow(loc, 0);
		
		for (int i = 1; i < BoardHeight; i++)
		{
			loc.y += 1;
			CreateBorderedEdgesRow(loc, i);
		}
	}
		
	void CreateBorderRow(Vector3 loc, int rowNumber)
	{
		Transform borderRow = Instantiate(BorderRow, loc, Quaternion.identity) as Transform;		
		borderRow.gameObject.transform.parent = Background.gameObject.transform;
		
		for (int i = 0; i < BoardWidth; i++) // Bottom row is all border
		{
			mBoard[rowNumber, i] = true;
		}
	}
	
	void CreateBorderedEdgesRow(Vector3 loc, int rowNum)
	{
		Transform borderedEdgesRow = Instantiate(BorderedEdgesRow, loc, Quaternion.identity) as Transform;		
		borderedEdgesRow.gameObject.transform.parent = Background.gameObject.transform;
		
		mBoard[rowNum, 0] = true;
		for (int j = 1; j < (BoardWidth - 1); j++)
		{
			mBoard[rowNum, j] = false;
		}
		mBoard[rowNum, BoardWidth - 1] = true;
	}
	
	public void TranslateCoordtoGridCell(float x, float y, out int gridX, out int gridY)
	{
		
		gridX = Mathf.FloorToInt(x - Background.position.x);
		gridY = Mathf.FloorToInt(y - Background.position.y);
	}
	
	public bool GridCellOccupied(int gridX, int gridY)
	{
		if ((gridY >= 0) && (gridY < BoardHeight))
		{
			return mBoard[gridY, gridX];
		}
		else
		{
			return false;	// Grid isn't there, so... no, not occupied.
		}
	}
	
	public void OccupyGridCell(int gridX, int gridY)
	{
		mBoard[gridY, gridX] = true;
	}
	
	void Create_BackL()
	{
		Transform newPiece = Instantiate(Piece_BackL, StartLocation.transform.position, Quaternion.identity) as Transform;
		Drop dropScript = newPiece.gameObject.GetComponent<Drop>(); 
		dropScript.TileContainer = TileContainer;
		dropScript.MainLoopScriptObject = this.gameObject;
	}
	
	void Create_L()
	{
		Transform newPiece = Instantiate(Piece_L, StartLocation.transform.position, Quaternion.identity) as Transform;
		Drop dropScript = newPiece.gameObject.GetComponent<Drop>(); 
		dropScript.TileContainer = TileContainer;
		dropScript.MainLoopScriptObject = this.gameObject;
	}
	
	void Create_Long()
	{
		Transform newPiece = Instantiate(Piece_Long, StartLocation.transform.position, Quaternion.identity) as Transform;
		Drop dropScript = newPiece.gameObject.GetComponent<Drop>(); 
		dropScript.TileContainer = TileContainer;
		dropScript.MainLoopScriptObject = this.gameObject;
	}
	
	void Create_S()
	{
		Transform newPiece = Instantiate(Piece_S, StartLocation.transform.position, Quaternion.identity) as Transform;
		Drop dropScript = newPiece.gameObject.GetComponent<Drop>(); 
		dropScript.TileContainer = TileContainer;
		dropScript.MainLoopScriptObject = this.gameObject;
	}
	
	void Create_Square()
	{
		Transform newPiece = Instantiate(Piece_Sqaure, StartLocation.transform.position, Quaternion.identity) as Transform;
		Drop dropScript = newPiece.gameObject.GetComponent<Drop>(); 
		dropScript.TileContainer = TileContainer;
		dropScript.MainLoopScriptObject = this.gameObject;
	}
	
	void Create_T()
	{
		Transform newPiece = Instantiate(Piece_T, StartLocation.transform.position, Quaternion.identity) as Transform;
		Drop dropScript = newPiece.gameObject.GetComponent<Drop>(); 
		dropScript.TileContainer = TileContainer;
		dropScript.MainLoopScriptObject = this.gameObject;
	}
	
	void Create_Z()
	{
		Transform newPiece = Instantiate(Piece_Z, StartLocation.transform.position, Quaternion.identity) as Transform;
		Drop dropScript = newPiece.gameObject.GetComponent<Drop>(); 
		dropScript.TileContainer = TileContainer;
		dropScript.MainLoopScriptObject = this.gameObject;
	}
		
	void CheckForCompleteLines()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
		// TODO -- decide if it's time to drop a piece
		//         handle game pause -- or not?  Maybe something else just pauses this and the Drop script?
		//  TODO -- preview next piece to drop while waiting.
		
		
		// PLACEHOLDER/TEST STUFF!
		bool createPiece = Input.GetKeyUp("space");
		
		if (createPiece)
		{
			Create_BackL();
		}
		
		CheckForCompleteLines();
		
		// TODO -- level up - speed up drop rate
	}
}
