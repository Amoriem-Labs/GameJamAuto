using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    public GameObject spellButtonPrefab;
    public Transform spellButtonParent;

    public GameObject tooltip;
    public GameObject helpfulTip;

    public Slider heroHealthSlider;
    public Slider shieldSlider;
    public Slider manaSlider;
    //public Slider turnSlider;
    public Image turnImage;
    public Image dreamImage;
    public Image dreamBackground;

    public Image pauseButton;
    public Sprite pauseSprite;
    public Sprite playSprite;

    public TextMeshProUGUI healthText;
    public TextMeshProUGUI toolTipTitle;
    public TextMeshProUGUI toolTipBody;
    public TextMeshProUGUI currentManaText;
    public TextMeshProUGUI drawText;
    public TextMeshProUGUI discardText;
    public TextMeshProUGUI helpfulText;
    //public TextMeshProUGUI maxManaText;

    public Color dreamCompleteColor;
    public Color dreamLoadingColor;

    public bool lockTooltip;

    void Awake()
    {
    }

    void OnEnable()
    {
        Game.onChangeResource += updateUI;
        Game.onDrawSpell += addSpellButton;
        Game.onDiscardSpell += removeSpellButton;
        Game.onControlTooltip += tooltipControl;
        Game.onPause += changePauseButton;
        Game.onSelectSpellButton += onSpellButtonStatusChanged;
    }
    void OnDisable()
    {
        Game.onChangeResource -= updateUI;
        Game.onDrawSpell -= addSpellButton;
        Game.onDiscardSpell -= removeSpellButton;
        Game.onControlTooltip -= tooltipControl;
        Game.onPause -= changePauseButton;
        Game.onSelectSpellButton -= onSpellButtonStatusChanged;
    }

    void Update() {
    }

    public void updateUI()
    {
        Game currGame = GameManager.Instance.game;
        heroHealthSlider.value = currGame.heroHealth / currGame.heroMaxHealth;
        shieldSlider.value = currGame.heroShield / currGame.heroMaxHealth;
        manaSlider.value = currGame.currentMana / currGame.maxMana;
        turnImage.fillAmount = currGame.currTurnTimer / currGame.turnMaxTime;
        if (turnImage.fillAmount > .999) {
            dreamBackground.color = dreamCompleteColor;
        }
        else {
            dreamBackground.color = dreamLoadingColor;
        }
        //turnSlider.value = currGame.currTurnTimer / currGame.turnMaxTime;


        healthText.text = $"{currGame.heroHealth:0}/{currGame.heroMaxHealth:0} ({currGame.heroShield:0})" ;
        currentManaText.text = $"{currGame.currentMana:0}/{currGame.maxMana:0}";

        drawText.text = $"{currGame.drawPile.Count}";
        discardText.text = $"{currGame.discardPile.Count}";
    }

    public void selectNewSpellButton(SpellButton sb) {
        GameManager.Instance.game.selectNewSpellButton(sb);
    }

    public void onSpellButtonStatusChanged(bool active) {
        if (active) {
            helpfulTipControl(true, "Left Click n shit to use the selected spell. Right Click to cancel.");
        }
        else {
            helpfulTipControl(false, "");
        }
    }

    public void changePauseButton(bool paused) {
        if (paused) {
            pauseButton.sprite = pauseSprite;
        }
        else {
            pauseButton.sprite = playSprite;
        }
    }

    public void addSpellButton(BaseSpell spell) {
        GameObject newSpellButton = Instantiate(spellButtonPrefab, spellButtonParent);
        newSpellButton.GetComponent<SpellButton>().heldSpell = spell;
    }

    public void removeSpellButton(BaseSpell spell) {
        for (int i = spellButtonParent.childCount - 1; i > -1; i--) {
            if (spellButtonParent.GetChild(i).GetComponent<SpellButton>().heldSpell == spell) {
                Destroy(spellButtonParent.GetChild(i).gameObject);
            }
        }
    }

    public void changeLockTooltip(bool active, string title = "", string body = "") {
        lockTooltip = active;

        tooltip.SetActive(active);
        toolTipTitle.text = title;
        toolTipBody.text = body;
    }

    public void tooltipControl(bool active, string title="", string body = "") {
        if (active == false && body != "") {
            if (body != toolTipBody.text) {
                return;
            }
        }
        if (!lockTooltip) {
            tooltip.SetActive(active);
            toolTipTitle.text = title;
            toolTipBody.text = body;
        }
    }

    public void helpfulTipControl(bool active, string body = "") {
        helpfulTip.SetActive(active);
        helpfulText.text = body;
    }
}
