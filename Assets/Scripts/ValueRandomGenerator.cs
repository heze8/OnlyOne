using System;
using System.Collections.Generic;

public class ValueRandomGenerator
{
    public static T Random<T>(List<Tuple<T, int>> listOfValues )
    {
        int totalChancePool = 0;

        foreach (var pairs in listOfValues)
        {
            totalChancePool += pairs.Item2;
        }
        
        int randomNumber = new Random().Next(0, totalChancePool) + 1;
        
        int accumulatedProbability = 0;
        
        for (int i = 0; i < listOfValues.Count; i++)
        {
            accumulatedProbability += listOfValues[i].Item2;
            if (randomNumber <= accumulatedProbability)
                return listOfValues[i].Item1;
        }

        return default(T);
    }
}
