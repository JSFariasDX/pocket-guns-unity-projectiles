using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChallengeManager : MonoBehaviour
{
    List<Enemy> possibleEnemies = new List<Enemy>();
    Dictionary<EnemyType, Enemy> _cache = new Dictionary<EnemyType, Enemy>();
    [SerializeField]
    public List<DungeonRoomSizesRating> dungeonsRateConfig = new List<DungeonRoomSizesRating>();
    Dictionary<int, RoomRatingLimit> _sizeConfig = new Dictionary<int, RoomRatingLimit>();
    RoomRatingLimit roomLimits;

    private void Awake()
    {
        foreach (var c in dungeonsRateConfig)
        {
            _sizeConfig[c.key] = c.val;
        }
    }

    public void Setup(string folder)
    {
        possibleEnemies.Clear();
        int currentDungeon = GameplayManager.Instance.currentDungeonOrder - 1;
        roomLimits = _sizeConfig[currentDungeon];
        GameObject[] gameObjectsArray = Resources.LoadAll<GameObject>("Data/Enemies/" + folder);
        foreach (var e in gameObjectsArray)
        {
            possibleEnemies.Add(e.GetComponent<Enemy>());
        }
    }

    public List<Enemy> GetChallengeForRoom(RoomSize rs)
    {
        _cache.Clear();
        
        float totalChallengeRating = 0;
        float limitRating = GetLimitForSize(rs);
        Debug.Log($"Room size: {rs} limitRating: {limitRating}");
        var possibleTypes = GetEnemyTypes();

        List<Enemy> challengeEnemies = new List<Enemy>();
        while (totalChallengeRating < limitRating)
        {
            int sortedType = UnityEngine.Random.Range(0, possibleTypes.Count);
            EnemyType typeToUse = possibleTypes[sortedType];
            Enemy sortedEnemy = SortEnemyFromType(typeToUse);
            challengeEnemies.Add(sortedEnemy);
            Debug.Log($"Current rating: {totalChallengeRating} adding: {sortedEnemy.challengeRating}");
            totalChallengeRating += sortedEnemy.challengeRating;
        }
        Debug.Log($"Total rating: {totalChallengeRating}");
        return challengeEnemies;
    }

    List<Enemy> GetEnemiesFromType(EnemyType et)
    {
        List<Enemy> t = new List<Enemy>();
        foreach (var e in possibleEnemies)
        {
            if (e.enemyType == et)
            {
                t.Add(e);
            }
        }
        return t;
    }

    public List<EnemyType> GetEnemyTypes()
    {
        List<EnemyType> availableTypes = new List<EnemyType>();

        var possibleTypes = Enum.GetValues(typeof(EnemyType)).Cast<EnemyType>().ToList();
        foreach (var t in possibleTypes)
        {
            bool hasEnemiesFromThisType = GetEnemiesFromType(t).Count > 0;
            if (hasEnemiesFromThisType)
            {
                availableTypes.Add(t);
            }
        }
        return availableTypes;
    }

    Enemy SortEnemyFromType(EnemyType et)
    {
        if (_cache.ContainsKey(et))
        {
            return _cache[et];
        }
        List<Enemy> allFromType = GetEnemiesFromType(et);
        int sorted = UnityEngine.Random.Range(0, allFromType.Count);
        return allFromType[sorted];
    }

    int GetLimitForSize(RoomSize rs)
    {
        switch (rs)
        {
            case RoomSize.Small:
                return roomLimits.small;
            case RoomSize.Medium:
                return roomLimits.medium;
            default:
                return roomLimits.big;
        }
    }
}

[Serializable]
public class DungeonRoomSizesRating
{
    public int key;
    public RoomRatingLimit val;
}
