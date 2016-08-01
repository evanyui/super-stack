/*
 * 	Camera Gradient Script
 *  Works with any camera set as CameraMain
 * 	Also working with the camera angles - 
 *  if you look down or upwards, the camera script
 *  automatically defines the UV of the created mesh.
 * 
 *  Tweak the script via inspector, allocate the equator,
 *  bottom and upper bounds, use one of the predefined 
 *  materials, reverse the colors if needed and you are 
 *  ready to go.
 * 
 *  Created by Domen Koneski, www.domenkoneski.com
 *  Give credits if you are using this in your projects.
 *  Feel free to share!
 * */

using UnityEngine;
using System.Collections;

public class CameraGradient : MonoBehaviour
{
	private Mesh thisMesh; 
	private Camera thisCam;
	private GameObject backgroundPlane;

	public Material material;
	public bool reverseColors;

	[Header("Advanced: Gradient factors")]
	public float equator = 1.6f;
	public float upperLimit;
	public float bottomLimit;
	private float currentFactor;

	[Space(2)]
	public float cameraAngleLimit;

	[Space(2)]
	public bool smoothFading;
	public float smoothFactor;

	void Awake()
	{	
		thisCam = Camera.main;

		//	Clear flags - color only
		thisCam.clearFlags = CameraClearFlags.Color;
	}

	void Start () 
	{
		//	You should assign this script to the main camera, fail check
		if (thisCam == null) 
		{
			throw new UnityException("Camera not found. " +
				"Did you attach me to the main " +
				"camera (or no camera was found " +
				"with the tag MainCamera)?");

			return;
		}

		//	Get the far clip distance
		float farClip = thisCam.farClipPlane - 0.01f;

		//	Find the vertices
		Vector3 
			topLeftPosition  = thisCam.ViewportToWorldPoint(new Vector3(0, 1, farClip)),
			topRightPosition = thisCam.ViewportToWorldPoint(new Vector3(1, 1, farClip)),
			btmLeftPosition  = thisCam.ViewportToWorldPoint(new Vector3(0, 0, farClip)),
			btmRightPosition = thisCam.ViewportToWorldPoint(new Vector3(1, 0, farClip));

		//	Create vertices using the clipping range in the procedure
		Vector3[] verts = new Vector3[] 
		{
			topLeftPosition, topRightPosition, btmLeftPosition, btmRightPosition
		};

		//	Create triangles for our mesh
		int[] tris = new int[] 
		{
			0, 1, 2, 
			2, 1, 3
		};

		thisMesh = new Mesh ();

		thisMesh.vertices = verts;

		//	Important!!! Set the uv of the mesh as signed
		int reverse = reverseColors ? 1 : 0;
		thisMesh.uv = new Vector2[]
		{
			new Vector2 (1 - reverse, 0 * currentFactor + reverse),
			new Vector2 (1 - reverse, 0 * currentFactor + reverse),
			new Vector2 (0 + reverse, 1 * currentFactor - reverse),
			new Vector2 (0 + reverse, 1 * currentFactor - reverse)
		};

		thisMesh.triangles = tris;
		backgroundPlane = new GameObject("_backgroundPlane");
		backgroundPlane.transform.parent = transform;
		backgroundPlane.AddComponent<MeshFilter>().mesh = thisMesh;
		backgroundPlane.AddComponent<MeshRenderer> ().material = material;
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.R)) 
		{
			Recreate ();
		}

		UpdateUV ();
	}

	//	Update this each update (or so), 
	//	so the UV changes (colors behave correctly)
	public void UpdateUV()
	{
		float cameraXRotation = thisCam.transform.rotation.eulerAngles.x;
		float calculatedFactor = 0;
		if (cameraXRotation < cameraAngleLimit) 
		{
			//	Go to the upper limit
			calculatedFactor = equator + 
				cameraXRotation / cameraAngleLimit * (upperLimit);
		}
		if (cameraXRotation >= 360 - cameraAngleLimit) 
		{
			calculatedFactor = equator - 
				(360 - cameraXRotation) / (cameraAngleLimit) * 
				(equator - bottomLimit);
		}

		//	Smooth fading enabled?
		currentFactor = smoothFading ? 
			Mathf.Lerp (currentFactor, calculatedFactor, Time.deltaTime * smoothFactor) :
			calculatedFactor;

		//	Clamp it, just in case
		currentFactor = Mathf.Clamp (currentFactor, bottomLimit, upperLimit);

		//	Update the UV of the mesh
		int reverse = reverseColors ? 1 : 0;
		thisMesh.uv = new Vector2[] 
		{
			new Vector2 (1 - reverse, 0 * currentFactor + reverse),
			new Vector2 (1 - reverse, 0 * currentFactor + reverse),
			new Vector2 (0 + reverse, 1 * currentFactor - reverse),
			new Vector2 (0 + reverse, 1 * currentFactor - reverse)
		};
	}

	//	Call this when you change the screen size, so the camera can generate 
	//	new mesh with the same material but includes appopriate width and height
	public void Recreate()
	{
		//	Destroy current
		Destroy(backgroundPlane);
		Start ();
	}
}