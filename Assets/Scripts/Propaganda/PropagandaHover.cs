using SwissArmyKnife;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PropagandaHover : Singleton<PropagandaHover>
{
    public float Distance = 100;
    public float Duration = 0.2f;
    public AnimationCurve Curve;

    private bool _ready = false;
    private bool _active = false;
    private float _time = 0;
    private Vector3 _lastPosition;
    private Vector3 _nextPosition;
    private Text _text;

    private Propaganda _hoveredPropaganda = null;

    void Start()
    {
        _text = GetComponentInChildren<Text>();
    }

    void Update () {
		if(_active)
        {
            _time += Time.deltaTime;
            var percent = Curve.Evaluate(Mathf.Clamp01(_time / Duration));

            transform.position = Vector3.Lerp(_lastPosition, _nextPosition, percent);

            if(_time > Duration)
            {
                _active = false;
            }
        }

        if(Input.GetKeyDown(KeyCode.Joystick1Button2) && _hoveredPropaganda != null)
        {
            bool launchable = !PropagandaDescription.Instance._ongoingPropaganda && _hoveredPropaganda.Usable;
            if(launchable)
            {
                _hoveredPropaganda.Trigger();
                PropagandaDescription.SetPropaganda(null);
            }            
        }
	}

    internal void Init(PropagandaButton button)
    {
        StartCoroutine(AfterInit(button));
    }

    private IEnumerator AfterInit(PropagandaButton button)
    {
        yield return new WaitForSeconds(0.5f);
        var position = button.transform.position;
        position.x -= Distance;
        _nextPosition = position;
        _lastPosition = position;
        _ready = true;
    }

    public void SetHover(Propaganda propaganda, Vector3 position)
    {
        if (!_ready)
            return;
        
        position.x -= Distance;
        _text.text = propaganda.Name;
        Move(position);
        _hoveredPropaganda = propaganda;
    }

    public void Hide()
    {
        _hoveredPropaganda = null;

        var position = transform.position;
        position.x += 2 * Distance;
        Move(position);
    }

    private void Move(Vector3 location)
    {
        _lastPosition = transform.position;
        _nextPosition = location;
        _time = 0;
        _active = true;
    }
}
