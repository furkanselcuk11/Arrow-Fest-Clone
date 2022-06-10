using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject takeOutObjects;   // "Enemy" nesnesine isabet edip oklarýn toplandýðý nense
    [SerializeField] ParticleSystem coinEffect; // coin efekti

    private void Start()
    {
        coinEffect.Stop();  // "Enemy" nesnelerinin coin efekti oyun baþýnda false olur
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("pickObjects") && gameObject.CompareTag("enemy") && ArrowController.instance.collectedArrowsCount!=0 )
        {   // Eðer düþmana çarpan nesne "pickObjects" ve tagý "enemy" ve eðer toplanan oklar 0 dan büyükse
            GameManager.instance.AddCoin(10);   // Coin Score 10 ekle 
            coinEffect.Play();  // "coinEffect" efekti çalýþýr
            coinEffect.GetComponentInChildren<AudioSource>().Play();    // "coinEffect" compenentinin altýndaki altýn toplama sei çalar
            other.gameObject.transform.GetChild(ArrowController.instance.collectedArrows.transform.childCount - 1).transform.parent = takeOutObjects.transform;
            // "enemy" nesnesine temas edildikten sonra Temas edilen nesnesinin en alt nesnesi  "takeOutObjects" nesnesini alt nesnesi olur
            takeOutObjects.transform.GetChild(takeOutObjects.transform.childCount - 1).transform.position = gameObject.transform.position;
            // "enemy" nesnesine temas edildikten sonra "takeOutObjects" nesnesinin en alt nesnesinin pozisyonu Temas edilen "enemy" nesnesin pozisyonuna eþitlenir
            takeOutObjects.transform.GetChild(takeOutObjects.transform.childCount - 1).gameObject.GetComponent<Rigidbody>().useGravity = true;
            // "enemy" nesnesine temas edildikten sonra "takeOutObjects" nesnesinin en alt nesnesinin Rigidbody componenti aktif olur
            takeOutObjects.transform.GetChild(takeOutObjects.transform.childCount - 1).gameObject.GetComponent<BoxCollider>().enabled = true;
            //  "enemy" nesnesine temas edildikten sonra "takeOutObjects" nesnesinin en alt nesnesinin BoxCollider componenti aktif olur
            takeOutObjects.transform.GetChild(takeOutObjects.transform.childCount - 1).transform.rotation= Random.rotation;
            // "enemy" nesnesine temas edildikten sonra "takeOutObjects" nesnesinin en alt nesnesinin Rotasyon random deðer alarak saplanma görüntüsü oluþur
        }

        if (other.gameObject.CompareTag("pickObjects") && gameObject.CompareTag("bigEnemy") && ArrowController.instance.collectedArrowsCount != 0)
        {   // Eðer düþmana çarpan nesne "pickObjects" ve tagý "bigEnemy" ve eðer toplanan oklar 0 dan büyükse
            GameManager.instance.AddCoin(50); // Coin Score 50 ekle 
            coinEffect.Play();  // "coinEffect" efekti çalýþýr
            coinEffect.GetComponentInChildren<AudioSource>().Play();    // "coinEffect" compenentinin altýndaki altýn toplama sei çalar
            other.gameObject.transform.GetChild(ArrowController.instance.collectedArrows.transform.childCount - 1).transform.parent = takeOutObjects.transform;
            // "bigEnemy" nesnesine temas edildikten sonra temas edilen nesnesinin en alt nesnesi  "takeOutObjects" nesnesini alt nesnesi olur
            takeOutObjects.transform.GetChild(takeOutObjects.transform.childCount - 1).transform.position = gameObject.transform.position;
            // "bigEnemy" nesnesine temas edildikten sonra "takeOutObjects" nesnesinin en alt nesnesinin pozisyonu Temas edilen "bigEnemy" nesnesin pozisyonuna eþitlenir
            takeOutObjects.transform.GetChild(takeOutObjects.transform.childCount - 1).gameObject.GetComponent<Rigidbody>().useGravity = true;
            // "bigEnemy" nesnesine temas edildikten sonra "takeOutObjects" nesnesinin en alt nesnesinin Rigidbody componenti aktif olur
            takeOutObjects.transform.GetChild(takeOutObjects.transform.childCount - 1).gameObject.GetComponent<BoxCollider>().enabled = true;
            // "bigEnemy" nesnesine temas edildikten sonra "takeOutObjects" nesnesinin en alt nesnesinin BoxCollider componenti aktif olur
            takeOutObjects.transform.GetChild(takeOutObjects.transform.childCount - 1).transform.rotation = Random.rotation;
            // "bigEnemy" nesnesine temas edildikten sonra "takeOutObjects" nesnesinin en alt nesnesinin Rotasyon random deðer alarak saplanma görüntüsü oluþur
        }
        if (other.gameObject.CompareTag("Player") && ArrowController.instance.collectedArrowsCount == 0 && gameObject.CompareTag("enemy"))
        {   // Eðer düþmana çarpan nesnenin Tagý "Player" ve çarptýðý nesnenin tagý "enemy" ve eðer toplanan oklar 0 ise 
            GameManager.instance.RestartGame(); // Oyun tekrar baþlar
        }
    }
}
