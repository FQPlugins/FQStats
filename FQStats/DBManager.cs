using MySql.Data.MySqlClient;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FQStats
{
    public class DBManager
    {

        internal DBManager()
        {
            MySqlConnection connection = CreateConnection();
            try
            {
                connection.Open();
                connection.Close();

                CreateCheckSchema();
            }
            catch (MySqlException ex)
            {
                Logger.LogException(ex);
                Main.Instance.UnloadPlugin();
            }
        }

        private static MySqlConnection CreateConnection()
        {
            MySqlConnection connection = null;
            try
            {
                connection = new MySqlConnection($"SERVER={Main.Instance.Configuration.Instance.DatabaseAddress};DATABASE={Main.Instance.Configuration.Instance.DatabaseName};UID={Main.Instance.Configuration.Instance.DatabaseUsername};PASSWORD={Main.Instance.Configuration.Instance.DatabasePassword};PORT={Main.Instance.Configuration.Instance.DatabasePort};");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return connection;
        }

        private void CreateCheckSchema()
        {
            using (MySqlConnection connection = CreateConnection())
            {
                try
                {
                    MySqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = "SHOW TABLES LIKE '" + Main.Instance.Configuration.Instance.DatabaseTableName + "';";

                    object test = command.ExecuteScalar();
                    if (test == null)
                    {
                        Logger.Log("Tables not found, creating!");
                        command.CommandText =
                            $@"CREATE TABLE `{Main.Instance.Configuration.Instance.DatabaseTableName}`
                            (
                                `steam64` BIGINT(20) DEFAULT NULL,
                                `player_kills` INT DEFAULT NULL,
                                `zombie_kills` INT DEFAULT NULL,
                                `player_deaths` INT DEFAULT NULL
                            ) COLLATE = 'utf8_general_ci' ENGINE = InnoDB;";
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
        }

        public async Task<bool> CheckExists(string id)
        {
            using (MySqlConnection connection = CreateConnection())
            {
                try
                {
                    MySqlCommand command = new MySqlCommand
                    (
                        $@"SELECT EXISTS(SELECT 1 FROM `{Main.Instance.Configuration.Instance.DatabaseTableName}`
                        WHERE `steam64` = @steam64);", connection
                    );

                    command.Parameters.AddWithValue("@steam64", id);
                    await connection.OpenAsync();

                    var status = Convert.ToInt32(await command.ExecuteScalarAsync());

                    await connection.CloseAsync();
                    return status > 0;
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                    return false;
                }
            }
        }

        public async Task<List<string>> GetPlayerInfo(string steam64)
        {
            using (MySqlConnection connection = CreateConnection())
            {
                string playerKills = "";
                string zombieKills = "";
                string deaths = "";

                try
                {
                    MySqlCommand command = new MySqlCommand
                    (
                        $@"SELECT * FROM {Main.Instance.Configuration.Instance.DatabaseTableName}
                        WHERE steam64 = @Steam64", connection
                    );

                    command.Parameters.AddWithValue("@Steam64", steam64);
                    await connection.OpenAsync();
                    var dataReader = await command.ExecuteReaderAsync(System.Data.CommandBehavior.SingleRow);

                    while (await dataReader.ReadAsync())
                    {
                        playerKills = Convert.ToString(dataReader["player_kills"]);
                        zombieKills = Convert.ToString(dataReader["zombie_kills"]);
                        zombieKills = Convert.ToString(dataReader["deaths"]);
                    }

                    dataReader.Close();
                    await connection.CloseAsync();

                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
                var list = new List<String> { playerKills, zombieKills, deaths };
                return list;
            }
        }

    }
}
