using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Effect : MonoBehaviour
{
    private Animator _animator;
    private AnimatorOverrideController _overrideController;

    public Vector3 offsetPos { private get; set; }

    public AnimationClip clip
    {
        set
        {
            _overrideController["Effect"] = value;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
		_animator = GetComponent<Animator>();
        _overrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
        _animator.runtimeAnimatorController = _overrideController;
	}

	private void Start()
	{
        transform.position += offsetPos;
	}
}
