﻿using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

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
    //--------------- PRIVATE VARIABLES ---------------//

    private Rigidbody2D rigid;
    private Slider energyBar;

    //--------------- PUBLIC VARIABLES ----------------//

    // Gravity
    private bool hasGravity = false;
    public bool HasGravity
    {
        get
        {
            return hasGravity;
        }
        set
        {
            hasGravity = value;
            if (value == true)
                rigid.gravityScale = 1;
            else
                rigid.gravityScale = 0;
        }
    }

    // Inertia
    private bool hasInertia = false;
    public bool HasInertia
    {
        get
        {
            return hasInertia;
        }
        set
        {
            hasInertia = value;
            if (value == true)
            {
                rigid.drag = 1;
            }
            else
            {
                rigid.drag = 50;
            }
        }
    }

    private float energy = 1000; // Max = 1000
    public float Energy
    {
        get
        {
            return energy;
        }
        set
        {
            energy = value;
            energyBar.value = value;
        }
    }

    public float EnergyConsumptionMultiplier = 2.0f;

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

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        energyBar = GameManager.Instance.Canvas.transform.FindChild("EnergyBar").GetComponent<Slider>();
    }

    void Update()
    {
        
    }

	void FixedUpdate()
    {
        // Take the inputs
        Vector2 movDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (movDirection.magnitude > 1)
            movDirection.Normalize();
        if(movDirection.magnitude > 0)
            Energy -= Time.fixedDeltaTime * EnergyConsumptionMultiplier;

        // Create random movement
        Vector2 _brownian = GetBrownianMovement();

        // Move the player + add brownian movement
        Move(movDirection + _brownian);
        RotateDirection(movDirection);
    }

    /// <summary>
    /// Takes a Vector2 and add forces to the rigidbody2D
    /// </summary>
    void Move(Vector2 dir)
    {
        rigid.AddForce(dir * Time.fixedDeltaTime * Speed * 1000);
    }

    void RotateDirection(Vector2 direction)
    {
        if (direction.magnitude <= 0.1f) // yep, to avoid dead zones{
        {
            transform.DOKill();
            return;
        }

        float _angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion _q = Quaternion.AngleAxis(_angle - 180, Vector3.forward);
        //transform.rotation = Quaternion.Slerp(transform.rotation, _q, 0.9f);

        transform.DORotate(_q.eulerAngles, 0.5f, RotateMode.Fast);

        //transform.DORotate(_rot, 0.25f, RotateMode.FastBeyond360);
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