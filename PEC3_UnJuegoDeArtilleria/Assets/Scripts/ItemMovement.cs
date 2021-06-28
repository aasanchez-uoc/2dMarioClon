using UnityEngine;

/// <summary>
/// Clase que implementa el movimiento de los objetos y los enemigos
/// </summary>
public class ItemMovement : MonoBehaviour
{
    #region Propiedades
    /// <summary>
    /// La velocidad a la que se moverá este objeto
    /// </summary>
    public float Speed = 5;

    /// <summary>
    /// La dirección hacia la que se mueve el enemigo. 
    /// Se usará también para establecer la dirección inicial del objeto
    /// </summary>
    public Direction MoveDirection;

    /// <summary>
    /// Booleano que indica si el objeto empieza moviendose o espera a estar en pantalla (se usa para los enemigos)
    /// </summary>
    public bool Moving = true;

    /// <summary>
    /// Booleano que fija si el objeto de destruirá al collisionar con el jugador (se usa para las setas)
    /// </summary>
    public bool DestroyOnPlayerCollision = true;
    
    #endregion

    #region Atributos privados
    /// <summary>
    /// Referencia al componente BoxCollider2D del gameObject
    /// </summary>
    private BoxCollider2D boxCollider;

    /// <summary>
    /// Referencia al componente Rigidbody2D del gameObject
    /// </summary>
    private Rigidbody2D rb;

    /// <summary>
    /// Vector en el que almacenamos el vector con la dirección en la que el objeto se mueve
    /// </summary>
    private Vector3 dir;
    #endregion
    void Start()
    {
        boxCollider = GetComponentInChildren<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Moving) //si nos estamos moviendo
        {            
            bool isGrounded = Physics2D.BoxCastAll(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.1f).Length > 1;
            if (isGrounded)
            {
                dir = (MoveDirection == Direction.Right) ? transform.right : -transform.right;
                //se comprueba si hay un obstaculo en la direccion en la que se esta moviendo
                bool obstacle = Physics2D.BoxCastAll(boxCollider.bounds.center, boxCollider.bounds.size, 0f, dir, 0.01f).Length > 1;
                if (obstacle)
                {
                    //cambiamos de direccion
                    MoveDirection = (MoveDirection == Direction.Right) ? Direction.Left : Direction.Right;
                    dir = (MoveDirection == Direction.Right) ? transform.right : -transform.right;
                }
                Move();
            }
        }
        else //en caso de que no nos estemos moviendo
        {
            Vector3 pos = boxCollider.bounds.center - boxCollider.bounds.extents;
            //comprobamos si está a la vista de la cámara
            if ( Camera.main.WorldToViewportPoint( pos).x < 1)
            {
                //empezamos a movernos
                Moving = true;
            }
        }

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //si la colisión es con el jugador y el objeto debe destruirse, entonces lo destruimos
        if (collision.gameObject.CompareTag("Player") && DestroyOnPlayerCollision) Destroy(transform.root.gameObject);
    }

    void Move()
    {
        rb.velocity = new Vector2(Speed * dir.x, 0) ;
    }
}

/// <summary>
/// Enum para representar las direcciones en las que puede moverse el gameObject
/// </summary>
public enum Direction
{
    Left,
    Right
}
