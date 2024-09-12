using UnityEngine;

public class Swoosh : MonoBehaviour
{
    [SerializeField] Animator _animator;
    [SerializeField] string _layerName = "Attack"; // Le nom de la couche d'animation à surveiller
    [SerializeField] string[] _animationStateNames; // Les noms des états d'animation à surveiller dans la couche Attack

    [SerializeField] int _start = 0; // Frame de début de l'effet
    [SerializeField] int _end = 0; // Frame de fin de l'effet

    float _startN = 0.0f;
    float _endN = 0.0f;

    float _time = 0.0f;
    float _prevTime = 0.0f;
    float _prevAnimTime = 0.0f;

    [SerializeField] MeleeWeaponTrail _trail;

    bool _firstFrame = true;
    int _layerIndex;

    void Start()
    {
        // Obtenez l'index de la couche Attack
        _layerIndex = _animator.GetLayerIndex(_layerName);

        if (_layerIndex == -1)
        {
            Debug.LogError($"Layer '{_layerName}' not found in the Animator.");
            return;
        }

        // Assuming all attack animations have the same frame rate and length
        AnimatorClipInfo[] clipInfo = _animator.GetCurrentAnimatorClipInfo(_layerIndex);
        if (clipInfo.Length > 0)
        {
            float frames = clipInfo[0].clip.frameRate * clipInfo[0].clip.length;
            _startN = (float)_start / frames;
            _endN = (float)_end / frames;
        }
        else
        {
            Debug.LogWarning("No clip info found. Ensure the animator is properly set up.");
        }

        _trail.Emit = false;
    }

    void Update()
    {
        if (_layerIndex == -1) return; // Skip update if layer index is invalid

        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(_layerIndex);

        // Vérifiez si l'état actuel correspond à l'un des états d'animation que nous voulons surveiller
        foreach (var stateName in _animationStateNames)
        {
            if (stateInfo.IsName(stateName))
            {
                _time += stateInfo.normalizedTime - _prevAnimTime;
                if (_time > 1.0f || _firstFrame)
                {
                    if (!_firstFrame)
                    {
                        _time -= 1.0f;
                    }
                    _firstFrame = false;
                }

                if (_prevTime < _startN && _time >= _startN)
                {
                    _trail.Emit = true;
                }
                else if (_prevTime < _endN && _time >= _endN)
                {
                    _trail.Emit = false;
                }

                _prevTime = _time;
                _prevAnimTime = stateInfo.normalizedTime;

                return; // Exit early since we found a matching state
            }
        }
    }
}
