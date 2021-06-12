using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 rotation;

	void Update()
    {
        transform.Rotate(Time.deltaTime * rotation);
	}
}
