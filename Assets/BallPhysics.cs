using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPhysics : MonoBehaviour
{
    public float m = 1;                                     //masa
    public Vector2 v = new Vector2(0,0);                    //prêdkoœæ
    public float g = 9.81f;                                 //sta³a przyspieszenia
    public float u = 0.5f;                                  //wspó³czynnik tarcia kinetycznego
    Vector2 p;                                              //pozycja
    List<GameObject> vUpdated = new List<GameObject>();     //lista kul z obs³u¿onymi kolizjami

    void Start()
    {
        p = transform.position;
    }

    void FixedUpdate()
    {
        if (v.magnitude < 0.01)
            v = Vector2.zero;

        if (v.magnitude > 0)    //opór tarcia
        {
            Vector2 fO = -m * g * u * v.normalized;
            v += fO * Time.fixedDeltaTime;
        }

        p += v * Time.fixedDeltaTime;

        transform.position = p;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Table")          //kolizja z krawêdzi¹ sto³u
        {
            Vector2 collisionVector = ((Vector2)transform.position - collision.GetContact(0).point).normalized;
            v = v - (2 * Vector2.Dot(v, collisionVector) / (collisionVector.magnitude * collisionVector.magnitude)) * collisionVector;
        }
        else if (collision.collider.tag == "Ball")      //kolizja z innymi kulami
        {
            if (!IsVelocityUpdated(collision.collider.gameObject))  //je¿eli kolizja nie zosta³a obs³u¿ona aktualizowana jest prêdkoœæ obu kul
            {
                BallPhysics cB = collision.collider.GetComponent<BallPhysics>();
                Vector2 tempV;

                tempV = v - (2 * cB.m / (m + cB.m)) * Vector2.Dot(v - cB.v, p - cB.p) /
                    ((p - cB.p).magnitude * (p - cB.p).magnitude) * (p - cB.p);

                cB.v = cB.v - ((2 * m) / (cB.m + m)) * Vector2.Dot(cB.v - v, cB.p - p) /
                    ((cB.p - p).magnitude * (cB.p - p).magnitude) * (cB.p - p);

                v = tempV;

                cB.VelocityUpdated(gameObject);
            }
            else
            {
                vUpdated.Remove(collision.collider.gameObject);
            }

        }
    }

    public void VelocityUpdated(GameObject ball)
    {
        vUpdated.Add(ball);
    }

    bool IsVelocityUpdated(GameObject ball)
    {
        if(vUpdated.Contains(ball))
            return true;
        return false;
    }
}

