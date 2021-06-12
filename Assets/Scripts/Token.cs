using UnityEngine;

public class Token : MonoBehaviour
{
    public const string TAG = "Token";

	public int _points = 1;


    public void Collect()
    {
        Destroy(gameObject);
    }
}
