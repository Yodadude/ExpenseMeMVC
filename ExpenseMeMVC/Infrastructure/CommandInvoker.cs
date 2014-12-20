using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExpenseMeMVC.Infrastructure
{
    public interface ICommandHandler<T>
    {
        void Handle(T input);
    }

    public interface ICommandHandler<T, TRet>
    {
        TRet Handle(T input);
    }

    public interface ICommandInvoker
    {
        void Invoke<T>(T input);
        TRet Invoke<T, TRet>(T input);
    }

    public class CommandInvoker : ICommandInvoker
    {
        private IContainer _container;

        public CommandInvoker(IContainer container)
        {
            _container = container;
        }

        public void Invoke<T>(T input)
        {
            var handler = _container.GetInstance<ICommandHandler<T>>();
            handler.Handle(input);
        }

        public TRet Invoke<T, TRet>(T input)
        {
            var handler = _container.GetInstance<ICommandHandler<T, TRet>>();
            return handler.Handle(input);
        }
    }
}