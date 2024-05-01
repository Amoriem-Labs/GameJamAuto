using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SpellButton : MonoBehaviour
{
    public BaseSpell heldSpell;

    public TextMeshProUGUI manaText;
    public Image spellImage;

    public GameObject selOverlay;

    private GameplayUI gameplayUI;

    void Start() {
        gameplayUI = transform.parent.parent.GetComponent<GameplayUI>();
        if (gameplayUI == null) {
            Debug.LogError("no gameplayUI found?? weirdo");
        }
    }

    public void setup() {
        manaText.text = heldSpell.statsDict["manaCost"].ToString("0");
    }

    public void select() {
        selOverlay.SetActive(true);
        gameplayUI.selectNewSpellButton(this);
        gameplayUI.changeLockTooltip(true, heldSpell.spellName, heldSpell.getDescription());
    }

    public void deselect() {
        selOverlay.SetActive(false);
        gameplayUI.changeLockTooltip(false);
    }

    public void tooltipActivate() {
        Game.tooltipControl(true, heldSpell.spellName, heldSpell.getDescription());
    }

    public void hoveringOver(bool val) {
        GameManager.Instance.game.hoveringOverSpellButton = val;
    }

    public void tooltipDeactivate() {
        Game.tooltipControl(false, heldSpell.spellName, heldSpell.getDescription());
    }
}
