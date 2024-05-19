using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResManager
{
    private static ResManager _inst = null;
    public static ResManager Inst
    {
        get
        {
            if (_inst == null)
            {
                _inst = new ResManager();
            }
            return _inst;
        }
    }

    public Dictionary<string, GameObject> _dicLoadFile = new Dictionary<string, GameObject>();

    public GameObject LoadFile(string path, string filename)
    {
        GameObject go = null;
        if (_dicLoadFile.TryGetValue(filename, out go) == false)
        {
            string filePath = string.Format("{0}/{1}", path, filename);
            go = (GameObject)Resources.Load(filePath);
            if (go == null)
            {
                Debug.LogErrorFormat("File Path Error! path : {0}", filePath);
                return null;
            }

            _dicLoadFile.Add(filename, go);
        }


        return go;
    }

    /// <summary>
    /// 발사체 로드.
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public GameObject LoadProjectile(string filename)
    {
        GameObject go = null;
        if (_dicLoadFile.TryGetValue(filename, out go) == false)
        {
            string filePath = string.Format("Projectile/{0}", filename);
            go = (GameObject)Resources.Load(filePath);
            if (go == null)
            {
                Debug.LogErrorFormat("File Path Error! path : {0}", filePath);
                return null;
            }

            _dicLoadFile.Add(filename, go);
        }

        return go;
    }
}
