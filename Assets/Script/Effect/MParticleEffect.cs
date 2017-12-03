using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MParticleEffect : MonoBehaviour {

//	[SerializeField] ParticleSystem core;
//	[SerializeField] ParticleSystem bounce;
//	[SerializeField] ParticleSystem shine;
//	[SerializeField][ReadOnly] ParticleSystem[] List;
	[SerializeField][ReadOnly] List<ParticleData> particleDatas;

	[System.Serializable]
	public class ParticleData
	{
		public float timeRate;
		public ParticleSystem ps;

		public ParticleData( ParticleSystem _ps )
		{
			ps = _ps;
			timeRate = ps.emission.rateOverTimeMultiplier;
		}

		public void SetColor( Color col )
		{
			var main = ps.main;
			col.a = main.startColor.color.a;
			main.startColor = new ParticleSystem.MinMaxGradient (col);
		}

		public void SetColor( Color col1 , Color col2 )
		{
			var main = ps.main;
			col1.a = main.startColor.color.a;
			col2.a = main.startColor.color.a;
			main.startColor = new ParticleSystem.MinMaxGradient (col1,col2);
		}

		public void SetRate( float r )
		{
			var e = ps.emission;
			e.rateOverTimeMultiplier = timeRate * r;
		}
	};

//	[SerializeField][ReadOnly] float coreRate;
//	[SerializeField][ReadOnly] float bounceRate;
//	[SerializeField][ReadOnly] float shineRate;

	void Awake() {
		var List = GetComponentsInChildren<ParticleSystem> ();

		foreach (var p in List) {
			particleDatas.Add (new ParticleData (p));
		}

//		coreRate = core.emission.rateOverTimeMultiplier;
//		bounceRate = bounce.emission.rateOverTimeMultiplier;
//		shineRate = shine.emission.rateOverTimeMultiplier;
	}

	public void SetColor( Color col )
	{
		foreach (var p in particleDatas) {
			p.SetColor (col);
		}
	}

	public void SetColor( Color colMin , Color colMax )
	{
		foreach (var p in particleDatas) {
			p.SetColor (colMin, colMax );
		}
	}

	public void SetValue( float v )
	{
		float rate = (v > 1) ? Mathf.Log10 (v) : Mathf.Pow (v, 0.3f);

		foreach (var p in particleDatas) {
			p.SetRate (rate);
		}
	}

}
