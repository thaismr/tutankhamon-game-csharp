using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {

	public Slider _volume;

	public void MusicVolume () 
	{
		AudioListener.volume = _volume.value;
	}

	public void SfxVolume () 
	{
		AudioListener.volume = _volume.value;
	}

}
