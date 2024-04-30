using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    public Slider heroHealthSlider;
    public Slider shieldSlider;
    public Slider manaSlider;
    public Slider turnSlider;
    public Image dreamImage;

    public TextMeshProUGUI healthText;
    public TextMeshProUGUI toolTipText;
    public TextMeshProUGUI currentManaText;
    public TextMeshProUGUI maxManaText;

    private Color dreamCompleteColor = Color.magenta;
    private Color dreamLoadingColor;

    void Awake()
    {
        dreamLoadingColor = dreamImage.color;
    }

    void OnEnable()
    {
        Game.onChangeResource += updateUI;
    }
    void OnDisable()
    {
        Game.onChangeResource += updateUI;
    }

    public void updateUI()
    {
        Game currGame = GameManager.Instance.game;
        heroHealthSlider.value = currGame.heroHealth / currGame.heroMaxHealth;
        shieldSlider.value = currGame.heroShield / currGame.heroMaxHealth;
        manaSlider.value = currGame.currentMana / currGame.maxMana;
        turnSlider.value = currGame.currTurnTimer / currGame.turnMaxTime;

        if (turnSlider.value / turnSlider.maxValue > .999)
        {
            dreamImage.color = dreamCompleteColor;
        }
        else
        {
            dreamImage.color = dreamLoadingColor;
        }

        healthText.text = $"{currGame.heroHealth:0}/{currGame.heroMaxHealth:0} ({currGame.heroShield:0})" ;
        currentManaText.text = $"{currGame.currentMana:0}";
        maxManaText.text = $"{currGame.maxMana:0}";
    }
}
