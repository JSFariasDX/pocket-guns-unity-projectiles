using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JikiObject : MonoBehaviour
{
    [SerializeField] private bool useRigidbody;
    [SerializeField, Range(0f, 1f)] private float weight;

    private const float FORCE_ADJUSTMENT = 0.0025f;

    private List<JikiArea> _jikis;

    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _jikis = new();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (_jikis.Count == 0)
            return;

        Vector3 finalDirection = Vector3.zero;
        float forceSum = 0f;

        foreach (JikiArea jiki in _jikis)
        {
            var direction = jiki.IsAttracting ? jiki.transform.position - transform.position : transform.position - jiki.transform.position;
            direction.Normalize();
            finalDirection += direction * jiki.GetForceRatio();
            forceSum += jiki.Force;
        }

        var finalForce = forceSum / _jikis.Count;
        var forceInfluence = 1 - weight;

        if (useRigidbody && _rigidbody != null)
            _rigidbody.AddForce(finalDirection.normalized * finalForce * forceInfluence * 10 * Time.deltaTime, ForceMode2D.Force);
		else
            transform.Translate(finalDirection.normalized * finalForce * FORCE_ADJUSTMENT * forceInfluence * Time.deltaTime);
    }

    public void AddJiki(JikiArea jiki)
    {
        if (GetComponent<Player>() && GetComponent<Player>().hasDegravitizer && !jiki.GetCharge().controlable) return;

        if (_jikis.Contains(jiki))
            return;
        
        _jikis.Add(jiki);
    }

    public void RemoveJiki(JikiArea jiki)
    {
        if (!_jikis.Contains(jiki))
            return;
        
        _jikis.Remove(jiki);
    }
}
