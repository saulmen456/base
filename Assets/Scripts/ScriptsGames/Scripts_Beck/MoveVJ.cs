using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MoveVJ : MonoBehaviour
{
    [SerializeField]private Transform _trf;
    [SerializeField]private float _speed=0;
    [SerializeField] CharacterController _characterController;
    private Vector3 move;
   

    //joystick
    [SerializeField] private Joystick _FloatingJoystick = null;
    public Joystick joystickgiro;
    public Transform cam;
    public float speedgiro;

    //mostrar descripcion 
    public GameObject infopanel;
    private float _time;
    public GameObject _image;
    public GameObject _botones1;
    public GameObject _botones2;
    public GameObject corainfo;
    public GameObject tiempoinfo;
    public GameObject familiainfo;
    public GameObject rompeinfo;


    private void Start()
    {
        cam = Camera.main.transform;
        infopanel.SetActive(false);
    }
    void Update()
    {
        float movHor = Input.GetAxisRaw("Horizontal");
        float movVer = Input.GetAxisRaw("Vertical");
       
        movVer = _FloatingJoystick.Vertical;
        movHor = _FloatingJoystick.Horizontal;

        move=_trf.right*movHor+_trf.forward*movVer;
        _characterController.Move(move*_speed*Time.deltaTime);

        rotate();

    }
    void rotate()
    {
        float rotateH;
        float rotateV;

        rotateH = joystickgiro.Horizontal * speedgiro;
        rotateV = -(joystickgiro.Vertical * speedgiro);
        cam.Rotate(rotateV, 0, 0);
        _trf.Rotate(0, rotateH, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Corazon")) 
        {
            infopanel.SetActive(true);
            corainfo.SetActive(true);
            familiainfo.SetActive(false);
            tiempoinfo.SetActive(false);
            rompeinfo.SetActive(false);


            Destroy(other.gameObject);
            StartCoroutine(MostrarPanel());
            StartCoroutine(HideMinimap());
        }
        if (other.CompareTag("Familia"))
        {
            familiainfo.SetActive(true);
            tiempoinfo.SetActive(false);
            rompeinfo.SetActive(false);
            corainfo.SetActive(false);

            infopanel.SetActive(true);

            Destroy(other.gameObject);
            StartCoroutine(MostrarPanel());
            StartCoroutine(HideMinimap());
        }
        if (other.CompareTag("Tiempo"))
        {
            infopanel.SetActive(true);

            tiempoinfo.SetActive(true);      
            familiainfo.SetActive(false);
            rompeinfo.SetActive(false);
            corainfo.SetActive(false);

            Destroy(other.gameObject);
            StartCoroutine(MostrarPanel());
            StartCoroutine(HideMinimap());
        }
        if (other.CompareTag("Rompecabezas"))
        {
            rompeinfo.SetActive(true);
            corainfo.SetActive(false);
            tiempoinfo.SetActive (false);
            familiainfo.SetActive (false);

            infopanel.SetActive(true);

            Destroy(other.gameObject);
            StartCoroutine(MostrarPanel());
            StartCoroutine(HideMinimap());
        }
    }
    IEnumerator MostrarPanel()
    {
        yield return new WaitForSeconds(10f);
        infopanel.SetActive(false);
    }
    IEnumerator HideMinimap()
    {
        _botones1.SetActive(false);
        _botones2.SetActive(false);
        _image.SetActive(false);
        _speed = 0f;
        yield return new WaitForSeconds(10f);
        _image.SetActive(true);
        _botones1.SetActive(true);
        _botones2.SetActive(true);
        _speed = 5f;
    }
}
