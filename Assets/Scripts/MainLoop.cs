using UnityEngine;
using System.Collections;


public class MainLoop : MonoBehaviour
{
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
	}
	
	public void DestroyCurrentPiece()
	{
		Destroy(CurrentFallingPiece.gameObject);
		CurrentFallingPiece = null;
	}
	
	void CreateRandomPiece(int dropOnFrame)
	{
		// TODO -- randomize piece selection
		
		CreatePiece(dropOnFrame, PiecePrefabs[0]);
	}
	
	// Update is called once per frame
	void Update ()
	{
		// TODO -- decide if it's time to drop a piece
		//         handle game pause -- or not?  Maybe something else just pauses this and the Drop script?
		//  TODO -- preview next piece to drop while waiting.
		
		
		// TODO -- input handler!
		// PLACEHOLDER/TEST STUFF!
		bool createPiece = Input.GetKeyUp("space") && (CurrentFallingPiece == null);
		
		if (createPiece)
		{
			CreateRandomPiece(LevelDropFrames[CurrentLevel]);
		}
		
		CheckForCompleteLines();
		
		// TODO -- level up - speed up drop rate
	}
}
