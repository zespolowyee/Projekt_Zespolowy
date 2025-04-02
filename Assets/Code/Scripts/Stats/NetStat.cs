using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.Collections;
using Unity.Netcode;

[Serializable]
public class NetStat
{
    [SerializeField] private NetStatType type;
    [SerializeField] private float baseValue;
    [SerializeField] private float maxValue;
    [SerializeField] private float minValue;

    public delegate void ModifiersChangedHandler();
    public event ModifiersChangedHandler OnModifiersChanged;

    private float _value;
    private bool isDirty = true;

    private List<NetStatModifier> statModifiers = new List<NetStatModifier>();

    public NetStatType Type { get => type; set => type = value; }
    public float Value
    {
        get
        {
            if (isDirty)
            {
                _value = CalculateFinalValue();
                isDirty = false;
            }
            return _value;
        }
    }

    public NetStat(float baseValue, float minValue, float maxValue)
    {
        statModifiers = new List<NetStatModifier>();
        this.baseValue = baseValue;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }

    public NetStat(float baseValue) : this(baseValue, 0f, 1000f) { }
    public NetStat() : this(1f, 0f, 1000f) { }

    public void AddModifier(NetStatModifier modifier)
    {

        statModifiers.Add(modifier);
        isDirty = true;
        statModifiers.Sort(CompareModifierOrder);
        if (OnModifiersChanged != null)
        {
            OnModifiersChanged();
        }

    }

    private int CompareModifierOrder(NetStatModifier a, NetStatModifier b)
    {
        if (a.ModType < b.ModType)
            return -1;
        else if (a.ModType > b.ModType)
            return 1;
        return 0;
    }

    public bool RemoveModifier(NetStatModifier modifier)
    {
        for (int i = 0; i < statModifiers.Count; i++)
        {
            if (statModifiers[i].Equals(modifier))
            {
                statModifiers.RemoveAt(i);
                isDirty = true;
                if (OnModifiersChanged != null)
                {
                    OnModifiersChanged();
                }
                return true;
            }
        }
        return false;
    }

    private float CalculateFinalValue()
    {
        float finalValue = baseValue;
        float sumPercentAdd = 0;

        for (int i = 0; i < statModifiers.Count; i++)
        {
            var modifier = statModifiers[i];
            if (modifier.ModType == StatModType.Flat)
            {
                finalValue += modifier.Value;
            }
            else if (modifier.ModType == StatModType.PercentAdd)
            {
                sumPercentAdd += modifier.Value;

                if (i + 1 >= statModifiers.Count || statModifiers[i + 1].ModType != StatModType.PercentAdd)
                {
                    finalValue *= 1 + sumPercentAdd;
                    sumPercentAdd = 0;
                }
            }
            else if (modifier.ModType == StatModType.Multiplier)
            {
                finalValue *= 1 + modifier.Value;
            }
        }

        finalValue = Math.Clamp(finalValue, minValue, maxValue);

        return (float)Math.Round(finalValue, 4);
    }
}