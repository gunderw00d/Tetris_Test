using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputHandler : MonoBehaviour
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
	
	enum ActiveMapType
	{
		Playing = 0,
		NoPiece = 1
	};
	
	Dictionary<KeyCode, InputData>[] InputMaps;
	
	ActiveMapType ActiveMap;
	
	public float DebounceInterval = 0.5f;
	#endregion // vars
	
	
	void Start ()
	{
		MainLoopScript = gameObject.GetComponent<MainLoop>();
		
		mDoNothing = new InputData(MainLoopScript.DoNothing, 0);
		
		InputMaps = new Dictionary<KeyCode, InputData>[2];
		InputMaps[(int)ActiveMapType.Playing] = new Dictionary<KeyCode, InputData>();
		InputMaps[(int)ActiveMapType.NoPiece] = new Dictionary<KeyCode, InputData>();
		
		InputMaps[(int)ActiveMapType.Playing][KeyCode.Space] = new InputData(MainLoopScript.MovePieceDown, DebounceInterval);
		InputMaps[(int)ActiveMapType.Playing][KeyCode.LeftArrow] = new InputData(MainLoopScript.MovePieceLeft, DebounceInterval);
		InputMaps[(int)ActiveMapType.Playing][KeyCode.RightArrow] = new InputData(MainLoopScript.MovePieceRight, DebounceInterval);
		InputMaps[(int)ActiveMapType.Playing][KeyCode.UpArrow] = new InputData(MainLoopScript.RotatePieceCW, DebounceInterval);
		InputMaps[(int)ActiveMapType.Playing][KeyCode.DownArrow] = new InputData(MainLoopScript.RotatePieceCCW, DebounceInterval);
		InputMaps[(int)ActiveMapType.Playing][KeyCode.None] = mDoNothing;
		
		// TODO -- DoNothing
		InputMaps[(int)ActiveMapType.NoPiece][KeyCode.Space] = new InputData(MainLoopScript.CreateRandomPiece, DebounceInterval);
		InputMaps[(int)ActiveMapType.NoPiece][KeyCode.LeftArrow] = mDoNothing;
		InputMaps[(int)ActiveMapType.NoPiece][KeyCode.RightArrow] = mDoNothing;
		InputMaps[(int)ActiveMapType.NoPiece][KeyCode.UpArrow] = mDoNothing;
		InputMaps[(int)ActiveMapType.NoPiece][KeyCode.DownArrow] = mDoNothing;
		InputMaps[(int)ActiveMapType.NoPiece][KeyCode.None] = mDoNothing;
		
		ActiveMap = ActiveMapType.NoPiece;
	}
	
	void Update ()
	{
		HandleInput();
	}
	
	public void PieceFallingInput()
	{
		ActiveMap = ActiveMapType.Playing;
	}
	
	public void NoPieceFallingInput()
	{
		ActiveMap = ActiveMapType.NoPiece;
	}
	
	KeyCode CheckForInput()
	{
		// TODO -- is there really no better way to do this?  Can I not just get keyboard events directly?
		if (Input.GetKey(KeyCode.Space))
		{
			return KeyCode.Space;
		}
		else if (Input.GetKey(KeyCode.LeftArrow))
		{
			return KeyCode.LeftArrow;
		}
		else if (Input.GetKey(KeyCode.RightArrow))
		{
			return KeyCode.RightArrow;
		}
		else if (Input.GetKey(KeyCode.UpArrow))
		{
			return KeyCode.UpArrow;
		}
		else if (Input.GetKey(KeyCode.DownArrow))
		{
			return KeyCode.DownArrow;
		}
		else
		{
			return KeyCode.None;
		}
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
	
}
