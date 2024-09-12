using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PathPlacing : MonoBehaviour
{
    [SerializeField]
    private PlacingPanel _placingPanel;

    private LineRenderer _lineRenderer;
    private MeshCollider _meshCollider;

    private bool _isPlacing = false;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _meshCollider = GetComponent<MeshCollider>();

        if(_meshCollider == null )
            _meshCollider = gameObject.AddComponent<MeshCollider>();

        Mesh mesh = new Mesh();
        _lineRenderer.BakeMesh(mesh);
        _meshCollider.sharedMesh = mesh;
    }


    private void Update()
    {
        if(_isPlacing || EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (_meshCollider.Raycast(ray, out hit, 100))
            {
                _isPlacing = true;
                _placingPanel.ShowPanel(hit.point, delegate { _isPlacing = false; });
            }
        }
    }
}
