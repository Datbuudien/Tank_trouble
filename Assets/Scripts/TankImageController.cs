using UnityEngine;
public class TankImageController : MonoBehaviour
{
    [Header("Tank Image")]
    public Sprite [] tankSprites;
    private SpriteRenderer tankRenderer;
    void Start(){
        tankRenderer = GetComponent<SpriteRenderer>();
    }
    public void setTankSprite(int gunMode){
        tankRenderer.sprite = tankSprites[gunMode];
    }
}