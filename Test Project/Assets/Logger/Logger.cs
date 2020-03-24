using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Runtime.InteropServices;


/*public class log_message {
	public double time;
	public string origin;
	public string valueName;
	public float value;
	public MonoBehaviour originMONO;

}*/

public class simplifiedLogMessage {
  public int entries;
  public List<String> Names;
  public List<float> Values;

  public simplifiedLogMessage() {
    Names = new List<string>();
    Values = new List<float>();
    entries = 0;
  }
  public void push(string name, float val) {
    Names.Add(name);
    Values.Add(val);
    entries++;
  }
  //public byte[] ThisToByteArray()
  //{
  //  using (MemoryStream aStream = new MemoryStream())
  //  {
  //    BinaryFormatter bf = new BinaryFormatter();
  //    bf.Serialize(aStream, this);
  //    return aStream.ToArray();
  //  }
  //}
  public byte[] ValuesToByteArray() {
    var floatArray = this.Values.ToArray();
    var byteArray = new byte[floatArray.Length * sizeof(float)];
    Buffer.BlockCopy(floatArray, 0, byteArray, 0, byteArray.Length);
    return byteArray;
  }
  public string NamesToCsvString() {
    return String.Join(",", Names.ToArray());
  }
}


public class Logger : MonoBehaviour {


  [DllImport("__Internal")]
  private static extern void InitializeWebSocket(string host, string port, string id);
  [DllImport("__Internal")]
  private static extern bool SendDataToWebSocketString(string str);
  [DllImport("__Internal")]
  private static extern bool SendDataToWebSocketArray(byte[] array, int size);
  [DllImport("__Internal")]
  private static extern void CloseWebSocket();
  [DllImport("__Internal")]
  private static extern bool WebSocketIsUp();
  [DllImport("__Internal")]
  private static extern bool IsBufferOk();
  [DllImport("__Internal")]
  private static extern int GetParameterData(string key);




  public static Logger Instance { get; private set; }

  public PlayerController player;

  public string ipAddress = "localhost";
  public string port = "8123";


  private System.Guid myGUID; // for multiple clients we generate a GUID 
  int publishCount = 0;

  List<RandomMovingAgent> trackObject = new List<RandomMovingAgent>();
  Queue<simplifiedLogMessage> log_messages = new Queue<simplifiedLogMessage>();

  bool keepRunning = true;
  bool IsLogging = false;

  private void Awake() {
    if (Instance == null) {
      Instance = this;
      DontDestroyOnLoad(gameObject);
    } else {
      Destroy(gameObject);
    }
  }


  void Start() {
    Application.targetFrameRate = 30;
    //LogitechGSDK.LogiSteeringInitialize(false);
    myGUID = System.Guid.NewGuid();

    var localId = myGUID.ToString();
    if (Application.platform == RuntimePlatform.WebGLPlayer) {
      localId = GetParameterData("key").ToString();
    }

    IsLogging = true;
    keepRunning = true;
#if UNITY_WEBGL
    if (Application.platform == RuntimePlatform.WebGLPlayer) {
      InitializeWebSocket(ipAddress, port, localId);
    } else {
      Debug.Log("Normally I'd be opening the ws now!");
    }
#endif

    StartCoroutine(writeToWebsocket());
  }


  public void LogMessage(simplifiedLogMessage incomming_log_message)// for additional log messages
  {
    if (IsLogging) {
      log_messages.Enqueue(incomming_log_message);
    }
  }


  // Update is called once per frame
  void Update() {
    if (trackObject.Count == 0) {
      foreach (RandomMovingAgent a in FindObjectsOfType<RandomMovingAgent>()) {
        trackObject.Add(a);
      }
      Debug.Log("We'll be logging " + trackObject.Count + " objects!");
    }

    LogMessageFor(player);
    for (int i = 0; i < trackObject.Count; i++) {
      LogMessageFor(i + 1, trackObject[i]);
    }
  }

  void LogMessageFor(int index, RandomMovingAgent obj) {
    simplifiedLogMessage newFrame = new simplifiedLogMessage();
    newFrame.push("i", index);
    newFrame.push("t", Time.time);

    addEntryFor(ref newFrame, obj.transform, obj.agent);

    LogMessage(newFrame);
  }

  void LogMessageFor(PlayerController obj) {
    simplifiedLogMessage newFrame = new simplifiedLogMessage();
    newFrame.push("i", 0);
    newFrame.push("t", Time.time);

    addEntryFor(ref newFrame, obj.transform, obj.agent);

    LogMessage(newFrame);
  }

  void addEntryFor(ref simplifiedLogMessage msg, Transform t, UnityEngine.AI.NavMeshAgent a) {
    addSingleEntry(ref msg, "px", t.position.x);
    addSingleEntry(ref msg, "py", t.position.y);
    addSingleEntry(ref msg, "pz", t.position.z);
    addSingleEntry(ref msg, "rx", t.rotation.eulerAngles.x);
    addSingleEntry(ref msg, "ry", t.rotation.eulerAngles.y);
    addSingleEntry(ref msg, "rz", t.rotation.eulerAngles.z);
    addSingleEntry(ref msg, "vx", a.velocity.x);
    addSingleEntry(ref msg, "vy", a.velocity.y);
    addSingleEntry(ref msg, "vz", a.velocity.z);
  }

  void addSingleEntry(ref simplifiedLogMessage msg, string name, float value) {
    if (float.IsNaN(value) || float.IsInfinity(value)) {
      value = 0.0f;
    }
    msg.push(name, value);
  }

  public void halt() {
    IsLogging = false;
  }

  IEnumerator writeToWebsocket() {
    Debug.Log("starting coroutine...");
    bool firstTransmission = true;

    while (keepRunning) {
      while (log_messages.Count > 0) {
#if UNITY_WEBGL
        if (Application.platform == RuntimePlatform.WebGLPlayer) {
          while (!WebSocketIsUp() || !IsBufferOk()) {
            Debug.Log("websocket not yet up or buffer not ok...");
            yield return 0;
          }
        }
#endif
        publishCount++;
        simplifiedLogMessage msg = (simplifiedLogMessage)log_messages.Peek();
#if UNITY_WEBGL
        if (Application.platform == RuntimePlatform.WebGLPlayer) {
          bool status;
          if (firstTransmission) {
            status = SendDataToWebSocketString(msg.NamesToCsvString());
            if (!status) {
              Debug.Log("failed to send header...");
              continue;
            }
            firstTransmission = false;
          }
          byte[] outputArray = msg.ValuesToByteArray();
          status = SendDataToWebSocketArray(outputArray, outputArray.Length);
          if (!status) {
            Debug.Log("failed to send values...");
            continue;
          }
        } else {
          Debug.Log("Would've sent data!");
        }
#endif
        log_messages.Dequeue();
      }
      yield return 0;
    }
  }

  void OnApplicationQuit() {
#if UNITY_WEBGL
    if (Application.platform == RuntimePlatform.WebGLPlayer) {
      CloseWebSocket();
    } else {
      Debug.Log("Normally I'd be closing the ws!");
    }
#endif
    Debug.Log("Final logger publish Count: " + publishCount);
    Debug.Log("End Time: " + Time.time);
    keepRunning = false; // stopping the writing processes
  }

}