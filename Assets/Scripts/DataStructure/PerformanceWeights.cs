using System;

[System.Serializable]
public class CategoryWeights
{
    public float completionTimeWeight;
    public float accuracyWeight;
    public float responseTimeWeight;
    public float healthWeight;
}

[System.Serializable]
public class PerformanceWeights
{
    public CategoryWeights intellectWeights;
    public CategoryWeights physicalWeights;
    public CategoryWeights instinctWeights;
}