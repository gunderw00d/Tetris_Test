// Copyright Greg Underwood, 2015.
// All files in this project, including this one, are covered under the GNU Public License, V3.0.
// See the file gpl-3.0.txt included in this repository for full details of the license.

using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour, IModeChanger
{
	#region vars
	public int BoardHeight = 20;
	public int BoardWidth = 10;
	public int BoardTopBufferHeight = 4;
	
	bool[,] mBoard;
	#endregion vars



	public void TranslateCoordtoGridCell(float x, float y, out int gridColumn, out int gridRow)
	{
		gridColumn = Mathf.FloorToInt(x - gameObject.transform.position.x);
		gridRow = Mathf.FloorToInt(y - gameObject.transform.position.y);
	}
	
//	void TranslateGridCelltoCoord(int gridColumn, int gridRow, out float x, out float y)
//	{
//		x = gameObject.transform.position.x + gridColumn;
//		y = gameObject.transform.position.y + gridRow;
//	}
	
	public bool InGridRange(int gridColumn, int gridRow)
	{
		return (gridRow >= 0) && (gridRow < BoardHeight) && (gridColumn >= 0) && (gridColumn < BoardWidth);
	}
	
	public bool InGirdBuffer(int gridColumn, int gridRow)
	{
		return (((gridRow >= BoardHeight) && (gridRow < (BoardHeight + BoardTopBufferHeight))) &&
		        ((gridColumn >= 0) && (gridColumn < BoardWidth)));
	}
	
	public bool GridCellOccupied(int gridColumn, int gridRow)
	{
		return mBoard[gridRow, gridColumn];
	}
	
	public void OccupyGridCell(int gridColumn, int gridRow)
	{
		if (InGridRange(gridColumn, gridRow))
		{
			mBoard[gridRow, gridColumn] = true;
		}
	}
	
	public void ClearGridRow(int gridRow)
	{
		WipeRow(gridRow);
		CompactGrid(gridRow);
	}
	
	void WipeRow(int gridRow)
	{
		if ((gridRow >= 0) && (gridRow < BoardHeight))
		{
			for (int column = 0; column < BoardWidth; column++)
			{
				mBoard[gridRow, column] = false;
			}
		}
	}
	
	void CompactGrid(int startGridRow)
	{
		// move all rows above startGridRow down by one.
		for (int row = startGridRow; row < (BoardHeight - 1); row++)
		{
			int targetRow = row;
			int sourceRow = row + 1;
			for (int column = 0; column < BoardWidth; column++)
			{
				mBoard[targetRow, column] = mBoard[sourceRow, column];
				mBoard[sourceRow, column] = false;
			}
		}
	}

	
	public int FindFullRows(bool[] fullRows)
	{
		int retCount = fullRows.Length;
		
		for (int row = 0; row < BoardHeight; row++)
		{
			fullRows[row] = true;
			for (int column = 0; column < BoardWidth; column++)
			{
				if (mBoard[row,column] == false)
				{
					fullRows[row] = false;
					retCount--;
					break;
				}
			}
		}
		
		return retCount;
	}
	
	
	public bool SpotOpen(Vector3 worldLoc)
	{		
		int gridColumn = 0;
		int gridRow = 0;
		TranslateCoordtoGridCell(worldLoc.x, worldLoc.y, out gridColumn, out gridRow);
		
		if (InGirdBuffer(gridColumn, gridRow))
		{
			return true;
		}
		if (!InGridRange(gridColumn, gridRow))
		{
			return false;
		}
		if (GridCellOccupied(gridColumn, gridRow))
		{
			return false;
		}
		
		return true;
	}
	
	#region IModeChanger
	public void ChangeMode(MainLoop.Mode newMode)
	{
		if (newMode == MainLoop.Mode.StartPlay)
		{
			for (int i = 0; i < BoardHeight; i++)
			{
				WipeRow(i);
			}
		}
	}
	
	#endregion IModeChanger

	void Start()
	{
		// Init board with a 1-tile boarder on sides and bottom, top is open, all other slots open
		mBoard = new bool[BoardHeight, BoardWidth];
		
		for (int row = 0; row < BoardHeight; row++)
		{
			for (int column = 0; column < BoardWidth; column++)
			{
				mBoard[row, column] = false;
			}
		}
	}
	
	void Update()
	{
	
	}
}
