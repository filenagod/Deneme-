using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackController : MonoBehaviour
{
    [SerializeField] Weapon currentWeapon;

    private Transform mainCamera;
    private Animator anim;

    private bool isAttack = false;

    private void Awake()
    {
        mainCamera = GameObject.FindWithTag("CameraPoint").transform;
        anim = mainCamera.transform.GetChild(0).GetComponent<Animator>();
        if(currentWeapon != null)
        {
            SpawnWeapon();
        }
        
    }
    
    // Update is called once per frame
    void Update()
    {
        Attack();
    }

    private void Attack()
    {
        if (Mouse.current.leftButton.isPressed && !isAttack)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private void SpawnWeapon()
    {
        if(currentWeapon == null)
        {
            return;
        }
        currentWeapon.SpawnNewWeapon(mainCamera.transform.GetChild(0).GetChild(0), anim);
    }

    public void EquipWeapon(Weapon weaponType)
    {
        if(currentWeapon != null)
        {
            currentWeapon.Drop();
        }
        currentWeapon = weaponType;
        SpawnWeapon();
    }

    private IEnumerator AttackRoutine()
    {
        isAttack = true;
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(currentWeapon.GetAttackRate);
        isAttack = false;
    }

    public int GetDamage()
    {
        if(currentWeapon != null)
        {
            return currentWeapon.GetDamage;
        }
        return 0;
    }
}
