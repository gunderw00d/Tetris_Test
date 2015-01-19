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
	
	public bool HoldInPlace;	// "gravity" doesn't affect this piece, leave it be.
	
	public GameObject MainLoopScriptObject;
	#endregion // vars
	
	// Use this for initialization
	void Start()
	{
		FrameCount = 0;
	}
	
	bool SpotOpen(Vector3 worldLoc)
	{
		MainLoop mlScript = MainLoopScriptObject.GetComponent<MainLoop>();
		
		int gridColumn = 0;
		int gridRow = 0;
		mlScript.TranslateCoordtoGridCell(worldLoc.x, worldLoc.y, out gridColumn, out gridRow);
		
		if (mlScript.InGirdBuffer(gridColumn, gridRow))
		{
			return true;
		}
		if (!mlScript.InGridRange(gridColumn, gridRow))
		{
			return false;
		}
		if (mlScript.GridCellOccupied(gridColumn, gridRow))
		{
			return false;
		}
		
		return true;
	}
	
	bool AllSpotsOpen(Vector3 offset)
	{
		Component[] tileXForms = this.gameObject.GetComponentsInChildren<Transform>();
		
		foreach (Transform t in tileXForms)
		{
			if (t.gameObject != this.gameObject)
			{
				Vector3 newLoc = transform.position + t.localPosition + offset;
				
				if (!SpotOpen(newLoc))
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
	
	Vector3 CWRotate = new Vector3(1f, -1f, 1f);
	public void RotatePieceCW()
	{
		// newX = y;
		// newY = -x;
		RotatePiece(CWRotate);
	}
	
	Vector3 CCWRotate = new Vector3(-1f, 1f, 1f);
	public void RotatePieceCCW()
	{
		// newX = -y;
		// newY = x;
		RotatePiece(CCWRotate);
	}
	
	public void RotatePiece(Vector3 offset)
	{
		Component[] tileXForms = this.gameObject.GetComponentsInChildren<Transform>();
		
		foreach (Transform t in tileXForms)
		{
			if (t.gameObject != this.gameObject)
			{
				// swap x and y
				Vector3 newLoc = new Vector3(t.localPosition.y * offset.x, t.localPosition.x * offset.y, t.localPosition.z * offset.z);
				newLoc = transform.position + newLoc;
				
				if (!SpotOpen(newLoc))
				{
					return;
				}
			}
		}
		
		// easier to redo the calc than store each sub-component w/ their new offset and apply after all passed.
		// TODO -- if this proves too slow, figure out a way to cache calc and apply rather than recalc
		
		foreach (Transform t in tileXForms)
		{
			if (t.gameObject != this.gameObject)
			{
				Vector3 newLoc = new Vector3(t.localPosition.y * offset.x, t.localPosition.x * offset.y, t.localPosition.z * offset.z);
				t.localPosition = newLoc;
			}
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		if (HoldInPlace)
		{
			return;
		}
		
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
