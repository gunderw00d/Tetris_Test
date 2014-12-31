﻿using UnityEngine;
using System.Collections;

public class MainLoop : MonoBehaviour
{
	public int BoardHeight;
	public int BoardWidth;
	
	// TODO -- create grid of piece locations... so's we can test for complete rows, etc.
	
	public Transform Piece_BackL;
	public Transform Piece_L;
	public Transform Piece_Long;
	public Transform Piece_S;
	public Transform Piece_Sqaure;
	public Transform Piece_T;
	public Transform Piece_Z;
	
	public Transform StartLocation;
	public Transform TileContainer;
	
	
	
	// Use this for initialization
	void Start ()
	{
		// TOOD -- load board edges and main area 
	
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
	
	//  TODO -- turn this into a function to register individual pieces on the grid.
	public void DoIt()
	{
		Create_Z();
	}
	
	public bool SquareOpen()	// TODO -- test if a given location is available
	{
		return true;	// TODO!
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
