using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debug_weaponBullet : WeaponBase {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void Fire()
    {
        base.Fire();
        Debug.Log("hoge fire!!");
    }
}
