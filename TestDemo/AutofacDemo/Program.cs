using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Autofac.Configuration;

namespace AutofacDemo
{
    class Program
    {
        //RegisterType是以注册类型；Register是以Lampda方式进行注册；RegisterInstance是注册对象实例。
        static void Main_1(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<DatabaseManager>();
            //builder.RegisterType<SqlDatabase>().As<IDatabase>();
            builder.RegisterType<OracleDatabase>().As<IDatabase>();
            using (var container = builder.Build())
            {
                var manager = container.Resolve<DatabaseManager>();
                manager.Search("SELECT * FORM USER");
            }
            Console.ReadKey();
        }

        static void Main_2(string[] args)
        {
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

        static void Main_3(string[] args)
        {
            var builder = new ContainerBuilder();
            //builder.RegisterType<DatabaseManager>();
            builder.RegisterModule(new ConfigurationSettingsReader("autofac"));
            builder.Register(c => new DatabaseManager(c.Resolve<IDatabase>()));
            using (var container = builder.Build())
            {
                var manager = container.Resolve<DatabaseManager>();
                manager.Search("SELECT * FORM USER");
            }
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            User user = new User { Id = 1, Name = "leepy" };
            //User user = new User { Id = 2, Name = "zhangsan" };
            var builder = new ContainerBuilder();
            builder.RegisterModule(new ConfigurationSettingsReader("autofac"));
            builder.RegisterInstance(user).As<User>();
            builder.Register(c => new DatabaseManager(c.Resolve<IDatabase>(), c.Resolve<User>()));

            using (var container = builder.Build())
            {
                var manager = container.Resolve<DatabaseManager>();
                manager.Add("INSERT INTO USER ...");
            }

            Console.ReadKey();
        }
    }
}
