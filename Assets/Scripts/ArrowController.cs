using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ArrowController : MonoBehaviour
{
    public static ArrowController instance;

    Rigidbody rb;
    public GameObject collectedArrows;  // Toplanan oklarýn tutulduðu obje

    public bool isFinish=false;
    [SerializeField]private float speed=10f;    // ArrowPlayer ileri hýzý
    [SerializeField]private float horizontalspeed=10f; // ArrowPlayer yana hareket hýzý
    [SerializeField]private float defaultSwipe=3.4f;    // // ArrowPlayer default kaydýrma mesafesi

    [SerializeField]private float collectedArrowsScaleSpeed = 5f;   // Toplnanan okalrýn Scale boyutunun deðiþim hýzý
    Vector3 vec;

    public int collectedArrowsCount;  // Toplanan ok sayýsý
    public int value=0;  // Temas edilen sayýlarýn sayýsý - Eklenek veya çýkarýlacak olan
    float capsulRadiusImpact=0.006f;
    float currentRadius=0.1f;   // Toplanan oklarýn buluduðu objenin collider radius deðeri

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
        // Toplanan oklarýn sayýsý 
    }
    private void FixedUpdate()
    {              
        MoveInput();    // ArrowPlayer hareket fonksiyonu
    }
    void MoveInput()
    {
        float moveX = transform.position.x; // ArrowPlayer nesnesinin x pozisyonun deðerini alýr
        transform.Translate(0, 0, speed * Time.fixedDeltaTime); // ArrowPlayer nesnesi oyun baþladýðýnda sürekli ileri hareket eder
        if (isFinish == false)
        {   // Eðer ArrowPlayer "Finis" alanýna gelmediyse
            if (Input.GetKey(KeyCode.LeftArrow) || MobileInput.Instance.swipeLeft)
            {   // Eðer klavyede sol ok tuþuna basýldýysa yada "MobileInput" scriptinin swipeLeft deðeri True ise  Sola gider               
                moveX = Mathf.Clamp(moveX - 1 * horizontalspeed * Time.fixedDeltaTime, -defaultSwipe, defaultSwipe);
                // ArrowPlayer nesnesinin x (sol) pozisyonundaki gideceði min-max sýnýrý belirler

            }
            else if (Input.GetKey(KeyCode.RightArrow) || MobileInput.Instance.swipeRight)
            {   // Eðer klavyede sað ok tuþuna basýldýysa yada "MobileInput" scriptinin swipeRight deðeri True ise Saða gider         
                moveX = Mathf.Clamp(moveX + 1 * horizontalspeed * Time.fixedDeltaTime, -defaultSwipe, defaultSwipe);
                // ArrowPlayer nesnesinin  x (sað) pozisyonundaki gideceði min-max sýnýrý belirler
            }
            else
            {
                rb.velocity = Vector3.zero; //Eðer sað-sol hareket yapýlmadýysa ArrowPlayer nesnesi sabit kalsýn
            }
            transform.position = new Vector3(moveX, transform.position.y, transform.position.z);
            // ArrowPlayer nesnesinin pozisyonun moveX deðerine x yönünde göre sað-sola hareket eder y ve z sabit kalýr 
        }

        CollectedArrowsScale(); // Toplanan oklar belli bir pozisyondan sonra sað-sol yaparken boyutu küçültülür ve büyültülür
    }
    private void OnTriggerEnter(Collider other)
    {            
        if (other.gameObject.CompareTag("plusWall"))
        {   // Temas edilen nesnenin tagý "plusWall" ise
            value = int.Parse(other.transform.GetChild(0).transform.GetComponent<TMP_Text>().text); // Temas edilen nesnedeki deðeri alýr (Eklenecek ok sayýsý)
            RadiusSize();
            GameManager.instance.IncreaseArrow(value);  // "GameManager" üzerinden oklar eklenir
            other.gameObject.transform.GetComponent<BoxCollider>().enabled = false; // Temas edilen nesnenin BoxCollider componenti pasif olur
            collectedArrows.GetComponent<CapsuleCollider>().radius += value * capsulRadiusImpact;   
            //  Her ok alýndýðýnda, alýnan ok baðlý olarak toplanan nesnelerin bulunduðu  "collectedArrows" nesnesin CapsuleCollider'nýn radius deðeri büyür  
            currentRadius = collectedArrows.GetComponent<CapsuleCollider>().radius;
            // Her temas edildiðinde "collectedArrows" nesnenin CapsuleCollider  o anki radius deðerini alýr 
        }

        if (other.gameObject.CompareTag("minusWall"))
        {   // Temas edilen nesnenin tagý "minusWall" ise
            value = int.Parse(other.transform.GetChild(0).transform.GetComponent<TMP_Text>().text); // Temas edilen nesnedeki deðeri alýr (Çýkarýlacak ok sayýsý)
            RadiusSize();
            GameManager.instance.DecreaseArrow(value);  // "GameManager" üzerinden oklar yok edilir
            other.gameObject.transform.GetComponent<BoxCollider>().enabled = false; // Temas edilen nesnenin BoxCollider componenti pasif olur
            currentRadius = collectedArrows.GetComponent<CapsuleCollider>().radius; 
            // Her temas edildiðinde "collectedArrows" nesnenin CapsuleCollider  o anki radius deðerini alýr
            collectedArrows.GetComponent<CapsuleCollider>().radius -= value * capsulRadiusImpact;
            //  Her ok alýndýðýnda, alýnan ok baðlý olarak toplanan nesnelerin bulunduðu  "collectedArrows" nesnesin CapsuleCollider'nýn radius deðeri küçülür    
            if (value > collectedArrows.transform.childCount)
            {
                GameManager.instance.RestartGame();
                // Eðer toplanamýþ ok sayýsý çýkarýlacak ok sayýsýndan az ise oyun yeniden baþlatýlýr
            }
        }

        if (other.gameObject.CompareTag("impactWall"))
        {   // Temas edilen nesnenin tagý "impactWall" ise
            value = int.Parse(other.transform.GetChild(0).transform.GetComponent<TMP_Text>().text); // Temas edilen nesnedeki deðeri alýr (Eklenecek ok sayýsý)
            RadiusSize();
            value = (value-1) * collectedArrowsCount;
            GameManager.instance.IncreaseArrow(value);  // "GameManager" üzerinden oklar eklenir
            other.gameObject.transform.GetComponent<BoxCollider>().enabled = false; // Temas edilen nesnenin BoxCollider componenti pasif olur
            collectedArrows.GetComponent<CapsuleCollider>().radius += value * capsulRadiusImpact;
            //  Her ok alýndýðýnda, alýnan ok baðlý olarak toplanan nesnelerin bulunduðu  "collectedArrows" nesnesin CapsuleCollider'nýn radius deðeri büyür  
            currentRadius = collectedArrows.GetComponent<CapsuleCollider>().radius;
            // Her temas edildiðinde "collectedArrows" nesnenin CapsuleCollider  o anki radius deðerini alýr 
        }

        if (other.gameObject.CompareTag("chamberWall"))
        {   // Temas edilen nesnenin tagý "chamberWall" ise              
            value = int.Parse(other.transform.GetChild(0).transform.GetComponent<TMP_Text>().text); // Temas edilen nesnedeki deðeri alýr (Çýkarýlacak ok sayýsý)
            RadiusSize();
            value = collectedArrowsCount / value;
            GameManager.instance.DecreaseArrow(value);  // "GameManager" üzerinden oklar yok edilir
            other.gameObject.transform.GetComponent<BoxCollider>().enabled = false; // Temas edilen nesnenin BoxCollider componenti pasif olur
            currentRadius = collectedArrows.GetComponent<CapsuleCollider>().radius; 
            // Her temas edildiðinde "collectedArrows" nesnenin CapsuleCollider  o anki radius deðerini alýr
            collectedArrows.GetComponent<CapsuleCollider>().radius -= value * capsulRadiusImpact;
            //  Her ok alýndýðýnda, alýnan ok baðlý olarak toplanan nesnelerin bulunduðu  "collectedArrows" nesnesin CapsuleCollider'nýn radius deðeri küçülür  
            if (value > collectedArrows.transform.childCount)
            {
                GameManager.instance.RestartGame();
                // Eðer toplanamýþ ok sayýsý çýkarýlacak ok sayýsýndan az ise oyun yeniden baþlatýlýr
            }
        }

        if (other.gameObject.CompareTag("Finish"))
        {   // "ArrowPlayer" nesnesi finish tagýnýn bulunduðu collidere temas ederse            
            isFinish = true;
            horizontalspeed = 2f;   // "ArrowPlayer" nesnesinin ileri hareket hýzý yavaslar
            GameManager.instance.collectedArrowsTxt.enabled = false;    // "ArrowPlayer" nesnesi üzerinde gösterilen ok sayýsý asif olur    
            GameManager.instance.NextlevelPanel();  // Finish alanýna girine "NextlevelPanel" fonksiyonu çalýþýr
        }
    }
    void RadiusSize()
    {   // Toplanan ok sayýsýna göre toplanan oklarýn toplandýðý "collectedArrows" nesnesinin CapsuleCollider'nýn radius deðerinin büyüme çarpaný ayarlanýr
        if (collectedArrowsCount * value < 200)
        {   // Toplanan ok sayýsý 200 den az ise
            capsulRadiusImpact = 0.006f;   
        }
        else
        {   // Toplanan ok sayýsý 200 den fazla ise
            capsulRadiusImpact = 0.005f;
        }
    }
    void CollectedArrowsScale()
    {
        //----- Oyun alanýnda "ArrowsCollected" ölçek boyutu ayarý ---- //
        if (transform.position.x <= -3f || transform.position.x >= 3f && isFinish == false)
        {   // Eðer "ArrowPlayer" nesnesinin x pozisyonu -3f'den küçük veya 3f'den büyük ise "collectedArrows" nesnesinin Scale deðerini küçült
            Vector3 currentScale = new Vector3(
                collectedArrows.transform.localScale.x,
                collectedArrows.transform.localScale.y,
                collectedArrows.transform.localScale.z);
            // "collectedArrows" nesnesinin ilk scale deðeri
            Vector3 targetScale = new Vector3(
                0.5f,
                collectedArrows.transform.localScale.y,
                collectedArrows.transform.localScale.z);
            // "collectedArrows" nesnesinin alacaðý scale deðeri

            collectedArrows.transform.localScale = Vector3.Lerp(currentScale, targetScale, Time.fixedDeltaTime * collectedArrowsScaleSpeed);
            // "collectedArrows" nesnesinin ilk scale deðeri,alacaðý scale deðerine yumuþak geçiþ yapar

            collectedArrows.GetComponent<CapsuleCollider>().radius = currentRadius / 2f;
            // "collectedArrows" nesnesinin CapsuleCollider'nýn radius deðeri yarýya düþürülür toplanan oklar sýklaþtýðý zaman

            for (int i = 0; i < collectedArrows.transform.childCount; i++)
            {   // Oklarýn toplandýðý  "collectedArrows" altýnfdaki alt nesnelerin scale deðeri 2 katý kadar büyüt
                collectedArrows.transform.GetChild(i).transform.localScale = new Vector3(
                    2f,
                    collectedArrows.transform.GetChild(i).transform.localScale.y,
                    collectedArrows.transform.GetChild(i).transform.localScale.z);
            }
        }
        else if (transform.position.x >= -3f || transform.position.x <= 3f && isFinish == false)
        {   // Eðer "ArrowPlayer" nesnesinin x pozisyonu -3f'den büyük veya 3f'den küçük ise "collectedArrows" nesnesinin Scale deðerini büyült
            Vector3 currentScale = new Vector3(
                collectedArrows.transform.localScale.x,
                collectedArrows.transform.localScale.y,
                collectedArrows.transform.localScale.z);
            // "collectedArrows" nesnesinin ilk scale deðeri
            Vector3 targetScale = new Vector3(
                1f,
                collectedArrows.transform.localScale.y,
                collectedArrows.transform.localScale.z);
            // "collectedArrows" nesnesinin alacaðý scale deðeri

            collectedArrows.transform.localScale = Vector3.Lerp(currentScale, targetScale, Time.fixedDeltaTime * collectedArrowsScaleSpeed);
            // "collectedArrows" nesnesinin ilk scale deðeri,alacaðý scale deðerine yumuþak geçiþ yapar

            collectedArrows.GetComponent<CapsuleCollider>().radius = currentRadius;
            // "collectedArrows" nesnesinin CapsuleCollider'nýn radius deðeri eski deðerini toplanan oklar tekrar eski haline geldiði zaman

            for (int i = 0; i < collectedArrows.transform.childCount; i++)
            {   // Oklarýn toplandýðý  "collectedArrows" altýnfdaki alt nesnelerin scale deðeri normal deðerine döner
                collectedArrows.transform.GetChild(i).transform.localScale = new Vector3(
                    1f,
                    collectedArrows.transform.GetChild(i).transform.localScale.y,
                    collectedArrows.transform.GetChild(i).transform.localScale.z);
            }
        }

        //----- Finish alanýnda "ArrowsCollected" ölçek boyutu ayarý ---- //
        if (isFinish)
        {   // Finih alanýna gelince Toplanan oklarýn tutulduðu "collectedArrows" objesinin colliderýn radius deðeri ayarlanýr
            if (Input.GetKey(KeyCode.LeftArrow) || MobileInput.Instance.swipeLeft || Input.GetKey(KeyCode.RightArrow) || MobileInput.Instance.swipeRight)
            {   //"collectedArrows" nesnesinin Scale deðerini saða- veya sola hareket ettirerek büyültülür
                Vector3 currentScale = new Vector3(
                    collectedArrows.transform.localScale.x,
                    collectedArrows.transform.localScale.y,
                    collectedArrows.transform.localScale.z);
                // "collectedArrows" nesnesinin ilk scale deðeri
                Vector3 targetScale = new Vector3(
                    6f,
                    collectedArrows.transform.localScale.y,
                    collectedArrows.transform.localScale.z);
                // "collectedArrows" nesnesinin alacaðý scale deðeri

                collectedArrows.transform.localScale = Vector3.Lerp(currentScale, targetScale, Time.fixedDeltaTime * collectedArrowsScaleSpeed);
                // "collectedArrows" nesnesinin ilk scale deðeri,alacaðý scale deðerine yumuþak geçiþ yapar

                collectedArrows.GetComponent<CapsuleCollider>().radius = currentRadius;
                // "collectedArrows" nesnesinin CapsuleCollider'nýn radius deðeri eski deðerini toplanan oklar tekrar eski haline geldiði zaman
                for (int i = 0; i < collectedArrows.transform.childCount; i++)
                {   // Oklarýn toplandýðý  "collectedArrows" altýnfdaki alt nesnelerin scale deðeri 0.25f deðerini alýr
                    collectedArrows.transform.GetChild(i).transform.localScale = new Vector3(
                        0.25f,
                        collectedArrows.transform.GetChild(i).transform.localScale.y,
                        collectedArrows.transform.GetChild(i).transform.localScale.z);
                }
            }
        }
    }
}
