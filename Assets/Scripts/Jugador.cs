using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class Jugador : MovimientoDeObjetos
{
    //Variables.
    public int dañoParded = 1; //es el daño que le hace el jugador a las paredes cuando las golpea.
    public int puntoPorComida = 10; // los puntos que obtiene el jugador por recoger comida.
    public int puntoPorSoda = 20; // los puntos que obtiene el jugador por recoger una soda.
    public float reiniciarNivel = 1f;
    public Text textoDeComida;
    public AudioClip sonidoCaminando1; // para guardar el sonido + 1 variacion.
    public AudioClip sonidoCaminando2; // para guardar el sonido + 1 variacion.
    public AudioClip sonidoComiendo1; // para guardar el sonido + 1 variacion.
    public AudioClip sonidoComiendo2; // para guardar el sonido + 1 variacion.
    public AudioClip sonidoBebiendo1; // para guardar el sonido + 1 variacion.
    public AudioClip sonidoBebiendo2; // para guardar el sonido + 1 variacion.
    public AudioClip sonidoDeJuegoTermiando; // para guardar el sonido.

    private Animator animacion; // para almacenar una referencia del componente de animacion del jugador.
    private int comida; // guarda el puntaje del jugador antes de mandarsela al administrador de juego mientras se cambia el nivel. 
    private Vector2 toqueOriginal = -Vector2.one; // almacena el lugar donde el jugador toco la pantalla por primera vez.



    // Start is called before the first frame update
    protected override void Start()
    {
        animacion = GetComponent<Animator>();

        comida = AdministradorDeJuego.instancia.puntosDeComidaDelJugador; // para que guarde el puntaje del jugador el el ADMINISTRADOR DE JUEGO mientras se cambia de nivel.
        
        textoDeComida.text = "Comida: " + comida;

        base.Start(); //llama a la funcion "start" de la clase "movimiento de objetos".
    }

    // Guarda el puntaje en "administrador del juego" mientras cambia de nivel.
    private void OnDisable()
    {
        AdministradorDeJuego.instancia.puntosDeComidaDelJugador = comida;
    }

    // Update is called once per frame
    void Update()
    {
        //verifica si es el turno del jugador, si no lo es vuelve (return) significando que no va ejecutar ningun otro codigo que lo siga.
        if (!AdministradorDeJuego.instancia.turnoDelJugador)
        {
            return;
        }

        //Variables.
        int horizontal = 0; // guarda la dirrecion donde el jugador se mueve ya sea 1 o -1.
        int vertical = 0; // guarda la dirrecion donde el jugador se mueve ya sea 1 o -1.

        // SI UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER.
        horizontal = (int) (Input.GetAxisRaw("Horizontal"));
        vertical = (int) (Input.GetAxisRaw("Vertical"));

        //verifica si el jugador se mueve en forma hotizontal.
        if (horizontal != 0)
        {
            vertical = 0; // y si lo es pone vertical a 0. (es para que el jugador no pueda moverse en diagonal.)
        }

        // DE LO CONTRATIO SI EL JUGADOR USA OTRO DISPOSITIVO.
        if (Input.touchCount > 0) // si es mayot a 0 significa que el sistema de input registro 1 o mas toques.
        {
            Touch miToque = Input.touches[0]; // si es correcto almaceno el primero toque detectado en una variable de tipo "Touch".

            if (miToque.phase == TouchPhase.Began)
            {
                toqueOriginal = miToque.position;
            }
            else if (miToque.phase == TouchPhase.Ended && toqueOriginal.x >= 0) 
            {
                Vector2 toqueFinal = miToque.position;
                float x = toqueFinal.x - toqueOriginal.x;
                float y = toqueFinal.y - toqueOriginal.y;
                toqueOriginal.x = -1;
                if (Mathf.Abs(x) > Mathf.Abs(y))
                {
                    horizontal = x > 0 ? 1 : -1;
                }
                else
                {
                    vertical = y > 0 ? 1 : -1;
                }
            }
        }
        //FINAL.

        //verifica si hay un valor distinto a 0 en los ejes horizontal y vertical.
        if (horizontal != 0 || vertical != 0)
        {
            //si hay un valor distinto a 0, significa que el jugador se intanta mover y uso la siguiente funcion.
            IntentoDeMovimiento<Pared>(horizontal, vertical); // especifico que en el porametro <T> (Pared) el jugador podria encontrarsela, lo cual es un objeto con el que puede interactuar.
        }
    }

    protected override void IntentoDeMovimiento<T>(int xDir, int yDir)
    {
        comida--; //cada vez que el jugador se mueva pierde 1 de comida;
        textoDeComida.text = "Comida: " + comida;

        base.IntentoDeMovimiento<T> (xDir, yDir);

        RaycastHit2D golpe;

        if (Movimiento(xDir, yDir, out golpe))
        {
            AdministradorDeSonido.instancia.EfectoDeSonidoAleatorio(sonidoCaminando1, sonidoCaminando2);
        }

        ControlarSiElJuegoTermino();

        AdministradorDeJuego.instancia.turnoDelJugador = false;
    }

    //Controla si el jugador colisiono con el tag "comida, soda o salida"
    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.tag == "Exit")
        {
            Invoke("Reinicio", reiniciarNivel);
            enabled = false;
        }
        else if (otro.tag == "Food")
        {
            comida += puntoPorComida;
            textoDeComida.text = "+" + puntoPorComida + "Comida: " + comida;
            AdministradorDeSonido.instancia.EfectoDeSonidoAleatorio(sonidoComiendo1, sonidoComiendo2);
            otro.gameObject.SetActive(false);
        }
        else if (otro.tag == "Soda")
        {
            comida += puntoPorComida;
            textoDeComida.text = "+" + puntoPorSoda + "Comida: " + comida;
            AdministradorDeSonido.instancia.EfectoDeSonidoAleatorio(sonidoBebiendo1, sonidoBebiendo2);
            otro.gameObject.SetActive(false); 
        }
    }

    protected override void NoPuedeMoverse<T>(T componente)
    {
        Pared golpeoPared = componente as Pared; // es igual al componente del parametro mientras se castea directamenta al objeto PARED.
        golpeoPared.DañoPared(dañoParded); // el daño que el jugador le va a hacer a la pared.
        animacion.SetTrigger("jugadorCortando");
    }

    private void Reinicio() 
    {
        Application.LoadLevel(Application.loadedLevel); // carga el mismo nivel ya que los niveles los genera el script.        
    }

    //cheque si un enemigo ataco y golpeo al jugador.
    public void PerdidaPuntosDeComida(int perdida) 
    {
        animacion.SetTrigger("jugadorGolpeado");
        comida-=perdida; //cuando recibe un golpe le resta comida.
        textoDeComida.text = "-" + perdida + "Comida: " + comida; 
        ControlarSiElJuegoTermino(); //como recibio daño contralo si no termino el juego.
    }

    //Controla si se termino el juego
    private void ControlarSiElJuegoTermino() 
    {
        if (comida <= 0)
        {
            AdministradorDeSonido.instancia.EfectoDeSonidoAleatorio(sonidoDeJuegoTermiando);
            AdministradorDeSonido.instancia.origenDeSonido.Stop(); // la musica.
            AdministradorDeJuego.instancia.JuegoTerminado();
        }
    }
}
