using NPoco;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ExpenseMeMVC.Infrastructure;

namespace ExpenseMeMVC.Controllers
{
    public class BaseController : Controller
    {
        public IDatabase DataContext { get; set; }
        public ICommandInvoker Command { get; set; }

        public BaseController()
            : this(ObjectFactory.GetInstance<IDatabase>(), ObjectFactory.GetInstance<ICommandInvoker>())
        {
        }

        public BaseController(IDatabase database, ICommandInvoker invoker)
        {
            DataContext = database;
            Command = invoker;
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
        }
    }
}