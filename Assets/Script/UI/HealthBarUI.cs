using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthUIPrefab;
    public Transform barPoint;
    public bool alwaysVisble;
    public float visibleTime;
    private float timeLeft;
    Image healthSlider;
    Transform UIbar;
    Transform cam;
    CharacterStats currentStats;

    void Awake()
    {
        currentStats = GetComponent<CharacterStats>();

        currentStats.UpdateHealthBarOnAttack += UpdateHealthBar;
    }


    void OnEnable()
    {
        cam = Camera.main.transform;

        foreach(Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if(canvas.renderMode == RenderMode.WorldSpace)
            {
                UIbar = Instantiate(healthUIPrefab,canvas.transform).transform;
                healthSlider = UIbar.GetChild(0).GetComponent<Image>();
                UIbar.gameObject.SetActive(alwaysVisble);
            }
        }
    }

    private void UpdateHealthBar (int currentHealth, int maxHealth)
    {
        if (currentHealth!=null && maxHealth!=null)
        {
            if(currentHealth <= 0)
                Destroy(UIbar.gameObject);

            UIbar.gameObject.SetActive(true);
            timeLeft = visibleTime;

            float sliderPercent = (float)currentHealth / maxHealth;
            healthSlider.fillAmount = sliderPercent;
        }
    }

    void LateUpdate()
    {
        if(UIbar != null)
        {
            UIbar.position = barPoint.position;
            UIbar.forward = -cam.forward;

            if(timeLeft <= 0 && !alwaysVisble)
                UIbar.gameObject.SetActive(false);
            else
                timeLeft -= Time.deltaTime;
        }
    }
}
