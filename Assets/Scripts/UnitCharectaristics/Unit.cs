using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public partial class Unit: PhysicalObject
{
    
    public int Group;
    
    public bool IsInPlayingGroup
    {
        get { return _gameController.PlayerGroup == Group;}
    }
    
    
    public void Start()
    {
        base.Start();
        WalkerStart();
        if (IsInPlayingGroup)
        {
            StartPathLine();
            StartSelectionCircle();
        }
        ShooterStart();
    }

    public void Update()
    {
        //base.Update();
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
        if (HealthSlider != null)
            HealthSlider.value = _currentHealth;
        if (_currentHealth < MinimumHealth)
        {
            Die();
        }
    }

}