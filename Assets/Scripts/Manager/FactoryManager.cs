using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryManager : BaseManager
{
    private RoleFactory roleFactory;

    public RoleFactory RoleFactory { get => roleFactory; set => roleFactory = value; }
    public override void OnInit(GameEngine gameEngine)
    {
        base.OnInit(gameEngine);
        roleFactory = new RoleFactory(true, 10);
    }
    public override void OnDestroy()
    {
        base.OnDestroy();

    }
}

public class BaseFactory<T, X, Y, Z> where T : class, new()
{
    private ClassObjectPool<T> m_ObjectPool;
    private bool m_UseObjectPool;
    private int poolObjectCount;

    public BaseFactory(bool usePool, int maxCount = 0)
    {
        m_UseObjectPool = usePool;
        poolObjectCount = maxCount;
        m_ObjectPool = new ClassObjectPool<T>(poolObjectCount);
    }

    public T Spawn(X data1, Y data2, Z data3)
    {
        T product;
        if (m_UseObjectPool)
        {
            product = m_ObjectPool.Spawn(true);
        }
        else
        {
            product = new T();
        }

        return Make(product, data1, data2, data3);
    }
    public void Recycle(T product)
    {
        DisMake(product);
        if (m_UseObjectPool)
        {
            m_ObjectPool.Recycle(product);
        }
    }

    protected virtual T Make(T product, X data1, Y data2, Z data3) { return product; }

    protected virtual void DisMake(T product) { }

}
public class RoleFactory : BaseFactory<object, object, object, object>
{
    public RoleFactory(bool usePool, int maxCount = 0) : base(usePool, maxCount)
    {
        //skillManager = FightEngine.Instance.GetManager<FSkillManager>();
    }

}

