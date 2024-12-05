using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat
{
	[SerializeField] private float baseValue;
	[SerializeField] private float maxValue = 1000;
	[SerializeField] private float minValue = 0;
	
	// Zmienna _value przechowuje ostatni¹ obliczon¹ wartoœæ statystyki, 
	// Zmienna isDirty mówi czy wartoœæ ta zmieni³a siê od ostatniego przeliczenia i czy nale¿y j¹ przeliczyæ ponownie
	private float _value;
	private bool isDirty = true;

	protected readonly List<StatModifier> statModifiers;

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

	public Stat(float baseValue, float minValue, float maxValue)
	{
		statModifiers = new List<StatModifier>();
		this.baseValue = baseValue;
		this.minValue = minValue;
		this.maxValue = maxValue;
	}

	public Stat(float baseValue) : this(baseValue, 0f, 1000f) { }
	public Stat() : this(1f, 0f, 1000f) { }

	//Funkcja sortuj¹ca modyfikatory, aby najpierw by³y liczone te z mniejszym "order"
	public void AddModifier(StatModifier modifier)
	{
		isDirty = true;
		statModifiers.Add(modifier);
		statModifiers.Sort(CompareModifierOrder);
	}

	//Komparator porównuj¹cy modyfikatory statystyk po ich wartoœciach "order"
	private int CompareModifierOrder(StatModifier a, StatModifier b)
	{
		if (a.order < b.order)
			return -1;
		else if (a.order > b.order)
			return 1;
		return 0;
	}

	public bool RemoveModifier(StatModifier modifier)
	{
		isDirty = true;
		return statModifiers.Remove(modifier);

	}

	public bool RemoveAllModsFromSource(object source)
	{
		bool didRemove = false;
		for (int i = statModifiers.Count - 1; i > 0; i--)
		{
			if (statModifiers[i].source == source)
			{
				statModifiers.RemoveAt(i);
				isDirty = true;
				didRemove = true;
			}
		}
		return didRemove;
	}

	private float CalculateFinalValue()
	{
		float finalValue = baseValue;
		float sumPercentAdd = 0;

		for (int i = 0; i < statModifiers.Count; i++)
		{
			StatModifier modifier = statModifiers[i];
			if (modifier.modType == StatModType.Flat)
			{
				finalValue += statModifiers[i].value;
			}
			else if (modifier.modType == StatModType.PercentAdd)
			{
				sumPercentAdd += modifier.value;

				if (i + 1 >= statModifiers.Count || statModifiers[i + 1].modType != StatModType.PercentAdd)
				{
					finalValue *= 1 + sumPercentAdd;
					sumPercentAdd = 0;
				}
			}
			else if (modifier.modType == StatModType.Multiplier)
			{
				finalValue *= 1 + modifier.value;
			}


		}

		if (finalValue > maxValue)
		{
			finalValue = maxValue;
		}

		if (finalValue < minValue)
		{
			finalValue = minValue;
		}
	
		return (float)Math.Round(finalValue, 4);
	}
}
