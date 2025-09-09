using UnityEngine; 
public class Bullet : MonoBehaviour
{
    [Header("Bullet Properties")]
    public float speed = 10f;
    public int damage =1; 
    public float lifetime =10f;
    public int bounceCount = 5;
    public float fireRate = 0.5f;
    public int maxBullets = 5;
    [Header("Bullet Physics")]
    public bool useGravity = false; 
    public float alpha = 0.03f;
    public float bounceForce = 10f; 
    [Header("Visual Effects")]
    public GameObject hitEffect;
    public Color bulletColor = Color.black;
    [Header("Audio")]
    public GameObject collisionAudio;
    private Rigidbody2D rb;
    private Vector2 direction;
    private float currentLifetime; 
    private int currentBounceCount;
    private Vector2 lastVelocity;
    private TankControl owner;
    private AudioSource audioSource;
    void Awake(){
        rb = GetComponent<Rigidbody2D>();
        if(rb ==null){
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = useGravity ? 1f : 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        if (hitEffect == null){
            Debug.Log("HitEffect is null chưa có hit effect được gán vào bullet");
        }
        audioSource = collisionAudio.GetComponent<AudioSource>();
    }
    void Start(){
        currentLifetime = lifetime;
        currentBounceCount = 0;
        //direction = transform.right;
    }
    void Update(){
        currentLifetime -= Time.deltaTime;
        if(currentLifetime <=0 || currentBounceCount == bounceCount ){
            DestroyBullet();
            return;
        }
       rb.linearVelocity = direction*speed;
    }
    private void DestroyBullet(){
        Destroy(gameObject);
        owner.OnBulletDestroyed();
    }
    private void HandleWallCollision(Collision2D collision){
        Vector2 normal = collision.GetContact(0).normal;
        Debug.Log(direction);
        direction = Vector2.Reflect(direction,normal);
        // Debug.Log($"Direction sau khi phan xa: {direction}");
        currentBounceCount++;
        if(currentBounceCount == bounceCount){
            DestroyBullet();
            return;
        }
    }
    void OnCollisionEnter2D(Collision2D collision){
        if (collision.gameObject.CompareTag("Tank")){
            Tank tank = collision.gameObject.GetComponent<Tank>();
            if(tank!=null){
                tank.TakeDamage(damage);
            }
            // Vector2 v = rb.linearVelocity;
            Vector3 hitPos =(Vector3)collision.GetContact(0).point-(Vector3)(lastVelocity*alpha);
            GameObject vfx = Instantiate(hitEffect,hitPos,Quaternion.identity);
            Destroy(vfx,1f);
            AudioManager.Play2DOneShot(audioSource.clip,audioSource.volume);
            DestroyBullet();
            return;
        }
        if(collision.gameObject.CompareTag("Wall")){
            HandleWallCollision(collision);
        }
    }
    void FixedUpdate(){
        lastVelocity = rb!=null ? rb.linearVelocity : Vector2.zero;
    }
    public void SetDirection(Vector2 direction){
        this.direction = direction;
    }
    public void SetOwner(TankControl owner){
        this.owner = owner;
    }
}