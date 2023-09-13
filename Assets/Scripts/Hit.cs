using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Hit : MonoBehaviour
{
    private Transform owner;
    private int damage;
    private Collider hitcollider;
    private Rigidbody rb;
    private Animator animator;
    private void Awake()
    {
        owner = transform.root;
        hitcollider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        hitcollider.isTrigger = true;
        rb.isKinematic = true;
        rb.useGravity = false;
        hitcollider.enabled = false;

    }
    void Start()
    {
        if (owner.tag == "Player")
        {
           damage = owner.GetComponent<AttackController>().GetDamage();
            animator = GetComponentInChildren<Transform>().GetComponentInParent<Animator>();
        }
        else if (owner.tag == "Enemy")
        {
           damage = owner.GetComponent<EnemyConroller>().GetDamage();
            animator = GetComponentInParent<Animator>();
        }
        else
        {
            enabled = false;
        }
    }
    private void Update()
    {
        if (!animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")
            && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f 
            && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.55f) 
        {
            ControlTheCollider(true);
        }
        else
        {
            ControlTheCollider(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Health health = other.GetComponent<Health>();
        if(health != null && health.gameObject != owner.gameObject)
        {
            health.GiveDamage(damage);
        }
    }
    private void ControlTheCollider(bool open)
    {
        hitcollider.enabled = open;
    }
}
