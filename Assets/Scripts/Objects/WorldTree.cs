using UnityEngine;
using System.Collections;

public class WorldTree : BaseWorldObject {

    public int wood;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    override public void Use() {
        Debug.Log("Tree was used");
    }
}
