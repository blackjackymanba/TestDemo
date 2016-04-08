using Autofac;
using Autofac.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoFac
{
    class Program
    {
        static void Main(string[] args)
        {

            // Autofac是一款IOC框架，比较于其他的IOC框架，如Spring.NET，Unity，Castle等等所包含的，它很轻量级性能上也是很高的。

            //var builder = new ContainerBuilder();
            //builder.RegisterType<DatabaseManager>();
            //builder.RegisterType<SqlDatabase>().As<IDatabase>();
            //using (var container = builder.Build())
            //{
            //    var manager = container.Resolve<DatabaseManager>();
            //    manager.Search("SELECT * FORM USER");
            //}

            /*
            这里通过ContainerBuilder方法RegisterType对DatabaseManager进行注册，当注册的类型在相应得到的容器中可以Resolve你的DatabaseManager实例。
            builder.RegisterType<SqlDatabase>().As<IDatabase>();通过AS可以让DatabaseManager类中通过构造函数依赖注入类型相应的接口。
            Build()方法生成一个对应的Container实例，这样，就可以通过Resolve解析到注册的类型实例。

            以上的程序中，SqlDatabase或者OracleDatabase已经暴露于客户程序中了，现在我想将该类型选择通过文件配置进行读取。Autofac自带了一个Autofac.Configuration.dll 非常方便地对类型进行配置，避免了程序的重新编译。
            修改App.config：
            */

            var builder = new ContainerBuilder();
            builder.RegisterType<DatabaseManager>();
            builder.RegisterModule(new ConfigurationSettingsReader("autofac"));
            using (var container = builder.Build())
            {
                var manager = container.Resolve<DatabaseManager>();
                manager.Search("SELECT * FORM USER");
            }


            Console.ReadKey();
        }
    }
}
