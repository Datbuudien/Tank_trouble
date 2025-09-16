using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class Laser : MonoBehaviour
{
    [Header("Laser Properties")]
    public LineRenderer laserLine;
    public LayerMask tankLayer;
    private LineRenderer instanceLine;
    private List<Vector3> pts;
    private Coroutine laserCoroutine;
    public float fireRate = 0.5f;
    public float damage = 20f;
    private Color c;
    [Header("Visual Effects")]
    public GameObject hitEffect;
    [Header("Audio")]
    public GameObject collisionAudio;
    private AudioSource audioSource;
    public void Awake(){
        instanceLine = Instantiate(laserLine,transform);
        instanceLine.enabled = false;
        instanceLine.useWorldSpace = true;
        audioSource = collisionAudio.GetComponent<AudioSource>();
    }
    public void setPts(List<Vector3> pts){
        c = GetComponent<Tank>().GetColor();
        instanceLine.startColor = c;
        instanceLine.endColor = c;
        Debug.Log("pts: " + pts.Count);
        this.pts = pts;
        instanceLine.enabled = true;
        laserCoroutine = StartCoroutine(OnLaser());
    }
    private IEnumerator OnLaser(){
        List<Vector3> temp = new List<Vector3>{pts[0]};
        Debug.Log("temp: " + temp.Count);
        for(int i=1;i<pts.Count;i++){
            for(int j=1;j<=4;j++){
                Vector3 nextPt = (pts[i-1]*(5-j)+pts[i]*j)/5f;
                temp.Add(nextPt);
            }
            temp.Add(pts[i]);
        }
        for(int i=0;i<temp.Count-1;i++){
            instanceLine.positionCount = 2;
            Vector3 start = temp[i];
            Vector3 end = temp[i+1];
            Vector3 dir = (end-start).normalized;
            float dis = Vector3.Distance(start,end);
            RaycastHit2D hit = Physics2D.Raycast(start,dir,dis,tankLayer);
            if(hit.collider != null)
            {
                instanceLine.SetPosition(0,start);
                instanceLine.SetPosition(1,hit.point);
                Tank tank = hit.collider.GetComponent<Tank>();
                tank.TakeDamage(damage);
                Vector3 hitPos =(Vector3)  hit.point-dir*0.03f;
                GameObject vfx = Instantiate(hitEffect,hitPos,Quaternion.identity);
                AudioManager.Play2DOneShot(audioSource.clip,audioSource.volume);
                Destroy(vfx,1f);
                // yield return new WaitForSeconds(0.0f);
                break;
            }
            instanceLine.SetPosition(0,temp[i]);
            instanceLine.SetPosition(1,temp[i+1]);
            yield return new WaitForSeconds(0.025f);
        }
        instanceLine.enabled = false;
    }
}