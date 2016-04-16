using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class cameramovement : MonoBehaviour {
	private bool shift = false;
	public float flySpeed;
	public float accelerationRatio;
	public float slowDownRatio;
	private bool ctrl = false;
	public GameObject PlayerCam;
	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public static RotationAxes axes= RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;
	public float minimumX = -360F;
	public float maximumX = 360F;
	public float minimumY = -60F;
	public float maximumY = 60F;
	public Texture2D cursorTexture;
	public CursorMode cursorMode = CursorMode.Auto;
	public Vector2 hotSpot = Vector2.zero;

	public Text pauseText;
	Quaternion originalRotation;
	float rotationX = 0F;
	float rotationY = 0F;
	public bool pause;
	// Use this for initialization
	void Start () {
		originalRotation = transform.localRotation; pause = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.LeftAlt)) {
			pause = !pause;
			if (pause) {
				Time.timeScale = 0;	Screen.lockCursor = false;

				//showtext
			}
			else{
				Time.timeScale = 1; Screen.lockCursor = true;

			}
		}
	
		if (!pause) {
			if (axes == RotationAxes.MouseXAndY)
			{
				// Read the mouse input axis
				rotationX += Input.GetAxis("Mouse X") * sensitivityX;
				rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
				rotationX = ClampAngle (rotationX, minimumX, maximumX);
				rotationY = ClampAngle (rotationY, minimumY, maximumY);
				Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
				Quaternion yQuaternion = Quaternion.AngleAxis (rotationY, -Vector3.right);
				transform.localRotation = originalRotation * xQuaternion * yQuaternion;
			}
			else if (axes == RotationAxes.MouseX)
			{
				rotationX += Input.GetAxis("Mouse X") * sensitivityX;
				rotationX = ClampAngle (rotationX, minimumX, maximumX);
				Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
				transform.localRotation = originalRotation * xQuaternion;
			}
			else
			{
				rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
				rotationY = ClampAngle (rotationY, minimumY, maximumY);
				Quaternion yQuaternion = Quaternion.AngleAxis (-rotationY, Vector3.right);
				transform.localRotation = originalRotation * yQuaternion;
			}
			if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
			{
				shift = true;
				flySpeed *= accelerationRatio;
			}
			
			if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
			{
				shift = false;
				flySpeed /= accelerationRatio;
			}
			
			//use ctrl to slow up flight
			if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
			{
				ctrl = true;
				flySpeed *= slowDownRatio;
			}
			
			if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
			{
				ctrl = false;
				flySpeed /= slowDownRatio;
			}
			//
			if (Input.GetAxis("Vertical") != 0)
			{
				transform.Translate(Vector3.forward * flySpeed * Input.GetAxis("Vertical"));
			}
			
			
			if (Input.GetAxis("Horizontal") != 0)
			{
				transform.Translate(Vector3.right * flySpeed * Input.GetAxis("Horizontal"));
			}
			
			
			if (Input.GetKey(KeyCode.E))
			{
				transform.Translate(Vector3.up * flySpeed);
			}
			else if (Input.GetKey(KeyCode.Q))
			{
				transform.Translate(Vector3.down * flySpeed);
			}
			//if (Input.GetKeyDown(KeyCode.F12))
			//switchCamera(); don't need right now
			
			if (Input.GetKeyDown(KeyCode.M))
				PlayerCam.transform.position = transform.position;
				}
	}
	public static float ClampAngle (float angle, float min, float max)
	{
		if (angle <= -360F)
			angle += 360F;
		if (angle >= 360F)
			angle -= 360F;
		return Mathf.Clamp (angle, min, max);
	}
	void OnMouseEnter() {
		Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
	}
	void OnMouseExit() {
		Cursor.SetCursor(null, Vector2.zero, cursorMode);
	}
}
