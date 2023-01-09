using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public abstract class MovimientoDeObjetos : MonoBehaviour
{
    //Variables.
    public float tiempoDeMovimiento = 0.1f; // sirve para saber los segundos que tarda un objeto en moverse.
    public LayerMask capaDeBloqueo; // esta variable va a controlar las coliciones mientras el jugador se mueve para ver si ese espacio donde quiere ir esta ocupado o no.

    private BoxCollider2D cajaDeColision; 
    private Rigidbody2D rd2D; // almacena la referencia del rb.
    private float tiempoDeMovimientoInverso; // para hacer los calculos de movimiento mas eficientes.

    // Start is called before the first frame update
    protected virtual void Start()
    {
        cajaDeColision = GetComponent<BoxCollider2D>();
        rd2D = GetComponent<Rigidbody2D>();
        tiempoDeMovimientoInverso = 1f / tiempoDeMovimiento;
    }

    protected bool Movimiento(int xDir, int yDir, out RaycastHit2D golpe) // la palabra clave "out" permite retornar mas de 1 mismo valor a la funcion. 
    {
        //Variable. 
        Vector2 inicio = transform.position;
        Vector2 final = inicio + new Vector2(xDir, yDir);


        cajaDeColision.enabled = false; //desactiva la etiqueta "boxcollider" para cuando castee el array, no choque contra el propio colisionador.
        golpe = Physics2D.Linecast(inicio, final, capaDeBloqueo);
        cajaDeColision.enabled = true;

        if (golpe.transform == null)
        {
            StartCoroutine(MovimientoSuave(final).GetEnumerator());
            return true;
        }
        return false;
    }

    // Sirve para mover unidades de un lugar a otro y toma un parametro llamado final, para especificar donde se tiene que mover. 
    protected IEnumerable MovimientoSuave(Vector3 final) 
    {
        //Variable.
        float distanciaRestante = (transform.position - final).sqrMagnitude;

        // chequea que la distancia restante sea mayot a un numero casi igual a 0.
        while (distanciaRestante > float.Epsilon)
        {
            Vector3 nuevaPosicion = Vector3.MoveTowards(rd2D.position, final, tiempoDeMovimientoInverso * Time.deltaTime); // chequea si se puede mover.
            rd2D.MovePosition(nuevaPosicion); // se mueve a la nueva direccion que se encontro.
            distanciaRestante = (transform.position - final).sqrMagnitude; // re calcula la nueva distancia faltante una vez que el jugador ya se movio.
            yield return null; // espera un frame antes de re evaluar la condicion del while loop.            
        }
    }


    protected virtual void IntentoDeMovimiento<T>(int xDir, int yDir)
        where T : Component
    {
        RaycastHit2D golpe;
        bool noPuedeMoverse = Movimiento(xDir, yDir, out golpe);

        //chequea si colisiono o no con algo mientras se movia
        if (golpe.transform == null)
        {
            return;
        }

        T componenteDelGolpe = golpe.transform.GetComponent<T>();

        if (!noPuedeMoverse && componenteDelGolpe != null)
        {
            NoPuedeMoverse(componenteDelGolpe);
        }
    }

    protected abstract void NoPuedeMoverse<T>(T componente)
        where T : Component;
    

}
