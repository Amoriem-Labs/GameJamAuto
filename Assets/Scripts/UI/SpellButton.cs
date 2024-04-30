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

    public void setup() {
        manaText.text = heldSpell.statsDict["manaCost"].ToString("0");
    }

    public void select() {
        selOverlay.SetActive(true);
    }

    public void deselect() {
        selOverlay.SetActive(false);
    }

    public void tooltipActivate() {
        Game.tooltipControl(true, heldSpell.spellName, heldSpell.getDescription());
    }

    public void tooltipDeactivate() {
        Game.tooltipControl(false, heldSpell.spellName, heldSpell.getDescription());
    }
}
