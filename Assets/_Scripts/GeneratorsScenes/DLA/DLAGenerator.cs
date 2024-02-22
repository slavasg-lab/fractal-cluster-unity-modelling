using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public abstract class Savable : MonoBehaviour
{
    public abstract SaveInfo GetSaveInfo();
    public abstract void LoadInfo(SaveInfo saveInfo);
}

public struct SaveInfo
{
    public int size;
    public float radius;
    public float dimension;
    public List<Vector3> points;
}

public class DLAGenerator : Savable
{
    [SerializeField] private Transform _particleRoot;
    [SerializeField] private GameObject _particlePrefab;
    private List<Vector3> _generatedPoints = new List<Vector3>();
    private List<Vector3> _toGeneratePoints = new List<Vector3>();
    private bool _isGenerating;
    private bool _isFastGenerating;
    private int _size;
    private float _dimension;

    public void Generate(int size, int count, Action callback = null)
    {
        if (_isGenerating)
        {
            StopAllCoroutines();
        }

        _size = size;
        Utils.DestroyAllChildren(_particleRoot);
        _generatedPoints.Clear();
        _toGeneratePoints.Clear();
        StartCoroutine(
            _isFastGenerating ? FastGenerating(size, count, callback) : SlowGenerating(size, count, callback));
    }

    private IEnumerator SlowGenerating(int size, int count, Action callback)
    {
        _isGenerating = true;
        for (int x = 0; x < size; ++x)
        {
            for (int y = 0; y < size; ++y)
            {
                for (int z = 0; z < size; ++z)
                {
                    if (x == 0 && y == 0 && z == 0)
                        continue;
                    _toGeneratePoints.Add(new Vector3(x, y, z));
                }
            }
        }

        SlowCreateParticle(Vector3.one * size / 2);
        GeneratorSceneView.Instance.SetProgress(1f / count);
        yield return null;
        Vector3 position;
        for (int i = 1; i < count; i++)
        {
            position = _toGeneratePoints.GetRandom();
            SlowCreateParticle(MovePoint(position));
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
        var dimension = Mathf.Log(pointsCount) / Mathf.Log((int)possiblePointsCount) ;
        _dimension = dimension;

        GeneratorSceneView.Instance.SetDimensionText(dimension);
    }

    private void SlowCreateParticle(Vector3 position)
    {
        Instantiate(_particlePrefab, position, Quaternion.identity, _particleRoot);
        _generatedPoints.Add(position);
        _toGeneratePoints.Remove(position);
    }

    private Vector3 MovePoint(Vector3 fromPosition)
    {
        Vector3 toPosition = fromPosition;
        while (!IsParticleContact(toPosition))
        {
            List<Vector3> possibleDirections = new List<Vector3>();
            foreach (var direction in GlobalConstants.directions)
            {
                if (IsCorrectPosition(toPosition + direction))
                {
                    possibleDirections.Add(direction);
                }
            }

            toPosition = toPosition + possibleDirections.GetRandom();
        }

        return toPosition;
    }

    private bool IsParticleContact(Vector3 position)
    {
        return GlobalConstants.directions.Any(direction => _generatedPoints.Contains(position + direction));
    }

    private IEnumerator FastGenerating(int size, int count, Action callback)
    {
        _isGenerating = true;
        FastCreateParticle(Vector3.one * size / 2);
        GeneratorSceneView.Instance.SetProgress(1f / count);
        yield return null;
        for (int i = 1; i < count; i++)
        {
            var position = _toGeneratePoints.GetRandom();
            FastCreateParticle(position);
            GeneratorSceneView.Instance.SetProgress((float) (i + 1) / count);
            yield return null;
        }

        CalculateDimension();
        _isGenerating = false;
        callback?.Invoke();
    }

    private void FastCreateParticle(Vector3 position)
    {
        Instantiate(_particlePrefab, position, Quaternion.identity, _particleRoot);
        _generatedPoints.Add(position);
        _toGeneratePoints.Remove(position);
        foreach (var direction in GlobalConstants.directions)
        {
            var to = position + direction;
            if (!IsCorrectPosition(to)) continue;
            if (!_generatedPoints.Contains(to) && !_toGeneratePoints.Contains(to))
            {
                _toGeneratePoints.Add(to);
            }
        }
    }

    private bool IsCorrectPosition(Vector3 position)
    {
        return (position.x < _size && position.x >= 0) &&
               (position.y < _size && position.y >= 0) &&
               (position.z < _size && position.z >= 0);
    }

    public void SetIsFastGenerating(bool isFastGenerating)
    {
        _isFastGenerating = isFastGenerating;
    }

    public override SaveInfo GetSaveInfo()
    {
        return new SaveInfo
        {
            size = _size,
            radius = 0f,
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
        _toGeneratePoints.Clear();
        _size = saveInfo.size;
        foreach (var point in saveInfo.points)
        {
            _generatedPoints.Add(point);
            Instantiate(_particlePrefab, point, Quaternion.identity, _particleRoot);
        }

        GeneratorSceneView.Instance.SetMenuActive(false);
        ControlsHelper.Instance.SetGeneratedState(true);
        ControlsHelper.Instance.UnPauseControlls();
        GeneratorSceneView.Instance.SetStatusText(GlobalConstants.ClasterLoaded);
        GeneratorSceneView.Instance.SetParamsText("Parameters: Size = " + saveInfo.size + "; Count = " +
                                                  saveInfo.points.Count + "; Dimension = " + Math.Round(saveInfo.dimension + 1, 2) + "\nPress M to open menu\nArrows and mouse to move");
        _isGenerating = false;
    }
}