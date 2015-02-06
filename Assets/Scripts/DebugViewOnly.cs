// Copyright Greg Underwood, 2015.
// All files in this project, including this one, are covered under the GNU Public License, V3.0.
// See the file gpl-3.0.txt included in this repository for full details of the license.

using UnityEngine;
using System.Collections;

public class DebugViewOnly : MonoBehaviour
{

	#region vars
	public bool mVisible = false;
	#endregion // vars
		
	void Start()
	{
		gameObject.SetActive(mVisible);
	}
	
	void Update()
	{
		gameObject.SetActive(mVisible);	
	}
}
