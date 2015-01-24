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
		
	Dictionary<KeyCode, InputData>[] InputMaps;
	
	MainLoop.Mode ActiveMap;
	int NumModes;
	
	public float DebounceInterval = 0.5f;
	
	KeyCode[] Codes;
	#endregion // vars
	
	#region IModeChanger
	public void ChangeMode(MainLoop.Mode newMode)
	{
		ActiveMap = newMode;
	}
	
	#endregion IModeChanger
	
	KeyCode CheckForInput()
	{
		foreach (KeyCode code in Codes)
		{
			if (Input.GetKey(code))
			{
				return code;
			}
		}
		
		return KeyCode.None;
	}
	
	void HandleInput()
	{
		KeyCode inputType = CheckForInput();
		if (InputMaps[(int)ActiveMap].ContainsKey(inputType))
		{
			float debounceDelta = Time.time - InputMaps[(int)ActiveMap][inputType].mLastPressed;
			if (debounceDelta > InputMaps[(int)ActiveMap][inputType].mDebounceInterval)
			{
				InputMaps[(int)ActiveMap][inputType].mActionFunc();
				InputMaps[(int)ActiveMap][inputType].mLastPressed = Time.time;
			}
		}
	}
	
	void Start ()
	{
		MainLoopScript = gameObject.GetComponent<MainLoop>();
		
		Codes = new KeyCode[] {KeyCode.Space, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow};
		
		NumModes = System.Enum.GetNames(typeof(MainLoop.Mode)).Length;
		InputMaps = new Dictionary<KeyCode, InputData>[NumModes];
		mDoNothing = new InputData(MainLoopScript.DoNothing, 0);
		
		for (int i = 0; i < NumModes; i++)
		{
			InputMaps[i] = new Dictionary<KeyCode, InputData>();
			
			foreach (KeyCode code in Codes)
			{
				InputMaps[i][code] = mDoNothing;
			}
			InputMaps[i][KeyCode.None] = mDoNothing;
		}
		
		InputMaps[(int)MainLoop.Mode.Playing][KeyCode.Space] = new InputData(MainLoopScript.MovePieceDown, DebounceInterval);
		InputMaps[(int)MainLoop.Mode.Playing][KeyCode.LeftArrow] = new InputData(MainLoopScript.MovePieceLeft, DebounceInterval);
		InputMaps[(int)MainLoop.Mode.Playing][KeyCode.RightArrow] = new InputData(MainLoopScript.MovePieceRight, DebounceInterval);
		InputMaps[(int)MainLoop.Mode.Playing][KeyCode.UpArrow] = new InputData(MainLoopScript.RotatePieceCW, DebounceInterval);
		InputMaps[(int)MainLoop.Mode.Playing][KeyCode.DownArrow] = new InputData(MainLoopScript.RotatePieceCCW, DebounceInterval);
		// TODO -- ESC to pause?
		
		// TODO -- in pause, space to resume?
		
		InputMaps[(int)MainLoop.Mode.StartScreen][KeyCode.Space] = new InputData(MainLoopScript.StartPressed, DebounceInterval);
		InputMaps[(int)MainLoop.Mode.GameOver][KeyCode.Space] = new InputData(MainLoopScript.StartPressed, DebounceInterval);
		
		ActiveMap = MainLoop.Mode.StartScreen;
	}
	
	void Update ()
	{
		HandleInput();
	}
	
	
}
