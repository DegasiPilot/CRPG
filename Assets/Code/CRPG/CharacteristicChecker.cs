using UnityEngine;

public static class CharacteristicChecker
{
	public static CheckResult Check(int bonus, int difficulty, out int diceResult, out int finalResult)
	{
		diceResult = RoleD20();
		finalResult = diceResult + bonus;
		if (diceResult == 1 || finalResult <= difficulty - 20)
		{
			return CheckResult.CriticalFail;
		}
		else if (diceResult == 20 || finalResult >= difficulty + 20)
		{
			return CheckResult.CriticalSucces;
		}
		else if (finalResult >= difficulty)
		{
			return CheckResult.Succes;
		}
		else
		{
			return CheckResult.Fail;
		}
	}

	public static int RoleCharacteristic(PersonageInfo personageInfo, Characteristics characteristic)
	{
		int diceResult = RoleD20();
		int bonus = personageInfo.GetCharacteristicBonus(characteristic);
		return diceResult + bonus;
	}

	private static int RoleD20()
	{
		return Random.Range(1, 21);
	}
}