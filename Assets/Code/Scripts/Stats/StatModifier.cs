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

	// Wymaga warto�ci, typu modyfikatora i �r�d�a, ustawia order na domy�ln� warto�� dla typu
	public StatModifier(float value, StatModType type, object source) : this(value, type, (int)type, source) { }

	// Wymaga warto�ci i typu, ustawia �r�d�o na null i order na na domy�ln� warto�� dla typu
	public StatModifier(float value, StatModType type) : this(value, type, (int)type, null) { }

	// Wymaga warto�ci, typu modyfikatora i order (kolejno�ci), ustawia �r�d�o na null
    //Raczej nie b�dziemy go uzywa�, ale mo�e si� przyda�
	public StatModifier(float value, StatModType type, int order) : this(value, type, order, null) { }



	#endregion
}
