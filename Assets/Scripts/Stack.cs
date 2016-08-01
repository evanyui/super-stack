using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Stack : MonoBehaviour {

	public Color32 stackColor;
	public Color32 burntColor;
	public Material stackMaterial;
	public GameObject endPanel;
	public GameObject flameParticle;
	public GameObject circle;
	public GameObject particle;
	public Text highScoreText;
	public Text comboCount;
	public Text scoreText;
	public AudioClip[] erik = new AudioClip[9];
	public AudioClip blop;

	private const float BOUNDS_SIZE = 5.0f;
	private const float STACK_SIZE = 3.5f;
	private const float MOVE_SPEED = 5.0f;
	private const float ERR_MARGIN = 0.1f;
	private const float STACK_GAIN = 0.5f;
	private const int COMBO_START_GAIN = 1;
	private const float ORIG_TILE_SPEED = 2.5f;
	private const float TILE_SPEEDUP = 0.02f;

	private AudioSource audio;

	private GameObject[] stacks;

	private int stackIndex;
	private int scoreCount = 0;
	private int combo = 0;

	private float tileTransition = 0.0f;
	private float tileSpeed = 2.0f;
	private float alignPosition;

	private bool isMovingOnX = true;
	private bool gameOver = false;

	private Vector3 desiredPosition;
	private Vector3 lastPosition;

	private Vector2 stackBounds = new Vector2(STACK_SIZE, STACK_SIZE);


	private void Start () {

		audio = GetComponent<AudioSource> ();

		highScoreText.text = PlayerPrefs.GetInt ("score").ToString();
		
		stacks = new GameObject[transform.childCount];

		for (int i = 0; i < transform.childCount; i++) {
			stacks[i] = transform.GetChild(i).gameObject;
			ColorMesh(stacks[i].GetComponent<MeshFilter>().mesh);
		}

		stackIndex = transform.childCount - 1;

		comboCount.enabled = false;
	}
	
	private void Update () {

		if (gameOver)
			return;
		
		if (Input.GetMouseButtonDown (0)) {

			//sfx
			AudioSource audio = GetComponent<AudioSource>();
			audio.Play();

			if (PlaceTile ()) {
				SpawnTile ();

				scoreCount++;
				scoreText.text = scoreCount.ToString ();

				tileSpeed = ORIG_TILE_SPEED+(TILE_SPEEDUP * scoreCount);

			} else {
				End ();
			}

		}

		MoveTile ();

		transform.position = Vector3.Lerp (transform.position, desiredPosition, MOVE_SPEED * Time.deltaTime);
	}

	private void MoveTile() {

		tileTransition += Time.deltaTime * tileSpeed;

		if(isMovingOnX)
			stacks [stackIndex].transform.localPosition = new Vector3 (Mathf.Sin (tileTransition) * BOUNDS_SIZE, scoreCount, alignPosition);
		else 
			stacks [stackIndex].transform.localPosition = new Vector3 (alignPosition, scoreCount, Mathf.Sin (tileTransition) * BOUNDS_SIZE);
		
	}

	private bool PlaceTile() {

		if (isMovingOnX) {
			float deltaX = lastPosition.x - stacks [stackIndex].transform.position.x;

			// Not perfect
			if (Mathf.Abs (deltaX) > ERR_MARGIN) {
				
				audio.clip = blop;
				audio.Play ();

				combo = 0;
				comboCount.enabled = false;

				stackBounds.x -= Mathf.Abs (deltaX);

				if (stackBounds.x <= 0) {
					return false;
				}

				float middle = lastPosition.x + stacks [stackIndex].transform.localPosition.x / 2;
				stacks [stackIndex].transform.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
				createRubble (
					new Vector3( (stacks [stackIndex].transform.position.x > 0) ?
						stacks [stackIndex].transform.position.x + (stacks [stackIndex].transform.localScale.x/2) :
						stacks [stackIndex].transform.position.x - (stacks [stackIndex].transform.localScale.x/2),
						stacks [stackIndex].transform.position.y,
						stacks [stackIndex].transform.position.z),
					new Vector3(Mathf.Abs(deltaX), 1, stacks [stackIndex].transform.localScale.z)
				);
				stacks [stackIndex].transform.localPosition = new Vector3 (middle - (lastPosition.x / 2), scoreCount, lastPosition.z);

			// If perfect
			} else {

				int clipNum = (combo % (erik.Length));
				audio.clip = erik[clipNum];
				audio.Play ();
				
				// Continue Combo
				if (combo >= COMBO_START_GAIN) {
					stackBounds.x += STACK_GAIN;

					if (stackBounds.x > STACK_SIZE)
						stackBounds.x = STACK_SIZE;
					
					float middle = lastPosition.x + stacks [stackIndex].transform.localPosition.x / 2;
					stacks [stackIndex].transform.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
					stacks [stackIndex].transform.localPosition = new Vector3 (middle - (lastPosition.x / 2), scoreCount, lastPosition.z);
				}
				combo++;
				comboCount.enabled = true;
				comboCount.text = "+" + combo;

				stacks [stackIndex].transform.localPosition = new Vector3 (lastPosition.x, scoreCount, lastPosition.z);

				//sfx
				playCircle(stacks [stackIndex].transform.localPosition.x, stacks [stackIndex].transform.localPosition.z);

			}

		} else {	//if moving on z
			float deltaZ = lastPosition.z - stacks [stackIndex].transform.position.z;

			// Not Perfect
			if (Mathf.Abs (deltaZ) > ERR_MARGIN) {
				
				audio.clip = blop;
				audio.Play ();

				combo = 0;
				comboCount.enabled = false;

				stackBounds.y -= Mathf.Abs (deltaZ);

				if (stackBounds.y <= 0) {
					return false;
				}

				float middle = lastPosition.z + stacks [stackIndex].transform.localPosition.z / 2;
				stacks [stackIndex].transform.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
				createRubble (
					new Vector3( stacks [stackIndex].transform.position.x,
						stacks [stackIndex].transform.position.y,
						(stacks [stackIndex].transform.position.z > 0) ?
						stacks [stackIndex].transform.position.z + (stacks [stackIndex].transform.localScale.z/2) :
						stacks [stackIndex].transform.position.z - (stacks [stackIndex].transform.localScale.z/2)),
					new Vector3(stacks [stackIndex].transform.localScale.x, 1, Mathf.Abs(deltaZ))
				);
				stacks [stackIndex].transform.localPosition = new Vector3 (lastPosition.x, scoreCount, middle - (lastPosition.z / 2));

			// If Perfect
			} else {

				int clipNum = (combo % (erik.Length));
				audio.clip = erik[clipNum];
				audio.Play ();

				// Continue Combo
				if (combo >= COMBO_START_GAIN) {
					stackBounds.y += STACK_GAIN;

					if (stackBounds.y > STACK_SIZE)
						stackBounds.y = STACK_SIZE;
						
					float middle = lastPosition.z + stacks [stackIndex].transform.localPosition.z / 2;
					stacks [stackIndex].transform.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
					stacks [stackIndex].transform.localPosition = new Vector3 (lastPosition.x, scoreCount, middle - (lastPosition.z / 2));
				}
				combo++;
				comboCount.enabled = true;
				comboCount.text = "+" + combo;

				stacks [stackIndex].transform.localPosition = new Vector3 (lastPosition.x, scoreCount, lastPosition.z);

				//sfx
				playCircle(stacks [stackIndex].transform.localPosition.x, stacks [stackIndex].transform.localPosition.z);

			}

		}

		alignPosition = (isMovingOnX) ? 
			stacks [stackIndex].transform.localPosition.x : 
			stacks [stackIndex].transform.localPosition.z;

		isMovingOnX = !isMovingOnX;

		//sfx
		playParticle (stacks [stackIndex].transform.localPosition.x, stacks [stackIndex].transform.localPosition.z);

		return true;
	}

	private void SpawnTile() {

		lastPosition = stacks [stackIndex].transform.localPosition;

		stackIndex--;

		if (stackIndex < 0) {
			stackIndex = transform.childCount - 1;
		}

		desiredPosition = (Vector3.down) * scoreCount;

		stacks [stackIndex].transform.localPosition = new Vector3 (0, scoreCount, 0);
		stacks [stackIndex].transform.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);

		ColorMesh (stacks [stackIndex].GetComponent<MeshFilter> ().mesh);

	}

	private void createRubble(Vector3 pos, Vector3 scale) {
		GameObject go = GameObject.CreatePrimitive (PrimitiveType.Cube);
		go.transform.localPosition = pos;
		go.transform.localScale = scale;
		go.AddComponent<Rigidbody> ();

		GameObject flame =  Instantiate(flameParticle);
		flame.transform.parent = go.transform;
		flame.transform.localPosition = new Vector3 (0,0,0);
		flame.transform.localScale = go.transform.localScale;
		if (isMovingOnX) {
			flame.transform.Rotate (90,0,0);
		}

		go.GetComponent<MeshRenderer> ().material = stackMaterial;

		ColorMeshBurnt (go.GetComponent<MeshFilter> ().mesh);
	}

	private void ColorMesh(Mesh mesh) {
		Vector3[] vertices = mesh.vertices;
		Color32[] colors = new Color32[vertices.Length];

		for (int i = 0; i < vertices.Length; i++) {
			colors [i] = stackColor;
		}

		mesh.colors32 = colors;
	}

	private void ColorMeshBurnt(Mesh mesh) {
		Vector3[] vertices = mesh.vertices;
		Color32[] colors = new Color32[vertices.Length];

		for (int i = 0; i < vertices.Length; i++) {
			colors [i] = burntColor;
		}

		mesh.colors32 = colors;
	}

	private void playCircle(float x, float z) {
		circle.transform.position = new Vector3(x, -0.5f, z);
		ParticleSystem circleParticle = circle.transform.GetComponentInChildren<ParticleSystem> ();
		if (!circleParticle.isPlaying) {
			circleParticle.Play ();
		} else {
			circleParticle.Stop ();
		}
	}

	private void playParticle(float x, float z) {
		particle.transform.position = new Vector3(x, -0.5f, z);
		ParticleSystem smallParticle = particle.transform.GetComponent<ParticleSystem> ();
		if (!smallParticle.isPlaying) {
			smallParticle.Play ();
		} else {
			smallParticle.Stop ();
		}
	}

	private void End() {
		if (PlayerPrefs.GetInt ("score") < scoreCount) {
			PlayerPrefs.SetInt ("score", scoreCount);
		}
		gameOver = true;
		endPanel.SetActive (true);
		stacks [stackIndex].AddComponent<Rigidbody> ();
	}

	public void OnButtonClick(string sceneName) {
		SceneManager.LoadScene (sceneName);
	}
				
}
