using UnityEngine;

/// <summary>
/// Clase encargada de controllar el comportamiento de las balas
/// </summary>
public class BulletController : MonoBehaviour
{
    #region Propiedades
    /// <summary>
    /// Prefab con el sistema de partículas de la explosión
    /// </summary>
    public GameObject ExplosionPrefab;

    /// <summary>
    /// Velocidad de la bala
    /// </summary>
    public float Speed;

    /// <summary>
    /// Indicia si la dirección hacia la que se mueve la bala es la derecha o no
    /// </summary>
    public bool IsRightDir = true;
    #endregion

    #region Atributos privados
    /// <summary>
    /// Referencia al componente BoxCollider2D del gameObject
    /// </summary>
    private CircleCollider2D circleCollider;

    /// <summary>
    /// Booleano privado que indica si se ha producido la primera colisión con el suelo. Se usa para cambiar el ángulo de la velocidad
    /// </summary>
    private bool firstCollision = true;
    #endregion

    public delegate void EnemyDestroyed();

    /// <summary>
    /// Evento que se invocará cuando la bala destruya a un enemigo
    /// </summary>
    public event EnemyDestroyed OnEnemyDestroyed;
    void Start()
    {
        circleCollider = GetComponentInChildren<CircleCollider2D>();
        setVelocity(30);
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        //si la colisión es con un enemigo
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
            enemy.Kill(); //entonces le matamos
            OnEnemyDestroyed?.Invoke(); //invocamos el evento
            Instantiate(ExplosionPrefab, transform.position, transform.rotation); //e instanciamos la explosión
            Destroy(this.gameObject);

        }
        else
        {
            var isFLoor = Physics2D.BoxCastAll(circleCollider.bounds.center, circleCollider.bounds.size, 0f, Vector2.down, 0.15f).Length > 1;
            var isCeiling = Physics2D.BoxCastAll(circleCollider.bounds.center, circleCollider.bounds.size, 0f, Vector2.up, 0.15f).Length > 1;
            if (isFLoor || isCeiling) //comprobamos si la bala ha colisionado con suelo o techo, en cuyo caso dejamos que rebote
            {              
                if (firstCollision) //si es la primera colisión con el suelo, cambiamos el ángulo
                {
                    firstCollision = false;
                    setVelocity(10);
                }
            }
            else //si no ha cochado con el suelo, entonces destruimos la bala
            {
                Instantiate(ExplosionPrefab, transform.position, transform.rotation); //instanciamos la explosión
                Destroy(this.gameObject);
            }
        }

    }

    //método encargado de establecer la velocidad hacia la dirección que  se mueve la bala, en el ángulo indicado
    private void setVelocity(float angle)
    {
        if (IsRightDir) GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, -angle) * transform.right.normalized * Speed;
        else GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, angle) * new Vector2(-transform.right.x, transform.right.y).normalized * Speed;
    }
}
