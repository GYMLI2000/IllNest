using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BossRoom : Room
{
    public Transform spawnPos;
    public Boss boss;
    private Boss currentBoss;
    [SerializeField] private GameObject itemPedestalPrefab;
    [SerializeField] private GameObject staircasePrefab;

    public override void EnterRoom()
    {
        CameraController.Instance.StartBossCamera(worldPos.x - width/2, worldPos.x + width/2, worldPos.y - height/2, worldPos.y + height/2);
        Debug.Log($"{worldPos.x - width/2}, {worldPos.x + width/2}, {worldPos.y - height/2}, {worldPos.y + height/2}");
        if (isCleared)
        {
            return;
        }

        currentBoss = boss.SpawnBoss(this);
        currentBoss.BossDeath += BossDied;
        AudioManager.Instance.StopMusicSystem();
        AudioManager.Instance.PlayMusic("Boss1");
        GameObject.FindGameObjectWithTag("Player").transform.position = transform.position;
        
    }

    public override void LeaveRoom()
    {
        CameraController.Instance.StopBossCamera();
    }

    public void BossDied()
    {
        ClearRoom();
        currentBoss.BossDeath -= BossDied;

        AudioManager.Instance.StopMusic();


        AudioManager.Instance.StartMusicSystem();

        var rm = RoomManager.RM;

        if (rm.currentFloor == rm.maxFloors)
        {
            WinScreenManager.Instance.Show();
        }
        else
        {
            Instantiate(itemPedestalPrefab, spawnPos.position - new Vector3(0, 4), Quaternion.identity, grid);
            Instantiate(staircasePrefab, spawnPos.position, Quaternion.identity, grid);
        }
    }

}
