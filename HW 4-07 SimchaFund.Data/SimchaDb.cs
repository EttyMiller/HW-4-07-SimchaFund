using Microsoft.Data.SqlClient;
using System;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace HW_4_07_SimchaFund.Data
{
    public class SimchaDb
    {
        private readonly string _connectionString;

        public SimchaDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<SimchaInfo> GetAllSimchas()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT s.*, Count(c.ContributorId) AS 'NumOfContributors', SUM(c.Amount) AS 'TotalContributed' FROM Simchas s
                                    LEFT JOIN Contributions c
                                    ON s.Id = c.SimchaId
                                    GROUP BY s.Id, s.Name, s. Date";

            connection.Open();

            var reader = cmd.ExecuteReader();
            List<SimchaInfo> simchas = new();

            while (reader.Read())
            {
                simchas.Add(new SimchaInfo
                {
                    Id = (int)reader["Id"],
                    SimchaName = (string)reader["Name"],
                    Date = (DateTime)reader["Date"],
                    NumOfContributors = (int)reader["NumOfContributors"],
                    TotalContributed = reader.GetOrNull<decimal>("TotalContributed")
                });
            }

            return simchas;
        }

        public List<Contributor> GetAllContributors()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Contributors
                                ORDER BY LastName";

            connection.Open();

            var reader = cmd.ExecuteReader();
            List<Contributor> contributors = new();

            while (reader.Read())
            {
                var c = new Contributor()
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    CellNumber = (string)reader["CellNumber"],
                    Date = (DateTime)reader["Date"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"]
                };
                c.Balance = GetPersonsBalance(c.Id);
                contributors.Add(c);
            }

            return contributors;
        }

        public List<Contributor> GetContributorsForSimcha(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT c.Id, c.FirstName,c.LastName, c.CellNumber, c.CellNumber, c.AlwaysInclude,
                            CASE 
                                WHEN co.SimchaId = @id THEN co.Amount 
                                ELSE NULL 
                            END AS 'AmountContributed',
                            SUM(d.Amount) AS 'Balance' FROM Contributors c
                            JOIN Deposits d
                            ON c.Id = d.ContributorId
                            Left join Contributions co
                            ON co.ContributorId = c.Id
                            GROUP BY c.Id, c.FirstName,c.LastName, c.CellNumber, c.CellNumber, c.AlwaysInclude, co.Amount, co.SimchaId";

            cmd.Parameters.AddWithValue("@id", id);

            connection.Open();

            var reader = cmd.ExecuteReader();
            List<Contributor> contributors = new();

            while (reader.Read())
            {
                contributors.Add(new Contributor
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    CellNumber = (string)reader["CellNumber"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"],
                    Balance = (decimal)reader["Balance"],
                    AmountContributed = reader.GetOrNull<decimal>("AmountContributed")
                });
            }

            return contributors;
        }

        public int GetTotalContributors()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Contributors";
            connection.Open();

            return (int)cmd.ExecuteScalar();
        }

        public void AddSimcha(SimchaInfo simcha)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();

            cmd.CommandText = "INSERT INTO Simchas " +
                "VALUES (@name, @date)";
            cmd.Parameters.AddWithValue("@name", simcha.SimchaName);
            cmd.Parameters.AddWithValue("@date", simcha.Date);

            connection.Open();

            cmd.ExecuteNonQuery();
        }

        public void AddContibutor(Contributor contributor, decimal amt)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();

            cmd.CommandText = @"INSERT INTO Contributors
                VALUES (@firstName, @lastName, @cellNumber, @date, @alwaysInclude) 
                SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@firstName", contributor.FirstName);
            cmd.Parameters.AddWithValue("@lastName", contributor.LastName);
            cmd.Parameters.AddWithValue("@cellNumber", contributor.CellNumber);
            cmd.Parameters.AddWithValue("@date", contributor.Date);
            cmd.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);

            connection.Open();

            contributor.Id = (int)(decimal)cmd.ExecuteScalar();

            AddDeposit(contributor.Id, amt, contributor.Date);
        }

        public void EditContibutor(Contributor contributor)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();

            cmd.CommandText = @"UPDATE Contributors 
                SET FirstName = @firstName, LastName = @lastName, CellNumber = @cellNumber, Date = @date, AlwaysInclude = @alwaysInclude 
                WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", contributor.Id);
            cmd.Parameters.AddWithValue("@firstName", contributor.FirstName);
            cmd.Parameters.AddWithValue("@lastName", contributor.LastName);
            cmd.Parameters.AddWithValue("@cellNumber", contributor.CellNumber);
            cmd.Parameters.AddWithValue("@date", contributor.Date);
            cmd.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);

            connection.Open();

            cmd.ExecuteNonQuery();
        }

        public void AddDeposit(int contributorId, decimal amnt, DateTime date)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();

            cmd.CommandText = @"INSERT INTO Deposits
                VALUES (@contributorId, @intialAmount, @date)";
            cmd.Parameters.AddWithValue("@contributorId", contributorId);
            cmd.Parameters.AddWithValue("@intialAmount", amnt);
            cmd.Parameters.AddWithValue("@date", date);

            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public decimal GetPersonsBalance(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT 
                                    COALESCE(
                                        (SELECT SUM(d.Amount) FROM Deposits d WHERE d.ContributorId = @id), 
                                        0
                                    ) - 
                                    COALESCE(
                                        (SELECT SUM(c.Amount) FROM Contributions c WHERE c.ContributorId = @id), 
                                        0
                                    ) AS NetBalance;";    
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();

            return (decimal)cmd.ExecuteScalar();
        }

        public string GetSimchaName(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT Name FROM Simchas 
                                WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();

            return (string)cmd.ExecuteScalar();
        }

        public decimal GetTotalBalance()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT 
                                    COALESCE(
                                        (SELECT SUM(d.Amount) FROM Deposits d), 
                                        0
                                    ) - 
                                    COALESCE(
                                        (SELECT SUM(c.Amount) FROM Contributions c), 
                                        0
                                    ) AS NetBalance;";
            connection.Open();

            return (decimal)cmd.ExecuteScalar();
        }

        public void UpdateContributions(List<Contribution> contributions)
        {
          
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();

            cmd.CommandText = @"DELETE FROM Contributions
                                WHERE SimchaId = @simchaId";

            connection.Open();
            cmd.Parameters.AddWithValue("@simchaId", contributions[0].SimchaId);

            cmd.ExecuteNonQuery();

            cmd.CommandText = @"INSERT INTO Contributions
                              VALUES (@simchaId, @contribId, @amnt)";

            foreach (Contribution c in contributions)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@simchaId", c.SimchaId);
                cmd.Parameters.AddWithValue("@contribId", c.ContributorId);
                cmd.Parameters.AddWithValue("@amnt", c.Amount);
                cmd.ExecuteNonQuery();
            }
        }

        public List<History> GetPersonsHistory(int contribId) // fix the list to correct values
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT 
                                    s.Name AS EventName, 
                                    s.Date AS Date, 
                                    c.Amount AS Amount,
                                    'Contribution' AS RecordType
                                FROM 
                                    Contributions c
                                JOIN 
                                    Simchas s ON c.SimchaId = s.Id
                                WHERE 
                                    c.ContributorId = @id

                                UNION ALL

                                SELECT 
                                    NULL AS EventName, -- Placeholder for alignment
                                    d.Date AS Date, 
                                    d.Amount AS Amount,
                                    'Deposit' AS RecordType
                                FROM 
                                    Deposits d
                                WHERE 
                                    d.ContributorId = @id

                                ORDER BY 
                                    Date DESC; -- Optional: orders the combined results by date";
            cmd.Parameters.AddWithValue("@id", contribId);
            connection.Open();
            var reader = cmd.ExecuteReader();
            List<History> history = new();

            while (reader.Read())
            {
                history.Add(new History
                {
                    EventName = reader.GetOrNull<string>("EventName"),
                    Date = (DateTime)reader["Date"],
                    Amount = (decimal)reader["Amount"],
                    RecordType = (string)reader["RecordType"]
                });
            }

            return history;
        }

        public string GetPersonNameById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT FirstName, LastName FROM Contributors 
                                WHERE Id = @id ";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();

            var reader = cmd.ExecuteReader();
            reader.Read();
            var name = (string)reader["FirstName"];
            name += " "; 
            name += (string)reader["LastName"];

            return name;
        }
    }
}
