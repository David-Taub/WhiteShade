using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhysicalObject : MonoBehaviour {
    public float MaxHealth = 100;
    protected const float MinimumHealth = 0.0f;
    protected float _currentHealth;
    protected GameController _gameController;
    public Slider HealthSlider;
    public Vector3 Position
    {
        get { return transform.position; }
    }

    // Use this for initialization
    public void Start () {
        _gameController = FindObjectOfType<GameController>();
        _currentHealth = MaxHealth;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
