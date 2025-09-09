using UnityEngine; 
public class Tank: MonoBehaviour
{
    [Header("Tank Stats")]
    public float maxHealth =100f;
    public int playerID = 1;
    public float speed = 5f;
    public float rotationSpeed = 180f;
    public float currrentHealth;
    [Header("Tank State")]
    public bool isAlive = true; 
    public bool canShoot = true;
    [Header("Tank VFX")]
    public GameObject crashEffect;
    [Header("Tank Audio")]
    public GameObject audioCrashing;
    private AudioSource audioSource;
    private void Awake(){
        gameObject.tag = "Tank";
        if(crashEffect == null){
            Debug.Log("CrashEffect is null");
        }
        string mess1= audioCrashing == null ? "AudioCrashing is null" : "AudioCrashing is not null";
        audioSource = audioCrashing.GetComponent<AudioSource>();
        string mess2= audioSource == null ? "AudioSource is null" : "AudioSource is not null";
        Debug.Log(mess1+" "+mess2);
    }
    private void Start(){
        currrentHealth = maxHealth; 
    }
    private void Die(){
        isAlive = false;
        canShoot = false; 
        gameObject.SetActive(false);
        GameObject vfx = Instantiate(crashEffect,transform.position,Quaternion.identity);
        AudioManager.Play2DOneShot(audioSource.clip,audioSource.volume);
        Destroy(vfx,1f);
    }
    public void TakeDamage(float damage){
        currrentHealth -= damage;
        if(currrentHealth <=0){
            Die();
            return;
        }
    }
}