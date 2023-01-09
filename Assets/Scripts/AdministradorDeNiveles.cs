using System;
using System.Collections.Generic;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion; // especifico que la clase "quaternion" que voy a usar es la de de UnityEngenie y no la clase de C#.
using Random = UnityEngine.Random; // especifico que la clase "random" que voy a usar es la de de UnityEngenie y no la clase de C#.
using Vector3 = UnityEngine.Vector3; // especifico que la clase "vector3" que voy a usar es la de de UnityEngenie y no la clase de C#.

public class AdministradorDeNiveles : MonoBehaviour
{
    [Serializable]
    public class Contador 
    {
        //Variables.
        public int minimo = 0;
        public int maximo = 0;

        //Constructor.
        public Contador(int min, int max) 
        {
            minimo = min;
            maximo = max;
        }
    }

    //Variables.
    public int columnas = 8; //con estas variables (columnas y filas) defino el tamaño del tablero (8x8).
    public int filas = 8;
    public Contador contadorDePared = new Contador(5, 9); // para controlar el minimo y maximo de cuantas paredes aparecen en un nivel.
    public Contador contadorDeComida = new Contador (1,5); // para controlar el minimo y maximo de cuanta comida aparece.
    public GameObject salida; //creo los objetos en codigo.
    public GameObject[] piso; //creo los objetos en codigo.
    public GameObject[] paredInterior; //creo los objetos en codigo.
    public GameObject[] comida; //creo los objetos en codigo.
    public GameObject[] enemigo; //creo los objetos en codigo.
    public GameObject[] paredExterior; //creo los objetos en codigo.

    private Transform ordenarTablero; // para mantener la jerarquia limpia ya que van a estar apareciendo muchos objetos y mantenerlos ordenados.
    private List<Vector3> posicionesTablero = new List<Vector3>(); // para rastrear las posiciones de los objetos dentro del tablero. 


    void IniciarLista() 
    {
        posicionesTablero.Clear(); //Limpio el tablero o "mapa".

        for (int x = 1; x < columnas - 1; x++) // para el eje X.
        {
            for (int y = 1; y < filas - 1; y++) // para el eje Y.
            {
                posicionesTablero.Add(new Vector3(x, y, 0f)); 
            }
        }
    }


    void ConfiguracionTablero() 
    {
        ordenarTablero = new GameObject("Tablero").transform;

        for (int x = - 1; x < columnas + 1; x++) // para el eje X.
        {
            for (int y = - 1; y < filas + 1; y++) // para el eje Y.
            {
                GameObject instanciar = piso[Random.Range(0, piso.Length)]; // para crear el piso. 

                if (x == -1 || x == columnas || y == -1 || y == filas)
                {
                    instanciar = paredExterior[Random.Range(0, paredExterior.Length)]; // para crear la pared.                
                }

                GameObject instancia = Instantiate(instanciar, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                instancia.transform.SetParent(ordenarTablero);
            }
        }
    }

    Vector3 PosicionesRandoms() 
    {
        int randomIndice = Random.Range(0, posicionesTablero.Count);
        Vector3 pocisionesRandoms = posicionesTablero[randomIndice];
        posicionesTablero.RemoveAt(randomIndice);
        return pocisionesRandoms;
    }


    void DiseñoAleatorioDeObjetos(GameObject[] arr, int min, int max) 
    {
        int contadorDeObjeto = Random.Range(min, max + 1); //esta variable es para contar cuantos objetos pueden aparecer en un nivel ej: la contidad de paredes.

        for (int i = 0; i < contadorDeObjeto; i++)
        {
            Vector3 posicionesRandoms = PosicionesRandoms();
            GameObject eleccion = arr[Random.Range(0, arr.Length)];
            Instantiate(eleccion, posicionesRandoms, Quaternion.identity);
        }
    }

    //Esta funcion es la que prepara y arma todo la escena final.
    public void ConfigurarEscena(int nivel)
    {
        ConfiguracionTablero();
        IniciarLista();
        DiseñoAleatorioDeObjetos(paredInterior, contadorDePared.minimo, contadorDePared.maximo);
        DiseñoAleatorioDeObjetos(comida, contadorDeComida.minimo, contadorDeComida.maximo);
        int contadorDeEnemigos = (int)Math.Log(nivel, 2f); // esta variable hace que por cada nivel los enemigos aumenten x2 asi aumenta la dificultad.
        DiseñoAleatorioDeObjetos(enemigo, contadorDeEnemigos, contadorDeEnemigos);
        Instantiate(salida, new Vector3(columnas - 1, filas - 1, 0f), Quaternion.identity); // para que la "salida" siempre este arriba a la derecha.

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
