﻿using UnityEngine;
using System.Collections;

// CODING STANDARDS / CONVENTIONS  
// Static fields : UpperCamelCase
// Public fields : UpperCamelCase
// Private fields : lowerCamelCase
// Functions / Methodes : UpperCamelCase
// fields created in a methods : _lowerCamelCase

public class CharacterBehaviour : MonoBehaviour {

    //-------------------------------------------------//
    //------------------- VARIABLES -------------------//
    //-------------------------------------------------//
    //--------------- RIVATE VARIABLES ----------------//

    private Rigidbody2D rigid;

    //--------------- PUBLIC VARIABLES ----------------//
  
    public float Speed = 15; // Speed of the player (control)
    public float BrownianIntensity = 0.1f; // Intensity of the random movement

    private bool isStuck; // Don't Get or Set isStuck -> use IsStuck
    public bool IsStuck
    {
        get
        {
            return isStuck;
        }
        set
        {
            isStuck = value;
            if (value == true)
                FreezeMovement();
            else
                RestoreMovement();
        }
    }

    //-------------------------------------------------//

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
	
	void FixedUpdate()
    {
        // Take the inputs
        Vector2 movDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (movDirection.magnitude > 1)
            movDirection.Normalize();

        // Create random movement
        Vector2 _brownian = GetBrownianMovement();

        // Move the player + add brownian movement
        Move(movDirection + _brownian);
    }

    /// <summary>
    /// Takes a Vector2 and add forces to the rigidbody2D
    /// </summary>
    void Move(Vector2 dir)
    {
        rigid.AddForce(dir * Time.fixedDeltaTime * Speed * 1000);
    }

    /// <summary>
    /// Adds random noise to the movement
    /// </summary>
    Vector2 GetBrownianMovement()
    {
        Vector2 _movement = new Vector2();
        float _noise = Mathf.PerlinNoise(Time.time, Time.time);
        _movement += new Vector2(Random.Range(-_noise, _noise), Random.Range(-_noise, _noise));

        return _movement * BrownianIntensity;
    }

    private void FreezeMovement()
    {
        rigid.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private void RestoreMovement()
    {
        // Bitwise thingy
        //rigid.constraints &= ~RigidbodyConstraints2D.FreezePositionX; // bitwise NOT
        //rigid.constraints &= ~RigidbodyConstraints2D.FreezePositionY; // for unfreeze

        rigid.constraints = RigidbodyConstraints2D.None;
    }
}