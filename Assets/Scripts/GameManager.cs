using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action OnSimulationStarted;
    public event Action OnSimulationStopped;

    private void Awake()
    {
        Time.timeScale = 0f;
    }

    public void StartSimulation()
    {
        Time.timeScale = 1f;
        OnSimulationStarted.Invoke();
    }

    public void StopSimulation()
    {
        Time.timeScale = 0f;
        OnSimulationStopped.Invoke();
    }
}
