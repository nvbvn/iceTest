using System;
using System.Collections.Generic;
using UnityEngine;

public class BlobGuide {
    private Transform _blob;
    private List<Vector3> _path;
    private long _latsTs = 0;

    public BlobGuide(Transform blob, List<Vector3> path) {
        _blob = blob;
        _path = path;
        _blob.localPosition = _path[0];
        _latsTs = DateTime.Now.Ticks;
    }

    public void Update() {
        DateTime now = DateTime.Now;
        long dt = now.Ticks - _latsTs;
        _latsTs = now.Ticks;
        Debug.Log(dt.ToString());
    }
}
