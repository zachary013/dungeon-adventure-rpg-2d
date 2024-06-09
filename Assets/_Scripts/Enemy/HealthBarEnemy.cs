using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarEnemy : MonoBehaviour
{
    [SerializeField] private float timeToDrain = 0.25f;
    [SerializeField] private Gradient gradient;
    public Image image;
    private float targetFillAmount = 1f;
    private Coroutine drainHealthBarCoroutine;

    private void Start()
    {
        image = GetComponent<Image>();
        UpdateHealthBarGradient();
        Debug.Log("HealthBarEnemy Start: Initial fill amount set to 1");
    }

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        targetFillAmount = currentHealth / maxHealth;
        Debug.Log($"UpdateHealthBar: targetFillAmount = {targetFillAmount}, currentHealth = {currentHealth}, maxHealth = {maxHealth}");
        if (drainHealthBarCoroutine != null)
        {
            StopCoroutine(drainHealthBarCoroutine);
        }
        drainHealthBarCoroutine = StartCoroutine(DrainHealthBar());
    }

    private IEnumerator DrainHealthBar()
    {
        float initialFillAmount = image.fillAmount;
        float elapsedTime = 0f;
        Debug.Log($"DrainHealthBar: Starting drain from {initialFillAmount} to {targetFillAmount}");
        while (elapsedTime < timeToDrain)
        {
            elapsedTime += Time.deltaTime;
            image.fillAmount = Mathf.Lerp(initialFillAmount, targetFillAmount, elapsedTime / timeToDrain);
            UpdateHealthBarGradient();
            yield return null;
        }
        image.fillAmount = targetFillAmount;
        UpdateHealthBarGradient();
        Debug.Log($"DrainHealthBar: Final fill amount = {image.fillAmount}");
    }

    private void UpdateHealthBarGradient()
    {
        float fillAmount = Mathf.Clamp01(image.fillAmount);
        image.color = gradient.Evaluate(fillAmount);
        Debug.Log($"UpdateHealthBarGradient: fillAmount = {fillAmount}, color = {image.color}");
    }
}
