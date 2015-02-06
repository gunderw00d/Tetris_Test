// Copyright Greg Underwood, 2015.
// All files in this project, including this one, are covered under the GNU Public License, V3.0.
// See the file gpl-3.0.txt included in this repository for full details of the license.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputHandler : MonoBehaviour, IModeChanger
{
	#region vars	
	public MainLoop MainLoopScript;
	
	class InputData
	{
		public MainLoop.dInputAction	mActionFunc;
		public float					mLastPressed;
		public float					mDebounceInterval;
		
		public InputData(MainLoop.dInputAction action, float debounceInterval)
		{
			mActionFunc = action;
			mLastPressed = 0;
			mDebounceInterval = debounceInterval;
		}
	};
	
	InputData mDoNothing;
		
	Dictionary<string, InputData>[] InputMaps;
	
	MainLoop.Mode ActiveMap;
	int NumModes;
	
	public float DebounceInterval = 0.175f;
	
	string[] AxisNames;
	string kHorizontal = "Horizontal";
	string kRotate = "Rotate";
	string kDrop = "Drop";
	string kSubmit = "Submit";
	string kCancel = "Cancel";
	#endregion // vars
	
	#region IModeChanger
	public void ChangeMode(MainLoop.Mode newMode)
	{
		foreach (KeyValuePair<string, InputData> kvp in InputMaps[(int)ActiveMap])
		{
			InputMaps[(int)newMode][kvp.Key].mLastPressed = kvp.Value.mLastPressed;
		}
		
		ActiveMap = newMode;
	}
	
	#endregion IModeChanger
	
	bool CheckForInput(out string axisName, out float value)
	{
		foreach (string axis in AxisNames)
		{
			float v = Input.GetAxis(axis);
			if (v != 0f)
			{
				axisName = axis;
				value = v;
				return true;
			}
		}
		
		axisName = "";
		value = 0f;
		return false;
	}
	
	void HandleInput()
	{
		string axisName;
		float value;
		
		if (CheckForInput(out axisName, out value))
		{
			float debounceDelta = Time.time - InputMaps[(int)ActiveMap][axisName].mLastPressed;
			if (debounceDelta > InputMaps[(int)ActiveMap][axisName].mDebounceInterval)
			{
				InputMaps[(int)ActiveMap][axisName].mActionFunc(value);
				InputMaps[(int)ActiveMap][axisName].mLastPressed = Time.time;
			}
		}
	}
	
	void Start ()
	{
		MainLoopScript = gameObject.GetComponent<MainLoop>();
		mDoNothing = new InputData(MainLoopScript.DoNothing, 0);
		
		NumModes = System.Enum.GetNames(typeof(MainLoop.Mode)).Length;
		InputMaps = new Dictionary<string, InputData>[NumModes];
		
		AxisNames = new string[] {kHorizontal, kRotate, kDrop, kSubmit, kCancel };
		
		for (int i = 0; i < NumModes; i++)
		{
			InputMaps[i] = new Dictionary<string, InputData>();
			
			foreach (string axis in AxisNames)
			{
				InputMaps[i][axis] = mDoNothing;
			}
		}
		
		InputMaps[(int)MainLoop.Mode.Playing][kDrop] = new InputData(MainLoopScript.MovePieceDown, DebounceInterval);
		InputMaps[(int)MainLoop.Mode.Playing][kHorizontal] = new InputData(MainLoopScript.MovePiece, DebounceInterval);
		InputMaps[(int)MainLoop.Mode.Playing][kRotate] = new InputData(MainLoopScript.RotatePiece, DebounceInterval);
		InputMaps[(int)MainLoop.Mode.Playing][kCancel] = new InputData(MainLoopScript.PausePressed, DebounceInterval);
		InputMaps[(int)MainLoop.Mode.Playing][kSubmit] = new InputData(MainLoopScript.SwapPressed, DebounceInterval);		
		
		InputMaps[(int)MainLoop.Mode.Paused][kSubmit] = new InputData(MainLoopScript.ResumePressed, DebounceInterval);				
		InputMaps[(int)MainLoop.Mode.Paused][kCancel] = new InputData(MainLoopScript.ResumePressed, DebounceInterval);
		
		
		InputMaps[(int)MainLoop.Mode.StartScreen][kSubmit] = new InputData(MainLoopScript.StartPressed, DebounceInterval);
		InputMaps[(int)MainLoop.Mode.StartScreen][kCancel] = new InputData(MainLoopScript.ExitPressed, DebounceInterval);
		
		
		InputMaps[(int)MainLoop.Mode.GameOver][kSubmit] = new InputData(MainLoopScript.StartPressed, DebounceInterval);
		InputMaps[(int)MainLoop.Mode.GameOver][kCancel] = new InputData(MainLoopScript.ExitPressed, DebounceInterval);
		
		ActiveMap = MainLoop.Mode.StartScreen;
	}
	
	void Update ()
	{
		HandleInput();
	}
	
	
}
