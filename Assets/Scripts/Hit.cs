using UnityEngine;

public class Hit : MonoBehaviour {

	public GameObject _destroyedObject;		// Prefab do Container destruído (/animado)

	public GameObject[] _collectablesLoot;	// Lista de itens contidos


	void OnCollisionEnter( Collision collision ) 
	{
		if (collision.relativeVelocity.magnitude > 6f)
			DestroyIt();
	}
	
		
	void DestroyIt()
	{
		if (_destroyedObject)
			Instantiate(_destroyedObject, transform.position, transform.rotation);

		if (_collectablesLoot.Length > 0 && _collectablesLoot[0] != null)
			Instantiate (_collectablesLoot[0], transform.position, transform.rotation);		// Por enquanto, só um conteúdo suportado

		/*
		float i = 0f;
		foreach (GameObject item in _collectablesLoot) 
		{
			i += 0.2f;
			Instantiate(item, transform.position + new Vector3(0,0,i), transform.rotation);
		}
		*/
		
		Destroy(gameObject);

	}
}