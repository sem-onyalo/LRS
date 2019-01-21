using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using LRS.ConsoleApp;
using LRS.Repository;

namespace LRS.Lib
{
    public class UnityFactory : IUnityFactory
    {
        private readonly IUnityContainer _unityContainer;

        public UnityFactory(IUnityContainer unityContainer)
        {
            if (unityContainer == null) throw new ArgumentNullException("unityContainer");

            _unityContainer = unityContainer;

        }

        public void Initialize()
        {
            _unityContainer.RegisterType<IPathRepository, PathRepository>();
            _unityContainer.RegisterType<IOrderRepository, OrderRepository>();
            _unityContainer.RegisterType<IFulfilledRepository, FulfilledRepository>();
            _unityContainer.RegisterType<IInventoryRepository, InventoryRepository>();
            _unityContainer.RegisterType<IFulfillmentRepository, FulfillmentRepository>();
            _unityContainer.RegisterType<ILoadingDockRepository, LoadingDockRepository>();

            _unityContainer.RegisterType<IRobotManager, RobotManager>();
            _unityContainer.RegisterType<ILogisticalProcessingUnit, LogisticalProcessingUnit>();

            //_unityContainer.RegisterType<IView, View>();
        }

        public T Resolve<T>()
        {
            return _unityContainer.Resolve<T>();
        }
    }
}
