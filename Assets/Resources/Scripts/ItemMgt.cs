using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMgt : MonoBehaviour {

    enum DepthZone { SURFACE, LOW, MIDDLE, DEEP};
    [SerializeField] private int m_Weight;
    [SerializeField] private int m_Value;
    [SerializeField] private DepthZone m_DepthLevel;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //GameObject explosionClone;
            GameObject scoreCounter = GameObject.Find("ScoreCounter");
            if (scoreCounter != null)
            {
                scoreCounter.GetComponent<ScoreCounter>().IncreaseScore(m_Value);
            }
            else
            {
                Debug.LogError(gameObject.name + " : scoreCounter not found");
            }

            this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            this.gameObject.GetComponent<Collider>().enabled = false;

            //audioManager.PlaySound("Coin");

            //explosionClone = Instantiate(m_ExplosionCoin, transform.position, Quaternion.identity);
            //explosionClone.transform.SetParent(GameObject.Find("Coins").transform);
            //Destroy(m_Aura);
            //Destroy(explosionClone, 1f);
            Destroy(this.gameObject, 1f);
        }
    }
}
