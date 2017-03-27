namespace MES.Identity.Data.Library.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MES.Identity.Data.Library.IdentityContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(MES.Identity.Data.Library.IdentityContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            //var manager = new UserManager<User>(new UserStore<User>(new ApplicationDbContext()));

            //for (int i = 0; i < 4; i++)
            //{
            //    var user = new User()
            //    {
            //        UserName = string.Format("User{0}", i.ToString()),
            //        // Add the following so our Seed data is complete:
            //        FirstName = string.Format("FirstName{0}", i.ToString()),
            //        LastName = string.Format("LastName{0}", i.ToString()),
            //        Email = string.Format("Email{0}@Example.com", i.ToString()),
            //        MiddleName = string.Format("MiddleName{0}", i.ToString()),
            //        //TitleId = Convert.ToInt16(i),
            //    };
            //    manager.Create(user, string.Format("Password{0}", i.ToString()));
            //}

        }
    }
}
