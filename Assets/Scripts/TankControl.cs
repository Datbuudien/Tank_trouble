using UnityEngine;
using UnityEngine.InputSystem;

public class TankControl : MonoBehaviour
{
    private float speed;
    private float rotationSpeed;
    private Vector2 moveInput;
    private InputSystem_Actions controls;
    private Tank tank;
    [Header("Input Config")]
    public InputConfig inputConfig;
    private int PlayerID ;
    bool is_created = false;
    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Transform fireEffectPoint;
    public GameObject shotGunPrefab;
    public GameObject miniGunPrefab;
    public GameObject beamPrefab;
    private float nextFireTime =0f;
    public int gunMode = 0;
    [Header("VFX")]
    public GameObject shootingVFXPrefab;
    public GameObject shootingMiniShotVFXPrefab;
    [Header("Audio")]
    // public AudioSource audioSource;
    public GameObject audioSource;
    public GameObject audioShotGun;
    public GameObject audioMiniGun;
    public GameObject audioLaser;
    private AudioSource fire;
    private AudioSource shotFire;
    private AudioSource miniFire;
    private AudioSource laserFire;
    private Bullet bullets;
    private ShotGun shotGun;
    private MiniGun miniGun;
    private LaserAim laserAim;
    private Laser laser;
    private Beam beam;

    
    // private AudioSource audioSource;
    private int currentBullets =0;
    private void Awake(){
        // audioSource = GetComponent<AudioSource>(); // dùng cho nếu chỉ có 1 audio source
        if(audioSource == null){
            Debug.Log("AudioSource is null");
        }
        fire = audioSource.GetComponent<AudioSource>();
        if (fire == null) Debug.Log("Fire is null");
        shotFire = audioShotGun.GetComponent<AudioSource>();
        miniFire = audioMiniGun.GetComponent<AudioSource>();
        laserFire = audioLaser.GetComponent<AudioSource>();
        bullets = bulletPrefab.GetComponent<Bullet>();
        shotGun = shotGunPrefab.GetComponent<ShotGun>();
        miniGun = miniGunPrefab.GetComponent<MiniGun>();
        laserAim = GetComponent<LaserAim>();
        laser = GetComponent<Laser>();
    }
    private void Start()
    {
        tank =GetComponent<Tank>();
        if (tank != null)
        {
            PlayerID = tank.playerID;
            speed = tank.speed;
            rotationSpeed = tank.rotationSpeed;
            Debug.Log("PlayerID: " + PlayerID);
            is_created = true;
        }
        controls = new InputSystem_Actions();
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Attack.performed += ctx => OnAttack() ;
        // Chỉ enable Input System cho PlayerID = 1

        // if (PlayerID == 1)
        // {
        //     controls.Enable();
        // }
    }

    // private void OnEnable() 
    // { 
    //     if (controls != null && PlayerID == 1) controls.Enable(); 
    // }
    
    private void OnDisable() 
    { 
        if (controls != null) controls.Disable(); 
    }
    private void OnAttack()
    {
        switch (gunMode)
        {
            case 0:
                OnNormalGun();
                break;
            case 1:
                OnShotGun();
                break; 
            case 2:
                OnMiniGun();
                break;
            case 3:
                OnLaserGun();
                gunMode = 0;
                laserAim.setIsAiming(false);
                break;
            case 4: 
                OnBeamGun();
                break;
        }

    }
    void Update()
    {   
        if(gunMode == 3){
            laserAim.setFirePoint(firePoint);
            laserAim.setIsAiming(true);
        }
        Vector2 finalMoveInput = Vector2.zero;
        if (is_created == true)
        {
            speed = tank.speed;
            rotationSpeed = tank.rotationSpeed;
            PlayerID = tank.playerID;
        }
        if(beam != null){
            beam.BeamUpdate();
        }
        
        if (inputConfig != null)
        {
            InputConfig.PlayerInput currentInput = null;

            switch (PlayerID)
            {
                case 1:
                    currentInput = inputConfig.player1;
                    break;
                case 2:
                    currentInput = inputConfig.player2;
                    break;
                case 3:
                    currentInput = inputConfig.player3;
                    break;
                case 4:
                    currentInput = inputConfig.player4;
                    break;
                default:
                    currentInput = inputConfig.player1;
                    break;
            }

            if (currentInput != null)
            {
                if (Input.GetKey(currentInput.moveForward)) finalMoveInput.y += 1;
                if (Input.GetKey(currentInput.moveBackward)) finalMoveInput.y -= 1;
                if (Input.GetKey(currentInput.rotateLeft)) finalMoveInput.x -= 1;
                if (Input.GetKey(currentInput.rotateRight)) finalMoveInput.x += 1;
                if (Input.GetKey(currentInput.fire)) {
                    OnAttack();
                }
            }
        }


        // Xoay
        transform.Rotate(0, 0, -finalMoveInput.x * rotationSpeed * Time.deltaTime);

        // Tiến/lùi theo hướng đang quay
        transform.Translate(Vector3.right * finalMoveInput.y * speed * Time.deltaTime, Space.Self);
    }
    public void OnBulletDestroyed(){
      if(gunMode ==0)  currentBullets--;
    }
    private void OnNormalGun(){
        if (Time.time < nextFireTime) return;
        if (currentBullets >= bullets.maxBullets) return;    
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.SetOwner(this);
            if (bulletScript != null)
            {
            Vector2 shootDirection = transform.right;
            bulletScript.SetDirection(shootDirection);
            }     
            nextFireTime = Time.time + bullets.fireRate;
            currentBullets++;
        }
        //AudioSource.PlayClipAtPoint(fire.clip, transform.position, fire.volume);
        AudioManager.Play2DOneShot(fire.clip,fire.volume);
        //fire.Play();
        // audioSource.Play();
        
        if (shootingVFXPrefab != null){
            GameObject vfx = Instantiate(shootingVFXPrefab, fireEffectPoint.position,fireEffectPoint.rotation);
            Animator animator = vfx.GetComponent<Animator>();
            if (animator !=null){
                animator.SetBool("isShooting", true);
            }
        Destroy(vfx,1f);
        }
    }
    private void OnShotGun()
    {
        if (Time.time < nextFireTime) return; 
        float angleStep = shotGun.spreadAngle/(shotGun.shotGunBullets-1);
        float startAngle = -shotGun.spreadAngle/2;
        for (int i=0; i<shotGun.shotGunBullets;i++){
            float angle = startAngle + i*angleStep;
            Vector2 shootDirection = RotateVector(transform.right,angle);
            GameObject bullet = Instantiate(shotGunPrefab,firePoint.position,firePoint.rotation);
            ShotGun bulletScript = bullet.GetComponent<ShotGun>();
            bulletScript.SetOwner(this);
            bulletScript.SetDirection(shootDirection);
        }
        AudioManager.Play2DOneShot(shotFire.clip,shotFire.volume);
        if(shootingMiniShotVFXPrefab != null){
            GameObject vfx = Instantiate(shootingMiniShotVFXPrefab,firePoint.position,firePoint.rotation);
            Destroy(vfx,0.15f);
        }
        nextFireTime = Time.time + shotGun.fireRate;
    }
    private void OnMiniGun()
    {   if (Time.time < nextFireTime) return;
        Vector2 direction = transform.up;
        float totalLength = (miniGun.miniGunBullets - 1) * miniGun.bulletSpacing;
        Vector2 startPos = (Vector2)firePoint.position - direction * (totalLength / 2);
        for (int i = 0; i < miniGun.miniGunBullets; i++)
        {
            Vector2 bulletpos = startPos + direction * (miniGun.bulletSpacing * i);
            GameObject bullet = Instantiate(miniGunPrefab, bulletpos, firePoint.rotation);
            Bullet bulletScript = bullet.GetComponent<MiniGun>();
            bulletScript.SetOwner(this);
            bulletScript.SetDirection(transform.right);
        }
        GameObject vfx = Instantiate(shootingMiniShotVFXPrefab,firePoint.position,firePoint.rotation);
        Destroy(vfx,0.15f);
        AudioManager.Play2DOneShot(miniFire.clip,miniFire.volume);
        nextFireTime = Time.time + miniGun.fireRate;
    }
    private void OnLaserGun(){

        // laserAim.DrawLaserAim();
        AudioManager.Play2DOneShot(laserFire.clip,laserFire.volume);
        Debug.Log("laserAim.getPts(): " + laserAim.getPts().Count);
        laser.setPts(laserAim.getPts());
        nextFireTime = Time.time + laser.fireRate;
    }
    private void OnBeamGun(){
        GameObject beamvfx = Instantiate(beamPrefab,firePoint.position,firePoint.rotation);
        beamvfx.transform.SetParent(firePoint);
        beamvfx.transform.localPosition = Vector3.zero;
        beamvfx.transform.localRotation = Quaternion.identity;
        beam = beamvfx.GetComponent<Beam>();
        beam.setFirePoint(firePoint);
        gunMode = 0;
    }
    private Vector2 RotateVector(Vector2 vector, float angle)
    {
        float radians = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        return new Vector2(vector.x*cos- vector.y*sin, vector.x*sin + vector.y*cos);
    }
    





    // private static void Play2DOneShot(AudioClip clip, float volume)
    // {
    //     if (clip == null) return;
    //     var go = new GameObject("OneShot2DAudio");
    //     var src = go.AddComponent<Audi
    // Source>();
    //     src.playOnAwake = false;
    //     src.spatialBlend = 0f;   // 2D
    //     src.dopplerLevel = 0f;   // tắt doppler
    //     src.volume = volume;
    //     src.clip = clip;
    //     src.Play();
    //     Object.Destroy(go, clip.length + 0.1f);
    // }
}