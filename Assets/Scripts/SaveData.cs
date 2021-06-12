using System;

[Serializable]
public class SaveData
{
	public int playerId = -1;

	public long passCode = -1;

	public int totalScore;

	public int isHoldingId = -1;

	public int[] itemsCollected = {-1,-1,-1,-1,-1};

	public int[] itemsCount = {0,0,0,0,0};
}
