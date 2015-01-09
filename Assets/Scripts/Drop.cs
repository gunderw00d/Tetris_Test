using UnityEngine;
using System.Collections;

public class Drop : MonoBehaviour {

	public int kWaitFramesBeforeDropping = 10;
	public int FrameCount = 0;
	public float yStep = -1;
	public float BottomYValue = 2;
	public Transform IndividualTile;
	public Transform TileContainer;
	
	public GameObject MainLoopScriptObject;
	
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
				int gridX = 0;
				int gridY = 0;
				mlScript.TranslateCoordtoGridCell(newLoc.x, newLoc.y, out gridX, out gridY);
				if (mlScript.GridCellOccupied(gridX, gridY))
				{
					return false;
				}
			}
		}
		
		return true;
	}
	
	// Update is called once per frame
	void Update()
	{
		// TODO -- input handling - rotate, shift left/right - be sure to test against pieces on board/edges
		
		if (FrameCount <= kWaitFramesBeforeDropping)
		{
			FrameCount += 1;
		}
		else
		{
			FrameCount = 0;
			Vector3 moveDown = new Vector3(0, yStep, 0);
			
			// TODO -- test for intersection with other, loose tiles.
			//         use mlScript!
			bool canMoveDown = AllSpotsOpen(moveDown);
			
			if (canMoveDown)
			{
				transform.Translate(moveDown);
			}
			else
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
						
						int gridX = 0;
						int gridY = 0;
						mlScript.TranslateCoordtoGridCell(newLoc.x, newLoc.y, out gridX, out gridY);
						mlScript.OccupyGridCell(gridX, gridY);
					}
				}
				
		
				Destroy(this.gameObject);
			}
		}
	}
}
