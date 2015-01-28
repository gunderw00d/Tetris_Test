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
