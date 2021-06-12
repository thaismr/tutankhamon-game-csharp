using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
	public GameObject _isHolding;

	// MOVIMENTOS

	static CharacterController _cc;

	void Start()
	{
		_cc = GetComponent<CharacterController> ();

	}

	public static void Freeze(bool freeze)	// (des) Congela movimentos para exibir inventário / janelas
	{
		_cc.enabled = !freeze;
	}


	// COLISÕES

    public void OnTriggerEnter(Collider other)
    {
       
		if (other.CompareTag (Goal.TAG))
		{
			GameManager.GM.SavePlayer ();
			SceneManager.LoadScene(other.gameObject.GetComponent<Goal>()._loadScene);
		}

		else if (other.CompareTag (Token.TAG))
			CollectToken (other.gameObject);
		
		else if (other.CompareTag (Collectable.TAG))
			CollectItem (other.gameObject);
		
		else if (other.CompareTag (PuzzleTrigger.TAG))
			CheckPuzzle (other.gameObject);

		else if (other.CompareTag (PuzzleDevice.TAG))
			CheckDevice (other.gameObject);
		
		else if (other.CompareTag (DeathZone.TAG))
			GameManager.GM.Restart ();
    
	}


	// COLECIONÁVEIS

    void CollectToken(GameObject gameObject)				// Colecionáveis para pontuação
    {
		int _points = gameObject.GetComponent<Token> ()._points;

        gameObject.GetComponent<Token>().Collect();

		GameManager.GM.AddScorePoints (_points);
    }

	void CollectItem(GameObject gameObject)					// Colecionáveis para Puzzles / Inventário
	{
		if (GameManager.GM.InventoryAdd (gameObject.GetComponent<Collectable> ()._collectableId))
			gameObject.GetComponent<Collectable> ().Collect ();
		else
			GameManager.GM.AlertMessage ("Inventory full.");
	}


	// ITENS

	public void DropItem()			// Destrói prefab do item em mãos
	{
		if (_isHolding)
		{
			_isHolding.GetComponent<Collectable> ().Collect ();
			_isHolding = null;
		}
	}

	public void ItemSpawn(GameObject holding)		// Inicializa Prefab do item a segurar
	{
		holding = Instantiate (holding);
		holding.transform.parent = gameObject.transform;
		holding.transform.localPosition = new Vector3 (.5f, 0, 1.5f);				// Posiciona em relação ao jogador

		foreach (Collider col in holding.GetComponents<Collider> ())		// Desativa colisões deste item
			col.enabled = false;

		_isHolding = holding;
	}

	public void Interact(int holdingId)
	{
		if (_isHolding.GetComponent<Collectable>()._canThrow)		// Item em mãos pode ser arremessado
		{
			GameObject holding = Instantiate (_isHolding);
			holding.transform.position = _isHolding.transform.position;
			holding.GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(0,.2f,1) * 10);

			foreach (Collider col in holding.GetComponents<Collider> ())	// Reativa colisões
				col.enabled = true;
			
			holding.GetComponent<Rigidbody> ().useGravity = true;

			GameManager.GM.InventoryDrop (holdingId);
		} 
		else 	// (A fazer: Deverá acionar alguma animação do prefab)
		{
			
		}
	}


	// PUZZLES

	void CheckDevice(GameObject gameObject)		// Mecanismo é uma armadilha/obstáculo
	{

		if (! gameObject.GetComponent<PuzzleDevice> ().isSolved) 	// Mecanismo não resolvido
			GameManager.GM.Restart();

	}

	void CheckPuzzle(GameObject gameObject)		// Mecanismo é um acionador
	{
		
		int _key = gameObject.GetComponent<PuzzleTrigger> ().mustHaveItem;


		if (_key == -1)		// Nenhum item necessário (botões, alavancas)
		{
			gameObject.GetComponent<PuzzleTrigger> ().SolvePuzzle ();

			GameManager.GM.AlertMessage ("Puzzle acionado.");
		}
		else
		{
			
		string _name = Inventory._item [_key]._name;


		if (GameManager.GM.InventoryHas (_key)) 	// Jogador possui o item necessário
		{
			if (gameObject.GetComponent<PuzzleTrigger> ().mustBeHolding && GameManager.GM.IsHolding(_key))	// Item está em uso
			{
				gameObject.GetComponent<PuzzleTrigger> ().SolvePuzzle ();

				GameManager.GM.AlertMessage ("Puzzle solved. You are holding: " + _name);

				if (gameObject.GetComponent<PuzzleTrigger> ().losesItem)	// Item se perde com o uso
				{
					GameManager.GM.AlertMessage ("You were holding: " + _name);
					GameManager.GM.InventoryDrop (_key);
				}
			}
			else if (!gameObject.GetComponent<PuzzleTrigger> ().mustBeHolding)	// Item não precisa estar em uso
			{
				gameObject.GetComponent<PuzzleTrigger> ().SolvePuzzle ();

				GameManager.GM.AlertMessage ("Puzzle solved. You have: " + _name);

				if (gameObject.GetComponent<PuzzleTrigger> ().losesItem)	// Item se perde com o uso
				{
					GameManager.GM.AlertMessage ("You had: " + _name);
					GameManager.GM.InventoryDrop (_key);
				}
			} 
			else
				GameManager.GM.AlertMessage ("You need a key to this puzzle.");	// Item não está em uso
		}
		else
			GameManager.GM.AlertMessage ("You miss a key to this puzzle.");	// Item não está no inventário


		}
	}
		

}
