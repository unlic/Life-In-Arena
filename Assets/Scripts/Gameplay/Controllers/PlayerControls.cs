using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] private CharacterBase controlledUnit;
    [SerializeField] private UIController UIController;
    private Camera mainCamera;
    private NavMeshAgent navMeshAgent;

    private const float doubleClickTimeLimit = 0.3f;
    private float lastClickTime = 0f;
    private bool isSingleClick = false;
    private bool abilityInProgress = false;
    private bool isCameraDragging = false;
    private float singleClickDelay = 0.2f;

    private Vector3 endPosition;
    private void Start()
    {
        mainCamera = Camera.main;
        
        if(controlledUnit != null)
        {
            navMeshAgent = controlledUnit.GetComponent<NavMeshAgent>();
            UIController.SetDataInView(controlledUnit);
        }
            
    }

    private void Update()
    {
        HandleMouseInput();
    }
    private void HandleMouseInput()
    {
        if(IsPointerOverUIObject())
            return;


        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time - lastClickTime < doubleClickTimeLimit)
            {
                DoubleClick();
                isSingleClick = false;
            }
            else
            {
                isSingleClick = true;
                isCameraDragging = false;
                StartCoroutine(SingleClickDelay());
            }

            lastClickTime = Time.time;
        }
    }
    private void DoubleClick()
    {
        RaycastHit hit;
        if (TryGetHitTarget(out hit))
        {
            endPosition = hit.point;

            var target = hit.collider?.GetComponent<CharacterBase>();
            if (target == null) return;

            controlledUnit = target;

            navMeshAgent = controlledUnit.GetComponent<NavMeshAgent>();
            if (navMeshAgent == null)
            {
                return;
            }

            UIController.SetDataInView(controlledUnit);
        }
    }

    private void MoveTo()
    {
        if (navMeshAgent == null)
            return;

        RaycastHit hit;
        if (TryGetHitTarget(out hit))
        {
            endPosition = hit.point;
            controlledUnit.ClearAttackTarget();
            if (hit.collider != null)
            {
                controlledUnit.SetHasReceivedNewCommand(true);

                var target = hit.collider.GetComponent<IDamageable>();

                if (target != null && ((MonoBehaviour)target) != controlledUnit)
                {
                    controlledUnit.CurrentTarget = target;
                    controlledUnit.IsMovingToEnemy = true;
                    controlledUnit.ChangeState(new AttackState(controlledUnit, target));
                }
                else
                {
                    controlledUnit.IsMovingToEnemy = false;
                    controlledUnit.ChangeState(new MovingState(controlledUnit, endPosition));
                }
            }
        }
    }
    public void SetCameraDragging(bool isDragging)
    {
        isCameraDragging = isDragging;
    }
    public void UseAbility(CharacterBase unit, Ability ability)
    {
        abilityInProgress = true;
        RaycastHit hit;
        if (TryGetHitTarget(out hit))
        {
            if (hit.collider != null)
            {
                var target = hit.collider.GetComponent<CharacterBase>();

                unit.ChangeState(new AbilityCastState(unit, ability, target, hit.point));
            }
        }

        Debug.Log("UseAbility " + ability.name);
    }
    private bool TryGetHitTarget(out RaycastHit hit)
    {
        return Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit);
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        return results.Count > 0;
    }
    private IEnumerator SingleClickDelay()
    {
       
        yield return new WaitForSeconds(singleClickDelay);

        if (isSingleClick&&!abilityInProgress&&!isCameraDragging)
        {
            MoveTo();
        }
        abilityInProgress = false;
        isCameraDragging = false;
    }


}