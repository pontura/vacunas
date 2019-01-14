using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : Singleton<InputManager>
{
	public Text debbug;
	public types type;

	public bool button1Clicked;
	public bool button2Clicked;

	public enum types
	{
		GATILLO_DOWN,
		PAD_DOWN,
		PAD_HOLD,
		SWIPE_LEFT,
		SWIPPING,
		SWIPE_RIGHT,
		GATILLO_HOLD
	}
	public UIDebugger uiDebugger;

	float timerGatillo;
	bool padDown;
	bool gatilloDown;
	float axis;

	void Update () {
		debbug.text = button1Clicked + "-" + button2Clicked + "-" + OVRInput.Get(OVRInput.Axis1D.Any);

		if (Input.GetKeyDown (KeyCode.Space)) {
			SetNewGesto (types.GATILLO_DOWN);
			return;
		} else if (Input.GetKeyDown (KeyCode.KeypadEnter)) {
			SetNewGesto (types.PAD_DOWN);
			return;
		} else if (Input.GetKeyDown (KeyCode.LeftControl)) {
			SetNewGesto (types.PAD_HOLD);
			return;
		} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			SetNewGesto (types.SWIPE_LEFT);
			return;
		} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
			SetNewGesto (types.SWIPE_RIGHT);
			return;
		}
//		if (gatilloDown) {
//			timerGatillo += Time.deltaTime;
//			if (timerGatillo > 1 && type == types.GATILLO_DOWN) {
//				SetNewGesto (types.GATILLO_HOLD);
//				gatilloDown = false;
//				timerGatillo = 0;
//			}
//		}
//		if (padDown) {
//			timer += Time.deltaTime;
//			if (timer > 1 && type == types.PAD_DOWN) {
//				SetNewGesto (types.PAD_HOLD);
//				padDown = false;
//				timer = 0;
//			}
//		}

		if( OVRInput.GetUp(OVRInput.Touch.Any)){
			button1Clicked = false;
		}

		if(OVRInput.Get(OVRInput.Axis1D.Any)>0.9f){
			if (!button2Clicked) {
				UnityEngine.XR.InputTracking.Recenter ();
			
				//UnityEngine.XR.InputTracking.Recenter();

				gatilloDown = true;
				//this.type = types.GATILLO_DOWN;
				SetNewGesto (types.GATILLO_HOLD);
				SetNewGesto (types.GATILLO_DOWN);
				button2Clicked = true;

				if (CheckDoubleInput ())
					return;
			}

		}
		else if(OVRInput.Get(OVRInput.Axis1D.Any)<0.2f){
			button2Clicked = false;

		//	UnityEngine.XR.InputTracking.Recenter();

			//SetNewGesto(types.GATILLO_DOWN);
		} 

		if(OVRInput.GetDown(OVRInput.Button.One) && padDown == false){
			button1Clicked = true;

			padDown = true;

			if (CheckDoubleInput ())
				return;
			this.type = types.PAD_DOWN;

		}  else  if(OVRInput.GetUp(OVRInput.Button.One) && padDown == true){
			button1Clicked = false;
			padDown = false;
			SetNewGesto(types.PAD_DOWN);
		} else if( OVRInput.Get(OVRInput.Touch.Any) && padDown == false ){
			type = types.SWIPPING;
			axis = OVRInput.Get (OVRInput.Axis2D.Any).x;		
		} else if( OVRInput.GetUp(OVRInput.Touch.Any) && type == types.SWIPPING ){
			float endAxis = OVRInput.Get (OVRInput.Axis2D.Any).x;
			if(axis < -0.4f)
				SetNewGesto(types.SWIPE_LEFT);
			else if(axis > 0.4f)
				SetNewGesto(types.SWIPE_RIGHT);	
		} 
		
//		if( OVRInput.GetDown(OVRInput.Touch.Any)){
//			button2Clicked = true;
//			if (CheckDoubleInput ())
//				return;
//			UnityEngine.XR.InputTracking.Recenter();
//			SetNewGesto (types.GATILLO_HOLD);
//			SetNewGesto(types.GATILLO_DOWN);
//
//		}


	}
	bool CheckDoubleInput()
	{
		
		if (button2Clicked && button1Clicked) {
			SetNewGesto (types.PAD_HOLD);
			button1Clicked = false;
			button2Clicked = false;
			return true;
		}
		return false;
	}
	void SetNewGesto(types type)
	{
		this.type = type;

		if(uiDebugger != null)
			uiDebugger.SetField (type.ToString ());

		OnInput (type);
	}
	public System.Action<types> OnInput = delegate { };
}
