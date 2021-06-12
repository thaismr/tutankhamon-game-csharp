using UnityEngine;

public class PuzzleTrigger : MonoBehaviour {

	public const string TAG = "PuzzleTrigger";

	public int mustHaveItem;			// Puzzle abre-se com qual item?

	public bool mustBeHolding;			// Item deve estar em uso?

	public bool losesItem;				// Item perde-se com uso?

	public bool isSolved;				// Está solucionado?


	public GameObject[] devicesToTrigger;		// Mecanismos diversos a acionar


	public void SolvePuzzle()
	{
		isSolved = !isSolved;			// Inveter estado

		gameObject.SetActive (false);	// (A fazer: Deverá ativar animação conforme)


		foreach (GameObject device in devicesToTrigger)				// Aciona cada elemento da lista, arrastados no Inspetor
			device.GetComponent<PuzzleDevice> ().TriggerDevice ();
			
	}

}
