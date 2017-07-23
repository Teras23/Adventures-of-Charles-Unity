using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    private int width, height;

    public bool fixView = true;
    public bool zoomedView = true;

	void Start () {
        ChangeCameraSize();
        width = Screen.width;
        height = Screen.height;
	}
	
	void Update () {
        if(width != Screen.width || height != Screen.height) {
            ChangeCameraSize();
            width = Screen.width;
            height = Screen.height; 
        }
	}

    void ChangeCameraSize() {
        if(fixView) {
            if(zoomedView) {
                Camera.main.orthographicSize = (Screen.height / 64f / 2.0f) / 2;
            }
            else {
                Camera.main.orthographicSize = (Screen.height / 64f / 2.0f);
            }
        }
    }
}
