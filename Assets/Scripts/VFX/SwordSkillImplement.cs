using System.Collections;
using System.Collections.Generic;
using Interfaces;
using Observer;
using UnityEngine;

public class SwordSkillImplement : MonoBehaviour, ISkillImplement
{
    void OnEnable()
    {
        ImplementSkill();
    }

    public void ImplementSkill()
    {
        MainSceneManager.Instance.player.GetComponent<VFXPlayerController>().PlaySwordSkill();
        this.PostEvent(EventID.OnSkillSwordActivate);
    }
}
