using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] Transform arrowPrefab; // Eklenecek oklar için seçilen perefab
    [SerializeField] GameObject nextLevelPanel; // Oyun sonu çýkan next level butonlarýnýn tutulduðu nesne
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
        Time.timeScale = 1f;    // Oyun baþladýpýnda time.scale deðrini 1 yapar
        int gameLevel = SceneManager.GetActiveScene().buildIndex + 1;   // Oyun sahnesindeki leveli "gameLevel" deðiþkeinne atar
        levelTxt.text = "Level " + gameLevel;   // "gameLevel" deki leveli ekrana yazar
        //PlayerPrefs.DeleteAll();    // Tüm skorlarý sýfýrlar
        nextLevelPanel.SetActive(false);    // Oyun açýlýndýðýnda "nextLevelPanel" panelini kapatýr

        if (PlayerPrefs.HasKey("TotalCoin"))    // Kayýtlý tulutlan "TotalCoin" indexinde veri var mý kontrol eder - varsa
        {
            totalCoin = PlayerPrefs.GetInt("TotalCoin");
            // Varsa kayýtlý tulutlan "TotalCoin" indexsindeki skor verisini "totalCoin" atar
            coinTxt.text = totalCoin.ToString();    // "totalCoin" deðerini ekranda gösterir
        }
        else
        {
            PlayerPrefs.SetInt("TotalCoin", 0);     // Kayýtlý tulutlan "TotalCoin" indexsinde veri yoksa "TotalCoin" index deðerini 0 yapar
            coinTxt.text = PlayerPrefs.GetInt("TotalCoin").ToString();  // "TotalCoin" indexsindeki deðerini ekranda gösterir
        }

    }
    private void Update()
    {
        if (ArrowController.instance.collectedArrowsCount >= 1)
        {
            collectedArrowsTxt.text = ArrowController.instance.collectedArrowsCount.ToString();
            // Toplanan ok sayýlarýný ekranda gösterir
        }
    }
    public void IncreaseArrow(int value)
    {   // value= "ArrowPlayer"ýn Temas ettiði nesnedeki deðeri alýr (Eklenecek ok sayýsý)
        // "ArrowController" den gelen value deðerine göre "collectedArrows" nesnesi içine value deðeri kadar ok ekler
        StartCoroutine(IncreaseArrowIE(value)); //IEnumerator iþlemi ile 0.1 sniye sonra çalýþýr
    }
    IEnumerator IncreaseArrowIE(int value)
    {   // value= "IncreaseArrow" gelen deðer
        float x = ArrowController.instance.collectedArrows.GetComponent<CapsuleCollider>().radius;
        // oklarýn tutulduðu "collectedArrows" nesnesinin CapsuleCollider çapýnýn deððerini alýr       
        x += value * 0.0035f;   // Eklencek oklarýn daha geniþ bir çapa girmesi için her ok toplandýðýnda tplanan ok kadar çap deðeri büyür
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < value; i++)
        {
            Vector3 randomPoint= Random.insideUnitCircle * x;   // Belirlenen yarýçap içinde oklarýn radnom pozisyonlarý belirlenir
            Vector3 pos = new Vector3(
                ArrowController.instance.collectedArrows.transform.position.x +randomPoint.x,
                ArrowController.instance.collectedArrows.transform.position.y +randomPoint.y,
                ArrowController.instance.collectedArrows.transform.position.z);
            // Instantiate edilecek olan oklarýn hangi posizyonda doðacaðý belirlenir

            newArrow = Instantiate(arrowPrefab, pos, Quaternion.identity);  
            // for döngüsü devam ettikçe pos deðiþkenin aldýðý pozisyon deðerinde yeni oklar oluþturulur
            newArrow.parent = ArrowController.instance.collectedArrows.transform;    // "NewArrow" nesneleri "collectedArrows"'in alt nesnesi olur
            newArrow.GetComponent<ArrowController>().enabled = false;
            // "NewArrow" nesnesin içinde bulundan "arrowController" scripti pasif olur ("ArrowPlayer"ýn alt nesnesi olacaý için "ArrowPlayer" hareketini izleyecektir)
            newArrow.gameObject.AddComponent<Arrow>();  
            // Instantiate edilen her "newArrow" nesnesine "arrow" scripti eklenerek Toplanan oklar kendi içinde ileri-geri hareketi saðlayacak script çalýþýr
        }
        ArrowController.instance.value = 0; 
        // Instantiate iþlemi bittikten sonra (for döngüsü bittiði zaman) Temas edilen nesne sayýsýný sýfýrlar
    }
    public void DecreaseArrow(int value)
    {   // value= "ArrowPlayer"ýn Temas ettiði nesnedeki deðeri alýr (Çýkartýlacak ok sayýsý)
        // "ArrowController" den gelen value deðerine göre "collectedArrows" nesnesi içinden value deðeri kadar ok çýkartýlýr
        StartCoroutine(DecreaseArrowIE(value)); // IEnumerator iþlemi ile 0.1 sniye sonra çalýþýr
    }
    IEnumerator DecreaseArrowIE(int value)
    {   // value= "DecreaseArrow" gelen deðer
        yield return new WaitForSeconds(0.1f);

        int x = 0;  // Föngüden tersden gitmek için x deðeri belirlenir ve 0 verilir
        for (int i = ArrowController.instance.collectedArrowsCount - 1; i >= 0; i--)
        {
            Destroy(ArrowController.instance.collectedArrows.transform.GetChild(i).gameObject);
            // "ArrowController"dan gelen value deðeri kadar nesneyi "collectedArrows" nesnesine en son eklenen alt nesnelerden baþlayarak yok eder          
            x++;    // Her nesne yok edildiðinde x deðeri artar
            if (x == value) 
                break;  // x deðeri çýkarýlacak ok sayýsýna eþit olduðunda döngüden çýkýlýr
        }

        ArrowController.instance.value = 0;
        // Destroy iþlemi bittikten sonra (for döngüsü bittiði zaman) Temas edilen nesne sayýsýný sýfýrlar
    }
    public void AddCoin(int value)
    {   // Altýn deðeri her "AddCoin" fonksiyonu çalýþtýðýnda aldýðý deðer kadar artar
        // value= "enemy" nesnesine temas edildiði an "enemy" sciripti üzerinden gelen deðer
        totalCoin += value; // Toplam altýn deðeri verisine gelen deðer kadar altýn ekler
        levelCoin += value; // Bonus Altýn kazanmak için leveldeki toplanan altýn bilgisi
        PlayerPrefs.SetInt("TotalCoin", totalCoin); // Toplam altýn deðerini "TotalCoin" indexinde kayýt eder
        coinTxt.text = totalCoin.ToString();    // Toplam altýn deðeri ekranýn sað üst köþesine gösterir
    }
    public void RestartGame()
    {   // Fonksiyon her çalýþtýðýnda o andaki sanhe yeniden baþlatýlýr
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void NextLevel()
    {
        //  Level bittikten sonra bir sonraki level geçmek için butona basýldýðý an çalýþan fonksiyon
        if (SceneManager.GetActiveScene().buildIndex == 2)  // Son seviye kaçsa (index deðerine göre 2) son seviye gelince ilk levele geri döner
        {
            SceneManager.LoadScene(0);  // Oyunun ilk sahnesinin Ýndex deðerini çalýþtýrýr
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            //Bir sonraki levele geçer
        }
    }
    public void BonusCoinButton()
    {
        // Bonus altýn kazanmak için bonus butonuna basýldýðýnda çalýþan fonksiyon
        // Reklam kodlarý vs.
        if (PlayerPrefs.HasKey("TotalCoin"))    // Kayýtlý tulutlan "TotalCoin" indexinde veri var mý kontrol eder - varsa
        {
            PlayerPrefs.SetInt("TotalCoin",(totalCoin+(bonusCoin-levelCoin)));
            // Varsa kayýtlý tulutlan "TotalCoin" indexine "totalCoin" + "bonusCoin" deðerleri toplanarak eklenir
        }
        NextLevel();    // Bir sonraki level geçiþ fonksiyonu çalýþýr
    }
    public void NextlevelPanel()
    {
        // "ArrowPlayer" nesnesi finish alanýna geldiðinde çalýþan fonksiyon
        StartCoroutine(NextlevelPanelIE()); // Belli bir süre sonra çalýþmasý için IEnumerator kullanýlýr
    }
    IEnumerator NextlevelPanelIE()
    {
        // "NextlevelPanel" üzerinden çalýþan fonksiyon "NextlevelPanel" çalýþtýktan 5f süre sonra çalýþýr
        yield return new WaitForSeconds(5f);
        nextLevelPanel.SetActive(true);    // Finish alanýnda enemy nesneleri vurulduktan sonra çýkan "nextLevelPanel" panelini açar
        bonusCoin = (levelCoin * 3);
        // bonusCoin deðeri Levelde toplanan toplam altýn deðerinin 3 katý olarak ekrana gösterilir
        // Eðer Bonus kazanmak istenirse X3 butonuna týklanarak level geçilir
        // Normal altn deðeri kazanmak isteniyorsa normal butona týklanarak level geçilir
        levelCoinTxt.text = levelCoin.ToString();   // Levelde toplanan altýn deðerini leevel geçiþ butonunda gösterir
        bonusCoinTxt.text = bonusCoin.ToString();   // Bonus iile birlikte toplanan altýn deðerini level geçiþ butonunda gösterir
        yield return new WaitForSeconds(2f);    // "nextLevelPanel" açýldýktan 2 saniye sonra iþleme girer
        Time.timeScale = 0f;    // Oyun sahnesindeki zamaný bir sonraki levele kadar duraklatýr
    }
}
