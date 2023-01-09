using System.Collections; 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdministradorDeJuego : MonoBehaviour
{
    //Variables.
    public float tiempoDeEsperaParaComenzarNivel = 2f; //es el tiempo de espera que espera el asistente de juego para comenzar un nivel nuevo en segundos.
    public float tiempoDeTurno = .1f; //el tiempo que va a esperar el juego entre turnos turno
    public static AdministradorDeJuego instancia = null; 
    public AdministradorDeNiveles scripDeNivel;
    public int puntosDeComidaDelJugador = 100; // vatiable para la salud del jugador.
    [HideInInspector] public bool turnoDelJugador = true; // variable para el turno de jugador, "HideInInspector" significa que la variable no va a aparecer en el editor de unity. 

    private Text textoDeNivel; // para almacenar texto de nivel que va a aparecer en el editor de unity.
    private GameObject imagenDeNivel;  // para almacenar la imagen de nivel que va a aparecer en el editor de unity.
    private int nivel = 1;
    private List<Enemigo> enemigos; //para mantener rastreo de los enemigos y mandarlos en orden a moverse.
    private bool enemigoMoviendose;
    private bool preparandoConfiguraciones; // para verificar las configuraciones de niveles y evitar que el jugador se pueda mover mientras se verifica.

    // Start is called before the first frame update
    void Awake()
    {
        //convierte al objeto AdministradorDeJuego en singleton.
        if (instancia == null)
        {
            instancia = this;
        }
        else if (instancia != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject); // para que no se destruya el administrador de juego
        enemigos = new List<Enemigo>();
        scripDeNivel = GetComponent<AdministradorDeNiveles>();
        EstadoDelJuego();
    }

    //es parte de unity api y es llamada cada vez que se carga una escena. 
    private void OnLevelWasLoaded(int index)
    {
        nivel++; //agrega + 1 al nivel.

        EstadoDelJuego(); 
    }
    //Verifica en que nivel del juego esta el jugador para determinar la cantidad de enemigos.
    void EstadoDelJuego() 
    {
        preparandoConfiguraciones = true;

        imagenDeNivel = GameObject.Find("ImagenDeNivel");
        textoDeNivel = GameObject.Find("TextoDeNivel").GetComponent<Text>();
        textoDeNivel.text = "Dia" + nivel;
        imagenDeNivel.SetActive(true);
        Invoke("EsconderImageneDeNivel", tiempoDeEsperaParaComenzarNivel); // una vez que esconde la imagen, espera 2 segundos para comenzar el nivel.

        enemigos.Clear(); //limpio la lista de enemigos cuando el juego reinicia o inicia por que el administrador de juego no lo resetea.
        scripDeNivel.ConfigurarEscena(nivel);
    }    

    // Esconde la imagen de nivel, cuando el nivel esta listo para comenzar.
    private void EsconderImageneDeNivel() 
    {
        imagenDeNivel.SetActive(false); // esconde la imagen.
        preparandoConfiguraciones = false; // una vez escondida la imagen, permite al jugador poder moverse.
    }

    //Desactiva el Administrador de juego.
    public void JuegoTerminado() 
    {
        textoDeNivel.text = "Despues de" + nivel + "dias, moriste de hambre"; // muestra al jugador cuandos dias sobrevivio.
        imagenDeNivel.SetActive(true);
        enabled = false;
    }


    // Update is called once per frame
    void Update()
    {
        //verifica si es el turno del jugador o del enemigo
        if (turnoDelJugador || enemigoMoviendose || preparandoConfiguraciones)
        {
            return;
        }
        StartCoroutine(MoviendoEnemigos());
    }

    public void AgregarEnemigoALaLista(Enemigo codigo) 
    {
        enemigos.Add(codigo);
    }

    //Sirve para mover enemigos en secuencia.
    IEnumerator MoviendoEnemigos() 
    {
        enemigoMoviendose = true;
        yield return new WaitForSeconds(tiempoDeTurno);
        //verifica que no haya aparecido ningun enemigo.
        if (enemigos.Count == 0) //verifico el largo de la lista "enemigos".
        {
            yield return new WaitForSeconds(tiempoDeTurno); // si no hay agrego un tiempo de espera adicional que causa que el jugador espere auque no haya enemigos todavia.
        }

        for (int i = 0; i < enemigos.Count; i++)
        {
            enemigos[i].MovimientoDelEnemigo();
            yield return new WaitForSeconds(enemigos[i].tiempoDeMovimiento); // espera para llamar al proximo de la lista.
        }

        turnoDelJugador = true;
        enemigoMoviendose = false;
    }
}
