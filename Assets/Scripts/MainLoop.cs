using UnityEngine;
using System.Collections;

public class MainLoop : MonoBehaviour
{
	public int BoardHeight;
	public int BoardWidth;
	
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
	
	
	
	// Update is called once per frame
	void Update ()
	{
		bool createPiece = Input.GetKeyUp("space");
		
		if (createPiece)
		{
			Create_BackL();
		}
	}
}
