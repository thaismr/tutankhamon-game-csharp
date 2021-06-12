using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI : MonoBehaviour
{
    public const string TAG = "UI";

	private const string INVENTORY_IMAGES_PATH  = "Sprites";

    public Text _totalScore;

	public Text _alerts;

	public Button[] _slots;

	public EventSystem _eventSystem;

	public Button _selectedButton;

	public bool _inventoryIsActive;


	// SCORE UI

    public void SetScore(int value)
    {
        _totalScore.text = value.ToString();
    }


	// ALERTS/DEBUG UI

	public void SetAlert(string text)
	{
		_alerts.text = text;
	}


	// INVENTORY PANEL

	public void SetItemCollected(int i, int collectableId, int count)	// Update single slot
	{
		_slots [i].interactable = true;
		_slots [i].GetComponentInChildren<RawImage> ().texture = Resources.Load<Texture> (INVENTORY_IMAGES_PATH +"/"+ collectableId);
		_slots [i].GetComponentInChildren<Text> ().text = Inventory._item [collectableId]._name + "(" + count + ")";
	}

	public void SetInventory(int[] collectableId, int[] count)	// Update all slots
	{
		for (int i = 0; i < _slots.Length; i++)
		{	
			if (collectableId [i] != -1)
			{
				_slots [i].interactable = true;
				_slots [i].GetComponentInChildren<RawImage> ().texture = Resources.Load<Texture> (INVENTORY_IMAGES_PATH +"/"+ collectableId[i]);
				_slots [i].GetComponentInChildren<Text> ().text = Inventory._item [collectableId [i]]._name + "(" + count[i] + ")";
			}
			else
			{
				_slots [i].interactable = false;
				_slots [i].GetComponentInChildren<RawImage> ().texture = null;
				_slots [i].GetComponentInChildren<Text> ().text = "";
			}
		}
	}


	// INVENTORY NAVIGATION

	public void SelectInventoryButton() 
	{			
		if (_inventoryIsActive) // If already active, then close
		{
			_eventSystem.SetSelectedGameObject (null);
			_inventoryIsActive = false;
			Player.Freeze (false);
//			Time.timeScale = 1;
		}
		else if (_selectedButton.interactable)
		{
			_selectedButton.Select ();
			_inventoryIsActive = true;
			Player.Freeze (true);
//			Time.timeScale = 0;
		}
	}

}
