using System;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    public event Action<Interactable> TargetChanged;

    public Interactable Target { get; private set; }
    public bool Interact { get; set; }
    public float InteractionHoldingTime { get; private set; }

    [field: SerializeField]
    public LayerMask TargetLayers { get; set; }

    [field: SerializeField]
    public LayerMask ObstacleLayers { get; set; }

    private bool _isReadyToInteract;
    private bool _isTargetRangeOut;

    private void LateUpdate()
    {
        if (Target == null)
        {
            return;
        }

        if (!Target.gameObject.activeSelf)
        {
            SetTarget(null);
            return;
        }

        if (Target.IsInteracted)
        {
            return;
        }

        if (_isTargetRangeOut)
        {
            SetTarget(null);
            return;
        }

        if (Interact)
        {
            if (_isReadyToInteract && Target.CanInteract)
            {
                if (InteractionHoldingTime < Target.InteractionHoldTime)
                {
                    InteractionHoldingTime += Time.deltaTime;
                }
                else
                {
                    InteractionHoldingTime = 0f;
                    Target.Interact();
                }
            }
        }
        else
        {
            InteractionHoldingTime = 0f;
            _isReadyToInteract = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.IsInLayerMask(TargetLayers))
        {
            return;
        }

        if (Target == null)
        {
            SetNotNullTarget(other);
        }
        else
        {
            if (Target.IsInteracted)
            {
                return;
            }

            if (Target.gameObject != other.gameObject)
            {
                float distanceToTarget = Vector3.Distance(transform.position, Target.transform.position);
                float distanceToOther = Vector3.Distance(transform.position, other.transform.position);
                if (distanceToTarget <= distanceToOther)
                {
                    return;
                }

                if (Physics.Linecast(transform.position, other.transform.position, ObstacleLayers))
                {
                    return;
                }

                SetNotNullTarget(other);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.IsInLayerMask(TargetLayers))
        {
            return;
        }

        if (Target.gameObject != other.gameObject)
        {
            return;
        }

        if (Target.IsInteracted)
        {
            _isTargetRangeOut = true;
        }
        else
        {
            SetTarget(null);
        }
    }

    private void SetNotNullTarget(Collider collider)
    {
        if (collider.TryGetComponent<Interactable>(out var interactable))
        {
            SetTarget(interactable);
        }
        else
        {
            Debug.LogWarning($"{collider.name} has no Interactable component.");
        }
    }

    private void SetTarget(Interactable target)
    {
        if (Target == target)
        {
            return;
        }

        if (Target != null)
        {
            Target.Undetected();
        }

        Target = target;
        InteractionHoldingTime = 0f;
        _isReadyToInteract = false;
        _isTargetRangeOut = false;

        if (target != null)
        {
            target.Detected();
        }

        TargetChanged?.Invoke(target);
    }
}
