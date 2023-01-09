using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pared : MonoBehaviour
{
    //Variables.
    public Sprite da�oSprite; // para que el jugador vea que tuve exito atacando la pared.
    public int salud = 4;
    public AudioClip sonidoCortando1; // guarda el audio de cuando el jugador golpea.
    public AudioClip sonidoCortando2; // guarda el audio de cuando el jugador golpea.

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Da�oPared(int perdida) 
    {
        AdministradorDeSonido.instancia.EfectoDeSonidoAleatorio(sonidoCortando1, sonidoCortando2);
        spriteRenderer.sprite = da�oSprite; // para que el jugador vea que da�o exitosamente la pared;
        salud -= perdida;

        if (salud <= 0)
        {
            gameObject.SetActive(false);
        }

    }
    
}
