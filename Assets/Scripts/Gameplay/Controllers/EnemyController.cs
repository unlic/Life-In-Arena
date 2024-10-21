using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    [SerializeField] private DrawRing drawRing;
    private NavMeshAgent navMeshAgent;
    private List<CharacterBase> targets;
    private CharacterBase currentTarget;
    private Vector3 pointToMove;
    Vector3 oldPoint = new Vector3();
    private NavMeshPath navMeshPath;

    private bool showDistanceAttackRing = false;
    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshPath = new NavMeshPath();
        enemy.EnemUnitTakeDamage += GetAttackerUnit;
        enemy.OnDie += Die;
        currentTarget = FindClosestTarget();
        if (currentTarget != null)
        {
            navMeshAgent.SetDestination(currentTarget.transform.position);
        }

        drawRing.SetRadius(enemy.CurrentUserStats.AttackDistance - 0.5f);

        drawRing.gameObject.SetActive(showDistanceAttackRing);

        StartCoroutine(TakePointToMove());
        StartCoroutine(UpdatePoint());
    }

    private void GetAttackerUnit(IDamageable attacker)
    {
        if (attacker == null) return;
        oldPoint = new Vector3();
        CharacterBase attackerCharacter = attacker as CharacterBase;

        if (currentTarget == attackerCharacter) return;

        Debug.Log(attacker);

        if (currentTarget != null && currentTarget.IsMovingToPosition)
        {
            currentTarget = attackerCharacter;
        }
        else
        {
            currentTarget = FindClosestTarget();
        }
    }
    private IEnumerator UpdatePoint()
    {
        while (true) 
        { 
            yield return new WaitForSeconds(15f);
            oldPoint = new Vector3();
        }
    
    }
    private IEnumerator TakePointToMove()
    {
        while (true) 
        {
            if (enemy.IsDie)
                yield break;

            if (targets == null || targets.Count == 0) yield break;

            if (currentTarget == null || currentTarget.IsDie)
            {
                currentTarget = FindClosestTarget();
                if (currentTarget == null) yield break;
            }

            yield return new WaitForSeconds(0.1f);

            if (currentTarget != null)
            {
                float distanceToPoint = Vector3.Distance(transform.position, pointToMove);
                float distanceFromPointToTarget = Vector3.Distance(pointToMove, currentTarget.transform.position);


                if (distanceToPoint > enemy.CurrentUserStats.AttackDistance)
                {
                    if (distanceFromPointToTarget > enemy.CurrentUserStats.AttackDistance)
                    {
                        GetPointToMove();
                    }
                    if (oldPoint != pointToMove)
                    {
                        ChangeEnemyState(pointToMove, null);
                        oldPoint = pointToMove;
                    }

                    float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
                    if (distanceToTarget < enemy.CurrentUserStats.AttackDistance)
                    {
                        ChangeEnemyState(null, currentTarget);
                    }
                }
                else
                {
                    float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
                    if (distanceToTarget < enemy.CurrentUserStats.AttackDistance)
                    {
                        ChangeEnemyState(null, currentTarget);
                    }
                    else
                    {
                        pointToMove = currentTarget.transform.position;
                        ChangeEnemyState(pointToMove, currentTarget);
                    }
                }
            }
        }
    }
    private void ChangeEnemyState(Vector3? pointToMove, CharacterBase target = null)
    {
        if (pointToMove == null)
        {
            enemy.ChangeState(new AttackState(enemy, target));
        }
        else
        {
            Vector3 moveTo = pointToMove.Value;
            enemy.ChangeState(new MovingState(enemy, moveTo, target));
        }
    }
    private void GetPointToMove()
    {
        bool pointIsFind = false;
        int count = 0;
        while (!pointIsFind)
        {
            NavMeshHit hit;
            NavMesh.SamplePosition(Random.insideUnitSphere * (enemy.CurrentUserStats.AttackDistance) + currentTarget.transform.position, out hit, enemy.CurrentUserStats.AttackDistance, NavMesh.AllAreas);
            pointToMove = hit.position;
            if (pointToMove.y != Mathf.Infinity)
            {
                navMeshAgent.CalculatePath(pointToMove, navMeshPath);
                if (navMeshPath.status == NavMeshPathStatus.PathComplete && !NavMesh.Raycast(currentTarget.transform.position, pointToMove, out hit, NavMesh.AllAreas))
                {
                    pointIsFind = true;
                }
            }
        }
    }

    private CharacterBase FindClosestTarget()
    {
        targets = CharacterManager.Instance.GetEnemies(enemy);

        if (targets.Count == 0) return null;

        if (currentTarget != null)
        {
            currentTarget.OnDie -= TargetDie;
        }

        CharacterBase closestTarget = null;
        float shortestDistance = Mathf.Infinity;

        foreach (var target in targets)
        {
            if (target == null) continue;

            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

            if (distanceToTarget < shortestDistance)
            {
                shortestDistance = distanceToTarget;
                closestTarget = target;
            }
        }

        if (closestTarget != null)
        {
            closestTarget.OnDie += TargetDie;
        }

        return closestTarget;
    }
    private void UnsubscribeTargetEvents()
    {
        if (currentTarget != null)
        {
            currentTarget.OnDie -= TargetDie;
            currentTarget = null;
        }
    }

    private void TargetDie()
    {
        UnsubscribeTargetEvents();
        currentTarget = FindClosestTarget();
    }
    private void Die()
    {
        Destroy(this);
    }

    private void OnDestroy()
    {
        UnsubscribeTargetEvents();
        enemy.EnemUnitTakeDamage -= GetAttackerUnit;
        enemy.OnDie -= Die;
    }
}
