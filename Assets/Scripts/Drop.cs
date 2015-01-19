using UnityEngine;
using System.Collections;

public class Drop : MonoBehaviour
{
	#region vars
	public int DropOnFrame = 10;
	public int FrameCount = 0;
	public float yStep = -1;
	public float BottomYValue = 2;
	public Transform IndividualTile;
	public Transform TileContainer;
	
	public GameObject MainLoopScriptObject;
	#endregion // vars
	
	// Use this for initialization
	void Start()
	{
		FrameCount = 0;
	}
	
	bool AllSpotsOpen(Vector3 offset)
	{
		Component[] tileXForms = this.gameObject.GetComponentsInChildren<Transform>();
		MainLoop mlScript = MainLoopScriptObject.GetComponent<MainLoop>();
		
		foreach (Transform t in tileXForms)
		{
			if (t.gameObject != this.gameObject)
			{
				Vector3 newLoc = transform.position + t.localPosition + offset;
				int gridColumn = 0;
				int gridRow = 0;
				mlScript.TranslateCoordtoGridCell(newLoc.x, newLoc.y, out gridColumn, out gridRow);
				if (mlScript.InGirdBuffer(gridColumn, gridRow))
				{
					continue;
				}
				if (!mlScript.InGridRange(gridColumn, gridRow))
				{
					return false;
				}
				if (mlScript.GridCellOccupied(gridColumn, gridRow))
				{
					return false;
				}
			}
		}
		
		return true;
	}
	
	void DecomposePiece()
	{
		Component[] tileXForms = this.gameObject.GetComponentsInChildren<Transform>();
		MainLoop mlScript = MainLoopScriptObject.GetComponent<MainLoop>();
		
		foreach (Transform t in tileXForms)
		{
			if (t.gameObject != this.gameObject)
			{
				Vector3 newLoc = transform.position + t.localPosition;
				Transform tileInstance = Instantiate(IndividualTile, newLoc, Quaternion.identity) as Transform;
				tileInstance.gameObject.transform.parent = TileContainer.gameObject.transform;
				
				int gridColumn = 0;
				int gridRow = 0;
				mlScript.TranslateCoordtoGridCell(newLoc.x, newLoc.y, out gridColumn, out gridRow);
				mlScript.OccupyGridCell(gridColumn, gridRow);
			}
		}
		
		
		mlScript.DestroyCurrentPiece();
	}
	
	public void MovePieceDown()
	{
		MainLoop mlScript = MainLoopScriptObject.GetComponent<MainLoop>();
		if (mlScript.DEBUG_DisableDrop)
		{
			return;
		}
		
		Vector3 moveDown = new Vector3(0, yStep, 0);
		
		bool canMoveDown = AllSpotsOpen(moveDown);
		
		if (canMoveDown)
		{
			transform.Translate(moveDown);
		}
		else
		{
			DecomposePiece();
		}
	}
	
	public void MovePieceLeft()
	{
		Vector3 moveLeft = new Vector3(-1, 0, 0);
		
		bool canMoveLeft = AllSpotsOpen(moveLeft);
		
		if (canMoveLeft)
		{
			transform.Translate(moveLeft);
		}
	}
	
	public void MovePieceRight()
	{
		Vector3 moveRight = new Vector3(1, 0, 0);
		
		bool canMoveRight = AllSpotsOpen(moveRight);
		
		if (canMoveRight)
		{
			transform.Translate(moveRight);
		}
	}
	
	public void RotatePieceCW()
	{
		// TODO
	}
	
	public void RotatePieceCCW()
	{
		// TODO
	}
	
	// Update is called once per frame
	void Update()
	{
		if (FrameCount <= DropOnFrame)
		{
			FrameCount += 1;
		}
		else
		{
			FrameCount = 0;
			MovePieceDown();
		}
	}
}
