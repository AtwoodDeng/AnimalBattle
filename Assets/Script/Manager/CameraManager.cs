using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using Sirenix.OdinInspector;

public class CameraManager : MBehavior {

	[SerializeField] Transform centerTarget;
	[SerializeField] float radius = 50f;
	[SerializeField][ReadOnly] float toRadius = 50f;
	[SerializeField] MinMax radiusRange = new MinMax (10f, 60f);
	[SerializeField] float angleY = 60f;
	[SerializeField] float angleX = 60f ;
	[SerializeField] float sensityY = 25f;
	[SerializeField] float sensityX = -25f;
	[SerializeField] float sensityZoom = 10f;
	[SerializeField] Vector3 offset = new Vector3 (0, -20f, 0);
	[SerializeField][ReadOnly] Vector3 toOffset ;

	protected override void MStart ()
	{
		base.MStart ();
		toRadius = radius;
		toOffset = offset;
	}

	protected override void MUpdate ()
	{
		base.MUpdate ();

		var inputDevice = InputManager.ActiveDevice;

		if (inputDevice != InputDevice.Null && inputDevice != TouchManager.Device)
		{
			TouchManager.ControlsEnabled = false;
		}

//		transform.Translate (Vector3.up * Time.deltaTime * inputDevice.Direction.Y);
//		transform.Translate (Vector3.right * Time.deltaTime * inputDevice.Direction.X);

		angleX = angleX + inputDevice.Direction.X * Time.deltaTime * sensityX;
		angleY = Mathf.Clamp (angleY + inputDevice.Direction.Y * Time.deltaTime * sensityY, 0, 90f);


		float posX = radius * Mathf.Sin (Mathf.Deg2Rad * angleY) * Mathf.Cos (Mathf.Deg2Rad * angleX);
		float posZ = radius * Mathf.Sin (Mathf.Deg2Rad * angleY) * Mathf.Sin (Mathf.Deg2Rad * angleX);
		float posY = radius * Mathf.Cos (Mathf.Deg2Rad * angleY);

		Vector3 off = toOffset;

		Vector3 pos = new Vector3 (posX, posY, posZ);
		Vector3 myPos = transform.position - centerTarget.position - off;
		Vector3 toPos = Vector3.Slerp (myPos, pos, 10f * Time.deltaTime);
		transform.position = toPos + centerTarget.position + off;

		transform.LookAt (centerTarget.position + off, Vector3.up);
	}


	public void OnPinch( PinchGesture gesture )
	{

		if( gesture.Phase == ContinuousGesturePhase.Started )
		{
		}
		else if( gesture.Phase == ContinuousGesturePhase.Updated )
		{
			float y = gesture.StartPosition.y / Screen.height;
			if (y > 0.5f && y < 0.95f ) {
				toRadius = Mathf.Clamp( toRadius + gesture.Delta.Centimeters () * sensityZoom , radiusRange.min , radiusRange.max);
				radius = Mathf.Lerp (radius, toRadius, 10f * Time.deltaTime);
				toOffset = radius * offset / 50f;
			}
		}
		else
		{
		}
		
	}

}
