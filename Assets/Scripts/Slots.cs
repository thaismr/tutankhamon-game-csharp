using UnityEngine;

public class Slots : MonoBehaviour {

//	private GameManager _gameManager;

	public int _slotId;

	public void Start()
	{
//		_gameManager = GameManager.instance;
	}

	public void SlotOnClick()
	{
//		_gameManager.InventoryUse (_slotId);
		GameManager.GM.InventoryUse (_slotId);
	}

}
