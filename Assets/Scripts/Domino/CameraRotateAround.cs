using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotateAround : MonoBehaviour
{
	public Transform target;
	public Vector3 offset;
	public float sensitivity = 3; // чувствительность мышки
	public float limit = 80; // ограничение вращения по Y
	public float zoomSensitivity = 0.25f; // чувствительность при увеличении, колесиком мышки
	public float zoomMax = 10; // макс. увеличение
	public float zoomMin = 3; // мин. увеличение
	public float startZoom;
	public Transform menuPosition, gameStartPosition;
	public float transitSpeed = 3f;
	private float X, Y;
	private bool isCameraRotationEnabled = false, isCameraRotating = false;
	private GameCore gameCore;
	private bool isInMenuState = true;
	


	void Start()
	{
		gameCore = GameObject.Find("GameCore").GetComponent<GameCore>();
		limit = Mathf.Abs(limit);
		if (limit > 90) limit = 90;
		if (startZoom > zoomMax) startZoom = zoomMax;
		if (startZoom > zoomMin) startZoom = zoomMin;
		offset = new Vector3(offset.x, offset.y, -startZoom);
		Y = transform.localEulerAngles.x;
		X = transform.localEulerAngles.y;
	}

	public bool IsInMenu()
    {
		return isInMenuState;
    }

	public void GoMenu()
    {
		StartCoroutine(_GoMenu());
    }

	IEnumerator _GoMenu()
	{
		isCameraRotationEnabled = false;
		while (transform.position != menuPosition.position)
        {
			transform.position = Vector3.MoveTowards(transform.position, menuPosition.position, transitSpeed * Time.deltaTime);
			transform.LookAt(target);
			yield return new WaitForEndOfFrame();
        }
		isInMenuState = true;
		yield break;
    }

	public void GoGame()
    {
		StartCoroutine(_GoGame());
	}

	IEnumerator _GoGame()
	{
		isInMenuState = false;
		while (transform.position != gameStartPosition.position)
		{
			transform.position = Vector3.MoveTowards(transform.position, gameStartPosition.position, transitSpeed * Time.deltaTime);
			transform.LookAt(target);
			yield return new WaitForEndOfFrame();
		}
		isCameraRotationEnabled = true;
		Y = transform.localEulerAngles.x;
		X = transform.localEulerAngles.y;
		offset.z = -Vector3.Distance(transform.position, target.position);
		gameCore.StartGame();
		yield break;
	}

	void Update()
	{
		if (isCameraRotationEnabled)
		{
			if (Input.GetKeyDown(KeyCode.Mouse1) && !gameCore.stoneBeingDragged)
			{
				isCameraRotating = true;
			}
			if (Input.GetKeyUp(KeyCode.Mouse1))
			{
				isCameraRotating = false;
			}
			if (isCameraRotating)
			{
				X += Input.GetAxis("Mouse X") * sensitivity;
				X = Mathf.Clamp(X, -limit, limit);

				Y += Input.GetAxis("Mouse Y") * sensitivity;
				Y = Mathf.Clamp(Y, -limit, limit);
				transform.localEulerAngles = new Vector3(-Y, X, 0);
			}
			if (Input.GetAxis("Mouse ScrollWheel") > 0) offset.z += zoomSensitivity;
			else if (Input.GetAxis("Mouse ScrollWheel") < 0) offset.z -= zoomSensitivity;
			offset.z = Mathf.Clamp(offset.z, -Mathf.Abs(zoomMax), -Mathf.Abs(zoomMin));
			transform.position = transform.localRotation * offset + target.position;
		}
	}
}