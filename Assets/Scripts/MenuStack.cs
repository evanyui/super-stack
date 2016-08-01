using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuStack : MonoBehaviour {

	public Color32 stackColor;
	public Material stackMaterial;
	public Text highScoreText;
	public AudioClip audioStart;
	public AudioClip audioIntro;

	private GameObject[] stacks;
	private AudioSource audio;

	private void Start () {

		audio = GetComponent<AudioSource> ();
		audio.clip = audioIntro;
		audio.Play ();

		highScoreText.text = PlayerPrefs.GetInt ("score").ToString();

		stacks = new GameObject[transform.childCount];

		for (int i = 0; i < transform.childCount; i++) {
			stacks[i] = transform.GetChild(i).gameObject;
			ColorMesh(stacks[i].GetComponent<MeshFilter>().mesh);
		}
	}

	private void ColorMesh(Mesh mesh) {
		Vector3[] vertices = mesh.vertices;
		Color32[] colors = new Color32[vertices.Length];

		for (int i = 0; i < vertices.Length; i++) {
			colors [i] = stackColor;
		}

		mesh.colors32 = colors;
	}
		
	public void OnButtonClick(string sceneName) {
		StartCoroutine(PlayAndStart (audioStart.length, sceneName));
	}

	private IEnumerator PlayAndStart(float time, string sceneName) {
		audio.clip = audioStart;
		audio.Play ();
		yield return new WaitForSeconds(time/audio.pitch);
		SceneManager.LoadScene (sceneName);
	}

}