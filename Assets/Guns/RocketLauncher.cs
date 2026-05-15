using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : Gun
{
    [SerializeField] GameObject prefabRocketLaucher; 
    public override bool AttemptFire()
    {
        if (!base.AttemptFire()) 
            return false; 
        
        
        var b = Instantiate(bulletPrefab, gunBarrelEnd.transform.position, gunBarrelEnd.rotation); 
        
        b.GetComponent<Projectile>().Initialize(100, 100, 2, 200, null); // version without special effect
       
        Instantiate(prefabRocketLaucher, gunBarrelEnd.transform.position, gunBarrelEnd.rotation); 
        
        
        
        anim.SetTrigger("shoot"); 
        elapsed = 0; 
        ammo -= 1; 
        return true; 
    } 
}

    