using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Panel Data", menuName ="Panel Data" )]
public class PanelesData : ScriptableObject
{
    public string _NombreObjeto;
    public string _descripcionObj;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Corazon"))
        {
           Debug.Log(_descripcionObj);
        }
    }
}
