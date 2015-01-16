using UnityEngine;
using System.Collections;

public class InputHandler : MonoBehaviour
 {
 	public enum InputType
 	{
 		Space_Up,
 		None
 	};

	// Use this for initialization
	void Start ()
	{
		// TODO -- init list of commands to be handed back and what input to hand them back on
	}
	
	// TODO -- return command object based on input detected
	public InputType CheckForInput()
	{
		// TODO -- lots more in here.  Simple dev test.
		if (Input.GetKeyUp("space"))
		{
			return InputType.Space_Up;
		}
		else
		{
			return InputType.None;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		// TODO -- anything to do?  Think we want to have this object respond to specific queries...
		
		// TODO -- perhaps queue up commands that happen, in frame order, then handle them in line?
	}
}
