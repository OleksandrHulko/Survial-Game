using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "CraftRecipesConfig", menuName = "ScriptableObjects/CreateNewCraftRecipesConfig")]
public class CraftRecipesConfig : ScriptableObject
{
    [SerializeField]
    private CraftRecipe[] craftRecipes = null;

    public CraftRecipe[] GetRecipes() => craftRecipes;
}

[Serializable]
public class CraftRecipe
{
    public ObjectTypeIntPairArray[] nestedRecipes;
    public ObjectTypeIntPair result;
}

[Serializable]
public struct ObjectTypeIntPairArray
{
    public ObjectTypeIntPair[] resources;

    ObjectTypeIntPairArray(ObjectTypeIntPair[] objectTypeIntPairs)
    {
        resources = objectTypeIntPairs;
    }
}

[Serializable]
public struct ObjectTypeIntPair
{
    public ObjectType objectType;
    public int count;
    
    ObjectTypeIntPair(ObjectType objectType, int count)
    {
        this.objectType = objectType;
        this.count = count;
    }
}