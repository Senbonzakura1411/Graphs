using UnityEngine;

public class GPUGraph : MonoBehaviour
{
    private const int MAXResolution = 2000;
    
    [SerializeField] private ComputeShader computeShader;
    [SerializeField, Range(10, MAXResolution)] private int resolution = 100;
    [SerializeField] private FunctionLibrary.FunctionName function;
    [SerializeField, Range(0f, 10f)] private float functionDuration = 1f, transitionDuration = 1f;
    [SerializeField] private Material material;
    [SerializeField] private Mesh mesh;
    enum TransitionMode
    {
        Cycle,
        Random
    }

    [SerializeField] private TransitionMode transitionMode = TransitionMode.Cycle;

    private ComputeBuffer _positionsBuffer;
    private float _duration;
    private bool _transitioning;
    private FunctionLibrary.FunctionName _transitionFunction;

    private static readonly int
        positionsId = Shader.PropertyToID("_positions"),
        resolutionId = Shader.PropertyToID("_resolution"),
        stepId = Shader.PropertyToID("_step"),
        timeId = Shader.PropertyToID("_time"),
        transitionProgressId = Shader.PropertyToID("_transitionProgress");

    // OnEnable() instead of Awake() to allow for hot reload.
    private void OnEnable()
    {
        _positionsBuffer = new ComputeBuffer(MAXResolution * MAXResolution, 3 * 4);
    }

    //Free up memory before hot reload. 
    private void OnDisable()
    {
        _positionsBuffer.Release();
        _positionsBuffer = null;
    }

    private void Update()
    {
        _duration += Time.deltaTime;
        if (_transitioning)
        {
            if (_duration >= transitionDuration)
            {
                _duration -= transitionDuration;
                _transitioning = false;
            }
        }
        else if (_duration >= functionDuration)
        {
            _duration -= functionDuration;
            _transitioning = true;
            _transitionFunction = function;
            PickNextFunction();
        }
        
        UpdateFunctionOnGPU();
    }

    private void PickNextFunction()
    {
        function = transitionMode == TransitionMode.Cycle
            ? FunctionLibrary.GetNextFunctionName(function)
            : FunctionLibrary.GetRandomFunctionNameOtherThan(function);
    }
    
    void UpdateFunctionOnGPU () {
        float step = 2f / resolution;
        computeShader.SetInt(resolutionId, resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetFloat(timeId, Time.time);
        if (_transitioning)
        {
            computeShader.SetFloat(transitionProgressId, Mathf.SmoothStep(0f, 1f, _duration / transitionDuration));
        }

        var kernelIndex = (int) function + (int)(_transitioning ? _transitionFunction : function) * FunctionLibrary.FunctionCount;
        computeShader.SetBuffer(kernelIndex, positionsId, _positionsBuffer);
        int groups = Mathf.CeilToInt(resolution / 8f);
        computeShader.Dispatch(kernelIndex, groups, groups, 1);
        material.SetBuffer(positionsId, _positionsBuffer);
        material.SetFloat(stepId, step);
        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, resolution * resolution);
    }
}