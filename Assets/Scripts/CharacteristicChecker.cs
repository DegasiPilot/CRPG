using UnityEngine;

public static class CharacteristicChecker
{
    public static CheckResult Check(int bonus, int difficulty)
    {
        int result = RoleD20();
        if(result == 1)
        {
            return CheckResult.CriticalFail;
        }
        else if(result == 20)
        {
            return CheckResult.CriticalSucces;
        }
        else if(result + bonus >= difficulty)
        {
            return CheckResult.Succes;
        }
        else
        {
            return CheckResult.Fail;
        }
    }

    private static int RoleD20()
    {
        return Random.Range(1, 21);
    }
}