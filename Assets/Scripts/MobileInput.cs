using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInput : MonoBehaviour
{
    public static MobileInput Instance;   // Diðer Script'ler üzrerinden eriþimi saðlar
           
    // Mouse Positions
    private Vector2 start_pos;
    Vector2 last_pos;
    Vector2 delta;

    [Header("Controllers")]
    public bool tap;
    public bool swipeLeft;
    public bool swipeRight;
    public bool swipe;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        // Bütün boollarý sýfýrlýyoruz
        tap = swipe = false;
        swipeLeft = false;  // Sola kaydýrma
        swipeRight = false; // Saða kaydýrma
    }
    private void FixedUpdate()
    {
        SwipeMove();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {   // Mosue tuþuna baýldýðýnda veya ekranda parmak ile basýldýðýndaki ilk pozisyon deðerini alýr
            start_pos = Input.mousePosition;    // Ýlk posizsyon deðeri tutulur
            tap = true; // Dokunma aktif olur
        }

        if (Input.GetMouseButton(0))
        {   // Mosue tuþuna baýlý tutulduðunda veya ekranda parmak ile basýlý tutularak gidildiðindeki son pozisyonun deðerini alýr
            last_pos = Input.mousePosition; // Son pozisyon deðeri tutulur
            delta = start_pos - last_pos;   // Toplam kaydýrýlan mesafe hesaplanýr ve delta deðerinde tutulur
            swipe = true;   // Kaydýrma aktif olur
            
        }

        if (Input.GetMouseButtonUp(0))
        {   // Mosue tuþuna basma býrakýldýðýnda veya ekranda parmak basma býrakýldýðýnda 
            if (start_pos == last_pos) swipe = false;   
            // Eðer dokunulan ilk pozisyon ile son pozisyon deðeri ayný ise kaydýrma pasif olur
            start_pos = Vector2.zero;
            last_pos = Vector2.zero;
            delta = Vector2.zero;
            // Tüm deðerler sýfýrlanýr tekrar dokunma iþlemine kadar

            swipeRight = false;
            swipeLeft = false;
            tap = false;
            // Tüm bool deðerler sýfýrlanýr tekrar dokunma iþlemine kadar
        }
    }
    void SwipeMove()
    {   // Kaydýrma hareketinin yönünü belirler
        // "ArrowController" scripti üzerinde "MoveInput" fonksiyonu saða veya sola hareket yönünü belirtir
        if (tap)    // Eðer dokunma iþlemi aktif ise çalýþýr
        {
            if (swipe)  // Eðer swipe(kaydýrma) iþlemi aktif ise çalýþýr
            {
                if (delta.magnitude > 100)  // delta deðerinin uzunluk bilgisini alýr ve 100 deðerinden büyükse çalýþýr
                    // 100 deðeri minimum kaydýrma mesafesi
                {
                    if (delta.x < 0)
                    {   // Eðer delta (Toplam kaydýrma mesafesi) vector'nün x deðeri 0 dan küçükse Saða kaydýrma aktif olur                       
                        swipeRight = true;
                        swipeLeft = false;
                        tap = false;
                        // swipeRight aktif diðer deðerler pasif olur 
                    }
                    else
                    {   // Eðer delta (Toplam kaydýrma mesafesi) vector'nün x deðeri 0 dan büyükse Sola kaydýrma aktif olur 
                        swipeRight = false;
                        swipeLeft = true;
                        tap = false;
                        // swipeLeft aktif diðer deðerler pasif olur 
                    }
                }
            }
            else if (!swipe)
            {   // Eðer kaydýrma iþlemi pasif ise 
                tap = false;    // Dokunma asif olur
            }
        }
    }
}
