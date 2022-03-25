using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType
    {
        FIRE,
        WATER,
        GRASS,
        EARTH,
        LIGHT
    }

    public ItemType itemType;
    public Vector3 slotPosition;
    public int points;
    public float fallingSpeed = 20.0f;
    public float swappingSpeed = 5.0f;

    private Animator _animator;
    private readonly float _topPosition = 10.0f;
    private readonly int _hashIdle = Animator.StringToHash("Idle");
    private readonly int _hashHint = Animator.StringToHash("Hint");
    private readonly int _hashMatch3 = Animator.StringToHash("Match3");

    private bool _isFalling;
    private bool _isMoving;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Update()
    {
        if (_isFalling)
        {
            transform.position = Vector3.MoveTowards(transform.position, slotPosition, fallingSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, slotPosition) < 0.001f)
            {
                _isFalling = false;
                Idle();
            }
        }

        if (_isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, slotPosition, swappingSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, slotPosition) < 0.001f)
            {
                _isMoving = false;
            }
        }
        
    }

    public void FallFromTop()
    {
        transform.position = new Vector3(slotPosition.x, _topPosition);
        _isFalling = true;
    }

    public void MoveToSlot(Vector3 slot)
    {
        slotPosition = slot;
        _isMoving = true;
    }

    public void Idle()
    {
        _animator.Play(_hashIdle);
    }
    
    public void Hint()
    {
        _animator.Play(_hashHint);
    }
    
    public void Match3()
    {
        _animator.Play(_hashMatch3);
    }

    public void DestroyItem()
    {
        Destroy(this);
    }
}
