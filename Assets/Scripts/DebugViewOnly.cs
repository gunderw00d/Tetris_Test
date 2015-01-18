using UnityEngine;
using System.Collections;

public class DebugViewOnly : MonoBehaviour
{

	#region vars
	public bool mVisible = false;
	#endregion // vars

	// Use this for initialization
	void Start()
	{
		gameObject.SetActive(mVisible);
	}
	
	// Update is called once per frame
	void Update()
	{
		gameObject.SetActive(mVisible);	
	}
}
