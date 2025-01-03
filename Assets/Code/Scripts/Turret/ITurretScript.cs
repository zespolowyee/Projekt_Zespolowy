using UnityEngine;

public interface ITurretScript
{
    public ITurretScript SetDetectionRange(float detectionRange);
    public ITurretScript SetShootingInterval(float shootingInterval);
    public ITurretScript SetDamage(int damage);
}
