using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class ControlScript : MonoBehaviour
{
    private GameObject _cube;
    private MLInputController _controller;
    //define the rotation speed of our cube
    private const float _rotationSpeed = 30.0f;
    //define the initial distance from the camera of our cube
    private const float _distance = 2.0f;
    private const float _moveSpeed = 1.2f;
    //used to activate and deactivate the cube
    private bool _enabled = false;
    private bool _bumper = false;

    void Awake()
    {
        _cube = GameObject.Find("Cube");
        _cube.SetActive(false);
        MLInput.Start();
        //OnButtonDown events refer to the bumper
        MLInput.OnControllerButtonDown += OnButtonDown;
        //OnButtonUp events refer to home button
        MLInput.OnControllerButtonUp += OnButtonUp;
        _controller = MLInput.GetController(MLInput.Hand.Left);
    }

    void OnDestroy()
    {
        //stop all input
        MLInput.OnControllerButtonDown -= OnButtonDown;
        MLInput.OnControllerButtonUp -= OnButtonUp;
        MLInput.Stop();
    }

    void Update()
    {
        //rotate the cube if the bumper is tapped - while the bumper variable is true
        if (_bumper && _enabled)
        {
            _cube.transform.Rotate(Vector3.up, +_rotationSpeed * Time.deltaTime);
        }
        CheckControl();
    }

    void CheckControl()
    {
        //if the current value of the trigger is greater than the threshold value of 0.2f, we rotate the cube
        if (_controller.TriggerValue > 0.2f && _enabled)
        {
            _bumper = false;
            _cube.transform.Rotate(Vector3.up, -_rotationSpeed * Time.deltaTime);
        }
        //if any force is applied to our touchpad, we apply the force to the cube  
        else if (_controller.Touch1PosAndForce.z > 0.0f && _enabled)
        {
            //get the x position of our touch
            float X = _controller.Touch1PosAndForce.x;
            //get the y position of our touch
            float Y = _controller.Touch1PosAndForce.y;
            //use three vectors to correlate the touch to the position of the camera
            Vector3 forward = Vector3.Normalize(Vector3.ProjectOnPlane(transform.forward, Vector3.up));
            Vector3 right = Vector3.Normalize(Vector3.ProjectOnPlane(transform.right, Vector3.up));
            Vector3 force = Vector3.Normalize((X * right) + (Y * forward));
            //apply force to the cube
            _cube.transform.position += force * Time.deltaTime * _moveSpeed;
        }
    }

    void OnButtonDown(byte controller_id, MLInputControllerButton button)
    {
        //if the bumper input is recognized, we turn our bumper variable to true
        if ((button == MLInputControllerButton.Bumper && _enabled))
        {
            _bumper = true;
        }
    }

    void OnButtonUp(byte controller_id, MLInputControllerButton button)
    {
        /*
        if the home tap input is recognized, we activate the cube,
        place it in front of the camera, and set the enable variable to true
        */
        if (button == MLInputControllerButton.HomeTap)
        {
            _cube.SetActive(true);
            _cube.transform.position = transform.position + transform.forward * _distance;
            _cube.transform.rotation = transform.rotation;
            _enabled = true;
        }
    }

}