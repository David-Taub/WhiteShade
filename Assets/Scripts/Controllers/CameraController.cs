using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float minZoom;
    private float maxZoom;
    private float cameraZ;
    private Vector3 bordersMin;
    private Vector3 bordersMax;

	// Use this for initialization
	void Start ()
	{
	    cameraZ = -10;
	    minZoom = 2;
	    maxZoom = 15;
        GameController gameController = FindObjectOfType<GameController>();
	    bordersMin = new Vector3(0.0f, 0.0f, cameraZ);
	    bordersMax = new Vector3(gameController.Rows, gameController.Columns, cameraZ);
	}
	
	// Update is called once per frame
	void Update ()
	{
        float xAxisValue = Input.GetAxis("Horizontal");
        float yAxisValue = Input.GetAxis("Vertical");
        float zoom = Input.GetAxis("zoom");
        
        if (Camera.current != null)
        {
            //Camera.current.transform.position 
            Camera.current.orthographicSize = Mathf.Max(minZoom, Mathf.Min(maxZoom, Camera.current.orthographicSize + zoom));
            Vector3 translation = 0.1f * Camera.current.orthographicSize * (new Vector3(xAxisValue, yAxisValue, 0.0f));
            Camera.current.transform.position = Vector3.Max(bordersMin, Vector3.Min(bordersMax, Camera.current.transform.position + translation)); 

        } 
    }
}
