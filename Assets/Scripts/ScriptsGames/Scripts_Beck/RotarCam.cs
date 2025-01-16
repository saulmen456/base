using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotarCam : MonoBehaviour
{
    public Transform cam;
    public Transform Player;
    public float speedgiro;
    public Joystick joystickgiro;

    private void Start()
    {
        cam=Camera.main.transform;
    }
    private void Update()
    {
        rotate();
    }
    void rotate()
    {
        float rotateH;
        float rotateV;

        rotateH =joystickgiro.Horizontal * speedgiro; 
        rotateV=joystickgiro.Vertical * speedgiro;
        cam.Rotate(rotateV, 0, 0);
        Player.Rotate(0, rotateH, 0);
    }
}
