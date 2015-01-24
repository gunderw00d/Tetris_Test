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
