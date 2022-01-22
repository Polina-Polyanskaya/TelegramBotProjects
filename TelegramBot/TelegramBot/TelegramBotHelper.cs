using Npgsql;
using System.Data;
using Telegram.Bot;
using Telegram.Bot.Types;

internal class TelegramBotHelper
{
    string token;
    TelegramBotClient client;
    NpgsqlConnection con;
    string sql;
    NpgsqlCommand cmd;
    string connstring = "";

    public TelegramBotHelper(string token)
    {
        this.token = token;
    }

    internal void GetUpdates()
    {
        client = new TelegramBotClient(token);
        var me = client.GetMeAsync().Result;
        if (me != null && !string.IsNullOrEmpty(me.Username))
        {
            int offset = 0;
            while (true)
            {
                try
                {
                    var updates = client.GetUpdatesAsync(offset).Result;
                    if (updates != null && updates.Count() > 0)
                    {
                        foreach (var update in updates)
                        {
                            processUpdate(update);
                            offset = update.Id + 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Thread.Sleep(1000);
            }
        }
    }

    private async void processUpdate(Telegram.Bot.Types.Update update)
    {
        connstring = $"Server=localhost;Port=5432;User Id=postgres;Password=Polina1521Misha;Database=myDb";
        Console.WriteLine(update.Type);
        switch (update.Type)
        {
            case Telegram.Bot.Types.Enums.UpdateType.Message:
               // case Telegram.Bot.Types.Enums.UpdateType.
                var text = update.Message.Text;
                Console.WriteLine(text);
                if (text.StartsWith("add"))
                {
                    con = new NpgsqlConnection(connstring);
                    string[] mas = text.Split(' ');
                    if (mas.Length == 3)
                        AddStudent(mas[1], mas[2]);
                    else
                        await client.SendTextMessageAsync(update.Message.Chat.Id, "Wrong string");
                }
                else if (text.StartsWith("update"))
                {
                    con = new NpgsqlConnection(connstring);
                    string[] mas = text.Split(' ');
                    if (mas.Length == 4)
                        UpdateStudent(Convert.ToInt32(mas[1]), mas[2], mas[3]);
                    else
                        await client.SendTextMessageAsync(update.Message.Chat.Id, "Wrong string");

                }
                else if (text.StartsWith("/get"))
                {
                    con = new NpgsqlConnection(connstring);
                    GetStudents();
                }
                else if (text.StartsWith("delete"))
                {
                    con = new NpgsqlConnection(connstring);
                    string[] mas = text.Split(' ');
                    if (mas.Length == 2)
                        DeleteStudent(Convert.ToInt32(mas[1]));
                    else
                        await client.SendTextMessageAsync(update.Message.Chat.Id, "Wrong string");
                }
                else
                {
                    await client.SendTextMessageAsync(update.Message.Chat.Id, $"Strange message {update.Message.Text} was deleted.");
                    await client.DeleteMessageAsync(update.Message.Chat.Id, update.Message.MessageId);
                }
                break;
            default:
                Console.WriteLine(update.Type + " not implemented ");
                break;
        }

        async void GetStudents()
        {
            try
            {
                con.Open();
                sql = @"select * from get_st()";
                //string s= @"create table T (a int)";
                cmd = new NpgsqlCommand(sql, con);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    await client.SendTextMessageAsync(update.Message.Chat.Id, reader["_id"].ToString() + " " +
                        reader["_firstname"].ToString() + " " + reader["_lastname"].ToString());
                    //reader.GetString(1) reader.GetInt32(0)
                }
                //NpgsqlCommand cmd2 = new NpgsqlCommand(s, con);
                con.Close();
                con.Open();
                //cmd2.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                con.Close();
                await client.SendTextMessageAsync(update.Message.Chat.Id, "Problems with database " + ex.Message);
            }
        }

        async void AddStudent(string name, string surname)
        {
            try
            {
                using (NpgsqlConnection c = new NpgsqlConnection(connstring))
                {
                    c.Open();
                    sql = @"select * from insert_st(:name,:surname)";
                    cmd = new NpgsqlCommand(sql, c);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("name", name);
                    cmd.Parameters.AddWithValue("surname", surname);
                    int res = (int)cmd.ExecuteScalar();
                    //con.Close();
                    if (res == 1)
                        await client.SendTextMessageAsync(update.Message.Chat.Id, "Inserted success");
                    else
                        await client.SendTextMessageAsync(update.Message.Chat.Id, "Inserted failed");
                }
            }
            catch (Exception ex)
            {
                con.Close();
                await client.SendTextMessageAsync(update.Message.Chat.Id, "Problems with database " + ex.Message);
            }
        }

        async void UpdateStudent(int id, string name, string surname)
        {
            try
            {
                con.Open();
                sql = @"select * from update_st(:id,:name,:surname)";
                cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("id", id);
                cmd.Parameters.AddWithValue("name", name);
                cmd.Parameters.AddWithValue("surname", surname);
                int res = (int)cmd.ExecuteScalar();
                con.Close();
                if (res == 1)
                    await client.SendTextMessageAsync(update.Message.Chat.Id, "Updated success");
                else
                    await client.SendTextMessageAsync(update.Message.Chat.Id, "Updated failed");

            }
            catch (Exception ex)
            {
                con.Close();
                await client.SendTextMessageAsync(update.Message.Chat.Id, "Problems with database " + ex.Message);
            }
        }

        async void DeleteStudent(int id)
        {
            try
            {
                con.Open();
                sql = @"select * from delete_st(:_id)";
                cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("_id", id);
                int res = (int)cmd.ExecuteScalar();
                con.Close();
                if (res == 1)
                    await client.SendTextMessageAsync(update.Message.Chat.Id, "Deleted success");
                else
                    await client.SendTextMessageAsync(update.Message.Chat.Id, "Deleted failed");

            }
            catch (Exception ex)
            {
                con.Close();
                await client.SendTextMessageAsync(update.Message.Chat.Id, "Problems with database " + ex.Message);
            }
        }
    }
}
