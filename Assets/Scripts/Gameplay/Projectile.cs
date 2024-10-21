using UnityEngine;

public class Projectile : MonoBehaviour
{
    private MonoBehaviour target;
    private float damage;
    private CharacterBase attaker;
    [SerializeField] private float speed = 10f;

    private ProjectilePool pool;

    public void SetTarget(MonoBehaviour target, float damage, CharacterBase attaker, ProjectilePool pool)
    {
        this.target = target;
        this.damage = damage;
        this.attaker = attaker;
        this.pool = pool; 
    }

    private void Update()
    {
        if (target == null)
        {
            ReturnToPool();
            return;
        }

        Vector3 direction = (target.transform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        if (Vector3.Distance(transform.position, target.transform.position) < 0.1f)
        {
            IDamageable damageableTarget = target.GetComponent<IDamageable>();

            if (damageableTarget != null)
            {
                var hasInventory = attaker as IHasInventory;

                if (hasInventory != null)
                {
                    float criticalMultiplier = target is IImmuneToCriticalDamage ? 1 : hasInventory.CalculateCriticalMultiplier();
                    IDamageEffect damageEffect = new CriticalDamageEffect(criticalMultiplier);
                    damageEffect.ApplyEffect(attaker, target.GetComponent<IDamageable>(), damage);
                }
                else
                {
                    IDamageEffect damageEffect = new HormalDamageEffect();
                    damageEffect.ApplyEffect(attaker, target.GetComponent<IDamageable>(), damage);
                }
            }
            ReturnToPool();
        }
    }
    private void ReturnToPool()
    {
        pool.ReturnProjectile(this);
    }
}
