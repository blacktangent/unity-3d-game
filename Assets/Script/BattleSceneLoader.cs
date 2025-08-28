using UnityEngine;
using Cinemachine;
public class BattleSceneLoader : MonoBehaviour
{
    // 1人用
    public Transform playerSpawnPoint; // スポーン位置を指定（インスペクターで設定）
    public Transform cpuSpawnPoint;    // CPUのスポーン位置

    // 4人用
    public Transform[] playerSpawnPoints4vs4;
    public Transform[] cpuSpawnPoints4vs4;

    private CinemachineVirtualCamera virtualCam;

    void Start()
    {
        //シネマシーンカメラ生成
        virtualCam = FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCam == null)
        {
            Debug.LogError("[Camera] シネマシーン仮想カメラがシーンに存在しません！");
        }
        // 選択されたキャラクター情報をログ出力
        if (SelectedCharacterData.Instance != null)
        {
            Debug.Log($"キャラ名: {SelectedCharacterData.Instance.characterName}");
            Debug.Log($"プレハブキー: {SelectedCharacterData.Instance.prefabKey}");
            Debug.Log($"属性: {SelectedCharacterData.Instance.elementType}");
        }
        else
        {
            Debug.LogWarning("SelectedCharacterData.Instance が見つかりません！");
        }

        Debug.Log($"ゲームモード: {GameSettings.Instance.mode}");
        switch (GameSettings.Instance.mode)
        {
            case GameMode.SingleTest:
                LoadAndInstantiatePlayer(playerSpawnPoint);
                break;

            case GameMode.OneVsCpu:
                LoadAndInstantiatePlayer(playerSpawnPoint);
                LoadAndInstantiateCPU(cpuSpawnPoint);
                break;
            case GameMode.FourVsFour:
                LoadAndInstantiateCharacters4vs4();
                break;

        }
    }

    void LoadAndInstantiatePlayer(Transform spawnPoint)
    {
        string playerKey = SelectedCharacterData.Instance.prefabKey;


        if (string.IsNullOrEmpty(playerKey))
        {
            Debug.LogError("プレハブキーが未設定です。");
            return;
        }

        GameObject prefab = Resources.Load<GameObject>("Characters/Player/" + playerKey);
        if (prefab == null)
        {
            Debug.LogError("Resources/Characters/Player/" + playerKey + " が見つかりませんでした。");
            return;
        }

        Vector3 playerSpawnPos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        playerSpawnPos.y += 1.5f;
        // プレハブを生成
        Debug.Log("キャラクター生成成功: " + playerKey);
        GameObject player = Instantiate(prefab, playerSpawnPos, Quaternion.identity);
        player.name = "Player_1";

        if (player.GetComponent<CharacterMovement>() == null)
            player.AddComponent<CharacterMovement>();

        SetupCinemachineCamera(player.transform);
    }

    void LoadAndInstantiateCPU(Transform spawnPoint)
    {
        string key = SelectedCharacterData.Instance.cpuPrefabKey;


        GameObject prefab = Resources.Load<GameObject>("Characters/CPU/" + key);
        if (prefab == null)
        {
            Debug.LogError("CPUプレハブが見つかりません");
            return;
        }

        Vector3 CpuSpawnPos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        //CpuSpawnPos.y += 1.5f;
        CpuSpawnPos.x += 1.0f;

        GameObject cpu = Instantiate(prefab, CpuSpawnPos, Quaternion.identity);
        cpu.name = "CPUCharacter";

        // Rigidbody初期化
        Rigidbody rb = cpu.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero; // 初期速度リセット
            rb.angularVelocity = Vector3.zero; // 回転速度リセット
            rb.freezeRotation = true; // 回転を完全に固定（必要ならYだけ残す）
        }
        Vector3 fixedPos = cpu.transform.position;
        fixedPos.y = 0f;
        cpu.transform.position = fixedPos;

        // ←★ここから追加（NavMesh に正しく吸着）
        UnityEngine.AI.NavMeshAgent agent = cpu.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.Warp(CpuSpawnPos); // 確実にNavMeshに着地させる
            Debug.Log("[CPU] NavMeshAgent.Warpで位置修正完了");
        }
        // UIキャンバスをOFFにする処理を追加
        Transform uiCanvas = cpu.transform.Find("Canvas");
        if (uiCanvas != null)
        {
            uiCanvas.gameObject.SetActive(false);
            Debug.Log("[CPU] UIキャンバスを非表示にしました");
        }
    }


    void LoadAndInstantiateCharacters4vs4()
    {
        Debug.Log("[4vs4] キャラクター生成 開始");

        var characters = SelectedCharacterData.Instance.selectedCharacters;
        if (characters == null || characters.Count < 8)
        {
            Debug.LogWarning("[4vs4] キャラクター数が足りません");
            return;
        }

        int team0 = 0;
        int team1 = 0;

        for (int i = 0; i < characters.Count; i++)
        {
            var data = characters[i];
            if (data == null || string.IsNullOrEmpty(data.prefabKey)) continue;

            string folder = data.isPlayerControlled ? "Player" : "CPU";
            string path = $"Characters/{folder}/{data.prefabKey}";
            GameObject prefab = Resources.Load<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogWarning($"[4vs4] プレハブが見つかりません: {path}");
                continue;
            }

            Transform spawnPoint = null;
            if (data.teamId == 0 && team0 < playerSpawnPoints4vs4.Length)
            {
                spawnPoint = playerSpawnPoints4vs4[team0++];
            }
            else if (data.teamId == 1 && team1 < cpuSpawnPoints4vs4.Length)
            {
                spawnPoint = cpuSpawnPoints4vs4[team1++];
            }

            Vector3 pos = spawnPoint != null ? spawnPoint.position : Vector3.zero;

            // 初期は y = 0 で固定（ベースライン）
            pos.y = 0f;

            GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
            obj.name = (data.isPlayerControlled ? "Player_" : "CPU_") + i;

            // CPUのUIキャンバスを非表示にする
            if (!data.isPlayerControlled)
            {
                Transform uiCanvas = obj.transform.Find("Canvas");  // Canvas名はCPUプレハブのものに合わせる
                if (uiCanvas != null)
                {
                    uiCanvas.gameObject.SetActive(false);
                    Debug.Log($"[CPU_{i}] UIキャンバスを非表示にしました");
                }
            }

            // 地形に合わせて高さ補正
            RaycastHit hit;
            if (Physics.Raycast(obj.transform.position + Vector3.up * 5f, Vector3.down, out hit, 10f, LayerMask.GetMask("Ground")))
            {
                Vector3 correctedPos = new Vector3(pos.x, hit.point.y, pos.z);
                obj.transform.position = correctedPos;
                Debug.Log($"[{obj.name}] 地形に合わせて高さ調整 → Y={correctedPos.y}");
            }

            // NavMeshAgentによるWarpでNavMeshに正確に吸着
            var agent = obj.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null)
            {
                agent.Warp(obj.transform.position);
                Debug.Log($"[{obj.name}] NavMeshAgent.Warp 完了");
            }

            // 属性・チームなど設定
            var attr = obj.GetComponent<CharacterAttributes>();
            if (attr != null)
            {
                attr.elementType = data.elementType;
                attr.isPlayerControlled = data.isPlayerControlled;
                attr.faction = (data.teamId == 0) ? FactionType.Player : FactionType.Enemy;
                Debug.Log($"[4vs4] {obj.name} → teamId:{data.teamId} faction:{attr.faction}");
            }

            // CPU の場合は Z軸固定
            if (!data.isPlayerControlled)
            {
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
                    Debug.Log($"[4vs4] Z軸固定: {obj.name}");
                }
            }

            // プレイヤーキャラにはカメラ設定
            if (data.isPlayerControlled)
            {
                SetupCinemachineCamera(obj.transform);
            }
        }

        Debug.Log("[4vs4] キャラクター生成完了");
    }
    void SetupCinemachineCamera(Transform target)
    {
        if (virtualCam != null)
        {
            virtualCam.Follow = target;
            virtualCam.LookAt = null;
            Debug.Log($"[Camera] Cinemachineターゲット設定完了: {target.name}");

            var tps = virtualCam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
            if (tps != null)
            {
                virtualCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = target;
                //virtualCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = null;

                tps.CameraSide = 0f;
                tps.ShoulderOffset = new Vector3(0.5f, 1.5f, 0f);
                tps.VerticalArmLength = 1.2f;
                tps.CameraDistance = 4.5f;
                tps.Damping = new Vector3(0.2f, 0.3f, 0.3f);

                // ↓追加：BindingMode を WorldSpace に
                //tps.BindingMode = Cinemachine.CinemachineTransposer.BindingMode.WorldSpace;
            }
            else
            {
                Debug.LogWarning("[Camera] Cinemachine3rdPersonFollow が見つかりません！");
            }
        }
        else
        {
            Debug.LogWarning("[Camera] virtualCam が null のため設定できません");
        }
    }
}

