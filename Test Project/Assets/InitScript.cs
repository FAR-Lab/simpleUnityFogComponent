using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


public class InitScript : MonoBehaviour {

  [DllImport("__Internal")]
  private static extern int GetParameterData(string key);

  public Camera mainCamera;
  public PlayerController player;
  public GameObject chair;

  public GameObject baseAvatar;
  public List<Material> materials;

  private void Awake() {
    Random.InitState(11);

    if (Application.platform == RuntimePlatform.WebGLPlayer) {
      var cameraIndex = GetParameterData("height");
      var cameraY = (new float[] { 0.18f, 0.5f, 1.0f, 2.0f })[cameraIndex];
      Debug.Log("setting camera Y to" + cameraY);
      var cp = mainCamera.transform.position;
      mainCamera.transform.position = new Vector3(cp.x, cameraY, cp.z);
      if (cameraIndex == 3) {
        // adjust camera for character following
      }

      var playerSpeed = (new float[] { 1f, 0.5f, 2f, 3.5f })[GetParameterData("speed")];
      Debug.Log("setting player speed to" + playerSpeed);
      player.speed = playerSpeed;
    }
  }

  // Use this for initialization
  void Start() {
    for (int i = 0; i < 75; i++) {
      var avatar = Instantiate(baseAvatar);
      avatar.GetComponentInChildren<SkinnedMeshRenderer>().material = materials[Random.Range(0, materials.Count)];
      avatar.SetActive(true);
    }
  }

  // Update is called once per frame
  //void Update () {
  //    if (Input.GetMouseButtonDown(0)) {
  //    }
  //}
}
