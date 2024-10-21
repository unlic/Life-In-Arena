using System;
using System.Collections.Generic;

[Serializable]
public class Recipe
{
    public Item EmptyItem; 
    public List<ItemObject> RequiredItems;

    public string GetItemsNameFromRecipe()
    {
        string result = "";

        foreach (var item in RequiredItems)
        {
            result += $"{item.name}\n";
        }

        return result;
    }

    public List<int> GetItemsIdFromRecipe()
    {
        List<int> result = new List<int>();

        foreach (var item in RequiredItems)
        {
            result.Add(item.Id);
        }

        return result;
    }
}
