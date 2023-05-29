using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FurnaceCharge : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Transform thermometerTransform;
    [SerializeField] private SpriteRenderer heatRenderer;

    [Header("Thermometer Positions")]
    [SerializeField] private Vector3 coldPosition = Vector3.zero;
    [SerializeField] private Vector3 halfPosition = Vector3.zero;
    [SerializeField] private Vector3 hotPosition = Vector3.zero;

    [Header("Heat Settings")]
    [SerializeField] private Vector2 sizeRange;
    [SerializeField] private Vector2 alphaRange;

    [Header("Timing Settings")]
    [SerializeField] private float decreaseTimer;
    [SerializeField] private float sizeSpeed;
    [SerializeField] private float alphaSpeed;
    [SerializeField] private float positionSpeed;

    private Vector3 _targetSize;
    private float _targetAlpha;
    private Vector3 _targetPosition;

    private int _furnacePhase;
    private Coroutine _decreaseCoroutine;

    private Transform _heatTransform;

    private void Awake()
    {
        _heatTransform = heatRenderer.transform;

        Initialize();
        EvaluatePhase();
    }

    private void Initialize()
    {
        _furnacePhase = 0;

        SetSize(sizeRange.x);
        SetAlpha(alphaRange.x);
        ChangeThermometerPosition(coldPosition);
    }

    private void Update()
    {
        UpdateValues();
    }

    private void UpdateValues()
    {
        _heatTransform.localScale = Vector3.MoveTowards(_heatTransform.localScale, _targetSize, sizeSpeed * Time.deltaTime);

        var newColor = heatRenderer.color;
        newColor.a = Mathf.MoveTowards(newColor.a, _targetAlpha, alphaSpeed * Time.deltaTime);
        heatRenderer.color = newColor;
        
        if (thermometerTransform == null)
            return;
            
        thermometerTransform.localPosition = Vector3.MoveTowards(thermometerTransform.localPosition, _targetPosition, positionSpeed * Time.deltaTime);
    }

    private void ChangePhase(int value)
    {
        value = Mathf.Clamp(value, -1, 1);

        _furnacePhase += value;
        _furnacePhase = Mathf.Clamp(_furnacePhase, 0, 2);

        EvaluatePhase();
        
        if (_decreaseCoroutine != null)
            StopCoroutine(_decreaseCoroutine);

        _decreaseCoroutine = StartCoroutine(DecreaseCoroutine());
    }

    private void EvaluatePhase()
    {
        switch (_furnacePhase)
        {
            case 0:
                SetSize(sizeRange.x);
                SetAlpha(alphaRange.x);
                ChangeThermometerPosition(coldPosition);
                break;
            
            case 1:
                SetSize((sizeRange.x + sizeRange.y) * 0.5f);
                SetAlpha((alphaRange.x + alphaRange.y) * 0.5f);
                ChangeThermometerPosition(halfPosition);
                break;
            
            case 2:
                SetSize(sizeRange.y);
                SetAlpha(alphaRange.y);
                ChangeThermometerPosition(hotPosition);
                break;
        }
    }

    private void SetSize(float size)
    {
        _targetSize = new Vector3(size, size, 1f);
    }

    private void SetAlpha(float size)
    {
        _targetAlpha = size;
    }

    private void ChangeThermometerPosition(Vector3 newPosition)
    {
        _targetPosition = newPosition;
    }

    private IEnumerator DecreaseCoroutine()
    {
        var waitForDecrease = new WaitForSeconds(decreaseTimer);

        while (_furnacePhase > 0)
        {
            yield return waitForDecrease;
            ChangePhase(-1);
        }

        _decreaseCoroutine = null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            ChangePhase(1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            ChangePhase(1);
        }
    }
}
