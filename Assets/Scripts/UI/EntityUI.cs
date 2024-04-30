using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityUI : MonoBehaviour
{
    public Slider healthSlider;
    public Slider shieldSlider;
    public Slider manaSlider;
    public Slider attackSlider;
    public void updateUI(Entity entity) {
        healthSlider.value = entity.health / entity.maxHealth;
        shieldSlider.value = entity.shield / entity.maxHealth;
        manaSlider.value = entity.currMana / entity.maxMana;
        attackSlider.value = entity.attackCooldownTimer / entity.attackCooldown;
    }
}
