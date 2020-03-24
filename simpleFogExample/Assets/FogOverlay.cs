using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FogOverlay : MonoBehaviour {
    Transform mainCamera;
    public float fogStartDistance;
    public float fogStopDistance;
    Image myImage;
    public RectTransform imageObject;
    
	// Use this for initialization
	void Start () {
        myImage = imageObject.GetComponent<Image>();
        mainCamera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update () {

        if (mainCamera == null){
            mainCamera = Camera.main.transform;
        }else{
            if (myImage == null){
                myImage = GetComponentInChildren<Image>();
            }
            else{
                Color c = myImage.color;
                float distance = (mainCamera.position - transform.position).magnitude;

                if (distance < fogStartDistance)
                {
                    c.a = 0;
                }
                else if (distance > fogStartDistance && distance < fogStopDistance)
                {
                    float changeValue=  (distance- (fogStopDistance - fogStartDistance)) / (fogStopDistance - fogStartDistance);
                    Debug.Log(changeValue);
                    c.a = changeValue;
                }
                else if (distance > fogStopDistance)
                {

                    c.a = 1;
                }
                myImage.color=c;
            }
        }



    }
}
