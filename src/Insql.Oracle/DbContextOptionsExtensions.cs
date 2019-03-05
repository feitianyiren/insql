﻿//using Insql.Oracle;
//using Oracle.ManagedDataAccess.Client;
//using System;
//using System.Data;

//namespace Insql
//{
//    public static partial class DbContextOptionsExtensions
//    {
//        public static DbContextOptions UseOracle(this DbContextOptions options, string connectionString)
//        {
//            if (string.IsNullOrWhiteSpace(connectionString))
//            {
//                throw new ArgumentNullException(nameof(connectionString));
//            }

//            options.SqlAdapter = new OracleAdapter(options, connectionString);

//            return options;
//        }

//        public static DbContextOptions UseOracle(this DbContextOptions options, string connectionString, OracleCredential credential)
//        {
//            if (string.IsNullOrWhiteSpace(connectionString))
//            {
//                throw new ArgumentNullException(nameof(connectionString));
//            }

//            options.SqlAdapter = new OracleAdapter(options, connectionString, credential);

//            return options;
//        }

//        public static DbContextOptions UseOracle(this DbContextOptions options, IDbConnection connection)
//        {
//            if (connection == null)
//            {
//                throw new ArgumentNullException(nameof(connection));
//            }

//            options.SqlAdapter = new OracleAdapter(options, connection);

//            return options;
//        }
//    }
//}
