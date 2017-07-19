using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tanker : MonoBehaviour, IShipBase {

	public GameObject flipperPrefab;
	[HideInInspector] public float moveSpeed = 1f;

	[HideInInspector] public MapLine curMapLine;

	private MapManager _mapManager;
	private GameManager _gameManager;
	private float _lastFire;
	private Rigidbody _rigidbody;
	private AudioSource _audioSource;
	private GameObject _playerRef;

	// Use this for initialization
	void Start () {
		_rigidbody = GetComponent<Rigidbody> ();
		_mapManager = GameObject.Find ("MapManager").GetComponent<MapManager> ();
		_gameManager = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		_audioSource = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate () {
		Move ();

		if (transform.position.z < 10)
			OnDeath ();
	}

	void Move(){
		Vector3 newPos = curMapLine.GetMidPoint();
		newPos = newPos + new Vector3 (0f, 0f, transform.position.z - moveSpeed * Time.deltaTime);

		_rigidbody.MovePosition (newPos);

		Vector3 curDirVec = curMapLine.GetDirectionVector ();
		Vector3 newDirVec = new Vector3 (-curDirVec.y, curDirVec.x, 0);
		//print (Quaternion.Euler(newDirVec));
		_rigidbody.MoveRotation (Quaternion.LookRotation(new Vector3(0f,0f,1f), newDirVec));
	}

	public void Fire() {

	}

	public void TakeDamage(int dmg) { 
		OnDeath ();
	}

	public void OnDeath() {

		// TODO spawn two flippers
		_gameManager.TankerDestroyed();

		if (curMapLine.leftLine != null) {
			SpawnFlipper (curMapLine.leftLine);
		} else {
			SpawnFlipper (curMapLine);
		}
		if (curMapLine.rightLine != null) {
			SpawnFlipper (curMapLine);
		} else {
		}

		Destroy (gameObject);
	}

	void SpawnFlipper(MapLine newMapLine) {
		Vector3 curDirVec = newMapLine.GetDirectionVector ();
		Vector3 newDirVec = new Vector3 (-curDirVec.y, curDirVec.x, 0);
		GameObject newShip = Instantiate (flipperPrefab, newMapLine.GetMidPoint() + new Vector3 (0, 0, transform.position.z), Quaternion.LookRotation(new Vector3(0f,0f,1f), newDirVec));
		newShip.GetComponent<Flipper>().SetMapLine (newMapLine);
		newShip.GetComponent<Flipper>().movementForce = _gameManager.currentRound * _gameManager.speedMulti;
	}
}
