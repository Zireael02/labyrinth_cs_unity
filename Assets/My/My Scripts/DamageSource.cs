using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DamageSource : MonoBehaviour {

    #region Private members

    private bool _isCausingDamage = false;

    #endregion

    #region Public Members

    public float DamageRepeatRate = 0.1f;

    public int DamageAmount = 1;

    public bool Repeating = true;

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        _isCausingDamage = true;

        FirstPersonMovement player = other.gameObject.GetComponent<FirstPersonMovement>();

        if (player != null)
        {
            if (Repeating)
            {
                // Repeating damage
                StartCoroutine(TakeDamage(player, DamageRepeatRate));
            }
            else
            {
                // Just one time damage
                player.TakeDamage(DamageAmount);
            }
        }
    }

    IEnumerator TakeDamage(FirstPersonMovement player, float repeatRate)
    {
        while (_isCausingDamage)
        {
            player.TakeDamage(DamageAmount);
            TakeDamage(player, repeatRate);

            if (player.IsDead)
                _isCausingDamage = false;

            yield return new WaitForSeconds(repeatRate);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        FirstPersonMovement player = other.gameObject.GetComponent<FirstPersonMovement>();
        if (player != null)
        {
            _isCausingDamage = false;
        }
    }
}
