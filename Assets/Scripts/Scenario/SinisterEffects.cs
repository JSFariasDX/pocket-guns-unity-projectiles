using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinisterEffects : MonoBehaviour
{
    Player player;

    [Header("Water Reflexes")]
    public bool controlWater = true;
    public Transform reflexes1;
    public Transform reflexes2;
    public AnimationCurve movementCurve;
    float waterCurveTime1;
    float waterCurveTime2;
    [Tooltip("X is for Reflexes 1, Y is for Reflexes 2")]
    public Vector2 waterSpeed;

    [Header("Pieces of glass")]
    public bool controlGlass = true;
    public Transform glass1;
    public Transform glass2;
    public AnimationCurve glassMovementCurve;
    float glassCurveTime1;
    float glassCurveTime2;
    [Tooltip("X is for Glass 1, Y is for Glass 2")]
    public Vector2 glassSpeed;

    [Header("Rocks")]
    public bool controlRocks = true;
    public Transform rocks1;
    public Transform rocks2;
    public Transform rocks3;
    public AnimationCurve rocksMovementCurve;
    float rocksCurveTime1;
    float rocksCurveTime2;
    float rocksCurveTime3;
    [Tooltip("X is for Rocks 1, Y is for Rocks 2, Z is for Rocks 3")]
    public Vector3 rocksSpeed;

    [Header("Eyes")]
    public bool controlEyes = true;
    public Transform eye;
    public List<EyeSocket> sockets = new List<EyeSocket>();
    Transform actualSocket;
    float actualSocketLimit;
    Vector3 targetDirection;
    float lerpTrackSpeed = 25;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(controlWater)
            WaterMovevent();
        if(controlGlass)
            GlassMovement();
        if(controlRocks)
            RocksMovement();
        if(controlEyes)
            GetEyeSocket();
    }

    void WaterMovevent()
    {
        if (waterCurveTime1 < 1) waterCurveTime1 += Time.deltaTime * waterSpeed.x;
        else waterCurveTime1 = 0;

        if (waterCurveTime2 < 1) waterCurveTime2 += Time.deltaTime * waterSpeed.y;
        else waterCurveTime2 = 0;

        reflexes1.localPosition = new Vector3(movementCurve.Evaluate(waterCurveTime1), 0, 0);
        reflexes2.localPosition = new Vector3(movementCurve.Evaluate(waterCurveTime2), 0, 0);
    }

    void GlassMovement()
    {
        if (glassCurveTime1 < 1) glassCurveTime1 += Time.deltaTime * glassSpeed.x;
        else glassCurveTime1 = 0;

        if (glassCurveTime2 < 1) glassCurveTime2 += Time.deltaTime * glassSpeed.y;
        else glassCurveTime2 = 0;

        glass1.localPosition = new Vector3(glassMovementCurve.Evaluate(glassCurveTime1), 0, 0);
        glass2.localPosition = new Vector3(glassMovementCurve.Evaluate(glassCurveTime2), 0, 0);
    }

    void RocksMovement()
    {
        if (rocksCurveTime1 < 1) rocksCurveTime1 += Time.deltaTime * rocksSpeed.x;
        else rocksCurveTime1 = 0;

        if (rocksCurveTime2 < 1) rocksCurveTime2 += Time.deltaTime * rocksSpeed.y;
        else rocksCurveTime2 = 0;

        if (rocksCurveTime3 < 1) rocksCurveTime3 += Time.deltaTime * rocksSpeed.z;
        else rocksCurveTime3 = 0;

        rocks1.localPosition = new Vector3(0, rocksMovementCurve.Evaluate(rocksCurveTime1), 0);
        rocks2.localPosition = new Vector3(0, rocksMovementCurve.Evaluate(rocksCurveTime2), 0);
        rocks3.localPosition = new Vector3(0, rocksMovementCurve.Evaluate(rocksCurveTime3), 0);
    }

    void GetEyeSocket()
    {
        Transform track = Camera.main.transform.Find("TargetGroup").transform;
        EyeSocket closestSocket = GetClosestTarget(track, sockets);

        actualSocket = closestSocket.socket;
        actualSocketLimit = closestSocket.socketLimit;

        EyeTracker(GetNearPlayer().transform);
    }

    EyeSocket GetClosestTarget(Transform track, List<EyeSocket> targets)
    {
        EyeSocket bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = track.position;
        foreach (var potentialTarget in targets)
        {
            Vector3 directionToTarget = potentialTarget.socket.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }

    void EyeTracker(Transform track)
    {
        if (track)
        {
            targetDirection = track.position - actualSocket.position;

            targetDirection = Vector2.ClampMagnitude(targetDirection, actualSocketLimit);

            eye.transform.position = Vector2.Lerp(eye.transform.position, actualSocket.position + targetDirection, lerpTrackSpeed * Time.deltaTime);
        }
    }

    public Player GetNearPlayer()
    {
        Player player = null;
        foreach (Player p in GameplayManager.Instance.GetPlayers(false))
        {
            if (!player)
            {
                if (p.GetHealth().IsAlive())
                {
                    player = p;
                }
            }
            else
            {
                if (player.GetHealth().IsAlive())
                {
                    if (Vector2.Distance(actualSocket.position, p.transform.position) < Vector2.Distance(actualSocket.position, player.transform.position))
                    {
                        player = p;
                    }
                }
            }
        }

        return player;
    }

    [System.Serializable]
    public class EyeSocket
    {
        public Transform socket;
        public float socketLimit = .1f;
    }
}
