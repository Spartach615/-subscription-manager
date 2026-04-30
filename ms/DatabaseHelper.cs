using System;
using System.Data;
using Npgsql;

namespace SubscriptionManager
{
    public static class DatabaseHelper
    {
        private static string connectionString = "Host=192.168.2.149;Port=5432;Database=ISP224/1 PK2;Username=pc_2;Password=1234";

        public static void InitializeDatabase()
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string checkTable = "SELECT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name='users')";
                using (var cmd = new NpgsqlCommand(checkTable, conn))
                {
                    bool exists = Convert.ToBoolean(cmd.ExecuteScalar());
                    if (!exists)
                    {
                        CreateTables(conn);
                        SeedTestData(conn);
                    }
                }
            }
        }

        private static void CreateTables(NpgsqlConnection conn)
        {
            string sql = @"
                CREATE TABLE IF NOT EXISTS users (
                    id SERIAL PRIMARY KEY,
                    name VARCHAR(25) NOT NULL,
                    email VARCHAR(100) UNIQUE NOT NULL,
                    password VARCHAR(255) NOT NULL,
                    role VARCHAR(20) DEFAULT 'user',
                    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
                    balance DECIMAL(10,2) DEFAULT 0
                );

                CREATE TABLE IF NOT EXISTS podpiski (
                    id SERIAL PRIMARY KEY,
                    name VARCHAR(25) NOT NULL,
                    description TEXT,
                    price DECIMAL(10,2) NOT NULL,
                    duration_days INTEGER NOT NULL,
                    trial_days INTEGER DEFAULT 0,
                    auto_renewal_default BOOLEAN DEFAULT TRUE,
                    is_active BOOLEAN DEFAULT TRUE
                );

                CREATE TABLE IF NOT EXISTS users_podpiski (
                    id SERIAL PRIMARY KEY,
                    user_id INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE,
                    podpiski_id INTEGER NOT NULL REFERENCES podpiski(id) ON DELETE CASCADE,
                    start_date TIMESTAMPTZ NOT NULL,
                    end_date TIMESTAMPTZ,
                    status VARCHAR(20) DEFAULT 'active',
                    auto_renewal BOOLEAN,
                    price_paid DECIMAL(10,2),
                    payment_method VARCHAR(50),
                    last_payment_date TIMESTAMPTZ,
                    cancelled_at TIMESTAMPTZ,
                    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
                );
            ";
            using (var cmd = new NpgsqlCommand(sql, conn))
                cmd.ExecuteNonQuery();
        }

        private static void SeedTestData(NpgsqlConnection conn)
        {
            string insertUsers = @"
                INSERT INTO users (name, email, password_hash, birth_date, role, balance) 
                VALUES ('Иван Петров', 'ivan@example.com', 'pass123', '1995-05-15', 'user', 0)
                ON CONFLICT (email) DO NOTHING;
            ";
            using (var cmd = new NpgsqlCommand(insertUsers, conn))
                cmd.ExecuteNonQuery();

            string insertPodpiski = @"
                INSERT INTO podpiski (name, description, price, duration_days, trial_days) VALUES 
                ('Базовая', 'Доступ к основным функциям', 299, 30, 0),
                ('Премиум', 'Все функции + поддержка', 599, 30, 7),
                ('Годовая', 'Экономия 20%', 2990, 365, 0)
                ON CONFLICT (name) DO NOTHING;
            ";
            using (var cmd = new NpgsqlCommand(insertPodpiski, conn))
                cmd.ExecuteNonQuery();
        }

        public static DataTable GetUsers()
        {
            var dt = new DataTable();
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT id, name, email, birth_date, role, is_active, created_at FROM users";
                using (var adapter = new NpgsqlDataAdapter(sql, conn))
                    adapter.Fill(dt);
            }
            return dt;
        }

        public static void AddUser(string name, string email, string passwordHash, string birthDate, string role, bool isActive)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO users (name, email, password_hash, birth_date, role, is_active, balance) 
                               VALUES (@name, @email, @pwd, @birth, @role, @active, 0)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@pwd", passwordHash);
                    cmd.Parameters.AddWithValue("@birth", string.IsNullOrEmpty(birthDate) ? (object)DBNull.Value : (object)DateTime.Parse(birthDate));
                    cmd.Parameters.AddWithValue("@role", role);
                    cmd.Parameters.AddWithValue("@active", isActive);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateUser(int id, string name, string email, string passwordHash, string birthDate, string role, bool isActive)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"UPDATE users SET name=@name, email=@email, password_hash=@pwd, birth_date=@birth, role=@role, is_active=@active WHERE id=@id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@pwd", passwordHash);
                    cmd.Parameters.AddWithValue("@birth", string.IsNullOrEmpty(birthDate) ? (object)DBNull.Value : (object)DateTime.Parse(birthDate));
                    cmd.Parameters.AddWithValue("@role", role);
                    cmd.Parameters.AddWithValue("@active", isActive);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteUser(int id)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("DELETE FROM users WHERE id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static DataTable GetPodpiski()
        {
            var dt = new DataTable();
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT id, name, description, price, duration_days, trial_days, auto_renewal_default, is_active FROM podpiski";
                using (var adapter = new NpgsqlDataAdapter(sql, conn))
                    adapter.Fill(dt);
            }
            return dt;
        }

        public static void AddPodpiska(string name, string description, decimal price, int durationDays, int trialDays, bool autoRenewalDefault, bool isActive)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO podpiski (name, description, price, duration_days, trial_days, auto_renewal_default, is_active) 
                               VALUES (@name, @desc, @price, @dur, @trial, @auto, @active)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@desc", description ?? "");
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@dur", durationDays);
                    cmd.Parameters.AddWithValue("@trial", trialDays);
                    cmd.Parameters.AddWithValue("@auto", autoRenewalDefault);
                    cmd.Parameters.AddWithValue("@active", isActive);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdatePodpiska(int id, string name, string description, decimal price, int durationDays, int trialDays, bool autoRenewalDefault, bool isActive)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"UPDATE podpiski SET name=@name, description=@desc, price=@price, duration_days=@dur, trial_days=@trial, 
                               auto_renewal_default=@auto, is_active=@active WHERE id=@id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@desc", description ?? "");
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@dur", durationDays);
                    cmd.Parameters.AddWithValue("@trial", trialDays);
                    cmd.Parameters.AddWithValue("@auto", autoRenewalDefault);
                    cmd.Parameters.AddWithValue("@active", isActive);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeletePodpiska(int id)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("DELETE FROM podpiski WHERE id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static DataTable GetUserSubscriptions()
        {
            var dt = new DataTable();
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"
                    SELECT up.id, u.name as user_name, p.name as podpiska_name, 
                           up.start_date, up.end_date, up.status, up.auto_renewal, 
                           up.price_paid, up.payment_method, up.last_payment_date, up.cancelled_at
                    FROM users_podpiski up
                    JOIN users u ON up.user_id = u.id
                    JOIN podpiski p ON up.podpiski_id = p.id
                    ORDER BY up.start_date DESC";
                using (var adapter = new NpgsqlDataAdapter(sql, conn))
                    adapter.Fill(dt);
            }
            return dt;
        }

        public static void AssignSubscription(int userId, int podpiskaId, DateTime startDate, int? trialDaysUsed = null)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string getTariff = "SELECT duration_days, price, trial_days FROM podpiski WHERE id=@id";
                int duration = 30;
                decimal price = 0;
                int trialDays = 0;
                using (var cmd = new NpgsqlCommand(getTariff, conn))
                {
                    cmd.Parameters.AddWithValue("@id", podpiskaId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            duration = reader.GetInt32(0);
                            price = reader.GetDecimal(1);
                            trialDays = reader.GetInt32(2);
                        }
                    }
                }

                DateTime endDate = startDate.AddDays(duration);
                decimal? pricePaid = (trialDaysUsed.HasValue && trialDaysUsed > 0) ? 0 : price;

                string insertSql = @"
                    INSERT INTO users_podpiski (user_id, podpiski_id, start_date, end_date, status, auto_renewal, price_paid)
                    VALUES (@uid, @pid, @start, @end, 'active', true, @price)";
                using (var cmd = new NpgsqlCommand(insertSql, conn))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@pid", podpiskaId);
                    cmd.Parameters.AddWithValue("@start", startDate);
                    cmd.Parameters.AddWithValue("@end", endDate);
                    cmd.Parameters.AddWithValue("@price", pricePaid ?? (object)DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void CancelSubscription(int subscriptionId)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string sql = "UPDATE users_podpiski SET status='cancelled', cancelled_at=CURRENT_TIMESTAMP WHERE id=@id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", subscriptionId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteSubscription(int subscriptionId)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("DELETE FROM users_podpiski WHERE id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", subscriptionId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static (int userId, string role) AuthenticateUser(string email, string password)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT id, password, role FROM users WHERE email = @email";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int userId = reader.GetInt32(0);
                            string storedPassword = reader.GetString(1);
                            string role = reader.GetString(2);
                            if (storedPassword == password)
                                return (userId, role);
                        }
                        return (-1, null);
                    }
                }
            }
        }

        public static bool RegisterUser(string name, string email, string password)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string check = "SELECT COUNT(*) FROM users WHERE email = @email";
                using (var cmd = new NpgsqlCommand(check, conn))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    if ((long)cmd.ExecuteScalar() > 0)
                        return false;
                }

                string sql = @"INSERT INTO users (name, email, password, role, balance) 
                               VALUES (@name, @email, @password, 'user', 0)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
        }

        public static void EnsureAdminExists()
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string check = "SELECT COUNT(*) FROM users WHERE role = 'admin'";
                long count = (long)new NpgsqlCommand(check, conn).ExecuteScalar();
                if (count == 0)
                {
                    string sql = @"INSERT INTO users (name, email, password, role, balance) 
                                   VALUES ('Administrator', 'admin@example.com', 'admin', 'admin', 0)";
                    using (var cmd = new NpgsqlCommand(sql, conn))
                        cmd.ExecuteNonQuery();
                }
            }
        }

        public static DataTable GetAllUsersWithBalance()
        {
            var dt = new DataTable();
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT id, name, email, role, balance FROM users ORDER BY id";
                using (var adapter = new NpgsqlDataAdapter(sql, conn))
                    adapter.Fill(dt);
            }
            return dt;
        }

        public static void UpdateUserBalance(int userId, decimal newBalance)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string sql = "UPDATE users SET balance = @balance WHERE id = @id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@balance", newBalance);
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void ChangeUserRole(int userId, string newRole)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string sql = "UPDATE users SET role = @role WHERE id = @id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@role", newRole);
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void AddNewSubscription(string name, string description, decimal price, int durationDays, int trialDays, bool autoRenew)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO podpiski (name, description, price, duration_days, trial_days, auto_renewal_default, is_active)
                               VALUES (@name, @desc, @price, @dur, @trial, @auto, true)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@desc", description ?? "");
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@dur", durationDays);
                    cmd.Parameters.AddWithValue("@trial", trialDays);
                    cmd.Parameters.AddWithValue("@auto", autoRenew);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateSubscriptionPrice(int podpiskaId, decimal newPrice)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string sql = "UPDATE podpiski SET price = @price WHERE id = @id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@price", newPrice);
                    cmd.Parameters.AddWithValue("@id", podpiskaId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static decimal GetUserBalance(int userId)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT balance FROM users WHERE id = @id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", userId);
                    object result = cmd.ExecuteScalar();
                    return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
                }
            }
        }

        public static bool AddToBalance(int userId, decimal amount)
        {
            if (amount <= 0) return false;
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string sql = "UPDATE users SET balance = balance + @amount WHERE id = @id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@id", userId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public static bool PurchaseSubscription(int userId, int podpiskaId, int durationDays, decimal pricePaid)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                decimal balance = GetUserBalance(userId);
                if (balance < pricePaid) return false;

                string deduct = "UPDATE users SET balance = balance - @price WHERE id = @id";
                using (var cmd = new NpgsqlCommand(deduct, conn))
                {
                    cmd.Parameters.AddWithValue("@price", pricePaid);
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.ExecuteNonQuery();
                }

                DateTime startDate = DateTime.Now;
                string getLast = @"SELECT end_date FROM users_podpiski 
                                   WHERE user_id = @uid AND podpiski_id = @pid AND status = 'active'
                                   ORDER BY end_date DESC LIMIT 1";
                using (var cmd = new NpgsqlCommand(getLast, conn))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@pid", podpiskaId);
                    var lastEnd = cmd.ExecuteScalar();
                    if (lastEnd != null && lastEnd != DBNull.Value)
                    {
                        DateTime last = Convert.ToDateTime(lastEnd);
                        if (last > DateTime.Now) startDate = last;
                    }
                }

                DateTime newEndDate = startDate.AddDays(durationDays);
                string insert = @"INSERT INTO users_podpiski (user_id, podpiski_id, start_date, end_date, status, auto_renewal, price_paid)
                                  VALUES (@uid, @pid, @start, @end, 'active', true, @price)";
                using (var cmd = new NpgsqlCommand(insert, conn))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@pid", podpiskaId);
                    cmd.Parameters.AddWithValue("@start", startDate);
                    cmd.Parameters.AddWithValue("@end", newEndDate);
                    cmd.Parameters.AddWithValue("@price", pricePaid);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
        }

        public class ActiveSubscriptionInfo
        {
            public DateTime EndDate { get; set; }
        }

        public static ActiveSubscriptionInfo GetActiveSubscriptionForUser(int userId, int podpiskaId)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"SELECT end_date FROM users_podpiski 
                       WHERE user_id = @uid AND podpiski_id = @pid 
                         AND status = 'active' AND end_date > CURRENT_TIMESTAMP
                       ORDER BY end_date DESC LIMIT 1";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@pid", podpiskaId);
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        return new ActiveSubscriptionInfo { EndDate = Convert.ToDateTime(result) };
                    }
                }
            }
            return null;
        }

        public static bool ExtendSubscription(int userId, int podpiskaId, int additionalDays, decimal additionalPricePaid)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string selectSql = @"SELECT id, end_date FROM users_podpiski 
                             WHERE user_id = @uid AND podpiski_id = @pid 
                               AND status = 'active' AND end_date > CURRENT_TIMESTAMP
                             ORDER BY end_date DESC LIMIT 1";
                int subId = 0;
                DateTime? currentEnd = null;
                using (var cmd = new NpgsqlCommand(selectSql, conn))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@pid", podpiskaId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            subId = reader.GetInt32(0);
                            currentEnd = reader.GetDateTime(1);
                        }
                    }
                }
                if (subId == 0) return false;

                string deduct = "UPDATE users SET balance = balance - @price WHERE id = @id";
                using (var cmd = new NpgsqlCommand(deduct, conn))
                {
                    cmd.Parameters.AddWithValue("@price", additionalPricePaid);
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.ExecuteNonQuery();
                }

                DateTime newEnd = currentEnd.Value.AddDays(additionalDays);
                string update = "UPDATE users_podpiski SET end_date = @end, price_paid = price_paid + @price WHERE id = @id";
                using (var cmd = new NpgsqlCommand(update, conn))
                {
                    cmd.Parameters.AddWithValue("@end", newEnd);
                    cmd.Parameters.AddWithValue("@price", additionalPricePaid);
                    cmd.Parameters.AddWithValue("@id", subId);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
        }

        public static DataTable GetUserSubscriptions(int userId)
        {
            var dt = new DataTable();
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"
                    SELECT p.name, up.start_date, up.end_date, up.status, up.price_paid
                    FROM users_podpiski up
                    JOIN podpiski p ON up.podpiski_id = p.id
                    WHERE up.user_id = @uid
                    ORDER BY up.end_date DESC";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    using (var adapter = new NpgsqlDataAdapter(cmd))
                        adapter.Fill(dt);
                }
            }
            return dt;
        }
    }
}