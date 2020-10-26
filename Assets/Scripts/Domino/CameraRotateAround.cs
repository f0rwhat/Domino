using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotateAround : MonoBehaviour
{

	public Transform target;
	public Vector3 offset;
	public float sensitivity = 3; // чувствительность мышки
	public float limit = 80; // ограничение вращения по Y
	public float zoom = 0.25f; // чувствительность при увеличении, колесиком мышки
	public float zoomMax = 10; // макс. увеличение
	public float zoomMin = 3; // мин. увеличение
	private float X, Y;
	private bool isCameraRotationEnabled = false;
	private GameCore gameCore;

	void Start()
	{
		gameCore = GameObject.Find("GameCore").GetComponent<GameCore>();
		limit = Mathf.Abs(limit);
		if (limit > 90) limit = 90;
		offset = new Vector3(offset.x, offset.y, -Mathf.Abs(zoomMax) / 2);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Mouse1) && !gameCore.stoneBeingDragged)
		{
			isCameraRotationEnabled = true;
		}
		if (Input.GetKeyUp(KeyCode.Mouse1))
		{
			isCameraRotationEnabled = false;
		}
		if (isCameraRotationEnabled)
		{
			X += Input.GetAxis("Mouse X") * sensitivity;
			X = Mathf.Clamp(X, -limit, limit);

			Y += Input.GetAxis("Mouse Y") * sensitivity;
			Y = Mathf.Clamp(Y, -limit, limit);
			transform.localEulerAngles = new Vector3(-Y, X, 0);
		}
		if (Input.GetAxis("Mouse ScrollWheel") > 0) offset.z += zoom;
		else if (Input.GetAxis("Mouse ScrollWheel") < 0) offset.z -= zoom;
		offset.z = Mathf.Clamp(offset.z, -Mathf.Abs(zoomMax), -Mathf.Abs(zoomMin));
		transform.position = transform.localRotation * offset + target.position;
	}
}