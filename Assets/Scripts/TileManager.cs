// Copyright Greg Underwood, 2015.
// All files in this project, including this one, are covered under the GNU Public License, V3.0.
// See the file gpl-3.0.txt included in this repository for full details of the license.

using UnityEngine;
using System.Collections;

public class TileManager : MonoBehaviour, IModeChanger
{
#region vars
	Grid GridScript;
	Transform[,] Tiles;
#endregion // vars

	public bool AddTile(Transform tile, Vector3 loc)
	{
		bool retVal = false;
		int gridColumn = 0;
		int gridRow = 0;
		GridScript.TranslateCoordtoGridCell(loc.x, loc.y, out gridColumn, out gridRow);
		
		if (GridScript.InGirdBuffer(gridColumn, gridRow))
		{
			retVal = true;
		}
		
		Transform tileInstance = Instantiate(tile, loc, Quaternion.identity) as Transform;
		Tiles[gridRow, gridColumn] = tileInstance;
		
		tileInstance.gameObject.transform.parent = gameObject.transform;
		
		GridScript.OccupyGridCell(gridColumn, gridRow);
		
		return retVal;
	}
	
	public void ClearRow(int row)
	{
		WipeRow(row);
		CompactTiles(row);
	}
	
	void WipeRow(int row)
	{
		for (int column = 0; column < GridScript.BoardWidth; column++)
		{
			if (Tiles[row, column] != null)
			{
				Destroy(Tiles[row, column].gameObject);
				Tiles[row, column] = null;
			}
		}
	}
	
	void CompactTiles(int startGridRow)
	{
		// compact all tiles down 1 row, starting at row above 'startGridRow'
		Vector3 downOne = new Vector3(0f, -1f, 0f);
		
		for (int row = startGridRow; row < (GridScript.BoardHeight - 1); row++)
		{
			int targetRow = row;
			int sourceRow = row + 1;
			
			for (int column = 0; column < GridScript.BoardWidth; column++)
			{
				Tiles[targetRow, column] = Tiles[sourceRow, column];
				Tiles[sourceRow, column] = null;
				if (Tiles[targetRow, column] != null)
				{
					Tiles[targetRow, column].Translate(downOne);
				}
			}
		}
	}
	
	#region IModeChanger
	public void ChangeMode(MainLoop.Mode newMode)
	{
		if (newMode == MainLoop.Mode.StartPlay)
		{
			for (int i = 0; i < GridScript.BoardHeight + GridScript.BoardTopBufferHeight; i++)
			{
				WipeRow(i);
			}
		}
		else if (newMode == MainLoop.Mode.Paused)
		{
			// TODO -- hide all tiles.  gameObject.SetActive(false);  ?
			gameObject.SetActive(false);
		}
		else if (newMode == MainLoop.Mode.Playing)
		{
			// TODO -- unhide tiles.  gameObject.SetActive(true);  ?
			gameObject.SetActive(true);
		}
	}
	
	#endregion IModeChanger
	
	void Start()
	{
		GridScript = gameObject.GetComponent<Grid>();
		
		Tiles = new Transform[GridScript.BoardHeight + GridScript.BoardTopBufferHeight, GridScript.BoardWidth];
		
		for (int row = 0; row < GridScript.BoardHeight + GridScript.BoardTopBufferHeight; row++)
		{
			for (int column = 0; column < GridScript.BoardWidth; column++)
			{
				Tiles[row, column] = null;
			}
		}
	}
	
	void Update ()
	{
	
	}
}
