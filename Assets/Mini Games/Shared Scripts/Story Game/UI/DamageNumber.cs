using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] float speed = 0.1f;
    [SerializeField] float moveUpBy = 2.5f;
    [SerializeField] TextMeshProUGUI damage;
    [SerializeField] TextMeshProUGUI poisonDamage;
    [SerializeField] TextMeshProUGUI bleedDamage;
    [SerializeField] TextMeshProUGUI stunned;

    private Vector3 damageStartPosition;
    private Vector3 damageEndPosition;
    private Vector3 poisonStartPosition;
    private Vector3 poisonEndPosition;
    private Vector3 bleedStartPosition;
    private Vector3 bleedEndPosition;
    private Vector3 stunStartPosition;
    private Vector3 stunEndPosition;

    public void DamageBy(float damage)
    {
        this.damage.gameObject.SetActive(true);
        this.damage.text = $"{ damage }";
    }

    public void PoisonBy(float damage)
    {
        poisonDamage.gameObject.SetActive(true);
        poisonDamage.text = $"{damage}";
    }

    public void BleedBy(float damage)
    {
        bleedDamage.gameObject.SetActive(true);
        bleedDamage.text = $"{damage}";
    }

    public void Stunned()
    {
        stunned.gameObject.SetActive(true);
    }

    private void Start()
    {
        damageStartPosition = damage.rectTransform.position;
        damageEndPosition = damage.rectTransform.position +         new Vector3(0, moveUpBy, 0);
        poisonStartPosition = poisonDamage.rectTransform.position;
        poisonEndPosition = poisonDamage.rectTransform.position +   new Vector3(0, moveUpBy, 0);
        bleedStartPosition = bleedDamage.rectTransform.position;
        bleedEndPosition = bleedDamage.rectTransform.position +     new Vector3(0, moveUpBy, 0);
        stunStartPosition = stunned.rectTransform.position;
        stunEndPosition = stunned.rectTransform.position + new Vector3(0, moveUpBy, 0);
    }

    // Update is called once per frame
    private void Update()
    {
        if (damage.gameObject.activeSelf)
        {
            damage.rectTransform.position = Vector3.MoveTowards(damage.rectTransform.position, damageEndPosition, speed);
            if (damage.rectTransform.position == damageEndPosition)
            {
                damage.gameObject.SetActive(false);
                damage.rectTransform.position = damageStartPosition;
            }
        }
        if (poisonDamage.gameObject.activeSelf)
        {
            poisonDamage.rectTransform.position = Vector3.MoveTowards(poisonDamage.rectTransform.position, poisonEndPosition, speed);
            if (poisonDamage.rectTransform.position == poisonEndPosition)
            {
                poisonDamage.gameObject.SetActive(false);
                poisonDamage.rectTransform.position = poisonStartPosition;
            }
        }
        if (bleedDamage.gameObject.activeSelf)
        {
            bleedDamage.rectTransform.position = Vector3.MoveTowards(bleedDamage.rectTransform.position, bleedEndPosition, speed);
            if (bleedDamage.rectTransform.position == bleedEndPosition)
            {
                bleedDamage.gameObject.SetActive(false);
                bleedDamage.rectTransform.position = bleedStartPosition;
            }
        }
        if (stunned.gameObject.activeSelf)
        {
            stunned.rectTransform.position = Vector3.MoveTowards(stunned.rectTransform.position, stunEndPosition, speed);
            if (stunned.rectTransform.position == stunEndPosition)
            {
                stunned.gameObject.SetActive(false);
                stunned.rectTransform.position = bleedStartPosition;
            }
        }
    }
}
