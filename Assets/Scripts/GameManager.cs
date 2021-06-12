using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
	// Base de dados do Inventário está no prefab Player, componente prefab Inventário

	// Premir "R" deleta o arquivo save.dat para modificações ao SaveData.cs

	public static GameManager GM;

	public const int INVENTORY_SIZE = 5;

	private const string SAVE_DATA_FILE     = "save26.dat";

	private const string PREFS_KEY_QUALITY_LEVEL = "QualityLevel";
	private const int    QUALITY_LEVEL_UNDEFINED = -1;

	private const KeyCode KEY_CODE_QUALITY_UP   = KeyCode.KeypadPlus;
	private const KeyCode KEY_CODE_QUALITY_DOWN = KeyCode.KeypadMinus;

	private const KeyCode KEY_CODE_INTERACT = KeyCode.Mouse0;
	private const KeyCode KEY_CODE_INVENTORY = KeyCode.E;
	private const KeyCode KEY_CODE_REFRESH = KeyCode.R;
	private const KeyCode KEY_CODE_CHEAT = KeyCode.C;
	private const KeyCode KEY_CODE_QUIT = KeyCode.Q;

	public string _cheat = "No help available right now.";

	private string   _saveDataPath;
	private SaveData _saveData;
	private UI       _ui;


	// CONEXÃO À BASE DE DADOS :

	const string SERVER_URL	= "http://localhost/unity/server.php";

	const int SERVER_ACTION_REGISTER	= 1;
	const int SERVER_ACTION_LOGIN		= 2;
	const int SERVER_ACTION_SAVE		= 3;

	const int MIN_PASSCODE	= 100000;
	const int MAX_PASSCODE	= 999999999;


	void Awake()
	{
		if (GM == null)
			GM = this;
		
		else if (GM != this)
			Destroy (gameObject);		
	}


	public void Start()
	{
		InitSaveData();
		InitQualityLevel();
		InitUI();

		InitPlayer();

		SceneManager.sceneLoaded += ProcessSceneLoaded;
	}


	// GERENCIAMENTO DO ARQUIVO DE SALVAMENTO

	private void InitSaveData()
	{
		_saveDataPath = Application.persistentDataPath + "/" + SAVE_DATA_FILE;

		if (File.Exists(_saveDataPath))
			LoadSaveData();
		else
			CreateSaveData();
	}

	private void CreateSaveData()
	{
		_saveData = new SaveData();
	}

	private void LoadSaveData()
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream      fileStream      = File.Open(_saveDataPath, FileMode.Open);

		_saveData = (SaveData)binaryFormatter.Deserialize(fileStream);

		fileStream.Close();
	}

	private void StoreSaveData()
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream      fileStream      = File.Create(_saveDataPath);

		binaryFormatter.Serialize(fileStream, _saveData);

		fileStream.Close();
	}

	private void DeleteSaveData()	// Apaga arquivo de salvamento
	{
		_saveDataPath = Application.persistentDataPath + "/" + SAVE_DATA_FILE;

		File.Delete (_saveDataPath);
	}

	private void InitPlayer()		// Inicializa o jogador
	{
		if (_saveData.isHoldingId != -1) 
			GetComponentInParent<Player> ().ItemSpawn (Inventory._item [_saveData.isHoldingId]._prefab);


		// BASE DE DADOS:

		if (PlayerIsRegistered ())
			LoginPlayer ();

		else
			RegisterPlayer ();
	}



	// GERENCIAMENTO DOS DADOS DO JOGADOR PARA A BASE DE DADOS :

	public int PlayerId()
	{
		return _saveData.playerId;		// retorna o ID guardado no arquivo de salvamento
	}

	public long PassCode()
	{
		return _saveData.passCode;		// retorna a passcode guardada no arquivo de salvamento
	}

	public bool PlayerIsRegistered()
	{
		return _saveData.playerId != -1;	// retorna Verdadeiro se o jogador tiver um ID salvo
	}

	public void RegisterPlayer()
	{
		_saveData.passCode = (long) Mathf.Round( Random.Range (MIN_PASSCODE, MAX_PASSCODE) );

		string requestURL = SERVER_URL + "?action=" + SERVER_ACTION_REGISTER + "&passcode=" + _saveData.passCode;


		StartCoroutine("CallRegisterPlayer", requestURL);

	}

	public void LoginPlayer()
	{

		string requestURL = SERVER_URL + "?action=" + SERVER_ACTION_LOGIN + "&id=" + _saveData.playerId + "&passcode=" + _saveData.passCode;

		StartCoroutine("CallLoginPlayer", requestURL);

	}

	public void SavePlayer()		// Salva dados do jogador na DB  (chamado ao colidir algum objecto 'Goal')
	{

		string requestURL = SERVER_URL + "?action=" + SERVER_ACTION_SAVE + "&id=" + _saveData.playerId + "&passcode=" + _saveData.passCode

			+ "&score=" + _saveData.totalScore + "&slot1=" + _saveData.itemsCollected[0] + "&slot2=" + _saveData.itemsCollected[1]

			+ "&slot3=" + _saveData.itemsCollected[2] + "&slot4=" + _saveData.itemsCollected[3] + "&slot5=" + _saveData.itemsCollected[4]

			+ "&count1=" + _saveData.itemsCount[0] + "&count2=" + _saveData.itemsCount[1] + "&count3=" + _saveData.itemsCount[2]

			+ "&count4=" + _saveData.itemsCount[3] + "&count5=" + _saveData.itemsCount[4];

		print (requestURL);

		StartCoroutine("CallSavePlayer", requestURL);

	}

	IEnumerator CallRegisterPlayer(string requestURL)
	{

		WWW www = new WWW (requestURL);

		yield return www;

		print (www.text);

		string[] reply = www.text.Split ('|');

		_saveData.playerId = int.Parse(reply[1]);

		StoreSaveData ();

	}

	IEnumerator CallLoginPlayer(string requestURL)
	{

		WWW www = new WWW (requestURL);

		yield return www;

		print (www.text);

		string[] reply = www.text.Split ('|');


		if (int.Parse(reply[1]) != -1)	// se a conexão não retornar erro :
		{
			_saveData.totalScore = int.Parse(reply[2]);		// recupera pontuação

			for (int i = 0; i < 5; i++) 
				_saveData.itemsCollected [i] = int.Parse (reply[3+i]);		// recupera id dos itens no inventário

			for (int i = 0; i < 5; i++) 
				_saveData.itemsCount [i] = int.Parse (reply[8+i]);			// recupera contagem do inventário

			StoreSaveData ();
		}

	}

	IEnumerator CallSavePlayer(string requestURL)
	{
		WWW www = new WWW (requestURL);

		yield return www;

		print (www.text);

	}



	// GERENCIAMENTO DAS PREFERÊNCIAS DO JOGADOR

	private void InitQualityLevel()
	{
		int qualityLevel = PlayerPrefs.GetInt(PREFS_KEY_QUALITY_LEVEL, QUALITY_LEVEL_UNDEFINED);

		if (qualityLevel != QUALITY_LEVEL_UNDEFINED)
			QualitySettings.SetQualityLevel(qualityLevel);    
	}

	private void SaveQualityLevel()
	{
		PlayerPrefs.SetInt(PREFS_KEY_QUALITY_LEVEL, QualitySettings.GetQualityLevel());
	}


	// GERENCIAMENTO DA UI

	private void InitUI()
	{
		_ui = GameObject.FindWithTag(UI.TAG).GetComponent<UI>();

		_ui.SetScore(_saveData.totalScore);

		_ui.SetInventory (_saveData.itemsCollected, _saveData.itemsCount);
	}

	private void ProcessSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		InitUI();
	}		

	private void CheatCode()	// Exibe texto de Ajuda / Cheat ("c")
	{
		_ui.SetAlert(_cheat);
	}

	public void AlertMessage(string alert)	// Atualizar alertas / mensagens da UI
	{
		_ui.SetAlert(alert);
	}


	// REINICIA CENA ATUAL:

	public void Restart()
	{
		SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
	}


	// GERENCIAMENTO DA PONTUAÇÃO:

	public void AddScorePoints(int points)
	{
		_saveData.totalScore += points;

		_ui.SetScore(_saveData.totalScore);

		StoreSaveData();
	}



	// GERENCIAMENTO DO INVENTÁRIO

	public bool InventoryAdd(int collectableId)	// Tenta acrescentar item ao inventário (true/false)
	{
		for (int i=0; i<INVENTORY_SIZE; i++)
			
			if (_saveData.itemsCollected[i] == collectableId)	// Incrementa quantidade apenas
			{
				++_saveData.itemsCount[i];
				_ui.SetItemCollected (i, collectableId, _saveData.itemsCount[i]);
				AlertMessage ("Collected " + Inventory._item[collectableId]._name);
				StoreSaveData ();
				return true;
			}

		for (int i=0; i<INVENTORY_SIZE; i++)
			
			if (_saveData.itemsCollected[i] == -1)		// Acrescenta novo item
			{
				_saveData.itemsCollected[i] = collectableId;
				_saveData.itemsCount[i] = 1;
				_ui.SetItemCollected (i, collectableId, 1);
				AlertMessage ("Collected " + Inventory._item[collectableId]._name);
				StoreSaveData ();
				return true;
			}

		return false;		// Se chegou até aqui sem retornar true...
	}

	public bool InventoryHas(int collectableId)	// Verifica se inventário possui item (true/false)
	{
		foreach (int item in _saveData.itemsCollected)
			if (item == collectableId)
				return true;

		return false;
	}

	public bool IsHolding(int collectableId)	// Verifica se item está a ser utilizado (true/false)
	{
		return _saveData.isHoldingId == collectableId;
	}

	public void InventoryUse(int slotId)	// Coloca item em uso (ou mescla com item em uso anterior)
	{

		int holding = _saveData.isHoldingId;
		int makeuse = _saveData.itemsCollected[slotId];

		if (holding == -1)	// Nenhum item em uso
		{
			_saveData.isHoldingId = makeuse;
		}
		else 				// Tenta produzir novo Item
		{	
			_saveData.isHoldingId = Inventory.Crafting (holding, makeuse);

			if (_saveData.isHoldingId == -1)		// Nenhum item foi produzido
				_saveData.isHoldingId = makeuse;
			else 									// Retira materiais do inventário
			{
				InventoryDrop (holding);
				InventoryDrop (makeuse);
				InventoryAdd (_saveData.isHoldingId);

				AlertMessage ("You crafted: " + Inventory._item[_saveData.isHoldingId]._name);
			}
		}

		GetComponentInParent<Player> ().DropItem ();													// Destrói Prefab do item anterior

		GetComponentInParent<Player> ().ItemSpawn (Inventory._item[_saveData.isHoldingId]._prefab);		// Inicializa Prefab do item em uso

		AlertMessage ("Holding " + Inventory._item[_saveData.isHoldingId]._name);
		_ui.SelectInventoryButton ();
		StoreSaveData ();

	}

	public void InventoryDrop(int collectableId)	// Retira item do inventário
	{
		for (int i = 0; i < INVENTORY_SIZE; i++)
			
			if (_saveData.itemsCollected [i] == collectableId)
			{
				if (_saveData.itemsCount[i] > 1) 	// Reduz apenas quantidade
				{
					--_saveData.itemsCount [i];
					_ui.SetInventory (_saveData.itemsCollected, _saveData.itemsCount);
					StoreSaveData ();
					break;
				}
				else
				{
					_saveData.itemsCollected [i] = -1;				// Caso último da lista, apenas apaga
					_saveData.itemsCount [i] = 0;

					while (i < (INVENTORY_SIZE - 1))				// Caso haja mais itens, re-organiza a matriz
					{
						_saveData.itemsCollected [i] = _saveData.itemsCollected [++i];
						_saveData.itemsCount [--i] = _saveData.itemsCount [++i];
					}
				
					if (_saveData.isHoldingId == collectableId)		// Caso item em uso, retira do uso
					{
						_saveData.isHoldingId = -1;
						GetComponentInParent<Player> ().DropItem ();
					}
				
					_ui.SetInventory (_saveData.itemsCollected, _saveData.itemsCount);
					StoreSaveData ();
					break;
				}
			}
	}

	void InventoryInteract()		// Utiliza item em mãos
	{
		if (_saveData.isHoldingId != -1)
			GetComponentInParent<Player> ().Interact (_saveData.isHoldingId);
	}



	// GERENCIAMENTO DE ATALHOS DO TECLADO

	public void Update()
	{
		if (Input.GetKeyDown(KEY_CODE_QUALITY_UP))
		{
			QualitySettings.IncreaseLevel();
			SaveQualityLevel();
		}
		else if (Input.GetKeyDown(KEY_CODE_QUALITY_DOWN))
		{
			QualitySettings.DecreaseLevel();
			SaveQualityLevel();
		}
		else if (Input.GetKeyDown(KEY_CODE_INTERACT))	// Utilizar objecto em mãos
		{
			InventoryInteract();
		}
		else if (Input.GetKeyDown(KEY_CODE_INVENTORY))	// Abrir / Fechar Inventário
		{
			_ui.SelectInventoryButton ();
		}
		else if (Input.GetKeyDown(KEY_CODE_REFRESH))	// Apagar arquivo SaveData
		{
			DeleteSaveData();
		}
		else if (Input.GetKeyDown(KEY_CODE_CHEAT))	// Cheat code
		{
			CheatCode();
		}
		else if (Input.GetKeyDown(KEY_CODE_QUIT))
		{
			#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
			#else
			Application.Quit();
			#endif
		}
	}


}
