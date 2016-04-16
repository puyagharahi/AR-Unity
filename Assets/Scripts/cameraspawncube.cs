using UnityEngine;
using System.Collections;

public class cameraspawncube : MonoBehaviour {
	public GameObject cubee;
	public GameObject planet;
	private float nextFire = 0f;
	public float fireRate = 0.1f;
	private float nextZ = 0f;
	public float maxZ = 40f;

	// Update is called once per frame
	void Update () {
		bool firing = Input.GetKey("space");
		bool fireplan = Input.GetKey ("p");
		bool remove = Input.GetKey ("r");
		if (!firing) {
			nextZ = maxZ;
		}
		if (firing && Time.time > nextFire) {
			Vector3 pos = Input.mousePosition;
			pos.z = nextZ;
			nextZ -= 1;
			Vector3 cubepos = Camera.main.ScreenToWorldPoint(pos);
			nextFire = Time.time + fireRate;
			GameObject c = Instantiate(cubee, cubepos, Random.rotation) as GameObject;
			c.GetComponent<Rigidbody>().AddForce (new Vector3(Random.value, Random.value, Random.value));
		}
		if (fireplan && Time.time > nextFire) {
			Vector3 pos = Input.mousePosition;
			pos.z = nextZ;
			nextZ -= 1;
			Vector3 planpos = Camera.main.ScreenToWorldPoint(pos);
			nextFire = Time.time + fireRate;
			GameObject c = Instantiate(planet, planpos, Random.rotation) as GameObject;
			c.GetComponent<Rigidbody>().AddForce (new Vector3(Random.value, Random.value, Random.value));
		}
		if (remove) {
			GameObject[] obj = GameObject.FindGameObjectsWithTag ("massed");// + GameObject.FindGameObjectsWithTag("planet");
						foreach (GameObject del in obj) {
								GameObject.Destroy (del);
						}
				}
	}
}