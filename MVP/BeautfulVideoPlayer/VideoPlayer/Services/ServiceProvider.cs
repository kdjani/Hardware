namespace TileAFile.Services
{
  using System;
  using System.Collections.Generic;

  public class ServiceProvider : IServiceProvider
  {
    static Dictionary<Type, object> _services;
    static IServiceProvider _provider;

    static ServiceProvider()
    {
      _services = new Dictionary<Type, object>();
    }
    private ServiceProvider()
    {
    }
    public static IServiceProvider Provider
    {
      get
      {
        if (_provider == null)
        {
          _provider = new ServiceProvider();
        }
        return (_provider);
      }
    }
    public void SetService(Type serviceType, object implementation)
    {
      if (!_services.ContainsKey(serviceType))
      {
        _services[serviceType] = implementation;
      }
    }
    public object GetService(Type serviceType)
    {
      return (_services[serviceType]);
    }
  }
}