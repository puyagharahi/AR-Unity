using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class getgrav : MonoBehaviour {

	const double G = 6.674e-5; // magnified 1M for unity!
	double massG;
	
	void Start () {
		// optimise some of the math - our mass and G are constant.
		massG = G * GetComponent<Rigidbody>().mass;
	}
	
	// Update is called once per frame
	public Material mat;
	void FixedUpdate () {
		GameObject[] objs = GameObject.FindGameObjectsWithTag ("massed");

		foreach (GameObject obj in objs) {
			//obj.transform.localScale += new Vector3(2,2,2);
			Rigidbody rbb = obj.GetComponent<Rigidbody>();
			// F = GMm/r^2
			double mass = rbb.mass;
			double r = Vector3.Distance (transform.position, obj.transform.position);
			double F = massG * mass / (r*r); 
			
			Vector3 v = transform.position - obj.transform.position;
			rbb.AddForce((float) F * v * Time.deltaTime);
		}
	}

}