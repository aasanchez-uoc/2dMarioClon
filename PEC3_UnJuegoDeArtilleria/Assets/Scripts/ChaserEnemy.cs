using System;
using UnityEngine;

/// <summary>
/// Clase que implementa en el enemigo el comportamiento de perseguir al jugador (a no ser que este se encuentre por encima, en cuyo caso huirá hasta una distancia segura)
/// </summary>
public class ChaserEnemy : MonoBehaviour
{
    /// <summary>
    /// La distancia a la que se alejará cuando el jugador esté por encima.
    /// </summary>
    public float SafeDistance = 5;

    /// <summary>
    /// Referencia al componente ItemMovement
    /// </summary>
    private ItemMovement movement;

    /// <summary>
    /// Referencia al componente BoxCollider2D del gameObject
    /// </summary>
    private BoxCollider2D boxCollider;

    /// <summary>
    /// Referencia al GameObject del jugador
    /// </summary>
    private GameObject player;
    void Start()
    {
        boxCollider = GetComponentInChildren<BoxCollider2D>();
        movement = GetComponentInChildren<ItemMovement>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        Vector3 pos = boxCollider.bounds.center - boxCollider.bounds.extents;
        //comprobamos si está a la vista de la cámara
        if (Camera.main.WorldToViewportPoint(pos).x < 1)
        {
            movement.Moving = true;

            //comprobamos si el jugador está por encima de nosotros y estamos mas cerca de la distancia segura
            if ( Math.Abs( player.transform.position.y - transform.position.y) > 0.1  && Math.Abs(player.transform.position.x - transform.position.x) < SafeDistance )
            {
                //en cuyo caso nos movemos en direccion opuesta al jugador
                if (player.transform.position.x <= transform.position.x) movement.MoveDirection = Direction.Right;
                else movement.MoveDirection = Direction.Left;
            }
            else
            {
                //en el resto de caso nos dirigimos hacia el jugador
                if (player.transform.position.x <= transform.position.x) movement.MoveDirection = Direction.Left;
                else movement.MoveDirection = Direction.Right;
            }

        }
        else
        {
            movement.Moving = false;
        }
    }
}
