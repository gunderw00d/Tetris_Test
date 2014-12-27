using UnityEngine;
using System.Collections;

public class CreatePiece_BackL : MonoBehaviour {

	public Transform Piece;
	public Transform StartLocation;
	public Transform TileContainer;

	// Use this for initialization
	void Start()
	{
	
	}
	
	// Update is called once per frame
	void Update()
	{
		// INITIAL TESTING --
		// Press space to create piece

		bool createPiece = Input.GetKeyUp("space");

		if (createPiece)
		{
			Transform newPiece = Instantiate(Piece, StartLocation.transform.position, Quaternion.identity) as Transform;
			Drop dropScript = newPiece.gameObject.GetComponent<Drop>(); 
			dropScript.TileContainer = TileContainer;
		}
		
	
	}
}
