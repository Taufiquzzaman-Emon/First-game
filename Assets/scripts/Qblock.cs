using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Qblock : MonoBehaviour
{
    public Sprite EmptySprite;
    public float bounceHeight = 0.5f;
    public float bounceSpeed = 4f;
    public float CoinSpeed = 8f;
    public float CoinHeight = 3f;
    public float CoinFall = 2f;
    public float EnemySpeed = 2f;
    public float EnemyHeight = 3f;
    public float EnemyFall = 2f;



    private bool canBounce = true;
    private Vector2 originalPosition;


    void Start()
    {
        originalPosition = transform.localPosition;
    }

    void changeSprite()
    {
        GetComponent<Animator>().enabled = false;
        GetComponent<SpriteRenderer>().sprite = EmptySprite;
    }

    void presentCoin()
    {
        GameObject SpinningCoin = (GameObject)Instantiate(Resources.Load("Prefabs/Spinning_Coin", typeof(GameObject)));
        SpinningCoin.transform.SetParent(this.transform.parent);
        SpinningCoin.transform.localPosition = new Vector2(originalPosition.x, originalPosition.y + 1);

        StartCoroutine(MoveCoin(SpinningCoin));
    } 


   public void questionBlock()
    {
        if (canBounce)
        {
            canBounce = false;
            StartCoroutine(Bounce());
           
        }

        IEnumerator Bounce()
        {
            changeSprite();
            presentCoin();
      
            while(true)
            {
                transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y + bounceSpeed * Time.deltaTime);

                if (transform.localPosition.y >= originalPosition.y + bounceHeight)
                    break;
                yield return null;
            }

            while(true)
            {
                transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y - bounceSpeed * Time.deltaTime);
                if(transform.localPosition.y<=originalPosition.y)

                {
                    transform.localPosition = originalPosition;
                    break;
                }
                yield return null;
            }
        }
    }


    IEnumerator MoveCoin (GameObject Coin)
    {
        while(true)
        {
            Coin.transform.localPosition = new Vector2(Coin.transform.localPosition.x, Coin.transform.localPosition.y + CoinSpeed * Time.deltaTime);
            if (Coin.transform.localPosition.y >= originalPosition.y + CoinHeight + 1)
                break;
            yield return null;
        }
        while(true)
        {
            Coin.transform.localPosition = new Vector2(Coin.transform.localPosition.x, Coin.transform.localPosition.y - CoinSpeed * Time.deltaTime);
            if (Coin.transform.localPosition.y <= originalPosition.y + CoinFall)
                break;
            Destroy(Coin.gameObject);
            yield return null;
        }
    }

   

}
