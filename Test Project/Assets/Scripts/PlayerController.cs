using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Runtime.InteropServices;

public class PlayerController : MonoBehaviour {

  [DllImport("__Internal")]
  private static extern void TaskHasCompleted();

  public float speed;
    public float rotSpeed;
    private Rigidbody rb;
    public NavMeshAgent agent;
    public Transform destinationWall;
  private bool complete = false;

    void Start()
    {
        //rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
    // count = 0;
    // SetCountText();
    // winText.text = "";
        agent.updateRotation = false;
    }

  void FixedUpdate()
  {
    float moveHorizontal = Input.GetAxis("Horizontal");
    float moveVertical = Input.GetAxis("Vertical");

    if (agent)
    {

      Vector3 movement = transform.forward;
      agent.velocity = movement * speed * moveVertical;

      Vector3 target = transform.forward + transform.right * moveHorizontal* rotSpeed;
      Vector3 direction = target.normalized;
      Quaternion lookRotation = Quaternion.LookRotation(direction);
      transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * rotSpeed);
      //print(transform.rotation);
    }
    else if (rb)
    {
      Vector3 movement = new Vector3(moveHorizontal * rotSpeed, 0.0f, moveVertical);

      rb.AddForce(movement * speed);
    }
    if (Mathf.Abs(transform.position.z - destinationWall.position.z) < 5)
    {
      if (!complete)
      {
        complete = true;
        TaskHasCompleted();
      }
    }
  }

    // void OnTriggerEnter(Collider other)
    // {
    //     if (other.gameObject.CompareTag("Pick Up")) {
    //         other.gameObject.SetActive(false);
    //         count += 1;
    //         SetCountText();
    //     }
    // }
    //
    // void SetCountText() {
    //     countText.text = "Count: " + count.ToString();
    //     if (count >= 10) {
    //         winText.text = "You Win!";
    //     }
    // }
}
