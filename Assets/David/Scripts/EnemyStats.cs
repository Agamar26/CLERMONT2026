using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyStats : MonoBehaviour
{
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float baseSpeed = 3.5f;

    public float MaxHealth => maxHealth;
    public float Health { get; private set; }
    public float BaseSpeed => baseSpeed;
    public float Speed => agent.speed;
    public bool IsDead => Health <= 0f;

    public event Action<EnemyStats> OnDeath;

    private NavMeshAgent agent;
    private readonly List<float> modificateurs = new List<float>();

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        Health = maxHealth;
        RecalculerVitesse();
    }

    // --- Vie ---

    public void TakeDamage(float amount)
    {
        if (IsDead || amount <= 0f) return;

        Health = Mathf.Max(Health - amount, 0f);
        if (IsDead) Die();
    }

    public void Heal(float amount)
    {
        if (IsDead || amount <= 0f) return;
        Health = Mathf.Min(Health + amount, maxHealth);
    }

    public void SetMaxHealth(float value, bool refill = false)
    {
        maxHealth = Mathf.Max(value, 1f);
        Health = refill ? maxHealth : Mathf.Min(Health, maxHealth);
    }

    // --- Vitesse ---

    // Change la vitesse de base (permanent)
    public void SetBaseSpeed(float value)
    {
        baseSpeed = Mathf.Max(value, 0f);
        RecalculerVitesse();
    }

    // Effet temporaire : 0.5 = ralenti de moitié, 2 = deux fois plus rapide, 0 = gelé
    public void ModifySpeed(float factor, float duration)
    {
        StartCoroutine(EffetVitesse(Mathf.Max(factor, 0f), duration));
    }

    private IEnumerator EffetVitesse(float factor, float duration)
    {
        modificateurs.Add(factor);
        RecalculerVitesse();

        yield return new WaitForSeconds(duration);

        modificateurs.Remove(factor);
        RecalculerVitesse();
    }

    private void RecalculerVitesse()
    {
        float v = baseSpeed;
        foreach (float m in modificateurs) v *= m;
        agent.speed = v;
    }

    // --- Mort ---

    private void Die()
    {
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }
}