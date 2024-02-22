using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DCLAGenerator : Savable
{
    [SerializeField] private Transform _particleRoot;
    [SerializeField] private GameObject _particlePrefab;
    private List<Vector3> _toGeneratePoints = new List<Vector3>();
    private List<List<Vector3>> _unions = new List<List<Vector3>>();
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
        _toGeneratePoints.Clear();
        _unions.Clear();
        StartCoroutine(Generating(size, count, callback));
    }

    private IEnumerator Generating(int size, int count, Action callback)
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
        for (int i = 0; i < count; i++)
        {
            CreateParticle(_toGeneratePoints.GetRandom());
        }
        MergeUnions();
        GeneratorSceneView.Instance.SetProgress((float)(count - _unions.Count) / (count - 1));
        yield return null;
        while (_unions.Count > 1)
        {
            MoveUnions();
            MergeUnions();
            GeneratorSceneView.Instance.SetProgress((float)(count - _unions.Count) / (count - 1));
            yield return null;
        }
        ShowPoints(_unions[0]);
        
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

        foreach (var point in _unions[0])
        {
            xMin = Mathf.Min(xMin, point.x);
            yMin = Mathf.Min(yMin, point.y);
            zMin = Mathf.Min(zMin, point.z);

            xMax = Mathf.Max(xMax, point.x);
            yMax = Mathf.Max(yMax, point.y);
            zMax = Mathf.Max(zMax, point.z);
        }

        var possiblePointsCount = Mathf.Pow((xMax - xMin + 1) * (yMax - yMin + 1) * (zMax - zMin + 1), 1f / 3f);
        float pointsCount = _unions[0].Count;
        var dimension = Mathf.Log(pointsCount) / Mathf.Log(possiblePointsCount);
        _dimension = 1 + dimension;
        
        GeneratorSceneView.Instance.SetDimensionText(dimension);
    }

    private void ShowPoints(List<Vector3> positions)
    {
        foreach (var position in positions)
        {
            Instantiate(_particlePrefab, position, Quaternion.identity, _particleRoot);
        }
    }
    
    private void MergeUnions()
    {
        for (int i = 0; i < _unions.Count; i++)
        {
            if (_unions[i].Count == 0)
                continue;
            for (int j = i + 1; j < _unions.Count; j++)
            {
                if (IsUnionsIntersect(_unions[i], _unions[j]))
                {
                    MergeUnions(_unions[i], _unions[j]);
                }
            }
        }
        for (int i = _unions.Count - 1; i >= 0; i--)
        {
            if (_unions[i].Count == 0)
                _unions.RemoveAt(i);
        }
    }
    
    private void MoveUnions()
    {
        foreach (var union in _unions)
        {
            List<Vector3> possibleDirections = new List<Vector3>();
            foreach (var direction in GlobalConstants.directions)
            {
                if (IsPossibleForUnion(union, direction))
                    possibleDirections.Add(direction);
            }
            if (possibleDirections.Count == 0)
                continue;
            MoveUnion(union, possibleDirections.GetRandom());
        }
    }

    private bool IsPossibleForUnion(List<Vector3> union, Vector3 direction)
    {
        foreach (var position in union)
        {
            if (!IsCorrectPosition(position + direction))
                return false;
        }
        return true;
    }
    
    private void MoveUnion(List<Vector3> union, Vector3 direction)
    {
        for (int i = 0; i < union.Count; i++)
        {
            union[i] += direction;
        }
    }

    private void MergeUnions(List<Vector3> unionA, List<Vector3> unionB)
    {
        unionA.AddRange(unionB);
        unionB.Clear();
    }

    private bool IsUnionsIntersect(List<Vector3> unionA, List<Vector3> unionB)
    {
        foreach (var positionA in unionA)
        {
            foreach (var positionB in unionB)
            {
                if (IsParticleContact(positionA, positionB))
                    return true;
            }
        }
        return false;
    }

    private void CreateParticle(Vector3 position)
    {
        _toGeneratePoints.Remove(position);
        List<Vector3> temp = new List<Vector3>();
        temp.Add(position);
        _unions.Add(temp);
    }
    public bool IsParticleContact(Vector3 position, Vector3 other)
    {
        foreach (var direction in GlobalConstants.directions)
        {
            if (other == position + direction)
            {
                return true;
            }
        }
        return false;
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
            radius = 0f,
            points = _unions[0],
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
        _unions.Clear();
        _toGeneratePoints.Clear();
        _size = saveInfo.size;
        foreach (var point in saveInfo.points)
        {
            _unions.Add(new List<Vector3>()); 
            _unions[0].Add(point);
            Instantiate(_particlePrefab, point, Quaternion.identity, _particleRoot);
        }
        GeneratorSceneView.Instance.SetMenuActive(false);
        ControlsHelper.Instance.SetGeneratedState(true);
        ControlsHelper.Instance.UnPauseControlls();
        GeneratorSceneView.Instance.SetStatusText(GlobalConstants.ClasterLoaded);
        GeneratorSceneView.Instance.SetParamsText("Parameters: Size = " + saveInfo.size + "; Count = " + saveInfo.points.Count +
                                                  "; Dimension = " + Math.Round(saveInfo.dimension + 1, 2) + "\nPress M to open menu\nArrows and mouse to move");
        _isGenerating = false;
    }
}
