using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ArrowController : MonoBehaviour
{
    public static ArrowController instance;

    Rigidbody rb;
    public GameObject collectedArrows;  // Toplanan oklar�n tutuldu�u obje

    public bool isFinish=false;
    [SerializeField]private float speed=10f;    // ArrowPlayer ileri h�z�
    [SerializeField]private float horizontalspeed=10f; // ArrowPlayer yana hareket h�z�
    [SerializeField]private float defaultSwipe=3.4f;    // // ArrowPlayer default kayd�rma mesafesi

    [SerializeField]private float collectedArrowsScaleSpeed = 5f;   // Toplnanan okalr�n Scale boyutunun de�i�im h�z�
    Vector3 vec;

    public int collectedArrowsCount;  // Toplanan ok say�s�
    public int value=0;  // Temas edilen say�lar�n say�s� - Eklenek veya ��kar�lacak olan
    float capsulRadiusImpact=0.006f;
    float currentRadius=0.1f;   // Toplanan oklar�n buludu�u objenin collider radius de�eri

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //isMove = true;
    }    
    void Update()
    {
        collectedArrowsCount = collectedArrows.transform.childCount;
        // Toplanan oklar�n say�s� 
    }
    private void FixedUpdate()
    {              
        MoveInput();    // ArrowPlayer hareket fonksiyonu
    }
    void MoveInput()
    {
        float moveX = transform.position.x; // ArrowPlayer nesnesinin x pozisyonun de�erini al�r
        transform.Translate(0, 0, speed * Time.fixedDeltaTime); // ArrowPlayer nesnesi oyun ba�lad���nda s�rekli ileri hareket eder
        if (isFinish == false)
        {   // E�er ArrowPlayer "Finis" alan�na gelmediyse
            if (Input.GetKey(KeyCode.LeftArrow) || MobileInput.Instance.swipeLeft)
            {   // E�er klavyede sol ok tu�una bas�ld�ysa yada "MobileInput" scriptinin swipeLeft de�eri True ise  Sola gider               
                moveX = Mathf.Clamp(moveX - 1 * horizontalspeed * Time.fixedDeltaTime, -defaultSwipe, defaultSwipe);
                // ArrowPlayer nesnesinin x (sol) pozisyonundaki gidece�i min-max s�n�r� belirler

            }
            else if (Input.GetKey(KeyCode.RightArrow) || MobileInput.Instance.swipeRight)
            {   // E�er klavyede sa� ok tu�una bas�ld�ysa yada "MobileInput" scriptinin swipeRight de�eri True ise Sa�a gider         
                moveX = Mathf.Clamp(moveX + 1 * horizontalspeed * Time.fixedDeltaTime, -defaultSwipe, defaultSwipe);
                // ArrowPlayer nesnesinin  x (sa�) pozisyonundaki gidece�i min-max s�n�r� belirler
            }
            else
            {
                rb.velocity = Vector3.zero; //E�er sa�-sol hareket yap�lmad�ysa ArrowPlayer nesnesi sabit kals�n
            }
            transform.position = new Vector3(moveX, transform.position.y, transform.position.z);
            // ArrowPlayer nesnesinin pozisyonun moveX de�erine x y�n�nde g�re sa�-sola hareket eder y ve z sabit kal�r 
        }

        CollectedArrowsScale(); // Toplanan oklar belli bir pozisyondan sonra sa�-sol yaparken boyutu k���lt�l�r ve b�y�lt�l�r
    }
    private void OnTriggerEnter(Collider other)
    {            
        if (other.gameObject.CompareTag("plusWall"))
        {   // Temas edilen nesnenin tag� "plusWall" ise
            value = int.Parse(other.transform.GetChild(0).transform.GetComponent<TMP_Text>().text); // Temas edilen nesnedeki de�eri al�r (Eklenecek ok say�s�)
            RadiusSize();
            GameManager.instance.IncreaseArrow(value);  // "GameManager" �zerinden oklar eklenir
            other.gameObject.transform.GetComponent<BoxCollider>().enabled = false; // Temas edilen nesnenin BoxCollider componenti pasif olur
            collectedArrows.GetComponent<CapsuleCollider>().radius += value * capsulRadiusImpact;   
            //  Her ok al�nd���nda, al�nan ok ba�l� olarak toplanan nesnelerin bulundu�u  "collectedArrows" nesnesin CapsuleCollider'n�n radius de�eri b�y�r  
            currentRadius = collectedArrows.GetComponent<CapsuleCollider>().radius;
            // Her temas edildi�inde "collectedArrows" nesnenin CapsuleCollider  o anki radius de�erini al�r 
        }

        if (other.gameObject.CompareTag("minusWall"))
        {   // Temas edilen nesnenin tag� "minusWall" ise
            value = int.Parse(other.transform.GetChild(0).transform.GetComponent<TMP_Text>().text); // Temas edilen nesnedeki de�eri al�r (��kar�lacak ok say�s�)
            RadiusSize();
            GameManager.instance.DecreaseArrow(value);  // "GameManager" �zerinden oklar yok edilir
            other.gameObject.transform.GetComponent<BoxCollider>().enabled = false; // Temas edilen nesnenin BoxCollider componenti pasif olur
            currentRadius = collectedArrows.GetComponent<CapsuleCollider>().radius; 
            // Her temas edildi�inde "collectedArrows" nesnenin CapsuleCollider  o anki radius de�erini al�r
            collectedArrows.GetComponent<CapsuleCollider>().radius -= value * capsulRadiusImpact;
            //  Her ok al�nd���nda, al�nan ok ba�l� olarak toplanan nesnelerin bulundu�u  "collectedArrows" nesnesin CapsuleCollider'n�n radius de�eri k���l�r    
            if (value > collectedArrows.transform.childCount)
            {
                GameManager.instance.RestartGame();
                // E�er toplanam�� ok say�s� ��kar�lacak ok say�s�ndan az ise oyun yeniden ba�lat�l�r
            }
        }

        if (other.gameObject.CompareTag("impactWall"))
        {   // Temas edilen nesnenin tag� "impactWall" ise
            value = int.Parse(other.transform.GetChild(0).transform.GetComponent<TMP_Text>().text); // Temas edilen nesnedeki de�eri al�r (Eklenecek ok say�s�)
            RadiusSize();
            value = (value-1) * collectedArrowsCount;
            GameManager.instance.IncreaseArrow(value);  // "GameManager" �zerinden oklar eklenir
            other.gameObject.transform.GetComponent<BoxCollider>().enabled = false; // Temas edilen nesnenin BoxCollider componenti pasif olur
            collectedArrows.GetComponent<CapsuleCollider>().radius += value * capsulRadiusImpact;
            //  Her ok al�nd���nda, al�nan ok ba�l� olarak toplanan nesnelerin bulundu�u  "collectedArrows" nesnesin CapsuleCollider'n�n radius de�eri b�y�r  
            currentRadius = collectedArrows.GetComponent<CapsuleCollider>().radius;
            // Her temas edildi�inde "collectedArrows" nesnenin CapsuleCollider  o anki radius de�erini al�r 
        }

        if (other.gameObject.CompareTag("chamberWall"))
        {   // Temas edilen nesnenin tag� "chamberWall" ise              
            value = int.Parse(other.transform.GetChild(0).transform.GetComponent<TMP_Text>().text); // Temas edilen nesnedeki de�eri al�r (��kar�lacak ok say�s�)
            RadiusSize();
            value = collectedArrowsCount / value;
            GameManager.instance.DecreaseArrow(value);  // "GameManager" �zerinden oklar yok edilir
            other.gameObject.transform.GetComponent<BoxCollider>().enabled = false; // Temas edilen nesnenin BoxCollider componenti pasif olur
            currentRadius = collectedArrows.GetComponent<CapsuleCollider>().radius; 
            // Her temas edildi�inde "collectedArrows" nesnenin CapsuleCollider  o anki radius de�erini al�r
            collectedArrows.GetComponent<CapsuleCollider>().radius -= value * capsulRadiusImpact;
            //  Her ok al�nd���nda, al�nan ok ba�l� olarak toplanan nesnelerin bulundu�u  "collectedArrows" nesnesin CapsuleCollider'n�n radius de�eri k���l�r  
            if (value > collectedArrows.transform.childCount)
            {
                GameManager.instance.RestartGame();
                // E�er toplanam�� ok say�s� ��kar�lacak ok say�s�ndan az ise oyun yeniden ba�lat�l�r
            }
        }

        if (other.gameObject.CompareTag("Finish"))
        {   // "ArrowPlayer" nesnesi finish tag�n�n bulundu�u collidere temas ederse            
            isFinish = true;
            horizontalspeed = 2f;   // "ArrowPlayer" nesnesinin ileri hareket h�z� yavaslar
            GameManager.instance.collectedArrowsTxt.enabled = false;    // "ArrowPlayer" nesnesi �zerinde g�sterilen ok say�s� asif olur    
            GameManager.instance.NextlevelPanel();  // Finish alan�na girine "NextlevelPanel" fonksiyonu �al���r
        }
    }
    void RadiusSize()
    {   // Toplanan ok say�s�na g�re toplanan oklar�n topland��� "collectedArrows" nesnesinin CapsuleCollider'n�n radius de�erinin b�y�me �arpan� ayarlan�r
        if (collectedArrowsCount * value < 200)
        {   // Toplanan ok say�s� 200 den az ise
            capsulRadiusImpact = 0.006f;   
        }
        else
        {   // Toplanan ok say�s� 200 den fazla ise
            capsulRadiusImpact = 0.005f;
        }
    }
    void CollectedArrowsScale()
    {
        //----- Oyun alan�nda "ArrowsCollected" �l�ek boyutu ayar� ---- //
        if (transform.position.x <= -3f || transform.position.x >= 3f && isFinish == false)
        {   // E�er "ArrowPlayer" nesnesinin x pozisyonu -3f'den k���k veya 3f'den b�y�k ise "collectedArrows" nesnesinin Scale de�erini k���lt
            Vector3 currentScale = new Vector3(
                collectedArrows.transform.localScale.x,
                collectedArrows.transform.localScale.y,
                collectedArrows.transform.localScale.z);
            // "collectedArrows" nesnesinin ilk scale de�eri
            Vector3 targetScale = new Vector3(
                0.5f,
                collectedArrows.transform.localScale.y,
                collectedArrows.transform.localScale.z);
            // "collectedArrows" nesnesinin alaca�� scale de�eri

            collectedArrows.transform.localScale = Vector3.Lerp(currentScale, targetScale, Time.fixedDeltaTime * collectedArrowsScaleSpeed);
            // "collectedArrows" nesnesinin ilk scale de�eri,alaca�� scale de�erine yumu�ak ge�i� yapar

            collectedArrows.GetComponent<CapsuleCollider>().radius = currentRadius / 2f;
            // "collectedArrows" nesnesinin CapsuleCollider'n�n radius de�eri yar�ya d���r�l�r toplanan oklar s�kla�t��� zaman

            for (int i = 0; i < collectedArrows.transform.childCount; i++)
            {   // Oklar�n topland���  "collectedArrows" alt�nfdaki alt nesnelerin scale de�eri 2 kat� kadar b�y�t
                collectedArrows.transform.GetChild(i).transform.localScale = new Vector3(
                    2f,
                    collectedArrows.transform.GetChild(i).transform.localScale.y,
                    collectedArrows.transform.GetChild(i).transform.localScale.z);
            }
        }
        else if (transform.position.x >= -3f || transform.position.x <= 3f && isFinish == false)
        {   // E�er "ArrowPlayer" nesnesinin x pozisyonu -3f'den b�y�k veya 3f'den k���k ise "collectedArrows" nesnesinin Scale de�erini b�y�lt
            Vector3 currentScale = new Vector3(
                collectedArrows.transform.localScale.x,
                collectedArrows.transform.localScale.y,
                collectedArrows.transform.localScale.z);
            // "collectedArrows" nesnesinin ilk scale de�eri
            Vector3 targetScale = new Vector3(
                1f,
                collectedArrows.transform.localScale.y,
                collectedArrows.transform.localScale.z);
            // "collectedArrows" nesnesinin alaca�� scale de�eri

            collectedArrows.transform.localScale = Vector3.Lerp(currentScale, targetScale, Time.fixedDeltaTime * collectedArrowsScaleSpeed);
            // "collectedArrows" nesnesinin ilk scale de�eri,alaca�� scale de�erine yumu�ak ge�i� yapar

            collectedArrows.GetComponent<CapsuleCollider>().radius = currentRadius;
            // "collectedArrows" nesnesinin CapsuleCollider'n�n radius de�eri eski de�erini toplanan oklar tekrar eski haline geldi�i zaman

            for (int i = 0; i < collectedArrows.transform.childCount; i++)
            {   // Oklar�n topland���  "collectedArrows" alt�nfdaki alt nesnelerin scale de�eri normal de�erine d�ner
                collectedArrows.transform.GetChild(i).transform.localScale = new Vector3(
                    1f,
                    collectedArrows.transform.GetChild(i).transform.localScale.y,
                    collectedArrows.transform.GetChild(i).transform.localScale.z);
            }
        }

        //----- Finish alan�nda "ArrowsCollected" �l�ek boyutu ayar� ---- //
        if (isFinish)
        {   // Finih alan�na gelince Toplanan oklar�n tutuldu�u "collectedArrows" objesinin collider�n radius de�eri ayarlan�r
            if (Input.GetKey(KeyCode.LeftArrow) || MobileInput.Instance.swipeLeft || Input.GetKey(KeyCode.RightArrow) || MobileInput.Instance.swipeRight)
            {   //"collectedArrows" nesnesinin Scale de�erini sa�a- veya sola hareket ettirerek b�y�lt�l�r
                Vector3 currentScale = new Vector3(
                    collectedArrows.transform.localScale.x,
                    collectedArrows.transform.localScale.y,
                    collectedArrows.transform.localScale.z);
                // "collectedArrows" nesnesinin ilk scale de�eri
                Vector3 targetScale = new Vector3(
                    6f,
                    collectedArrows.transform.localScale.y,
                    collectedArrows.transform.localScale.z);
                // "collectedArrows" nesnesinin alaca�� scale de�eri

                collectedArrows.transform.localScale = Vector3.Lerp(currentScale, targetScale, Time.fixedDeltaTime * collectedArrowsScaleSpeed);
                // "collectedArrows" nesnesinin ilk scale de�eri,alaca�� scale de�erine yumu�ak ge�i� yapar

                collectedArrows.GetComponent<CapsuleCollider>().radius = currentRadius;
                // "collectedArrows" nesnesinin CapsuleCollider'n�n radius de�eri eski de�erini toplanan oklar tekrar eski haline geldi�i zaman
                for (int i = 0; i < collectedArrows.transform.childCount; i++)
                {   // Oklar�n topland���  "collectedArrows" alt�nfdaki alt nesnelerin scale de�eri 0.25f de�erini al�r
                    collectedArrows.transform.GetChild(i).transform.localScale = new Vector3(
                        0.25f,
                        collectedArrows.transform.GetChild(i).transform.localScale.y,
                        collectedArrows.transform.GetChild(i).transform.localScale.z);
                }
            }
        }
    }
}
