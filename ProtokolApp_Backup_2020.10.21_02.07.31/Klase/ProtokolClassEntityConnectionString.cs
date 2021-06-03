namespace ProtokolApp
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public partial class protokolEntities1 : DbContext
    {

        public protokolEntities1(string connectionString) : base(connectionString)
        {

        }
    }
}
