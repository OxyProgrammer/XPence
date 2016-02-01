using System.Reflection;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace XPence.Models.DefaultImplementation
{
    /// <summary>
    /// A class providing singleton instance of session for DB communication.
    /// </summary>
    public class SessionProvider
    {
        #region Membar Variables

        private static ISessionFactory _sessionFactory;
        private static Configuration _config;

        #endregion


        #region Public Members

        /// <summary>
        /// Returns the singletom instance of SessionFactory.
        /// </summary>
        public static ISessionFactory SessionFactory
        {
            get { return _sessionFactory ?? (_sessionFactory = CreateSessionFactory()); }
        }

        /// <summary>
        /// Gets the singleton instance of <see cref="Configuration"/>.
        /// </summary>
        public static Configuration Config
        {
            get
            {
                if (_config == null)
                {
                    _config = new Configuration();
                    _config.AddAssembly(Assembly.GetCallingAssembly());
                }
                return _config;
            }
        }

        /// <summary>
        /// NEVER EVER call this method as this will rebuild your database.
        /// This is used to build the schema only once.
        /// </summary>
        public static void RebuildSchema()
        {
            var schema = new SchemaExport(Config);
            schema.Create(true, true);
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Creates the session factory.
        /// </summary>
        /// <returns></returns>
        private static ISessionFactory CreateSessionFactory()
        {
            return Config.BuildSessionFactory();
        }

        #endregion

    }
}
