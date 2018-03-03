using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public partial class Unit: MonoBehaviour
{
    private const float MinimumHealth = 0.0f;
    private float _currentHealth;
    private GameController _gameController;

    public Slider HealthSlider;
    public int Group;
    public float MaxHealth = 100;
    
    public bool IsInPlayingGroup
    {
        get { return _gameController.PlayerGroup == Group;}
    }


    public Vector3 Position
    {
        get { return transform.position; }
    }

    
    public void Start()
    {

        _gameController = FindObjectOfType<GameController>();
        _currentHealth = MaxHealth;
        Path = new LinkedList<Vector3>();
        DestinationPlaceholder = Instantiate(_gameController.DestinationPlaceholderPrefab);
        DestinationPlaceholder.transform.SetParent(transform);
        DestinationPlaceholder.GetComponent<DestinationPlaceholder>().Owner = this;
        if (IsInPlayingGroup)
        {
            StartPathLine();
            StartSelectionCircle();
        }
        ShooterStart();
        _animatedWalker = GetComponent<AnimatedWalker>();
    }

    public void Update()
    {
        UpdatePosition();
        UpdatePathLines();
        UpdateShooter();
        UpdateTask();
    }


    public void Die()
    {
        _gameController.Selected.Remove(this);
        _gameController.ClearPos((int)Position.x, (int)Position.y, gameObject);
        if (IsInPlayingGroup)
            _gameController.ClearPos((int)Position.x, (int)Position.y, DestinationPlaceholder);
        Destroy(gameObject);
    }


    
    public void ReceiveDamage(float impact)
    {
        _currentHealth -= impact;
        if(HealthSlider != null)
            HealthSlider.value = _currentHealth;
        if (_currentHealth < MinimumHealth)
        {
            Destroy(gameObject);
        }
    }

}