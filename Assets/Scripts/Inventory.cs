using UnityEngine;
using System.Collections.Generic;

public class Inventory: MonoBehaviour
{

	// ESQUELETO DA BASE DE DADOS DO INVENTÁRIO:

	[System.Serializable]		// Serializable para possibilitar a criação de uma Lista com esta classe
	public class Item
	{
		public GameObject _prefab;

		public int _id;

		public string _name;

		public int[] _materials = {-1,-1};
	}



	// INICIALIZAÇÃO DA BASE DE DADOS DO INVENTÁRIO:

	public static Inventory INV;

	public List<Item> _collectablesDatabase = new List<Item>();		// Lista "non-static" para preenchimento do inventário a partir do editor Unity

	public static List<Item> _item;		// Cópia "static" para acesso directo ao inventário, em qualquer script


	void Awake()
	{
		if (INV == null)
		{
			INV = this;
		}
		else if (INV != this)
		{
			Destroy (gameObject);
		}	

		_item = new List<Item>(_collectablesDatabase);	// Copia a base de dados preenchida no prefab "Inventário" a partir do editor Unity

	}


	// FUNÇÕES DE ACESSO A DADOS DO INVENTÁRIO:

	public static int Crafting(int holding, int makeuse)	// Retorna Id do Item a construir (ou -1), conforme materais
	{
		List<Item> hasMaterials = _item.FindAll (x => x._materials.Length > 1);		// Limita busca a Itens com mais de 1 material

		Item crafted = hasMaterials.Find (
			x => ((x._materials [0] == holding && x._materials [1] == makeuse) || (x._materials [0] == makeuse && x._materials [1] == holding))
		               );

		return crafted == null ? -1 : crafted._id;
	}


}
