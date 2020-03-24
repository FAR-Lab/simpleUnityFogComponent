using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class peopleSpanner : MonoBehaviour {

	bool active;
	public int targetCount;
	public List<GameObject> pedestrians;
	public List<Material>outfits;
	public GameObject Prefab;
	// Use this for initialization
	void Start () {
		pedestrians = new List<GameObject> ();
	
	}
	
	// Update is called once per frame
	void Update () {
		while (pedestrians.Count < targetCount) {
			float xOff = Random.value;
			float zOff = Random.value;
			Vector3 min=transform.GetComponent<Collider> ().bounds.min;
			Vector3 max=transform.GetComponent<Collider> ().bounds.max;
			Vector3 pos = new Vector3 (Mathf.Lerp (min.x, max.x, xOff), transform.position.y, Mathf.Lerp (min.z, max.z, zOff));
			Quaternion rotation = Quaternion.LookRotation (transform.right);
			if (Random.value > 0.5f) {
				rotation*=Quaternion.Euler (0, 180, 0);
			}
			GameObject newInput = Instantiate (Prefab, pos,rotation) as GameObject;
			pedestrians.Add (newInput);
			newInput.transform.parent = transform;

			newInput.GetComponentInChildren<SkinnedMeshRenderer> ().material = outfits[(int)Mathf.Round(Random.Range(0,   (outfits.Count)))];
		}
		GameObject targetObj = pedestrians[0];
		bool foundOne = false;
		foreach (GameObject obj in pedestrians) {

			if (! transform.GetComponent<Collider>().bounds.Contains (obj.transform.position)) {
				targetObj = obj;
				foundOne=true;
			//TODo spawn them randomly

				break;


				}
			}
		if(foundOne){
			pedestrians.Remove (targetObj);
			Destroy (targetObj);
		}

	
	}
	void setActive(bool input){

	}
}
	