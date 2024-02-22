using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class OffLaticeDLAGenerator : Savable
{
    [SerializeField] private Transform _particleRoot;
    [SerializeField] private GameObject _particlePrefab;
    private List<Vector3> _generatedPoints = new List<Vector3>();
    private bool _isGenerating;
    private bool _isFastGenerating;
    private int _size;
    private float _radius;
    private float _dimension;

    public void Generate(int size, int count, float radius, Action callback = null)
    {
        if (_isGenerating)
        {
            StopAllCoroutines();
        }

        _size = size;
        _radius = radius;
        Utils.DestroyAllChildren(_particleRoot);
        _generatedPoints.Clear();
        StartCoroutine(Generating(size, count, radius, callback));
    }

    private IEnumerator Generating(int size, int count, float radius, Action callback)
    {
        _isGenerating = true;
        CreateParticle(Vector3.one * size / 2, radius);
        GeneratorSceneView.Instance.SetProgress(1f / count);
        yield return null;
        Vector3 position;
        for (int i = 1; i < count; i++)
        {
            position = new Vector3(Random.Range(0f, size - 1), Random.Range(0f, size - 1), Random.Range(0f, size - 1));
            CreateParticle(MovePoint(position, radius), radius);
            GeneratorSceneView.Instance.SetProgress((float) (i + 1) / count);
            yield return null;
        }

        CalculateDimension();
        _isGenerating = false;
        callback?.Invoke();
    }

    private void CalculateDimension()
    {
        float xMin = Single.MaxValue,
            xMax = Single.MinValue,
            yMin = Single.MaxValue,
            yMax = Single.MinValue,
            zMin = Single.MaxValue,
            zMax = Single.MinValue;

        foreach (var point in _generatedPoints)
        {
            xMin = Mathf.Min(xMin, point.x);
            yMin = Mathf.Min(yMin, point.y);
            zMin = Mathf.Min(zMin, point.z);

            xMax = Mathf.Max(xMax, point.x);
            yMax = Mathf.Max(yMax, point.y);
            zMax = Mathf.Max(zMax, point.z);
        }

        var possiblePointsCount = Mathf.Pow((xMax - xMin + 1) * (yMax - yMin + 1) * (zMax - zMin + 1), 1f / 3f);
        float pointsCount = _generatedPoints.Count;
        var dimension = Mathf.Log(pointsCount) / Mathf.Log(possiblePointsCount);
        _dimension = 1 + dimension;

        GeneratorSceneView.Instance.SetDimensionText(dimension);
    }

    private void CreateParticle(Vector3 position, float radius)
    {
        var particle = Instantiate(_particlePrefab, position, Quaternion.identity, _particleRoot);
        particle.transform.localScale *= 2 * radius;
        _generatedPoints.Add(position);
    }

    private Vector3 MovePoint(Vector3 fromPosition, float radius)
    {
        Vector3 toPosition = fromPosition;
        bool isParticleContact = false;
        foreach (var point in _generatedPoints)
        {
            if (IsParticleContact(toPosition, point, radius))
            {
                isParticleContact = true;
                break;
            }
        }

        while (!isParticleContact)
        {
            List<Vector3> possibleDirections = new List<Vector3>();
            foreach (var direction in GlobalConstants.directions)
            {
                if (IsCorrectPosition(toPosition + direction * radius))
                {
                    possibleDirections.Add(direction);
                }
            }

            toPosition = toPosition + possibleDirections.GetRandom() * radius;
            foreach (var point in _generatedPoints)
            {
                if (IsParticleContact(toPosition, point, radius))
                {
                    isParticleContact = true;
                    break;
                }
            }
        }

        return toPosition;
    }

    public bool IsParticleContact(Vector3 position, Vector3 other, float radius)
    {
        return (position - other).magnitude <= 2 * radius;
    }

    private bool IsCorrectPosition(Vector3 position)
    {
        return (position.x < _size && position.x >= 0) &&
               (position.y < _size && position.y >= 0) &&
               (position.z < _size && position.z >= 0);
    }

    public override SaveInfo GetSaveInfo()
    {
        return new SaveInfo
        {
            size = _size,
            radius = _radius,
            points = _generatedPoints,
            dimension = _dimension
        };
    }

    public override void LoadInfo(SaveInfo saveInfo)
    {
        if (_isGenerating)
        {
            StopAllCoroutines();
        }

        _isGenerating = true;
        ControlsHelper.Instance.SetGeneratedState(false);
        ControlsHelper.Instance.PauseControlls();
        ControlsHelper.Instance.SetCameraPosition(new Vector3(saveInfo.size / 2f, saveInfo.size / 2f, -50f));
        Utils.DestroyAllChildren(_particleRoot);
        _generatedPoints.Clear();
        _size = saveInfo.size;
        foreach (var point in saveInfo.points)
        {
            var particle = Instantiate(_particlePrefab, point, Quaternion.identity, _particleRoot);
            particle.transform.localScale *= 2 * saveInfo.radius;
            _generatedPoints.Add(point);
        }

        GeneratorSceneView.Instance.SetMenuActive(false);
        ControlsHelper.Instance.SetGeneratedState(true);
        ControlsHelper.Instance.UnPauseControlls();
        GeneratorSceneView.Instance.SetStatusText(GlobalConstants.ClasterLoaded);
        GeneratorSceneView.Instance.SetParamsText("Parameters: Size = " + saveInfo.size + "; Count = " +
                                                  saveInfo.points.Count + "; Radius = " + saveInfo.radius +
                                                  "; Dimension = " + Math.Round(saveInfo.dimension + 1, 2) + "\nPress M to open menu\nArrows and mouse to move");
        _isGenerating = false;
    }
}