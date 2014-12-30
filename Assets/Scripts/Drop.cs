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
	
	// Update is called once per frame
	void Update()
	{
		// TODO -- input handling - rotate, shift left/right
		// TODO -- needs to know what other tiles are on the board.
		
		if (FrameCount <= kWaitFramesBeforeDropping)
		{
			FrameCount += 1;
		}
		else
		{
			FrameCount = 0;
			float tempY = transform.position.y;
			
			// TODO -- test for intersection with other, loose tiles.
			if (tempY > BottomYValue)
			{
				transform.Translate(0, yStep, 0);
			}
			else
			{
				Component[] tileXForms;
			
				tileXForms = this.gameObject.GetComponentsInChildren<Transform>();
				foreach (Transform t in tileXForms)
				{
					if (t.gameObject != this.gameObject)
					{
						Vector3 newLoc = transform.position;
						newLoc = newLoc + t.localPosition;
						Transform tileInstance = Instantiate(IndividualTile, newLoc, Quaternion.identity) as Transform;
						tileInstance.gameObject.transform.parent = TileContainer.gameObject.transform;
					}
				}
		
				Destroy(this.gameObject);
			}
		}
	}
}
