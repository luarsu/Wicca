using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    //Controll sensitivity of the camera
    public float mouseSensitivity;
    public Transform playerBody;

    float xAxisClamp = 0.0f;

    private void Awake()
    {
        //Lock cursor to center of the camera. If any problems, or whant to change this, do this in the update function
        Cursor.lockState = CursorLockMode.Locked;
    }
    // Update is called once per frame
    void Update () {
        RotateCamera();
	}

    void RotateCamera()
    {
        //get input angle from mouse
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        //Adjust to the selected sensitivity
        float rotateAmountX = mouseX * mouseSensitivity;
        float rotateAmountY = mouseY * mouseSensitivity;

        //xAxisClamp is a used variable to solve the flickering issue with the camera when you are looking directly up or down (-90.90)
        xAxisClamp -= rotateAmountY;

        //apply rotation to the transform. The rotationin Y is only applied to the camera.
        Vector3 targetRotationCamera = transform.rotation.eulerAngles;
        targetRotationCamera.x -= rotateAmountY;
        targetRotationCamera.z = 0;




        //The rotation in X is applpied to the character body. As the camera is parented to it, it will rotate as well
        Vector3 targetRotationPlayer = playerBody.rotation.eulerAngles;
        targetRotationPlayer.y += rotateAmountX;

        if (xAxisClamp > 90)
        {
            xAxisClamp = 90;
            targetRotationCamera.x = 90;
        }
        else if(xAxisClamp < -90)
        {
            xAxisClamp = -90;
            targetRotationCamera.x = -90;
        }


        //As we got the rotation as a vector 3, we need to transform it into a quaternion again
        transform.rotation = Quaternion.Euler(targetRotationCamera);
        playerBody.rotation = Quaternion.Euler(targetRotationPlayer);
    }
}
