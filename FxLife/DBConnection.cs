using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FxLife
{
    public class DBConnection : IDisposable
    {
        public DBConnection()
        {
            if (!File.Exists("Life.db"))
            {
                SQLiteConnection.CreateFile("Life.db");
                AddTables();
            }
            connection = new SQLiteConnection("DataSource = Life.db");
            connection.Open();
        }
        SQLiteConnection connection;
        public void AddTables()
        {
            connection = new SQLiteConnection("DataSource = Life.db");
            connection.Open();

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"create table [GamesTable](
                                [id] INTEGER PRIMARY KEY NOT NULL,
                                [dateTime] TEXT NOT NULL,
                                [data]TEXT NOT NULL
                            );";
                int i = cmd.ExecuteNonQuery();

                cmd.CommandText = @"create table [LogTable](
                                [dateTime] TEXT NOT NULL,
                                [action] TEXT NOT NULL,
                                [alignment] TEXT NOT NULL
                            );";
                int y = cmd.ExecuteNonQuery();
            }
        }
        public IEnumerable<string> GetReservedGame()
        {
            List<string> info = new List<string>();
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM 'GamesTable';", connection);
            SQLiteDataReader reader = command.ExecuteReader();
            foreach (DbDataRecord record in reader)
                yield return record["dateTime"].ToString() + " Расстановка №" + record["id"];
        }
        public int[] GetAllIdReservedGames()
        {
            List<int> list = new List<int>();
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM 'GamesTable';", connection);
            SQLiteDataReader reader = command.ExecuteReader();
            foreach (DbDataRecord record in reader)
                list.Add(Convert.ToInt32(record["id"]));
            return list.ToArray();
        }

        public void SaveGame(string savingData)
        {
            SQLiteCommand command = new SQLiteCommand(string.Format("INSERT INTO GamesTable ('dateTime', 'data') VALUES ('{0}', '{1}');", DateTime.Now, savingData),
                connection);
            command.ExecuteNonQuery();
        }
        public void RemoveEntery(int id)
        {
            SQLiteCommand command = new SQLiteCommand("DELETE FROM GamesTable WHERE id=" + id.ToString(), connection);
            command.ExecuteNonQuery();
        }
        public string ReservedData(int id)
        {
            SQLiteCommand command = new SQLiteCommand(string.Format("SELECT * FROM 'GamesTable' WHERE id={0}", id.ToString()), connection);
            SQLiteDataReader reader = command.ExecuteReader();
            foreach (DbDataRecord record in reader)
            {
                return record["data"].ToString();
            }
            return null;
        }
        public IEnumerable<string> GetLogs()
        {
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM 'LogTable';", connection);
            SQLiteDataReader reader = command.ExecuteReader();
            foreach (DbDataRecord record in reader)
                yield return record["dateTime"].ToString() + "  " + record["action"].ToString() + "  " + record["alignment"].ToString();
        }
        public void Log(string aligment, string action)
        {
            SQLiteCommand command = new SQLiteCommand(string.Format("INSERT INTO LogTable ('dateTime', 'action', 'alignment') VALUES ('{0}', '{1}', '{2}');", DateTime.Now, action, aligment),
    connection);
            command.ExecuteNonQuery();
        }
        public void Dispose()
        {
            connection.Close();
            connection.Dispose();
        }
    }
}
