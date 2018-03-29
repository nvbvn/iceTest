using System;
using System.Collections.Generic;
using UnityEngine;

public class BlobGuide {
    private int V = 1600;

    private Transform _blob;
    private List<Vector3> _path;
    private Vector3[] _directions;
    private long _latsTs = 0;

    private long _dt;
    private float _ds;
    private int _dirN = 0;
    private float _modDir = 0;
    private Vector3 _offset;

    public BlobGuide(Transform blob, List<Vector3> path) {
        _blob = blob;
        _path = path;
        createDirections();
        _blob.localPosition = _path[0];
        _latsTs = DateTime.Now.Ticks;
        //_modDir = _directions[0].magnitude;
    }

    public void Update() {
        DateTime now = DateTime.Now;
        _dt = now.Ticks - _latsTs;
        _latsTs = now.Ticks;
        //Debug.Log(dt.ToString());
        _ds = V / _dt;
        _offset = Vector3.zero;
        do {
            _modDir = _modDir==0? _directions[_dirN].magnitude : _modDir;
            if (_ds < _modDir) {
                _offset += (_ds / _directions[_dirN].magnitude) * _directions[_dirN];
                _modDir -= _ds;
            } else {
                _offset += (_modDir / _directions[_dirN].magnitude) * _directions[_dirN];
                _ds -= _modDir;
                _modDir = 0;
                _dirN++;
            }
            
        } while (_modDir > 0 && _dirN < _directions.Length);
        _blob.localPosition += _offset;
    }

    private void createDirections() {
        int l = _path.Count;
        _directions = new Vector3[l-1];
        for (int i=1; i<l; i++) {
            _directions[i-1] = _path[i] - _path[i - 1];
        }
    }
}
