

namespace ProtokolApp
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using EntityFramework.Triggers;

    public partial class protokolEntities1 : DbContextWithTriggers
    {

        public protokolEntities1(string connectionString) : base(connectionString)
        {

        }
    }
}
