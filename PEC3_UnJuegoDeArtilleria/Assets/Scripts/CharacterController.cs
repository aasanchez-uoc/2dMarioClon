using UnityEngine;

/// <summary>
/// Este script se encarga de gestionar toda la lógica y movimiento del personaje
/// </summary>
public class CharacterController : MonoBehaviour
{
    #region Propiedades
    /// <summary>
    /// La velocidad lineal del personaje en el eje x
    /// </summary>
    public float LinearSpeed = 30;

    /// <summary>
    /// La velocidad en el eje x a la que nos podremos mover en el aire
    /// </summary>
    public float SpeedOnAir = 10;

    /// <summary>
    /// Velocidad máxima en el eje X
    /// </summary>
    public float MaxSpeed;

    /// <summary>
    /// La velocidad inicial en el eje y al saltar
    /// </summary>
    public float JumpSpeed = 13;

    /// <summary>
    /// La duración máxima (en segundos) que se mantendrá la velocidad máxima de salto si seguimos pulsando el botón de saltar
    /// </summary>
    public float MaxJumpTime = 0.25f;


    /// <summary>
    /// El clip con el sonido del salto
    /// </summary>
    public AudioClip JumpSound;

    /// <summary>
    /// El clip con el sonido del powerUP
    /// </summary>
    public AudioClip PowerUpSound;

    /// <summary>
    /// El clip con el sonido de matar un enemigo
    /// </summary>
    public AudioClip EnemyKilledSound;

    /// <summary>
    /// El clip con el sonido de obtener una moneda
    /// </summary>
    public AudioClip CoinSound;

    /// <summary>
    /// El clip con el sonido de disparar
    /// </summary>
    public AudioClip ShootSound;

    /// <summary>
    /// El número de vidas con los que empieza el jugador
    /// </summary>
    public int InitialLifes = 3;

    /// <summary>
    /// El prefab de los proyectiles que instanciaremos al disparar
    /// </summary>
    public GameObject BulletPrefab;

    /// <summary>
    /// La velocidad a la que dispararemos las balas
    /// </summary>
    public float BulletSpeed;

    /// <summary>
    /// El ratio de balas por segundo
    /// </summary>
    public float FireRate = 2;
    #endregion

    #region Propiedades solo lectura
    /// <summary>
    /// Booleano que indica si el personaje está en modo super (si es grande) o no
    /// </summary>
    public bool IsBig { get; private set; } = false;

    /// <summary>
    /// LA puntuación de la partida
    /// </summary>
    public int Puntuacion { get; private set; } = 0;

    /// <summary>
    /// Las monedas recogidas
    /// </summary>
    public int Monedas { get; private set; } = 0;

    /// <summary>
    /// Las vidas restantes del jugador
    /// </summary>
    [HideInInspector]
    public int Lifes { get; set; }
    #endregion

    #region Atributos privados
    /// <summary>
    /// Referencia al componente Rigidbody2D del gameObject
    /// </summary>
    private Rigidbody2D rb;

    /// <summary>
    /// Referencia al componente Animator del gameObject
    /// </summary>
    private Animator animator;

    /// <summary>
    /// Referencia al componente SpriteRenderer del gameObject
    /// </summary>
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// Referencia al componente BoxCollider2D del gameObject
    /// </summary>
    private BoxCollider2D boxCollider;

    /// <summary>
    /// Referencia al componente AudioSource que usaremos para reproducir el sonido
    /// </summary>
    private AudioSource source;

    /// <summary>
    /// Booleano privado en el que almacenamos si el jugador se encuentra en el suelo o no
    /// </summary>
    private bool isGrounded = true;

    /// <summary>
    /// Booleano que indica si se ha pulsado el botón de saltar
    /// </summary>
    private bool pulsado = false;

    /// <summary>
    /// Booleano que indica si se ha soltado el botón de saltar
    /// </summary>
    private bool soltado = false;

    /// <summary>
    /// Duración del salto
    /// </summary>
    private float jumpTime = 0;

    /// <summary>
    /// Tiempo transcurrido desde el último disparo
    /// </summary>
    private float lastFireTime;

    /// <summary>
    /// Booleano privado que indica si el personaje puede disparar
    /// </summary>
    private bool fireMode = false;

    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponentInChildren<BoxCollider2D>();
        source = gameObject.AddComponent<AudioSource>();
        BulletPrefab.GetComponent<BulletController>().Speed = BulletSpeed;
        Lifes = InitialLifes;
        lastFireTime = 1 / FireRate;
    }

    private void Update()
    {
        //actualizamos el booleano que indica si estamos en el suelo
        isGrounded = Physics2D.BoxCastAll(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.1f).Length > 1;

        if (Input.GetKeyUp(KeyCode.Space))
        {
            //indicamos que se ha soltado el botón y que ya no se está pulsando
            soltado = true;
            pulsado = false;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //si el personaje está en el suelo, indicamos que se ha pulsado el botón
            if (isGrounded)
            {
                source.PlayOneShot(JumpSound);
                pulsado = true;
            }
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            MoveBackward();
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            MoveForward();
        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (!soltado && pulsado) //si hemos pulsado y no se ha soltado saltamos
            {
                Jump();
            }
        }

        if (isGrounded && !pulsado)
        {
            //si estamos en el suelo y no se ha pulsado reiniciamos el tiempo de salto y el booleano de soltado
            soltado = false;
            jumpTime = 0;
        }

        if (Input.GetKey(KeyCode.Z) && lastFireTime >= 1/FireRate) //además del botón de disparar, comprobamos que ha transcurrido suficiente tiempo
        {
            if (fireMode) //comprobamos que se puede disparar
            {
                Fire();
                lastFireTime = 0;
            }

        }

        //actualizamos las varialbes del componente Animator
        animator.SetFloat("velocity", Mathf.Abs(rb.velocity.x));
        animator.SetBool("grounded", isGrounded);
        animator.speed = Mathf.Clamp ( rb.velocity.sqrMagnitude, 0.5f , 1);
    }
    void FixedUpdate()
    {
        if (rb.velocity.x > MaxSpeed) //si la velocidad en el eje x supera la máxima, mantenemos la velocidad máxima
        {
            rb.velocity =  new Vector2 (MaxSpeed, rb.velocity.y);
        }
        if (lastFireTime < 1 / FireRate) lastFireTime += Time.deltaTime; //aumentamos el tiempo transcurrido desde el último disparo hasta su máximo
    }

    /// <summary>
    /// Método encargado de mover al personaje hacia delante
    /// </summary>
    private void MoveForward()
    {
        var right = transform.right;
        if (isGrounded)
        { 
            rb.velocity += new Vector2(right.x * LinearSpeed, right.y * LinearSpeed) * Time.deltaTime;
        }
        else
        {
            rb.velocity += new Vector2(right.x * SpeedOnAir, right.y * SpeedOnAir) * Time.deltaTime;
        }
        spriteRenderer.flipX = false;
    }

    /// <summary>
    /// Método encargado de mover al personaje hacia atrás
    /// </summary>
    private void MoveBackward()
    {
        var right = transform.right;
        if (isGrounded)
        {
 
            rb.velocity -= new Vector2(right.x * LinearSpeed, right.y * LinearSpeed) * Time.deltaTime;
        }
        else
        {
            rb.velocity -= new Vector2(right.x * SpeedOnAir, right.y * SpeedOnAir) * Time.deltaTime;
        }
        spriteRenderer.flipX = true;
    }

    /// <summary>
    /// Método encargado de saltar
    /// </summary>
    private void Jump()
    {
        if (isGrounded || (jumpTime < MaxJumpTime))
        {
            rb.velocity = new Vector2(rb.velocity.x, JumpSpeed);
            jumpTime += Time.deltaTime;
            if (jumpTime >= MaxJumpTime) soltado = true;
        }
    }

    /// <summary>
    /// Método encargado de disparar
    /// </summary>
    private void Fire()
    {
        source.PlayOneShot(ShootSound);
        Vector3 offset;
        if (spriteRenderer.flipX == false) //estamos mirando hacia la derecha
        {
            offset = new Vector3(spriteRenderer.bounds.size.x, spriteRenderer.bounds.size.y / 2, 0);
            BulletPrefab.GetComponent<BulletController>().IsRightDir = true;
        }
        else
        {
            offset = new Vector3(-spriteRenderer.bounds.size.x, spriteRenderer.bounds.size.y / 2, 0);
            BulletPrefab.GetComponent<BulletController>().IsRightDir = false;
        }
        var bulletInstance = Instantiate(BulletPrefab, transform.position + offset, transform.rotation);
        bulletInstance.GetComponent<BulletController>().OnEnemyDestroyed += EnemyKilled;

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //si la colisión es con un enemigo
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
            //comrpobamos si estamos por encima de su centro de colisión
            if (boxCollider.bounds.min.y > other.collider.bounds.center.y)
            {
                enemy.Kill(); //entonces le matamos
                EnemyKilled();
                rb.velocity = new Vector2(rb.velocity.x, JumpSpeed);
            }
            else //si no, sufrimos daño
            {
                takeDamage();
            }
        }

        if (other.gameObject.CompareTag("PowerUp")) //si la colisión es con un powerUP
        {
            AddPuntuacion(200);
            source.PlayOneShot(PowerUpSound);
            if (!IsBig) //si no es grande ya, crecemos
            {
                IsBig = true;
                animator.SetTrigger("grow");
            }
            else
            {
                fireMode = true;
            }
        }
    }

    /// <summary>
    /// Método encargado de aunmentar la puntuación os puntos indicados
    /// </summary>
    /// <param name="points"></param>
    private void AddPuntuacion(int points)
    {
        Puntuacion += points;
    }

    /// <summary>
    /// Método encargado de gestionar que el jugador reciba daño
    /// </summary>
    private void takeDamage()
    {
        if (IsBig) //si era grande, volvemos a ser pequeños
        {
            IsBig = false;
            fireMode = false;
            animator.SetTrigger("shrink");
        }
        else
        {
            die(); //en caso contrario, mormimos
        }
    }

    /// <summary>
    /// Método encargado de matar al personaje
    /// </summary>
    private void die()
    {
        Lifes--;
        animator.SetBool("dead", true); //cambiamos al sprite de muerte
        rb.velocity = new Vector2(rb.velocity.x, JumpSpeed); //lo movemos hacia arriba para hacer una animación de muerte
        gameObject.layer = 8; //lo cambiamos a una capa que no colisiona con el suelo para que parezca que cae
    }

    /// <summary>
    ///Método encargado de devolver aljugadpr a su estado inicial
    /// </summary>
    public void ResetPlayer()
    {
        animator.SetBool("dead", false); 
        gameObject.layer = 11;
        animator.Rebind();
        IsBig = false;
        fireMode = false;
    }

    /// <summary>
    /// Método que se llama al matar a un enemigo
    /// </summary>
    private void EnemyKilled()
    {
        source.PlayOneShot(EnemyKilledSound);
        AddPuntuacion(100);
    }

    /// <summary>
    /// Método al que se llama cuando obtenemos una moneda
    /// </summary>
    public void AddCoin()
    {
        source.PlayOneShot(CoinSound);
        Monedas++;
        AddPuntuacion(200);
    }

}
