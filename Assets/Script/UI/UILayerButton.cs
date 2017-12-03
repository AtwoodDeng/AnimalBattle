using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

public class UILayerButton : UIButton {

	[TabGroup("Contents", true )]
	[SerializeField] Image frame;

	[TabGroup("Contents", true )]
	[SerializeField] Image core;

	[TabGroup("Contents", true )]
	[SerializeField] Image icon;

	[TabGroup("Contents", true )]
	[SerializeField] Button button;

	public enum CallEnterType
	{
		None,
		OnStart,
		OnInit,
	}
	[TabGroup("Setting", true )]
	[SerializeField] CallEnterType m_callEnterType = CallEnterType.OnStart;
	[TabGroup("Setting", true )]
	[SerializeField] MinMax enterDelay ;
	[TabGroup("Setting", true )]
	[SerializeField] MinMax pressDelay ;
	[TabGroup("Setting", true )]
	[SerializeField] MinMax exitDelay ;
	[TabGroup("Setting", true )]
	[SerializeField] bool PressMessage = true;

	[TabGroup("Animation", true )]
	[SerializeField] float animationDuration=1f;
	[TabGroup("Animation", true )]
	[SerializeField] float normalScale = 1f;
	[TabGroup("Animation", true )]
	[SerializeField] float highLightScale = 1.5f;
	[TabGroup("Animation", true )]
	[SerializeField] EnterType m_enterType = EnterType.ScaleUp;
	[TabGroup("Animation", true )]
	[SerializeField] PressType m_pressType = PressType.Intense;
	[TabGroup("Animation", true )]
	[SerializeField] ExitType m_exitType = ExitType.Shrink;

	[TabGroup("Animation", true )]
	[SerializeField][Range(0,1f)] float enterAnimation=1f;
	[TabGroup("Animation", true )]
	[SerializeField][Range(0,1f)] float pressAnimation=1f;
	[TabGroup("Animation", true )]
	[SerializeField][Range(0,1f)] float exitAnimation = 1f;

	public enum EnterType
	{
		ScaleUp,
		Clock,
	}

	public enum PressType
	{
		Intense,
		Soft,
		Riffle,
	}

	public enum ExitType
	{
		Shrink,
		Fade,
		Clock,
	}

	[TabGroup("Sound", true )]
	[SerializeField] AudioClip enterSound;
	[TabGroup("Sound", true )]
	[SerializeField] AudioClip pressSound;
	[TabGroup("Sound", true )]
	[SerializeField] AudioClip exitSound;
	[TabGroup("Sound", true )]
	[SerializeField][ReadOnly] AudioSource m_source;
	[TabGroup("Sound", true )]
	[SerializeField] MinMax m_pitch = new MinMax(1f,1f);

	private float lastDuration;
	private float lastDelay;

	private GameObject rippleFrame1;
	private GameObject rippleFrame2;

	protected override void MAwake ()
	{
		base.MAwake ();

		m_source = gameObject.AddComponent<AudioSource> ();
		m_source.playOnAwake = false;
		m_source.volume = 0.5f;
		m_source.spatialBlend = 0;
	}

	protected override void MStart ()
	{
		base.MStart ();

		if ( m_callEnterType == CallEnterType.OnStart)
			OnEnter ( true );
	}

	public void Init( ButtonType type , IconType iconType = IconType.None )
	{
		// icon.sprite = Resources.Load ("Icon/" + type.ToString () +"_Icon") as Sprite;
		if (iconType != null && iconType != IconType.None ) {
			icon.sprite = Resources.Load<Sprite> ("Icon/" + iconType.ToString () + "_Icon");

		}
		m_type = type;

		if (m_callEnterType == CallEnterType.OnInit)
			OnEnter (true);

		if (type == ButtonType.FeedNormalMeat) {
			m_pitch.min *= 0.8f;
			m_pitch.max *= 0.8f;	
		} else if (type == ButtonType.FeedNormalGrass) {
			m_pitch.min *= 1.2f;
			m_pitch.max *= 1.2f;	
		}

	}

	override public void OnEnter ( bool isForce = false )
	{
		float duration = animationDuration * enterAnimation;
		float delay = enterDelay.rand; 

		if (m_enterType == EnterType.ScaleUp) {
			if (isForce) {
				if (frame != null)
					frame.transform.localScale = Vector3.zero;
				if (core != null)
					core.transform.localScale = Vector3.zero;
				button.interactable = false;
			}

			if (frame != null) {
				frame.transform.DOKill ();
				frame.DOKill ();
				frame.transform.DOScale (normalScale, duration * 0.7f).SetEase (Ease.OutBack).SetDelay (delay);
			}
			if (core != null) {
				core.DOKill ();
				core.transform.DOKill ();
				core.transform.DOScale (normalScale, duration * 0.7f).SetEase (Ease.OutBack).SetDelay (0.3f * duration + delay);
			}
			//		icon.transform.DOScale (endScale, duration * 0.7f).SetEase (Ease.OutBack).SetDelay (0.3f * duration);
		} else if (m_enterType == EnterType.Clock) {
			if (isForce) {
				if (frame != null)
					frame.fillAmount = 0;
				if (core != null)
					core.fillAmount = 0;
				if (icon != null) {
					var col = icon.color;
					col.a = 0;
					icon.color = col;
				}
			}

			if (frame != null) {
				frame.transform.DOKill ();
				frame.DOKill ();
				frame.DOFillAmount (1f, duration * 0.7f).SetEase (Ease.InOutCubic).SetDelay (delay );
			}

			if (core != null) {
				core.DOKill ();
				core.transform.DOKill ();
				core.DOFillAmount (1f, duration * 0.7f).SetEase (Ease.InOutCubic).SetDelay (delay + 0.3f * duration);
			}
			if (icon != null) {
				icon.DOKill ();
				icon.DOFade (1f, duration * 0.7f).SetDelay (delay+ 0.3f * duration);
			}
		}

		Invoke ( "OnEnterFinal" , delay + duration);
		PlaySound (enterSound , delay);
		lastDelay = delay;
		lastDuration = duration;
	}


	public void OnEnterFinal()
	{
		button.interactable = true;
	}

	public void OnPress( )
	{
		float duration = animationDuration * pressAnimation;
		float delay = pressDelay.rand;
		if (m_pressType == PressType.Intense) {
			float frameScale = highLightScale;
			float coreScale = Mathf.Lerp (highLightScale, normalScale, 0.7f);

			if (frame != null) {
				frame.transform.DOKill ();
				frame.DOKill ();
				frame.transform.DOScale (frameScale, duration * 0.03f).SetEase (Ease.OutCubic).SetDelay (delay);
				frame.transform.DOScale (normalScale, duration * 0.97f).SetEase (Ease.OutCubic).SetDelay (delay + duration * 0.03f);
			}

			if (core != null) {
				core.DOKill ();
				core.transform.DOKill ();
				core.transform.DOScale (coreScale, duration * 0.3f).SetEase (Ease.OutQuart).SetDelay (delay);
				core.transform.DOScale (normalScale, duration * 0.7f).SetEase (Ease.InOutQuart).SetDelay (delay + duration * 0.3f);
			}
		} else if (m_pressType == PressType.Soft) {
			float frameScale = highLightScale;
			float coreScale = highLightScale;

			if (frame != null) {
				frame.transform.DOKill ();
				frame.DOKill ();
				frame.transform.DOScale (frameScale, duration * 0.2f).SetEase (Ease.InCubic).SetDelay (delay);
				frame.transform.DOScale (normalScale, duration * 0.6f).SetEase (Ease.OutCubic).SetDelay (delay + duration * 0.3f);
			}

			if (core != null) {
				core.DOKill ();
				core.transform.DOKill ();
				core.transform.DOScale (coreScale, duration * 0.2f).SetEase (Ease.InCubic).SetDelay (delay + duration * 0.2f);
				core.transform.DOScale (normalScale, duration * 0.6f).SetEase (Ease.OutCubic).SetDelay (delay + duration * 0.2f);
			}
		} else if (m_pressType == PressType.Riffle) {
			PlayRippleEffect (duration, delay);
		}

		if (PressMessage) {
			M_Event.FireLogicEvent (LogicEvents.ButtonPress, new ButtonArg (this, m_type , this ));

		}

		PlaySound (pressSound , delay);

		lastDelay = delay;
		lastDuration = duration;
	}

	public bool PlayRippleEffect( float duration , float delay )
	{
		if (frame == null)
			return false;
		
		if ( rippleFrame1 == null )
			rippleFrame1 = Instantiate (frame.gameObject, frame.transform.parent) as GameObject;
		if ( rippleFrame2 == null )
			rippleFrame2 = Instantiate (frame.gameObject, frame.transform.parent) as GameObject;

		var Image1 = rippleFrame1.GetComponent<Image> ();
		var Image2 = rippleFrame2.GetComponent<Image> ();

		if (Image1 == null || Image2 == null)
			return false;

		float frameScale = highLightScale;

		rippleFrame1.transform.DOKill ();
		Image1.DOKill ();
		rippleFrame1.transform.localScale = Vector3.one;
		var col = Image1.color;
		col.a = 1f;
		Image1.color = col;


		rippleFrame2.transform.DOKill ();
		Image2.DOKill ();
		rippleFrame2.transform.localScale = Vector3.one;
		col = Image2.color;
		col.a = 1f;
		Image2.color = col;

		rippleFrame1.transform.DOScale (frameScale, duration).SetEase (Ease.InOutCubic).OnComplete (delegate {
			button.interactable = true;	
		});
		rippleFrame2.transform.DOScale (frameScale, duration * 0.66f ).SetEase (Ease.InOutCubic).SetDelay(0.33f * duration);
		Image1.DOFade (0, 0.33f * duration).SetDelay (0.66f * duration);
		Image2.DOFade (0, 0.33f * duration).SetDelay (0.66f * duration);

		button.interactable = false;
//		Sequence seq = DOTween.Sequence ();
//		seq.AppendInterval (delay);
//		seq.Append (rippleFrame1.transform.DOScale (1f, 0));
//		seq.Append (Image1.DOFade (1f, 0));
//		seq.Append (rippleFrame2.transform.DOScale (1f, 0));
//		seq.Append (Image2.DOFade (1f, 0));
//		seq.Append (rippleFrame1.transform.DOScale (frameScale, duration).SetEase (Ease.InOutCubic));
//		seq.Join (rippleFrame2.transform.DOScale (frameScale, duration * 0.66f ).SetEase (Ease.InOutCubic).SetDelay(0.33f * duration));
//		seq.Join (Image1.DOFade (0, duration * 0.33f ).SetEase (Ease.Linear).SetDelay( duration * 0.66f ) );
//		seq.Join (Image2.DOFade (0, duration * 0.33f ).SetEase (Ease.Linear).SetDelay( duration * 0.66f ) );

		return true;
	}


	override public void OnExit( bool isDestory )
	{

		float duration = animationDuration * exitAnimation;
		float delay = exitDelay.rand;

		button.interactable = false;

		if (m_exitType == ExitType.Shrink) {
//			float frameScale = Mathf.Lerp (highLightScale, normalScale, 0.7f);
//			float coreScale = Mathf.Lerp (highLightScale, normalScale, 0.7f);
			if (frame != null) {
				frame.transform.DOKill ();
				frame.DOKill ();
				frame.transform.DOScale (0, duration * 0.8f).SetDelay (delay + duration * 0.2f).SetEase (Ease.InBack);
			}
			if (core != null) {
				core.transform.DOScale (0, duration * 0.8f).SetDelay (delay ).SetEase (Ease.InBack);
			}
		} else if (m_exitType == ExitType.Clock) {
			if (frame != null) {
				frame.transform.DOKill ();
				frame.DOKill ();
				frame.DOFillAmount (0, duration * 0.8f).SetDelay (delay + duration * 0.2f).SetEase (Ease.InOutCubic);
			}

			if (core != null) {
				core.DOKill ();
				core.transform.DOKill ();
				core.DOFillAmount (0, duration * 0.8f).SetDelay (delay  ).SetEase (Ease.InOutCubic);
			}

			if (icon != null) {
				icon.DOKill ();
				icon.DOFade (0, duration * 0.8f).SetDelay (delay).SetEase (Ease.InCubic);
			}
		} else if (m_exitType == ExitType.Fade) {
			if (frame != null) {
				frame.transform.DOKill ();
				frame.DOKill ();
				frame.DOFade (0, duration * 0.8f).SetDelay (delay + duration * 0.2f).SetEase (Ease.InOutCubic);
				frame.transform.DOScale (0, duration * 0.8f).SetDelay (delay + duration * 0.2f).SetEase (Ease.InBack);
			}

			if (core != null) {
				core.DOKill ();
				core.transform.DOKill ();
				core.DOFade (0, duration * 0.8f).SetDelay (delay  ).SetEase (Ease.InOutCubic);
				core.transform.DOScale (0, duration * 0.8f).SetDelay (delay ).SetEase (Ease.InBack);
			}

			if (icon != null) {
				icon.DOKill ();
				icon.DOFade (0, duration * 0.8f).SetDelay (delay).SetEase (Ease.InCubic);
			}
		}

		PlaySound (exitSound , delay);
		lastDelay = delay;
		lastDuration = duration;

		if (isDestory) {
			Destroy (gameObject, Mathf.Max (exitSound ? exitSound.length : 0, duration + delay)); 
		}
	}

	public void PlaySound( AudioClip clip , float delay )
	{
		if (clip == null) {
			m_source.Stop ();
			return;
		}
		
		m_source.clip = clip;
		m_source.pitch = m_pitch.rand;
		m_source.PlayDelayed (delay);
	}

	public override void PlayMoveFrom (Vector3 fromPos, float duration = -1f, float delay = -1f )
	{
		if (duration < 0)
			duration = lastDuration;
		if (delay < 0)
			delay = lastDelay;

//		Debug.Log ("Play From " + duration + " " +  delay);
		transform.DOMove (fromPos, duration).From ().SetDelay (delay);
	}

}
