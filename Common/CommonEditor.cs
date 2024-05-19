using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public struct DungeonInfo
{
    public Dictionary<int, List<EnemyInfo>> dicInfos;
}

public struct EnemyInfo
{
    public string resName;
    public Vector3 pos;
    public Vector3 angle;
}

public class CommonEditor : MonoBehaviour
{
    [MenuItem("MyMenu/Export Enemy Info")]
    static void ExportEnemyInfo()
    {
        GameObject go = GameObject.Find("Enemy");
        Transform tEnemy = go.transform;
        Transform tLv = null;
        Transform tObj = null;
        string sz = null;
        int lv = 0;

        DungeonInfo dungeonInfo = new DungeonInfo();
        dungeonInfo.dicInfos = new Dictionary<int, List<EnemyInfo>>();

        for (int i = 0; i < tEnemy.childCount; i++)
        {
            tLv = tEnemy.GetChild(i);

            List<EnemyInfo> listEnemy = new List<EnemyInfo>();
            for (int j = 0; j < tLv.childCount; j++)
            {
                tObj = tLv.GetChild(j);
                EnemyInfo info = new EnemyInfo();

                info.resName = tObj.name;
                info.pos = tObj.position;
                info.angle = tObj.eulerAngles;

                listEnemy.Add(info);
            }

            
            if (int.TryParse(tLv.name, out lv) == false)
            {
                Debug.LogErrorFormat("오브젝트 이름 잘못됨 : {0}", lv);
                return;
            }

            dungeonInfo.dicInfos.Add(lv, listEnemy);
        }
        sz = Newtonsoft.Json.JsonConvert.SerializeObject(dungeonInfo);
        Debug.LogError(sz);

        FileStream fs = new FileStream("dungeon.txt", FileMode.Create); // 새 파일 생성
        StreamWriter w = new StreamWriter(fs);
        w.Write(sz);

        w.Close();
        fs.Close();
    }
}
