using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Earthquake : MonoBehaviour
{
    public float magnitude = 4f;
    public float slowDownFactor = 0.01f;

    [SerializeField] private GameManager _gameManager;

    private Vector3 _originalPosition;

    private bool _isShaking;

    private void OnEnable()
    {
        _gameManager.OnSimulationStarted += OnSimulationStarted;
        _gameManager.OnSimulationStopped += OnSimulationStopped;
    }

    private void OnDisable()
    {
        _gameManager.OnSimulationStarted -= OnSimulationStarted;
        _gameManager.OnSimulationStopped += OnSimulationStopped;
    }

    private void Start()
    {
        _originalPosition = transform.position;
    }

    private void OnSimulationStarted()
    {
        _isShaking = true;
    }
    
    private void OnSimulationStopped()
    {
        _isShaking = false;
    }

    
	
    private void FixedUpdate()
    {
        if (!_isShaking)
            return;
        //Shake the ground with some random values
        Vector2 randomPos = Random.insideUnitCircle * magnitude;

        float randomY = Random.Range(-1f, 1f) * magnitude;

        //Will generate a more realistic earthquake - otherwise the ground will jitter and not shake
        var position = transform.position;
        float randomX = Mathf.Lerp(position.x, randomPos.x, Time.fixedTime * slowDownFactor);
        float randomZ = Mathf.Lerp(position.z, randomPos.y, Time.fixedTime * slowDownFactor);

        randomY = Mathf.Lerp(position.y, randomY, Time.fixedTime * slowDownFactor * 0.1f);
        
        Vector3 moveVec = new Vector3(randomX, randomY, randomZ);

        position = _originalPosition + moveVec;
        transform.position = position;
    }
}
