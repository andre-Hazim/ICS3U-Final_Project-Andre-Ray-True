﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    [SerializeField]
    private float xMax;

    [SerializeField]
    private float yMax;

    [SerializeField]
    private float xMin;

    [SerializeField]
    private float yMin;

    private Transform target;

    // Use this for initialization
    void Start () {
        // set the target to the player
        target = GameObject.Find("player").transform;
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
        // makes the camera follow the player.
        transform.position = new Vector3(Mathf.Clamp(target.position.x,xMin, xMax), Mathf.Clamp(target.position.y, yMin, yMax), transform.position.z);
		
	}
}
