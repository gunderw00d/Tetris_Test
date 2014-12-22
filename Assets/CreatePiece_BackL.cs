using UnityEngine;
using System.Collections;

public class CreatePiece_BackL : MonoBehaviour {

	public Transform Piece;
	public Transform StartLocation;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		// INITIAL TESTING --
		// Press space to create piece

		bool createPiece = Input.GetKeyUp("space");

		if (createPiece)
		{
			Instantiate(Piece, StartLocation.transform.position, Quaternion.identity);
		}
		
	
	}
}
