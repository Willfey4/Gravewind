using UnityEngine;

public class EnemyAI : MonoBehaviour     
{
        Rigidbody2D rb;
        Movement movement;

        [SerializeField] float startDirection;

        float horMoveDirection;

        void Awake()
        {
            if(TryGetComponent<Rigidbody2D>(out rb))
            {
                Debug.Log("Rigidbody2D Script is Attached");
            }
            else
            {
                Debug.Log("No Rigidbody2D Script Attached");
            }


            if(TryGetComponent<Movement>(out movement))
            {
                Debug.Log("Movement Script is Attached");
            }
            else
            {
                Debug.Log("No Movement Script Attached");
            }
        }


        void FixedUpdate() 
        {
            movement.Move(startDirection);
        }

}
