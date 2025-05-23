using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public enum PlayerState { IDLE, RUN };
public class PlayerMover : MonoBehaviour
{
    Camera camera;
    Animator animator;
    public Vector3 CameraPos;
    public MapGenerator grid;
    public GameObject playerPrefab;

    private GameObject playerInstance;
    private S_Tile currentTile;

    HashSet<S_Tile> visitedTiles = new();
    List<S_Tile> currentVisibleTiles = new();


    public PlayerState playerState;
    bool isMove = false;

    private void Start()
    {
        camera = Camera.main;
        currentTile = grid.GetStartTile();
        playerInstance = Instantiate(playerPrefab, currentTile.worldPosition + new Vector3(0, 0.7f, 0), Quaternion.identity);
        animator = playerInstance.GetComponent<Animator>();
        camera.transform.position =
            new Vector3(playerInstance.transform.position.x + CameraPos[0],
            playerInstance.transform.position.y + CameraPos[1],
            playerInstance.transform.position.z + CameraPos[2]);

        AnimatorChange("IDLE");

        InitializeVision();
    }

    void StateChange(PlayerState state)
    {
        playerState = state;
        switch(playerState)
        {
            case PlayerState.IDLE:
                AnimatorChange("IDLE");
                break;
            case PlayerState.RUN:
                AnimatorChange("RUN");
                break;
        }
    }

    void InitializeVision()
    {
        currentVisibleTiles.Clear();

        foreach (var tile in grid.GetTilesInRange(currentTile, 2))
        {
            visitedTiles.Add(tile);
            tile.Visited(true);
            currentVisibleTiles.Add(tile);
        }
    }

    void UpdateVision()
    {
        var newVisibleTiles = grid.GetTilesInRange(currentTile, 2);

        foreach(var tile in currentVisibleTiles)
        {
            if(!newVisibleTiles.Contains(tile))
            {
                tile.Visited(false);
            }
        }

        foreach(var tile in newVisibleTiles)
        {
            tile.Visited(true);
        }

        currentVisibleTiles.Clear();
        currentVisibleTiles.AddRange(newVisibleTiles);
    }

    private void Update()
    {
        CameraMovement();

        if (isMove) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            S_Tile clickedTile = hit.collider.GetComponent<S_Tile>();

            if (clickedTile != null && clickedTile.isWalkable && clickedTile != currentTile)
            {
                if(Input.GetMouseButtonDown(0))
                {
                    TryMove(clickedTile);
                }
            }
        }
    }

    void TryMove(S_Tile target)
    {
        var path = grid.FindPath(currentTile, target);

        if(path != null)
        {
            isMove = true;
            StartCoroutine(MovePath(path));
        }
        else
        {
            Debug.Log("이동 불가: 이동력이 부족하거나 경로가 없습니다.");
        }
    }

    IEnumerator MovePath(List<S_Tile> path)
    {
        StateChange(PlayerState.RUN);

        for (int i = 1; i < path.Count; i++)
        {
            Vector3 start = playerInstance.transform.position;
            Vector3 end = path[i].worldPosition + new Vector3(0, 0.7f, 0);
            Vector3 dir = (end - start).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            float t = 0.0f;
            float distance = Vector3.Distance(start, end);
            float moveSpeed = 3.0f;

            while(t < 1.0f)
            {
                playerInstance.transform.rotation = targetRotation;
                float deltamove = Time.deltaTime * moveSpeed;
                t += deltamove / distance;
                playerInstance.transform.position = Vector3.Lerp(start, end, t);

                yield return null;
            }
            playerInstance.transform.position = end;
            currentTile = path[i];
            UpdateVision();
        }
        StateChange(PlayerState.IDLE);
        isMove = false;
    }

    private void CameraMovement()
    {
        camera.transform.position = Vector3.Lerp(camera.transform.position,
            new Vector3(playerInstance.transform.position.x + CameraPos[0],
            playerInstance.transform.position.y + CameraPos[1],
            playerInstance.transform.position.z + CameraPos[2]), Time.deltaTime * 2.0f);
    }

    private void AnimatorChange(string temp)
    {
        animator.SetBool("IDLE", false);
        animator.SetBool("RUN", false);

        animator.SetBool(temp, true);
    }
}
