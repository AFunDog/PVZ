using System;
using System.Collections;

public interface IEntity
{
    event Action<IEntity>? EntityDied;
}
