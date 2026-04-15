using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform playerLocation;
    private float moveSpeed = 0.001f;

    // Update is called once per frame
    void Update()
    {
        // print("In Update");
        Vector3 newPosition = Vector3.MoveTowards(transform.localPosition, playerLocation.localPosition, moveSpeed);
        transform.localPosition = newPosition;

        if(newPosition == playerLocation.localPosition)
        {
            Destroy(playerLocation.gameObject);
        }
    }

    //When it touches player, kill them
}
