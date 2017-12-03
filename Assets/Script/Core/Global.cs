using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global {

	public static EnergyType[] EnergyList = {
		EnergyType.Brightness,
		EnergyType.Darkness,
		EnergyType.Fire,
		EnergyType.Water,
		EnergyType.Rock,
		EnergyType.Tree,
		EnergyType.Wind,
	};

	public static void PlaySound( AudioClip _clip , GameObject _parent 
		, float _volume = 0.5f , float blend = 1f , bool isLoop = false, UnityEngine.Audio.AudioMixerGroup _group = null )
	{
		if (_clip == null)
			return;
		
		AudioSource source = _parent.AddComponent<AudioSource> ();
		source.clip = _clip;
		source.volume = _volume;
		source.spatialBlend = blend;
		source.loop = isLoop;

		source.Play ();
		GameObject.Destroy (source, _clip.length + 1f );
	}

	public static float GetDistance( GameObject obj1, GameObject obj2 )
	{
		return GetDistance (obj1.transform, obj2.transform);
	}

	public static float GetDistance( Transform obj1, Transform obj2 )
	{
		return GetDistance (obj1.position, obj2.position);
	}

	public static float GetDistance( Vector3 pos1, Vector3 pos2 )
	{
		Vector3 dis = pos1 - pos2;
		dis.y = 0;
		return dis.magnitude;
	}
}


[System.Serializable]
public class MinMax
{
	public float min;
	public float max;
	public float rand { get { return Random.Range (min, max); } } 
	public MinMax( float _min , float _max )
	{
		min = _min;
		max = _max;
	}
}
