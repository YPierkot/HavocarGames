using System;

public interface IDamageable
{
    /// <summary>
    /// Deal a certain amount of Damages to the Entity
    /// </summary>
    void TakeDamage(int damages);
    
    /// <summary>
    /// Kill the Entity, Setting life points to Zero and initiating the Death Sequence
    /// </summary>
    void Kill();
}
