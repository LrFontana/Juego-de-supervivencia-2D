using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigo : MovimientoDeObjetos
{

    //Variables.
    public int dañoAlJugador; //el daño que le hace el anemigo al jugador.
    public AudioClip enemigoAtacando1; // guarda el clip de cuando el enemigo ataca al jugador.
    public AudioClip enemigoAtacando2; // guarda el clip de cuando el enemigo ataca al jugador.z

    private Animator animacion;
    private Transform objetivo; // guarda la posicion del jugador, y decirle al enemigo donde moverse.
    private bool saltarMovimiento; // para que el enemigo se mueva cada 2 turnos.

    // Start is called before the first frame update
    protected override void Start()
    {
        AdministradorDeJuego.instancia.AgregarEnemigoALaLista(this);
        animacion = GetComponent<Animator>(); //guarda la referencia del componente animacion.
        objetivo = GameObject.FindGameObjectWithTag("Player").transform; //guarda el objeto con el tag "jugador" para identificarlo como objetivo.
        base.Start(); //llama a la funcion Star pero de la clase MovimeientoDeObjetos.
    }

    
    protected override void IntentoDeMovimiento<T>(int xDir, int yDir)
    {
        //hace que el enemigo se mueva cada 2 turnos.
        if (saltarMovimiento)
        {
            saltarMovimiento = false;
            return;
        }

        //indica donde esta el elemento T, en este caso el jugador y en que direccion.
        base.IntentoDeMovimiento<T>(xDir, yDir);

        //una vez que el enemigo ya se movio a esa direccion, vuelve a esperar 2 turnos.
        saltarMovimiento = true;
    }

    /// <summary>
    /// esta funcion va ser usada en el Asistente Del juego para mover a cada uno de los enemigos
    /// en la lista de enemigos.
    /// </summary>
    public void MovimientoDelEnemigo() 
    {
        //variables.
        int xDir = 0;
        int yDir = 0;

        //verifica si la distancia del objetivo(jugador) y la distancia del transform(enemigo) es menor a epsilon (casi igual a 0),estan en la misma columna. 
        if (Mathf.Abs(objetivo.position.x - transform.position.x) < float.Epsilon)
        {
            yDir = objetivo.position.y > transform.position.y ? 1 : -1; // verifica lo mismo pero para el eje Y, si es correcto se mueve arriva (1) y sino abajo (-1).
        }
        else //si no estan en la misma columna, el enemigo se mueve en el eje horizontal.
        {
            xDir = objetivo.position.x > transform.position.x ? 1 : -1;

            //una vez que se movio, le paso las coordenadas del Jugador
            IntentoDeMovimiento<Jugador> (xDir, yDir);
        }
    }

    protected override void NoPuedeMoverse<T>(T componente)
    {
        Jugador golpeJugador =  componente as Jugador;        

        golpeJugador.PerdidaPuntosDeComida(dañoAlJugador);

        animacion.SetTrigger("enemigoAtacando");

        AdministradorDeSonido.instancia.EfectoDeSonidoAleatorio(enemigoAtacando1, enemigoAtacando1);
    }
}
