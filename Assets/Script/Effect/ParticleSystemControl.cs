using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemControl : MonoBehaviour
{
    public ParticleSystem ps;

    public Sprite snowPoint;
    public Sprite snowFlower;

    public void SetSnowPoint()
    {
        var tex = ps.textureSheetAnimation;
        tex.enabled = true;
        tex.mode = ParticleSystemAnimationMode.Sprites;
        tex.SetSprite(0,snowPoint);

    }

    public void SetSnowFlower()
    {
        var tex = ps.textureSheetAnimation;
        tex.enabled = true;
        tex.mode = ParticleSystemAnimationMode.Sprites;
        tex.SetSprite(0, snowFlower);

    }



    public void SetSize(int num)
    {
        var emission = ps.emission;
        emission.enabled = true;
        emission.rateOverTime = num;
    }



    public void SetLinear(Vector3 linear)
    {
        var vel = ps.velocityOverLifetime;
        vel.enabled = true;
        vel.x = linear.x;
        vel.y = linear.y;
        vel.z = linear.z;

    }


    public void SetStrength(float strength)
    {
        var noise = ps.noise;
        noise.enabled = true;    
        noise.strength = strength;
    }

    public void SetRadius(float radius)
    {
        var shape = ps.shape;
        shape.enabled = true;
        shape.radius = radius;
    }




}
