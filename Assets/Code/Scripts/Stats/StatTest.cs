using UnityEngine;

public class StatTest : MonoBehaviour
{
    public Stat TestStat;
    public StatModifier statModifier1;
    public StatModifier statModifier2;
	public StatModifier statModifier3;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TestStat.AddModifier(statModifier1);
        }

		if (Input.GetKeyDown(KeyCode.O))
		{
			TestStat.AddModifier(statModifier2);
		}

		if (Input.GetKeyDown(KeyCode.I))
		{
			TestStat.AddModifier(statModifier3);
		}

		if (Input.GetKeyDown(KeyCode.U))
		{
			Debug.Log(TestStat.Value);
		}

	}
}
