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
	
	public bool HoldInPlace;	// "gravity" doesn't affect this piece, leave it be.
	public bool AtBottom;
	
	public Grid GridScript;
	public TileManager TileManagerScript;
	#endregion // vars
	
	void Start()
	{
		FrameCount = 0;
		AtBottom = false;
	}
	
	bool AllSpotsOpen(Vector3 offset)
	{
		Component[] tileXForms = gameObject.GetComponentsInChildren<Transform>();
		
		foreach (Transform t in tileXForms)
		{
			if (t.gameObject != gameObject)
			{
				Vector3 newLoc = transform.position + t.localPosition + offset;
				
				if (!GridScript.SpotOpen(newLoc))
				{
					return false;
				}
			}
		}
		
		return true;
	}
	
	public void DecomposePiece()
	{
		Component[] tileXForms = gameObject.GetComponentsInChildren<Transform>();
		
		foreach (Transform t in tileXForms)
		{
			if (t.gameObject != gameObject)
			{
				Vector3 newLoc = transform.position + t.localPosition;
				TileManagerScript.AddTile(IndividualTile, newLoc);
			}
		}
	}
	
	#region movement
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
			AtBottom = true;
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
		Component[] tileXForms = gameObject.GetComponentsInChildren<Transform>();
		
		foreach (Transform t in tileXForms)
		{
			if (t.gameObject != gameObject)
			{
				// swap x and y
				Vector3 newLoc = new Vector3(t.localPosition.y * offset.x, t.localPosition.x * offset.y, t.localPosition.z * offset.z);
				newLoc = transform.position + newLoc;
				
				if (!GridScript.SpotOpen(newLoc))
				{
					return;
				}
			}
		}
		
		// easier to redo the calc than store each sub-component w/ their new offset and apply after all passed.
		// TODO -- if this proves too slow, figure out a way to cache calc and apply rather than recalc
		
		foreach (Transform t in tileXForms)
		{
			if (t.gameObject != gameObject)
			{
				Vector3 newLoc = new Vector3(t.localPosition.y * offset.x, t.localPosition.x * offset.y, t.localPosition.z * offset.z);
				t.localPosition = newLoc;
			}
		}
	}
	#endregion // movement
	
	void Update()
	{
		if (HoldInPlace)
		{
			return;
		}
		
		if (!AtBottom)
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
}
