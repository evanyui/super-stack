using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainCamera : MonoBehaviour {

	public GameObject target;
	public Text scoreText;

	private float speed = 1.6f;
	private Vector3 point;
	private float scoreSpeed;

	private void Start () {
		point = target.transform.position;
		transform.LookAt(point);
	}

	private void Update () {
		if (scoreText != null) {
			scoreSpeed = (float.Parse (scoreText.text)) / 100;
			transform.RotateAround (point, new Vector3 (0.0f, 1.0f, 0.0f), 20 * Time.deltaTime * (speed + scoreSpeed));
		} else {
			transform.RotateAround (point, new Vector3 (0.0f, 1.0f, 0.0f), 20 * Time.deltaTime * (speed));
		}
	}
}
