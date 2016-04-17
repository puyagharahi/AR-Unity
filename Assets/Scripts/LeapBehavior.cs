using UnityEngine;
using System.Collections.Generic;
using Leap;
using Leap.Unity;


public class LeapBehavior : MonoBehaviour
{
    Controller LeapController;
    Frame frame;

    void Start()
    {
        LeapController = new Controller();
        while (!LeapController.IsConnected)
        {
            Debug.Log("Attempting to connect Leap Motion");
        }
        Debug.Log("Successfully connected!");
    }

    void Update()
    {
       
    }
}