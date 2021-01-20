using MySql.Data.MySqlClient;
using System;

namespace UsefullClassesDevelopment
{
    /// <summary>
    /// A dataclass which can be used in the <c>MySQLHandler</c> class.
    /// This dataclass simplifies development a bit as it pre-fills the most used values.
    /// </summary>
    /// <see cref="MySqlHandler"/>
    internal class MySqlClient
    {
        /// <value>Property <c>Server</c> represents the IP/hostname of the MySQL DB server.</value>
        public string Server { get; private set; }

        /// <value>Property <c>Port</c> represents the MySQL DB server listening port.</value>
        public string Port { get; private set; }

        /// <value>Property <c>Database</c> represents the database that will be accessed by the client.</value>
        public string Database { get; private set; }

        /// <value>Property <c>UserName</c> represents the user their username that will be used to access the MySQL server.</value>
        public string UserName { get; private set; }

        /// <value>Property <c>Password</c> represents the password for the user.</value>
        public string Password { get; private set; }

        /// <summary>
        /// Creates a MYSQLClient object, which can be used in the <c>MySQLHandler</c> class.
        /// </summary>
        /// <param name="userName">The user their username that will be used to access the MySQL db.</param>
        /// <param name="password">The user their password.</param>
        /// <param name="database">The database that will be accessed as default.</param>
        /// <param name="server">The MySQL server its IP or hostname.</param>
        /// <param name="port">The port on which the MySQL server runs.</param>
        /// <see cref="MySqlHandler"/>
        public MySqlClient(string userName, string password, string database, string server, string port)
        {
            // Lets assign all values to the properties so that our dataclass can function properly.
            this.UserName = userName;
            this.Password = password;
            this.Database = database;
            this.Server = server;
            this.Port = port;
        }

        /// <summary>
        /// Creates a <c>MySQLClient</c> from less props.
        /// </summary>
        /// <param name="userName">The user their username that will be used to access the MySQL db.</param>
        /// <param name="password">The user their password.</param>
        /// <param name="database">The database that will be accessed as default.</param>
        /// <returns>A proper MySQLClient which can be used in the <c>MySQLHandler</c></returns>
        /// <see cref="MySqlClient"/>
        /// <see cref="MySqlHandler"/>
        public static MySqlClient From(string userName, string password, string database)
            => new MySqlClient(userName, password, database, "127.0.0.1", "3306");

        /// <summary>
        /// Creates a <c>MySQLClient</c> from less props.
        /// (This one is probably the most used one)
        /// </summary>
        /// <param name="userName">The user their username that will be used to access the MySQL db.</param>
        /// <param name="password">The user their password.</param>
        /// <param name="database">The database that will be accessed as default.</param>
        /// <param name="server">The MySQL server its IP or hostname.</param>
        /// <returns>A proper MySQLClient which can be used in the <c>MySQLHandler</c></returns>
        /// <see cref="MySqlClient"/>
        /// <see cref="MySqlHandler"/>
        public static MySqlClient From(string userName, string password, string database, string server)
            => new MySqlClient(userName, password, database, server, "3306");
    }

    /// <summary>
    /// An easy way to manage your MySQL in C#.
    /// This class has some useful methods that simplify working with a MySQL DB.
    /// </summary>
    internal class MySqlHandler
    {
        /// <value>
        /// A MySQL connection to the DB.
        /// This is required to interact with the server.
        /// </value>
        /// <see cref="MySqlConnection"/>
        private readonly MySqlConnection _connection;

        /// <summary>
        /// Create a MySQL handler from <c>MySqlClient</c> object.
        /// </summary>
        /// <param name="client">The <c>MySqlClient</c> which contains the required information to create a connection.</param>
        /// <param name="sslMode">
        /// [OPTIONAL] The ssl method that gets used to connect with the DB.
        /// Default is "None". (NO SSL)
        /// </param>
        /// <see cref="MySqlClient"/>
        public MySqlHandler(MySqlClient client, string sslMode = "None")
        {
            // Create the connection string from the given data.
            string connectionString = $"Server={client.Server};" +
                                    $"Port={client.Port};" +
                                    $"SslMode={sslMode};" +
                                    $"Database={client.Database};" +
                                    $"Uid={client.UserName};" +
                                    $"Pwd={client.Password};";

            this._connection = new MySqlConnection(connectionString);
        }

        /// <summary>
        /// An inner helper function which executes a function within the connection open & close.
        /// </summary>
        /// <typeparam name="T">The executable its return type</typeparam>
        /// <param name="executable">A function which will be executed between the connection open & close.</param>
        /// <returns>The executable function its return instance.</returns>
        private T PerformSqlAction<T>(Func<T> executable)
        {
            this._connection.Open();
            var returnValue = executable();
            this._connection.Close();
            return returnValue;
        }

        /// <summary>
        /// Create a <c>MySqlCommand</c> from a sql statement with the current connection/client.
        /// </summary>
        /// <param name="sqlStatement">The sql that should be inserted into the command.</param>
        /// <returns>A fully prepared <c>MySqlCommand</c></returns>
        /// <see cref="MySqlCommand"/>
        public MySqlCommand Prepare(string sqlStatement)
            => new MySqlCommand(sqlStatement, this._connection);

        /// <summary>
        /// Create a <c>MySqlCommand</c> from a sql statement with parameters.
        /// </summary>
        /// <param name="sqlStatement">The SQL statement that should be contained in the <c>MySqlCommand</c></param>
        /// <param name="parameters">A list of parameters that can be added separately as args. (see examples)</param>
        /// <returns>A fully functional <c>MySqlCommand</c> with all given parameters implemented.</returns>
        ///
        /// <example>
        /// An example with one parameter:
        /// <code>
        /// MySqlHandler sqlHandler = new MySqlHandler(...args);
        ///
        /// int limit = 10;
        /// string sqlStatement = "SELECT * FROM examples LIMIT @limit;";
        ///
        /// MySqlCommand cmd = sqlHandler.Prepare(sqlStatement, ("@limit", limit));
        /// </code>
        ///
        /// An example for multiple parameters:
        /// <code>
        /// MySqlHandler sqlHandler = new MySqlHandler(...args);
        ///
        /// int limit = 10;
        /// string name = "user input";
        ///
        /// string sqlStatement = "SELECT * FROM examples WHERE foo = @bar LIMIT @limit;";
        ///
        /// MySqlCommand cmd = sqlHandler.Prepare(sqlStatement, ("@limit", limit), ("@bar", name));
        /// </code>
        /// </example>
        /// <see cref="MySqlCommand"/>
        public MySqlCommand Prepare(string sqlStatement, params (string, object)[] parameters)
        {
            MySqlCommand command = new MySqlCommand(sqlStatement, this._connection);

            // Iterate through every item and add it as parameter.
            foreach (var (parameterName, value) in parameters)
                command.Parameters.AddWithValue(parameterName, value);

            return command;
        }

        /// <summary>
        /// Execute a <c>string</c> with the current connection/client without having to prepare the command yourself.
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <returns>The amount of rows that changed because of the execute.</returns>
        public int Execute(string sqlCommand)
            => this.Execute(this.Prepare(sqlCommand));

        /// <summary>
        /// Execute a <c>MySqlCommand</c> with the current connection/client.
        /// </summary>
        /// <param name="command">The <c>MySqlCommand</c> which got prepared using <c>Prepare</c>.</param>
        /// <returns>The amount of rows that changed because of the execute.</returns>
        /// <see cref="Prepare(string)"/>
        /// <see cref="MySqlCommand"/>
        public int Execute(MySqlCommand command)
            => this.PerformSqlAction(command.ExecuteNonQuery);
    }
}