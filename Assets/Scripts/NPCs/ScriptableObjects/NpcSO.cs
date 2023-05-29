using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NPC", menuName = "NPC")]
public class NpcSO : ScriptableObject
{
    [Header("Item Settings")]
    public List<GameObject> PossibleItems = new List<GameObject>();
    public bool IsRepeatable = false;
    public bool IsSingleBuy = false;

    [Header("Tier Settings")]
    [Range(0f, 1f)] public float SaleChance;
    [Range(0f, 1f)] public float CheapChance;
    [Range(0f, 1f)] public float ExpensiveChance;
    [Range(0f, 1f)] public float VeryExpensiveChance;

    [Header("Price Settings")]
    public PriceType PriceType;
    [Range(0, 100)] public int BloodPercentage = 50;
    public Vector2 SalePriceRange;
    public Vector2 CheapPriceRange;
    public Vector2 ExpensivePriceRange;
    public Vector2 VeryExpensivePriceRange;

    [Header("Dialogue Settings")]
    public DialogueListSO ActivationDialogue;
    public DialogueListSO BuySuccessDialogue;
    public DialogueListSO BuyFailDialogue;
    public DialogueListSO DeactivationDialogue;
    public DialogueListSO NoItemDialogue;

    private float _chanceSaleCheap;
    private float _chanceSaleCheapExpensive;
    private float _totalChance;

    private void OnValidate()
    {
        _chanceSaleCheap = SaleChance + CheapChance;
        _chanceSaleCheapExpensive = _chanceSaleCheap + ExpensiveChance;
        _totalChance = _chanceSaleCheapExpensive + VeryExpensiveChance;

        if (_totalChance > 1f)
            Debug.LogWarning($"Total chance for {name} is greater than 100%.");
    }

    public int GetPrice(bool alreadyOnSale, out bool onSale)
    {
        onSale = false;
        
        if (PriceType == PriceType.BLOOD)
            return BloodPercentage;

        var randomTier = alreadyOnSale ? Random.Range(SaleChance, _totalChance) : Random.Range(0f, _totalChance);

        if (randomTier < SaleChance)
        {
            onSale = true;
            return GenerateRandomPrice(SalePriceRange);
        }
        
        if (randomTier < _chanceSaleCheap)
            return GenerateRandomPrice(CheapPriceRange);

        if (randomTier < _chanceSaleCheapExpensive)
            return GenerateRandomPrice(ExpensivePriceRange);

        return GenerateRandomPrice(VeryExpensivePriceRange);
    }

    private int GenerateRandomPrice(Vector2 priceRange)
    {
        return Random.Range((int)priceRange.x, (int)priceRange.y + 1);
    }
}