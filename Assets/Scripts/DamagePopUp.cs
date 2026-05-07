using TMPro;
using UnityEngine;

public class DamagePopUp : MonoBehaviour
{
   public static DamagePopUp Create(Vector3 position, int damageAmount)
   {
        Transform damagePopupTransform = Instantiate(GameAssets.i.pfDamagePopup, position, Quaternion.identity);

        DamagePopUp damagePopup = damagePopupTransform.GetComponent<DamagePopUp>();
        damagePopup.Setup(damageAmount);

        return damagePopup;
   }
    private TextMeshPro textMesh;
    private float disappearTimer;

    private void Awake()
    {
        textMesh = GetComponentInChildren<TextMeshPro>();
    }

    public void Setup(int damageAmount)
    {
        textMesh.SetText(damageAmount.ToString());
       disappearTimer = 1f;
    }

    private void Update()
    {
        float moveYSpeed = 2f;
        transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;

        disappearTimer -= Time.deltaTime;
        if(disappearTimer < 0)
        {
            float disappearSpeed = 3f;
            textMesh.alpha -= disappearSpeed * Time.deltaTime;
            if (textMesh.alpha <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}