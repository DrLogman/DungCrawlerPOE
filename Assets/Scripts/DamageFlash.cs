using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [ColorUsage(true, true)]
    [SerializeField] private Color _flashColor = Color.white;
    [SerializeField] private float flashTime = 0.25f;

    private SpriteRenderer _spriteRenderers;
    private Material _materials;

    private Coroutine _damageFlashCoroutine;

    private void Awake()
    {
        _spriteRenderers = GetComponent<SpriteRenderer>();
        Init();
    }

    private void Init()
    {
        _materials = _spriteRenderers.material;

    }

    public void CallDamageFlash()
    {
        _damageFlashCoroutine = StartCoroutine(DamageFlasher());
    }

    public void CallPlayerDamageFlash()
    {
        _damageFlashCoroutine = StartCoroutine(PlayerDamageFlasher());
    }

    public void CallGhostDamageFlash()
    {
        _damageFlashCoroutine = StartCoroutine(GhostDamageFlasher());
    }

    public void UndoGhost()
    {
        _materials.SetFloat("_FlashAmount", 0);
    }

    private IEnumerator DamageFlasher()
    {
        SetFlashColor();

        float currentFlashAmount = 0f;
        float elapsedTime = 0f;

        while(elapsedTime < flashTime)
        {
            elapsedTime += Time.deltaTime;

            currentFlashAmount = Mathf.Lerp(1f, 0f, (elapsedTime / flashTime));
            SetFlashAmount(currentFlashAmount);

            yield return null;
        }
    }

    private IEnumerator PlayerDamageFlasher()
    {
        SetFlashColor();

        float currentFlashAmount = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < flashTime)
        {
            elapsedTime += Time.deltaTime;

            currentFlashAmount = Mathf.Lerp(1f, 0f, (elapsedTime / flashTime));
            SetFlashAmount(currentFlashAmount);

            yield return null;
        }

        _materials.SetColor("_FlashColor", Color.white);

        for (int loop = 0; loop < 4; loop++)
        {
            elapsedTime = 0;

            while (elapsedTime < 0.2)
            {
                elapsedTime += Time.deltaTime;

                currentFlashAmount = Mathf.Lerp(0f, 1f, (elapsedTime / flashTime));
                SetFlashAmount(currentFlashAmount);

                yield return null;
            }

            elapsedTime = 0;

            while (elapsedTime < 0.2)
            {
                elapsedTime += Time.deltaTime;

                currentFlashAmount = Mathf.Lerp(1f, 0f, (elapsedTime / flashTime));
                SetFlashAmount(currentFlashAmount);

                yield return null;
            }
            
        }
        
    }

    private IEnumerator GhostDamageFlasher()
    {
        SetFlashColor();

        float currentFlashAmount = 0f;
        float elapsedTime = 0f;

        _materials.SetColor("_FlashColor", Color.white);

        for (int loop = 0; loop < 7; loop++)
        {
            elapsedTime = 0;

            while (elapsedTime < 0.2)
            {
                elapsedTime += Time.deltaTime;

                currentFlashAmount = Mathf.Lerp(0f, 1f, (elapsedTime / flashTime));
                SetFlashAmount(currentFlashAmount);

                yield return null;
            }

            elapsedTime = 0;

            while (elapsedTime < 0.2)
            {
                elapsedTime += Time.deltaTime;

                currentFlashAmount = Mathf.Lerp(1f, 0f, (elapsedTime / flashTime));
                SetFlashAmount(currentFlashAmount);

                yield return null;
            }

        }

    }

    private void SetFlashColor()
    {
        _materials.SetColor("_FlashColor", _flashColor);
    }

    private void SetFlashAmount(float amount)
    {
        _materials.SetFloat("_FlashAmount", amount);
    }
}
