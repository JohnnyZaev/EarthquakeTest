using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] private bool willFall;

    [SerializeField] private float timeToFall = 10f;

    [SerializeField] private GameManager _gameManager;
    //Drags
    public Transform[] floorArray;
    private Rigidbody[] _floorRb;

    //The ground that will shake so we can shake the building
    private Transform groundObj;

    //Needed in the Euler forward
    private Vector3[] posNew;
    private Vector3[] posOld;
    private Vector3[] velArray;

    //Building parameters
    //float m = 5000f; // kg/floor
    //float k = 10000f; // kg/s^2
    //float c = 1000f; // kg/s

    private float m = 5000;
    private float k;
    private float c;

    //Save the start position of the building in global coordinates relative to the center of the ground so the building can be positioned everywhere
    private Vector3 startPos;

    private bool startFall;
    private float timer;

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
    
    private void OnSimulationStarted()
    {
        _isShaking = true;
    }
    
    private void OnSimulationStopped()
    {
        _isShaking = false;
    }

	void Start() 
	{
	    //Get the ground
        groundObj = GameObject.FindGameObjectWithTag("Ground").transform;

        startPos = transform.position - groundObj.position;

        //Initialize the arrays
        posNew = new Vector3[floorArray.Length];
        posOld = new Vector3[floorArray.Length];
        velArray = new Vector3[floorArray.Length];

        //Add init values to the arrays
        for (int i = 0; i < posNew.Length; i++) 
        {
            posNew[i] = Vector3.zero;
            posOld[i] = floorArray[i].position;
            velArray[i] = Vector3.zero;
        }

        k = 2f * m;
        c = k / 10f;
        _floorRb = new Rigidbody[floorArray.Length];
        for (int i = 0; i < floorArray.Length; i++)
        {
            _floorRb[i] = floorArray[i].GetComponent<Rigidbody>();
        }
	}
	
	

    //Should be in fixed update because timestep is always 0.02
	void FixedUpdate()
    {
        if (!_isShaking)
            return;
        //Move the building with the ground
        transform.position = groundObj.position + startPos;
        
        ShakeBuilding();
        if (willFall)
            timer += Time.fixedDeltaTime;
    }



    void ShakeBuilding()
    {        
        //Time.deltatime might give an unstable result because we are using Euler forward
        float h = 0.02f;

        //Iterate through the floors to calculate the new position and velocity
        for (int i = 0; i < floorArray.Length; i++)
        {
            Vector3 oldPosVec = posOld[i];
            
            //
            //Calculate the floor's acceleration
            //
            Vector3 accVec = Vector3.zero;

            //First floor
            if (i == 0) 
            {
                accVec = (-k * (oldPosVec - transform.position) + k * (posOld[i + 1] - oldPosVec)) / m;
            }
            //Last floor
            else if (i == floorArray.Length - 1)
            {
                //m = 500f; //If the last floor is smaller
                accVec = (-k * (oldPosVec - posOld[i - 1])) / m;
            }
            //Middle floors
            else 
            {
                accVec = (-k * (oldPosVec - posOld[i - 1]) + k * (posOld[i + 1] - oldPosVec)) / m;
            }

            //Add damping to the final acceleration
            accVec -= (c * velArray[i]) / m;


            //
            //Euler forward
            //
            //Add the new position
            posNew[i] = oldPosVec + h * velArray[i];
            //Add the new velocity
            velArray[i] = velArray[i] + h * accVec;
        }
        

        //Add the new coordinates to the floors
        for (int i = 0; i < floorArray.Length; i++)
        {
            //Assume no spring-like behavior in y direction
            Vector3 newPos = new Vector3(
                posNew[i].x,
                floorArray[i].position.y,
                posNew[i].z);
            
            // Vector3 newPos = new Vector3(
            //     posNew[i].x,
            //     posNew[i].y,
            //     posNew[i].z);

            
            if (timer >= timeToFall)
            {
                if (i < 3)
                {
                    floorArray[i].position = newPos;
                    posOld[i] = posNew[i];
                }
                else
                {
                    // var forceToAdd = (posOld[i] - newPos).normalized;
                    // forceToAdd.y = 0;
                    // _floorRb[i].AddRelativeForce(forceToAdd * 50f, ForceMode.Impulse);
                }
            }
            else
            {
                floorArray[i].position = newPos;
                posOld[i] = posNew[i];
            }
            // _floorRb[i].AddRelativeForce((posOld[i] - newPos).normalized, ForceMode.Impulse);

            //Transfer the values from this update to the next
            
        }
    }
}
