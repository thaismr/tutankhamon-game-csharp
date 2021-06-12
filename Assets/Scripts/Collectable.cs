using UnityEngine;

public class Collectable : MonoBehaviour {

	public const string TAG = "Collectable";

	public int _collectableId;

	public bool _canThrow;		// Pode ser arremessado?


	public void Collect()
	{
		Destroy(gameObject);
	}
}
