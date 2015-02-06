// Copyright Greg Underwood, 2015.
// All files in this project, including this one, are covered under the GNU Public License, V3.0.
// See the file gpl-3.0.txt included in this repository for full details of the license.

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class GameVarWatcher : MonoBehaviour
{
	public string VarWatched;
	public GameObject MainLoopGameObj;
	MainLoop MainLoopScript;
	bool FirstLoop = false;

	void Start()
	{
		FirstLoop = false;
	}
	
	void Update()
	{
		if (FirstLoop == false)
		{
			MainLoopScript = MainLoopGameObj.GetComponent<MainLoop>();
			
			MainLoopScript.RegisterWatcher(VarWatched, GameVarChanged);
			
			FirstLoop = true;
		}
	}
	
	//  
	public void GameVarChanged(int oldValue, int newValue)
	{
		Text textField = gameObject.GetComponent<Text>();
		
		textField.text = String.Format("{0}", newValue);
	}
}
