using UnityEngine;

public class PuzzleDevice : MonoBehaviour {

	public const string TAG = "PuzzleDevice";

	public bool isSolved;				// Está solucionado?


	public void TriggerDevice()
	{
		isSolved = !isSolved;			// Inverter estado

		gameObject.SetActive (false);	// (A fazer: Mudar animação conforme)
	}

}
