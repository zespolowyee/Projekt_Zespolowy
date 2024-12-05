using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatModType
{
    Flat = 100,
	PercentAdd = 200,
    Multiplier = 300

}

[Serializable]
public class StatModifier
{
    public StatModType modType;
    public float value;
    public int order = 100;
    public object source;

	#region Contructors
	public StatModifier(float value, StatModType modType, int order, object source)
    {
        this.value = value;
        this.modType = modType;
        this.order = order;
        this.source = source;
    }

	// Wymaga wartoœci, typu modyfikatora i Ÿród³a, ustawia order na domyœln¹ wartoœæ dla typu
	public StatModifier(float value, StatModType type, object source) : this(value, type, (int)type, source) { }

	// Wymaga wartoœci i typu, ustawia Ÿród³o na null i order na na domyœln¹ wartoœæ dla typu
	public StatModifier(float value, StatModType type) : this(value, type, (int)type, null) { }

	// Wymaga wartoœci, typu modyfikatora i order (kolejnoœci), ustawia Ÿród³o na null
    //Raczej nie bêdziemy go uzywaæ, ale mo¿e siê przydaæ
	public StatModifier(float value, StatModType type, int order) : this(value, type, order, null) { }



	#endregion
}
