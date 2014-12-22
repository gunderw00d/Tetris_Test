using UnityEngine;
using System.Collections;

public class Drop : MonoBehaviour {

	public int kWaitFramesBeforeDropping = 10;
	public int FrameCount = 0;
	public float yStep = -1;
	public float BottomYValue = 2;
	
	// Use this for initialization
	void Start ()
	{
		FrameCount = 0;
	}
	
	// Update is called once per frame
	void Update ()
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
			
			if (tempY > BottomYValue)
			{
				transform.Translate(0, yStep, 0);
			}
			// else -- TODO -- instantiate a set of separate tiles where this piece's tiles are
			//      -- TODO -- demolish this piece.
			//      -- TODO -- write script to look for completed rows & handle removal.
		}
	}
}
