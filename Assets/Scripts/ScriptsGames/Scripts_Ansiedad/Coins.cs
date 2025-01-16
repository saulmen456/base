using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coins : MonoBehaviour
{
    public Acelerometro acelerometro;
    public Text coinText;

    void Start()
    {
        acelerometro = GameObject.FindGameObjectWithTag("Player").GetComponent<Acelerometro>();
        coinText = GameObject.Find("CoinText").GetComponent<Text>(); // Encuentra el objeto de texto
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            acelerometro.cantidad = acelerometro.cantidad + 1;
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        coinText.text = "Objetos: " + acelerometro.cantidad.ToString();
    }
}
