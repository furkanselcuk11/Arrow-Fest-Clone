using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] Transform arrowPrefab; // Eklenecek oklar i�in se�ilen perefab
    [SerializeField] GameObject nextLevelPanel; // Oyun sonu ��kan next level butonlar�n�n tutuldu�u nesne
    Transform newArrow;

    //bool isBonus;
    int totalCoin;
    int levelCoin;
    int bonusCoin;

    public TMP_Text collectedArrowsTxt;
    [SerializeField] TMP_Text coinTxt;
    [SerializeField] TMP_Text levelCoinTxt;
    [SerializeField] TMP_Text bonusCoinTxt;
    [SerializeField] TMP_Text levelTxt;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        Time.timeScale = 1f;    // Oyun ba�lad�p�nda time.scale de�rini 1 yapar
        int gameLevel = SceneManager.GetActiveScene().buildIndex + 1;   // Oyun sahnesindeki leveli "gameLevel" de�i�keinne atar
        levelTxt.text = "Level " + gameLevel;   // "gameLevel" deki leveli ekrana yazar
        //PlayerPrefs.DeleteAll();    // T�m skorlar� s�f�rlar
        nextLevelPanel.SetActive(false);    // Oyun a��l�nd���nda "nextLevelPanel" panelini kapat�r

        if (PlayerPrefs.HasKey("TotalCoin"))    // Kay�tl� tulutlan "TotalCoin" indexinde veri var m� kontrol eder - varsa
        {
            totalCoin = PlayerPrefs.GetInt("TotalCoin");
            // Varsa kay�tl� tulutlan "TotalCoin" indexsindeki skor verisini "totalCoin" atar
            coinTxt.text = totalCoin.ToString();    // "totalCoin" de�erini ekranda g�sterir
        }
        else
        {
            PlayerPrefs.SetInt("TotalCoin", 0);     // Kay�tl� tulutlan "TotalCoin" indexsinde veri yoksa "TotalCoin" index de�erini 0 yapar
            coinTxt.text = PlayerPrefs.GetInt("TotalCoin").ToString();  // "TotalCoin" indexsindeki de�erini ekranda g�sterir
        }

    }
    private void Update()
    {
        if (ArrowController.instance.collectedArrowsCount >= 1)
        {
            collectedArrowsTxt.text = ArrowController.instance.collectedArrowsCount.ToString();
            // Toplanan ok say�lar�n� ekranda g�sterir
        }
    }
    public void IncreaseArrow(int value)
    {   // value= "ArrowPlayer"�n Temas etti�i nesnedeki de�eri al�r (Eklenecek ok say�s�)
        // "ArrowController" den gelen value de�erine g�re "collectedArrows" nesnesi i�ine value de�eri kadar ok ekler
        StartCoroutine(IncreaseArrowIE(value)); //IEnumerator i�lemi ile 0.1 sniye sonra �al���r
    }
    IEnumerator IncreaseArrowIE(int value)
    {   // value= "IncreaseArrow" gelen de�er
        float x = ArrowController.instance.collectedArrows.GetComponent<CapsuleCollider>().radius;
        // oklar�n tutuldu�u "collectedArrows" nesnesinin CapsuleCollider �ap�n�n de��erini al�r       
        x += value * 0.0035f;   // Eklencek oklar�n daha geni� bir �apa girmesi i�in her ok topland���nda tplanan ok kadar �ap de�eri b�y�r
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < value; i++)
        {
            Vector3 randomPoint= Random.insideUnitCircle * x;   // Belirlenen yar��ap i�inde oklar�n radnom pozisyonlar� belirlenir
            Vector3 pos = new Vector3(
                ArrowController.instance.collectedArrows.transform.position.x +randomPoint.x,
                ArrowController.instance.collectedArrows.transform.position.y +randomPoint.y,
                ArrowController.instance.collectedArrows.transform.position.z);
            // Instantiate edilecek olan oklar�n hangi posizyonda do�aca�� belirlenir

            newArrow = Instantiate(arrowPrefab, pos, Quaternion.identity);  
            // for d�ng�s� devam ettik�e pos de�i�kenin ald��� pozisyon de�erinde yeni oklar olu�turulur
            newArrow.parent = ArrowController.instance.collectedArrows.transform;    // "NewArrow" nesneleri "collectedArrows"'in alt nesnesi olur
            newArrow.GetComponent<ArrowController>().enabled = false;
            // "NewArrow" nesnesin i�inde bulundan "arrowController" scripti pasif olur ("ArrowPlayer"�n alt nesnesi olaca� i�in "ArrowPlayer" hareketini izleyecektir)
            newArrow.gameObject.AddComponent<Arrow>();  
            // Instantiate edilen her "newArrow" nesnesine "arrow" scripti eklenerek Toplanan oklar kendi i�inde ileri-geri hareketi sa�layacak script �al���r
        }
        ArrowController.instance.value = 0; 
        // Instantiate i�lemi bittikten sonra (for d�ng�s� bitti�i zaman) Temas edilen nesne say�s�n� s�f�rlar
    }
    public void DecreaseArrow(int value)
    {   // value= "ArrowPlayer"�n Temas etti�i nesnedeki de�eri al�r (��kart�lacak ok say�s�)
        // "ArrowController" den gelen value de�erine g�re "collectedArrows" nesnesi i�inden value de�eri kadar ok ��kart�l�r
        StartCoroutine(DecreaseArrowIE(value)); // IEnumerator i�lemi ile 0.1 sniye sonra �al���r
    }
    IEnumerator DecreaseArrowIE(int value)
    {   // value= "DecreaseArrow" gelen de�er
        yield return new WaitForSeconds(0.1f);

        int x = 0;  // F�ng�den tersden gitmek i�in x de�eri belirlenir ve 0 verilir
        for (int i = ArrowController.instance.collectedArrowsCount - 1; i >= 0; i--)
        {
            Destroy(ArrowController.instance.collectedArrows.transform.GetChild(i).gameObject);
            // "ArrowController"dan gelen value de�eri kadar nesneyi "collectedArrows" nesnesine en son eklenen alt nesnelerden ba�layarak yok eder          
            x++;    // Her nesne yok edildi�inde x de�eri artar
            if (x == value) 
                break;  // x de�eri ��kar�lacak ok say�s�na e�it oldu�unda d�ng�den ��k�l�r
        }

        ArrowController.instance.value = 0;
        // Destroy i�lemi bittikten sonra (for d�ng�s� bitti�i zaman) Temas edilen nesne say�s�n� s�f�rlar
    }
    public void AddCoin(int value)
    {   // Alt�n de�eri her "AddCoin" fonksiyonu �al��t���nda ald��� de�er kadar artar
        // value= "enemy" nesnesine temas edildi�i an "enemy" sciripti �zerinden gelen de�er
        totalCoin += value; // Toplam alt�n de�eri verisine gelen de�er kadar alt�n ekler
        levelCoin += value; // Bonus Alt�n kazanmak i�in leveldeki toplanan alt�n bilgisi
        PlayerPrefs.SetInt("TotalCoin", totalCoin); // Toplam alt�n de�erini "TotalCoin" indexinde kay�t eder
        coinTxt.text = totalCoin.ToString();    // Toplam alt�n de�eri ekran�n sa� �st k��esine g�sterir
    }
    public void RestartGame()
    {   // Fonksiyon her �al��t���nda o andaki sanhe yeniden ba�lat�l�r
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void NextLevel()
    {
        //  Level bittikten sonra bir sonraki level ge�mek i�in butona bas�ld��� an �al��an fonksiyon
        if (SceneManager.GetActiveScene().buildIndex == 2)  // Son seviye ka�sa (index de�erine g�re 2) son seviye gelince ilk levele geri d�ner
        {
            SceneManager.LoadScene(0);  // Oyunun ilk sahnesinin �ndex de�erini �al��t�r�r
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            //Bir sonraki levele ge�er
        }
    }
    public void BonusCoinButton()
    {
        // Bonus alt�n kazanmak i�in bonus butonuna bas�ld���nda �al��an fonksiyon
        // Reklam kodlar� vs.
        if (PlayerPrefs.HasKey("TotalCoin"))    // Kay�tl� tulutlan "TotalCoin" indexinde veri var m� kontrol eder - varsa
        {
            PlayerPrefs.SetInt("TotalCoin",(totalCoin+(bonusCoin-levelCoin)));
            // Varsa kay�tl� tulutlan "TotalCoin" indexine "totalCoin" + "bonusCoin" de�erleri toplanarak eklenir
        }
        NextLevel();    // Bir sonraki level ge�i� fonksiyonu �al���r
    }
    public void NextlevelPanel()
    {
        // "ArrowPlayer" nesnesi finish alan�na geldi�inde �al��an fonksiyon
        StartCoroutine(NextlevelPanelIE()); // Belli bir s�re sonra �al��mas� i�in IEnumerator kullan�l�r
    }
    IEnumerator NextlevelPanelIE()
    {
        // "NextlevelPanel" �zerinden �al��an fonksiyon "NextlevelPanel" �al��t�ktan 5f s�re sonra �al���r
        yield return new WaitForSeconds(5f);
        nextLevelPanel.SetActive(true);    // Finish alan�nda enemy nesneleri vurulduktan sonra ��kan "nextLevelPanel" panelini a�ar
        bonusCoin = (levelCoin * 3);
        // bonusCoin de�eri Levelde toplanan toplam alt�n de�erinin 3 kat� olarak ekrana g�sterilir
        // E�er Bonus kazanmak istenirse X3 butonuna t�klanarak level ge�ilir
        // Normal altn de�eri kazanmak isteniyorsa normal butona t�klanarak level ge�ilir
        levelCoinTxt.text = levelCoin.ToString();   // Levelde toplanan alt�n de�erini leevel ge�i� butonunda g�sterir
        bonusCoinTxt.text = bonusCoin.ToString();   // Bonus iile birlikte toplanan alt�n de�erini level ge�i� butonunda g�sterir
        yield return new WaitForSeconds(2f);    // "nextLevelPanel" a��ld�ktan 2 saniye sonra i�leme girer
        Time.timeScale = 0f;    // Oyun sahnesindeki zaman� bir sonraki levele kadar duraklat�r
    }
}
