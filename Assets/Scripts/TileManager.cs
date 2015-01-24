using UnityEngine;
using System.Collections;

public class TileManager : MonoBehaviour, IModeChanger
{
#region vars
	Grid GridScript;
	Transform[,] Tiles;
#endregion // vars

	public void AddTile(Transform tile, Vector3 loc)
	{
		int gridColumn = 0;
		int gridRow = 0;
		GridScript.TranslateCoordtoGridCell(loc.x, loc.y, out gridColumn, out gridRow);
		
		Transform tileInstance = Instantiate(tile, loc, Quaternion.identity) as Transform;
		Tiles[gridRow, gridColumn] = tileInstance;
		
		tileInstance.gameObject.transform.parent = gameObject.transform;
		
		GridScript.OccupyGridCell(gridColumn, gridRow);
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
			for (int i = 0; i < GridScript.BoardHeight; i++)
			{
				WipeRow(i);
			}
		}
	}
	
	#endregion IModeChanger
	
	void Start()
	{
		GridScript = gameObject.GetComponent<Grid>();
		
		Tiles = new Transform[GridScript.BoardHeight, GridScript.BoardWidth];
		
		for (int row = 0; row < GridScript.BoardHeight; row++)
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
