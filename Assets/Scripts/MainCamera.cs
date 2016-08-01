using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {

	public GameObject target;

	private float speed = 1.6f;
	private Vector3 point;

	private void Start () {
		point = target.transform.position;
		transform.LookAt(point);
	}

	private void Update () {
		transform.RotateAround (point,new Vector3(0.0f,1.0f,0.0f),20 * Time.deltaTime * speed);
	}
}
