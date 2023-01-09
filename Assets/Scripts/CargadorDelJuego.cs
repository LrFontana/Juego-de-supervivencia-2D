using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Este script solamente se fija si el juego se fija si el AdministradorDeJuego fue instanciando y sino lo instancia.
public class CargadorDelJuego : MonoBehaviour
{
    public GameObject administrador;

    // Start is called before the first frame update
    void Awake()
    {
        if (AdministradorDeJuego.instancia == null)
        {
            Instantiate(administrador);
        }
    }

}
