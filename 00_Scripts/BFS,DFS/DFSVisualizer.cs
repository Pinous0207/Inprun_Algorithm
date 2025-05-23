using System.Collections;
using UnityEngine;

public class DFSVisualizer : MonoBehaviour
{
    PathFinder pathFinder;
    GridManager gridManager;
    private void Start()
    {
        gridManager = GetComponent<GridManager>();
        pathFinder = GetComponent<PathFinder>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(ShowPath());
        }
    }

    IEnumerator ShowPath()
    {
        var path = pathFinder.GetDFSPath();
        var tiles = gridManager.tiles;

        foreach(var pos in path)
        {
            tiles[pos.y, pos.x].SetColor(Color.green);
            yield return new WaitForSeconds(0.2f);
        }
    }
}
