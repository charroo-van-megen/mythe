using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody Player;
    
    [SerializeField] private float playerSpeed = 100f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
        /// WALKING
        
        if(Input.GetKey(KeyCode.W))
        {
            Player.AddForce(transform.forward *  playerSpeed,ForceMode.Force);
  
        }

        if (Input.GetKey(KeyCode.A))
        {
            Player.AddForce(transform.right * -playerSpeed, ForceMode.Force);
        }

        if (Input.GetKey(KeyCode.S))
        {
            Player.AddForce(transform.forward * -playerSpeed, ForceMode.Force);
        }

        if (Input.GetKey(KeyCode.D))
        {
            Player.AddForce(transform.right * playerSpeed, ForceMode.Force);
        }
        
        /// SPEEDCAP
        
        if(playerSpeed >= 10)
        {
            playerSpeed = -10;
        }
    }
}
