using System;
using System.Collections.Generic;
using UnityEngine;

public class BlobGuide {
    private float V = 1300;
    private float A = 2f;

    private Transform _blob;
    private List<Vector3> _path;
    private Vector3[] _directions;
    private long _latsTs = 0;

    private float _dt;
    private float _ds;
    private int _dirN = 0;
    private float _modDir = 0;
    private Vector3 _offset;

    /*private float _vCurrent = 0;
    private float _vCurrentMax;*/
    private float _a;
    private float _v0 = 0;
    private float _v = 0;

    public BlobGuide(Transform blob, List<Vector3> path) {
        _blob = blob;
        _path = path;
        createDirections();
        _blob.localPosition = _path[0];
        _latsTs = DateTime.Now.Ticks;
        //_modDir = _directions[0].magnitude;
    }

   /* public void Update() {
        DateTime now = DateTime.Now;
        _dt = now.Ticks - _latsTs;
        _latsTs = now.Ticks;

        _ds = V / _dt;

    }*/

    public void Update() {
        DateTime now = DateTime.Now;
        _dt = Time.deltaTime;// now.Ticks - _latsTs;
        _latsTs = now.Ticks;

        // _vCurrentMax = Vector3.Angle(Vector3.up, );
        //_ds = (float)V / _dt;
        int qq = 0;
        _offset = Vector3.zero;
        if (_dirN < _directions.Length) {
            do {
                _a = (float)(A * Math.Cos(Vector3.Angle(Vector3.down, _directions[_dirN])*Math.PI/180.0));
                _ds = _v0 * _dt + _a * _dt * _dt / 2;
                return;
                _modDir = _modDir == 0 ? _directions[_dirN].magnitude : _modDir;
                if (_ds < _modDir) {
                    _offset += (_ds / _directions[_dirN].magnitude) * _directions[_dirN];
                    _modDir -= _ds;
                    _v0 = _v0 + _a * _dt;
                    //Debug.Log("_v0=" + _v0);
                    //_modDir = _ds - _modDir;// -= _ds;
                } else {
                    _offset += (_modDir / _directions[_dirN].magnitude) * _directions[_dirN];

                    _v = Mathf.Sqrt(_v0*_v0 + _a*_modDir);
                    _dt = _dt - 2 * _modDir / (_v0 + _v);
                    _v0 = _v;

                    _modDir = 0;
                    _dirN++;
                }
                qq++;

            } while (_dt > 0 && _dirN < _directions.Length && qq<20);
//            Debug.Log(_dt.ToString()+"_"+_ds.ToString()+"_"+_offset.magnitude);
            _blob.localPosition += _offset;
        }
    }

    /*public void Update() {
        DateTime now = DateTime.Now;
        _dt = now.Ticks - _latsTs;
        _latsTs = now.Ticks;

       // _vCurrentMax = Vector3.Angle(Vector3.up, );
        _ds = (float)V / _dt;
        
        _offset = Vector3.zero;
        if (_ds != 0 && _dirN < _directions.Length) {
            do {
                _modDir = _modDir == 0 ? _directions[_dirN].magnitude : _modDir;
                if (_ds < _modDir) {
                    _offset += (_ds / _directions[_dirN].magnitude) * _directions[_dirN];
                    _modDir -= _ds;
                    _ds = 0;
                    //_modDir = _ds - _modDir;// -= _ds;
                } else {
                    _offset += (_modDir / _directions[_dirN].magnitude) * _directions[_dirN];
                    _ds -= _modDir;
                    _modDir = 0;
                    _dirN++;
                }

            } while (_ds > 0 && _dirN < _directions.Length);
//            Debug.Log(_dt.ToString()+"_"+_ds.ToString()+"_"+_offset.magnitude);
            _blob.localPosition += _offset;
        }
    }*/

    private void createDirections() {
        int l = _path.Count;
        _directions = new Vector3[l-1];
        for (int i=1; i<l; i++) {
            _directions[i-1] = _path[i] - _path[i - 1];
        }
    }
}
