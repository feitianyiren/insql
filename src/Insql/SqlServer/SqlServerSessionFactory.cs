﻿using System;
using System.Data;
using System.Data.SqlClient;

namespace Insql.SqlServer
{
    internal class SqlServerSessionFactory : IDbSessionFactory
    {
        private readonly DbContextOptions contextOptions;
        private readonly IDbConnection dbConnection;
        private readonly string connectionString;
        private readonly SqlCredential credential;

        public SqlServerSessionFactory(DbContextOptions contextOptions, IDbConnection dbConnection)
        {
            if (contextOptions == null)
            {
                throw new ArgumentNullException(nameof(contextOptions));
            }
            if (dbConnection == null)
            {
                throw new ArgumentNullException(nameof(dbConnection));
            }

            this.contextOptions = contextOptions;
            this.dbConnection = dbConnection;
        }

        public SqlServerSessionFactory(DbContextOptions contextOptions, string connectionString)
        {
            if (contextOptions == null)
            {
                throw new ArgumentNullException(nameof(contextOptions));
            }
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            this.contextOptions = contextOptions;
            this.connectionString = connectionString;
        }

        public SqlServerSessionFactory(DbContextOptions contextOptions, string connectionString, SqlCredential credential)
        {
            if (contextOptions == null)
            {
                throw new ArgumentNullException(nameof(contextOptions));
            }
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            this.contextOptions = contextOptions;
            this.connectionString = connectionString;
            this.credential = credential;
        }


        public IDbSession CreateSession()
        {
            if (this.dbConnection != null)
            {
                return new DbSession(this.dbConnection, false)
                {
                    CommandTimeout = this.contextOptions.CommandTimeout,
                };
            }

            if (this.credential != null)
            {
                return new DbSession(new SqlConnection(connectionString, credential), true)
                {
                    CommandTimeout = this.contextOptions.CommandTimeout,
                };
            }

            return new DbSession(new SqlConnection(connectionString), true)
            {
                CommandTimeout = this.contextOptions.CommandTimeout,
            };
        }
    }
}
