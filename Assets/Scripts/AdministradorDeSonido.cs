using Completed;
using System.Collections;
using UnityEngine;

public class AdministradorDeSonido : MonoBehaviour
{
    //Variables.
    public AudioSource origenDeEfecto;
    public AudioSource origenDeSonido;
    public static AdministradorDeSonido instancia = null;
    public float rangoDeTonoBajo = .95f; // para agregarle variaciones aletorias de tonos a los efectos de sonido. el valor repesenta + o - 5% del tono original.
    public float rangoDeTonoAlto = 1.05f; // para agregarle variaciones aletorias de tonos a los efectos de sonido.

     
    // Start is called before the first frame update
    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
        }
        else if (instancia != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    //Reproduce audos individeales 
    public void RepoducirAudiosIndividuales(AudioClip clip) //AudioClip son assets que contienen grabaciones digitales de audios.
    {
        origenDeEfecto.clip = clip;
        origenDeEfecto.Play();
    }


    public void EfectoDeSonidoAleatorio(params AudioClip[] clips) // la palabra reservada "params" permite poder mandar argumentos del mismo tipo siempre y cuando esten separados por una "," .
    {
        //variable.
        int indiceAleatorio = Random.Range(0, clips.Length); // para poder elegir un clip aleatorio del array para reproducirlo.
        float tonoAleatorio = Random.Range(rangoDeTonoBajo, rangoDeTonoAlto);

        origenDeEfecto.pitch = tonoAleatorio;
        origenDeEfecto.clip = clips[indiceAleatorio];
        origenDeEfecto.Play();
    }

}
