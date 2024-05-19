using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager
{
    private static ProjectileManager _inst = new ProjectileManager();
    public static ProjectileManager Inst
    {
        get
        {
            return _inst;
        }
    }

    private Dictionary<string, List<GameObject>> _dicProjectile = null;
    
    public Projectile CreateProjectile(string resName)
    {
        GameObject go = ResManager.Inst.LoadProjectile(resName);
        go = GameObject.Instantiate(go);

        Projectile projectile = go.GetComponent<Projectile>();
        if (projectile == null)
        {
            projectile = go.AddComponent<Projectile>();
        }

        return projectile;
    }
}
